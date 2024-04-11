using UnityEngine;
using System;

using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Level;
using DerailedDeliveries.Framework.Utils;
using System.Collections.Generic;
using GameKit.Utilities;
using System.Linq;

namespace DerailedDeliveries.Framework.Gameplay
{
    public class LevelTracker : AbstractSingleton<LevelTracker>
    {
        [SerializeField]
        private Levels _levels;

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

        public void SelectLevelToLoad(int index)
        {
            Dictionary<int, string> labelsAndIDs = new();

            StationLevelData[] levelData = _levels.levels[index].StationLevelData;
            System.Random random = new System.Random();

            for(int i = 0; i < levelData.Length; i++)
            {
                TrainStation targetStation = _allStations[levelData[i].StationID];
                string label = "";

                while(label == "" || labelsAndIDs.ContainsValue(label))
                {
                    label 
                        = CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString() 
                        + CHARACTERS[random.Next(0, CHARACTERS.Length)].ToString();
                }

                targetStation.UpdateLabelAndReturnID(label);
                labelsAndIDs.Add(targetStation.StationID, label);
            }

            for(int i = 0; i < levelData.Length; i++)
            {
                string stationLabel = labelsAndIDs[levelData[i].StationID];
            }

            // TO DO: Per station ... hoeveelheid packets worden gegenerate op een gehusselde lijst aan available stations die al geweeest zijn
            // De hoeveelheid packets die nog moeten gegenerate worden:
            // 15 / 1 chance to generate met random int decide
            // if random.Next(1, 15) == 1, int-- en 15--.




            for (int i = 0; i < levelData.Length - 1; i++)
            {
                StationLevelData stationData = levelData[i];
                int stationID = levelData[i].StationID;
                List<Transform> spawns = _allStations[stationID].SpawnTransforms.ToList();
                spawns.Shuffle();

                int maxSpawns = stationData.MaxPackagesAmount <= spawns.Count 
                    ? stationData.MaxPackagesAmount 
                    : spawns.Count;

                int amountOfSpawns = stationData.RandomizeSpawns
                    ? random.Next(0, maxSpawns)
                    : stationData.MinDeliverablePackages;

                for(int j = 0; j < amountOfSpawns; j++)
                {
                    GameObject spawnedPackage = Instantiate(_packagePrefab);

                    if (!spawnedPackage.TryGetComponent(out BoxGrabbable boxGrabbable))
                        return;

                    boxGrabbable.transform.position = spawns[j].transform.position;
                    boxGrabbable.transform.rotation = spawns[j].transform.rotation;
                    boxGrabbable.UpdateLabelAndID(_allStations[stationID + 1].StationLabel, stationID + 1);
                    boxGrabbable.PlaceOnGround();
                }
            }
        }

        public void HandlePackageDelivery(BoxGrabbable delivery)
        {
            _currentScore += delivery.PackageQuality;

            OnPackageDelivered?.Invoke(delivery.PackageID);
        }
    }
}