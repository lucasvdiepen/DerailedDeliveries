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

        [ContextMenu("Gather TrainStation's in scene")]
        private void GatherAllStationsInScene() => _allStations = FindObjectsOfType<TrainStation>();

        public void SelectLevelToLoad(int index)
        {
            StationLevelData[] levelData = _levels.levels[index].StationLevelData;

            for(int i = 0; i < levelData.Length; i++)
            {
                StationLevelData stationData = levelData[i];
                TrainStation targetStation = _allStations[stationData.StationID];
                List<Transform> spawns = targetStation.SpawnTransforms.ToList();

                string label = "";

                targetStation.AssignStationID(label, i);

                System.Random random = new System.Random();

                int maxSpawns = stationData.MaxPackagesAmount <= spawns.Count 
                    ? stationData.MaxPackagesAmount 
                    : spawns.Count;

                int amountOfSpawns = stationData.RandomizeSpawns
                    ? random.Next(0, maxSpawns)
                    : stationData.MinDeliverablePackages;

                for(int j = 0; j < amountOfSpawns; i++)
                {
                    GameObject spawnedPackage = Instantiate(_packagePrefab);

                    spawnedPackage.GetComponent<BoxGrabbable>().UpdateLabelAndID(label, i);
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