using System.Collections.Generic;
using GameKit.Utilities;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.DamageRepairManagement.Damageables;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
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

            List<string> labels = new();
            StationLevelData[] levelData = _levels.levels[index].StationLevelData;
            System.Random random = new System.Random();

            for(int i = 0; i < levelData.Length; i++)
            {
                TrainStation targetStation = _allStations[i];
                string label = string.Empty;

                while(label == string.Empty || labels.Contains(label))
                {
                    label 
                        = CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString() 
                        + CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString();
                }

                labels.Add(label);
                targetStation.UpdateLabelAndID(label, i);
            }

            List<Transform> usedSpawns = new();

            for(int i = levelData.Length - 1; i >= 0; i--)
            {
                List<Transform> availableSpawns = GetAvailableSpawnsForStation(i);
                int amountToSpawn = levelData[i].MinDeliverablePackages;

                while(amountToSpawn > 0 && availableSpawns.Count > 0)
                {
                    availableSpawns.Shuffle();
                    Transform targetSpawn = availableSpawns[0];

                    if (usedSpawns.Contains(targetSpawn))
                        continue;

                    BoxGrabbable boxGrabbable = SpawnBoxDelivery(targetSpawn, labels[i], i);

                    amountToSpawn--;
                    usedSpawns.Add(targetSpawn);
                    availableSpawns.Remove(targetSpawn);

                    _totalScore += _succesfullDeliveryBonus + boxGrabbable.GetComponent<BoxDamageable>().Health;
                }
            }

            List<Transform> freeSpawns = GetAllFreeSpawns(usedSpawns);

            int fakeDeliverySpawns = random.Next(0, freeSpawns.Count);
            freeSpawns.Shuffle();

            for (int i = 0; i < fakeDeliverySpawns; i++)
            {
                string label = string.Empty;
                while (label == string.Empty || labels.Contains(label))
                {
                    label
                        = CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString()
                        + CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString();
                }

                SpawnBoxDelivery(freeSpawns[i], label, -1);
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

        private BoxGrabbable SpawnBoxDelivery(Transform spawnTransform, string label, int id)
        {
            BoxGrabbable boxGrabbable = Instantiate(_packagePrefab).GetComponent<BoxGrabbable>();
            ServerManager.Spawn(boxGrabbable.gameObject);

            boxGrabbable.transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);

            boxGrabbable.PlaceOnGround();
            boxGrabbable.UpdateLabelAndID(label, id);

            return boxGrabbable;
        }

        /// <summary>
        /// A function that handles the delivery of a package.
        /// </summary>
        /// <param name="delivery">The package that was delivered.</param>
        /// <param name="stationID">The ID of the station it was delivered to.</param>
        public void HandlePackageDelivery(BoxGrabbable delivery, int stationID)
        {
            if (delivery.PackageID == stationID)
                _currentScore += _succesfullDeliveryBonus + delivery.GetComponent<BoxDamageable>().Health;
            else
                _currentScore -= _incorrectDeliveryPenalty;

            OnPackageDelivered?.Invoke(delivery.PackageID);
            ServerManager.Despawn(delivery.gameObject);
        }
    }
}