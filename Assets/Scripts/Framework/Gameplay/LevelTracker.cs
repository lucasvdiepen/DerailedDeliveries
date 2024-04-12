using System.Collections.Generic;
using GameKit.Utilities;
using UnityEngine;
using System;


using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Level;
using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.DamageRepairManagement.Damageables;

namespace DerailedDeliveries.Framework.Gameplay
{
    /// <summary>
    /// A class that generates the objectives and tracks the Level's progress.
    /// </summary>
    public class LevelTracker : AbstractSingleton<LevelTracker>
    {
        [SerializeField]
        private Levels _levels;

        [SerializeField]
        private int _succesfullDeliveryBonus = 5;

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
                string label = "";

                while(label == "" || labels.Contains(label))
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
                List<Transform> availableSpawns = new();
                int amountToSpawn = levelData[i].MinDeliverablePackages;

                for (int j = i - 1; j >= 0; j--)
                    for (int k = 0; k < _allStations[j].SpawnTransforms.Length; k++)
                        if (!usedSpawns.Contains(_allStations[j].SpawnTransforms[k]))
                            availableSpawns.Add(_allStations[j].SpawnTransforms[k]);

                while(amountToSpawn > 0 && availableSpawns.Count > 0)
                {
                    availableSpawns.Shuffle();
                    Transform targetSpawn = availableSpawns[0];

                    if (usedSpawns.Contains(targetSpawn))
                        continue;

                    GameObject newPackage = Instantiate(_packagePrefab);
                    BoxGrabbable boxGrabbable = newPackage.GetComponent<BoxGrabbable>();

                    newPackage.transform.position = targetSpawn.position;
                    newPackage.transform.rotation = targetSpawn.rotation;

                    amountToSpawn--;
                    usedSpawns.Add(targetSpawn);
                    availableSpawns.Remove(targetSpawn);

                    _totalScore += _succesfullDeliveryBonus + boxGrabbable.GetComponent<BoxDamageable>().Health;

                    boxGrabbable.UpdateLabelAndID(labels[i], i);
                    boxGrabbable.PlaceOnGround();
                }
            }

            List<Transform> freeSpawns = new();

            for (int i = 0; i < _allStations.Length; i++)
                for (int j = 0; j < _allStations[i].SpawnTransforms.Length; j++)
                    if (!usedSpawns.Contains(_allStations[i].SpawnTransforms[j]))
                        freeSpawns.Add(_allStations[i].SpawnTransforms[j]);

            int fakeDeliverySpawns = random.Next(0, freeSpawns.Count);
            freeSpawns.Shuffle();

            for (int i = 0; i < fakeDeliverySpawns; i++)
            {
                string label = "";
                while (label == "" || labels.Contains(label))
                {
                    label
                        = CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString()
                        + CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString();
                }

                GameObject newFakeDelivery = Instantiate(_packagePrefab);
                BoxGrabbable boxGrabbable = newFakeDelivery.GetComponent<BoxGrabbable>();

                newFakeDelivery.transform.position = freeSpawns[i].position;
                newFakeDelivery.transform.rotation = freeSpawns[i].rotation;

                boxGrabbable.PlaceOnGround();
                boxGrabbable.UpdateLabelAndID(label, -1);
            }
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
                _currentScore -= _succesfullDeliveryBonus;

            OnPackageDelivered?.Invoke(delivery.PackageID);
            Destroy(delivery.gameObject);
        }
    }
}