using System.Collections.Generic;
using GameKit.Utilities;
using FishNet.Object;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

using DerailedDeliveries.Framework.DamageRepairManagement.Damageables;
using DerailedDeliveries.Framework.Gameplay.Level;
using DerailedDeliveries.Framework.Utils;

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
        private GameObject _packagePrefab;

        [SerializeField]
        private TrainStation[] _allStations;

        /// <summary>
        /// An action that broadcasts when a package is delivered and the ID of the package.
        /// </summary>
        public Action<int> OnPackageDelivered;

        /// <summary>
        /// Invoked when the current score changes.
        /// </summary>
        public Action<int> OnScoreChanged;

        /// <summary>
        /// The total amount of points that can be achieved in this session.
        /// </summary>
        public int TotalAchievableScore { get; private set; }

        /// <summary>
        /// The current score of this session.
        /// </summary>
        public int CurrentScore { get; private set; }

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
            CurrentScore = 0;

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
                string label = GenerateNewLabel(labels);

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
                    int spawnIndex = Random.Range(0, availableSpawns.Count - 1);

                    PackageData package = SpawnBoxDelivery(availableSpawns[spawnIndex], labels[i], i);

                    amountToSpawn--;
                    usedSpawns.Add(availableSpawns[spawnIndex]);
                    availableSpawns.RemoveAt(spawnIndex);

                    TotalAchievableScore += _succesfullDeliveryBonus + package.GetComponent<BoxDamageable>().Health;
                }
            }

            return usedSpawns;
        }

        private void SpawnFakeDeliveries(List<Transform> spawns, List<string> usedLabels)
        {
            spawns.Shuffle();
            int fakeDeliverySpawns = Random.Range(0, spawns.Count - 1);

            for (int i = 0; i < fakeDeliverySpawns; i++)
            {
                string label = GenerateNewLabel(usedLabels);

                SpawnBoxDelivery(spawns[i], label, -1);
            }
        }

        private string GenerateNewLabel(List<string> usedLabels)
        {
            string label = string.Empty;
            while (label == string.Empty || usedLabels.Contains(label))
            {
                label
                    = CHARACTERS[Random.Range(0, CHARACTERS.Length - 1)].ToString()
                    + CHARACTERS[Random.Range(0, CHARACTERS.Length - 1)].ToString();
            }

            return label;
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
            int newScore = CurrentScore;

            if (package.PackageID == stationID)
                newScore += _succesfullDeliveryBonus + package.GetComponent<BoxDamageable>().Health;
            else
                newScore -= _incorrectDeliveryPenalty;

            HandleScoreUpdate(newScore, package.PackageID);
            ServerManager.Despawn(package.gameObject);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void HandleScoreUpdate(int newScore, int packageID)
        {
            CurrentScore = newScore;

            OnPackageDelivered?.Invoke(packageID);
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}