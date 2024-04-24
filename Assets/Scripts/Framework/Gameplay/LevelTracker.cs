using System.Collections.Generic;
using GameKit.Utilities;
using FishNet.Object;
using UnityEngine;
using System.Linq;
using System;

using DerailedDeliveries.Framework.DamageRepairManagement.Damageables;
using DerailedDeliveries.Framework.Gameplay.Level;
using DerailedDeliveries.Framework.Utils;
using Unity.Mathematics;

namespace DerailedDeliveries.Framework.Gameplay
{
    /// <summary>
    /// A class that generates the objectives and tracks the Level's progress.
    /// </summary>
    public class LevelTracker : NetworkAbstractSingleton<LevelTracker>
    {
        [SerializeField]
        private Levels _levels;

        [SerializeField]
        private int _succesfullDeliveryBonus = 5;

        [SerializeField]
        private int _incorrectDeliveryPenalty = 5;

        [SerializeField]
        private int _totalScore;

        [SerializeField]
        private int _currentScore;

        [SerializeField]
        private GameObject _packagePrefab;

        [SerializeField]
        private TrainStation[] _allStations;

        /// <summary>
        /// An action that broadcasts when a package is delivered and the amount of points that is added.
        /// </summary>
        public Action<int> OnPackageDelivered;

        private static readonly char[] CHARACTERS = "QWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();

            SelectLevelToLoad(0);
        }

        /// <summary>
        /// A function that is used to load a new level.
        /// </summary>
        /// <param name="index">The level ID.</param>
        [Server]
        private void SelectLevelToLoad(int index)
        {
            _currentScore = 0;

            StationLevelData[] levelData = _levels.levels[index].StationLevelData;
            List<string> labels = GenerateLabelsForStations(levelData);

            List<Transform> usedSpawns = SpawnDeliveries(levelData, labels);

            List<Transform> freeSpawns = GetAllFreeSpawns(usedSpawns);

            SpawnFakeDeliveries(freeSpawns, labels);
        }

        private List<string> GenerateLabelsForStations(StationLevelData[] stationData)
        {
            List<string> labels = new();

            for (int i = 0; i < stationData.Length; i++)
            {
                string label = string.Empty;
                while (label == string.Empty || labels.Contains(label))
                    label = GenerateNewLabel();

                labels.Add(label);
                _allStations[i].UpdateLabelAndID(label, i);
            }

            return labels;
        }

        private List<Transform> SpawnDeliveries(StationLevelData[] levelData, List<string> labels)
        {
            List<Transform> usedSpawns = new();

            for (int i = levelData.Length - 1; i >= 0; i--)
            {
                List<Transform> availableSpawns = GetAvailableSpawnsForStation(i, usedSpawns);
                int amountToSpawn = levelData[i].minDeliverablePackagesAmount;

                while (amountToSpawn > 0 && availableSpawns.Count > 0)
                {
                    int spawnIndex = UnityEngine.Random.Range(0, availableSpawns.Count - 1);

                    PackageData package = SpawnBoxDelivery(availableSpawns[spawnIndex], labels[i], i);

                    amountToSpawn--;
                    usedSpawns.Add(availableSpawns[spawnIndex]);
                    availableSpawns.RemoveAt(spawnIndex);

                    _totalScore += _succesfullDeliveryBonus + package.GetComponent<BoxDamageable>().Health;
                }
            }

            return usedSpawns;
        }

        private void SpawnFakeDeliveries(List<Transform> spawns, List<string> usedLabels)
        {
            spawns.Shuffle();
            int fakeDeliverySpawns = UnityEngine.Random.Range(0, spawns.Count - 1);

            for (int i = 0; i < fakeDeliverySpawns; i++)
            {
                string label = string.Empty;
                while (label == string.Empty || usedLabels.Contains(label))
                    label = GenerateNewLabel();

                SpawnBoxDelivery(spawns[i], label, -1);
            }
        }

        private string GenerateNewLabel()
        {
            return CHARACTERS[UnityEngine.Random.Range(0, CHARACTERS.Length - 1)].ToString()
                + CHARACTERS[UnityEngine.Random.Range(0, CHARACTERS.Length - 1)].ToString();
        }

        private List<Transform> GetAvailableSpawnsForStation(int stationIndex, List<Transform> usedSpawns)
        {
            return Enumerable.Range(0, stationIndex)
                .SelectMany(i => _allStations[i].SpawnTransforms)
                .Except(usedSpawns)
                .ToList();
        }

        private List<Transform> GetAllFreeSpawns(List<Transform> usedSpawns)
        {
            return _allStations
                .SelectMany(station => station.SpawnTransforms)
                .Except(usedSpawns)
                .ToList();
        }

        private PackageData SpawnBoxDelivery(Transform spawnTransform, string label, int id)
        {
            PackageData package = Instantiate(_packagePrefab).GetComponent<PackageData>();
            ServerManager.Spawn(package.gameObject);

            package.transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);

            package.UpdateLabelAndID(label, id);

            return package;
        }

        /// <summary>
        /// A function that handles the delivery of a package.
        /// </summary>
        /// <param name="delivery">The package that was delivered.</param>
        /// <param name="stationID">The ID of the station it was delivered to.</param>
        [Server]
        public void HandlePackageDelivery(PackageData package, int stationID)
        {
            if (package.PackageID == stationID)
                _currentScore += _succesfullDeliveryBonus + package.GetComponent<BoxDamageable>().Health;
            else
                _currentScore -= _incorrectDeliveryPenalty;

            HandleScoreUpdate(_currentScore, package.PackageID);
            ServerManager.Despawn(package.gameObject);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void HandleScoreUpdate(int newScore, int packageID)
        {
            _currentScore = newScore;

            OnPackageDelivered?.Invoke(packageID);
        }
    }
}