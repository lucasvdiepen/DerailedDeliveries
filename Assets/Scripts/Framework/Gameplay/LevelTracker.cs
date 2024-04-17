using System.Collections.Generic;
using GameKit.Utilities;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.DamageRepairManagement.Damageables;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
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

        [ContextMenu("Test Package Generation")]
        private void TestGeneration() => SelectLevelToLoad(0);

        /// <summary>
        /// A function that is used to load a new level.
        /// </summary>
        /// <param name="index">The level ID.</param>
        public void SelectLevelToLoad(int index)
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
            System.Random random = new System.Random();
            List<string> labels = new();

            for (int i = 0; i < stationData.Length; i++)
            {
                TrainStation targetStation = _allStations[i];
                string label = string.Empty;

                while (label == string.Empty || labels.Contains(label))
                {
                    label
                        = CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString()
                        + CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString();
                }

                labels.Add(label);
                targetStation.UpdateLabelAndID(label, i);
            }

            return labels;
        }

        private List<Transform> SpawnDeliveries(StationLevelData[] levelData, List<string> labels)
        {
            List<Transform> usedSpawns = new();

            for (int i = levelData.Length - 1; i >= 0; i--)
            {
                List<Transform> availableSpawns = GetAvailableSpawnsForStation(i);
                int amountToSpawn = levelData[i].MinDeliverablePackages;

                while (amountToSpawn > 0 && availableSpawns.Count > 0)
                {
                    availableSpawns.Shuffle();
                    Transform targetSpawn = availableSpawns[0];

                    if (usedSpawns.Contains(targetSpawn))
                        continue;

                    PackageData package = SpawnBoxDelivery(targetSpawn, labels[i], i);

                    amountToSpawn--;
                    usedSpawns.Add(targetSpawn);
                    availableSpawns.Remove(targetSpawn);

                    _totalScore += _succesfullDeliveryBonus + package.GetComponent<BoxDamageable>().Health;
                }
            }

            return usedSpawns;
        }

        private void SpawnFakeDeliveries(List<Transform> spawns, List<string> usedLabels)
        {
            System.Random random = new System.Random();
            spawns.Shuffle();
            int fakeDeliverySpawns = random.Next(0, spawns.Count);

            for (int i = 0; i < fakeDeliverySpawns; i++)
            {
                string label = string.Empty;
                while (label == string.Empty || usedLabels.Contains(label))
                {
                    label
                        = CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString()
                        + CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString();
                }

                SpawnBoxDelivery(spawns[i], label, -1);
            }
        }

        private List<Transform> GetAvailableSpawnsForStation(int stationIndex)
        {
            List<Transform> spawns = new();

            for (int i = stationIndex - 1; i >= 0; i--)
                for (int j = 0; j < _allStations[i].SpawnTransforms.Length; j++)
                    if (!spawns.Contains(_allStations[i].SpawnTransforms[j]))
                        spawns.Add(_allStations[i].SpawnTransforms[j]);

            return spawns;
        }

        private List<Transform> GetAllFreeSpawns(List<Transform> usedSpawns)
        {
            List<Transform> freeSpawns = new();

            for (int i = 0; i < _allStations.Length; i++)
                for (int j = 0; j < _allStations[i].SpawnTransforms.Length; j++)
                    if (!usedSpawns.Contains(_allStations[i].SpawnTransforms[j]))
                        freeSpawns.Add(_allStations[i].SpawnTransforms[j]);

            return freeSpawns;
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
        public void HandlePackageDelivery(PackageData package, int stationID)
        {
            if (package.PackageID == stationID)
                _currentScore += _succesfullDeliveryBonus + package.GetComponent<BoxDamageable>().Health;
            else
                _currentScore -= _incorrectDeliveryPenalty;

            OnPackageDelivered?.Invoke(package.PackageID);
            ServerManager.Despawn(package.gameObject);
        }
    }
}