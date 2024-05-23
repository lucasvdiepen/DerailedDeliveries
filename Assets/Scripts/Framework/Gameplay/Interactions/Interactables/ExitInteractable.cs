using FishNet.Object.Synchronizing;
using System.Collections.Generic;

using DerailedDeliveries.Framework.PlayerManagement;
using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.GameManagement;
using DerailedDeliveries.Framework.Station;
using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// An <see cref="Interactable"/> class that players can interact with to end their session.
    /// </summary>
    public class ExitInteractable : Interactable
    {
        private List<int> _arrivedStationIDs = new();

        [SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private int _amountOfStationsVisited;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();

            TrainStationController.Instance.OnParkStateChanged += ProcessStationArrival;

            PlayerManager.Instance.OnPlayersUpdated += CheckForSessionEnding;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopServer()
        {
            base.OnStopServer();

            if(TrainStationController.Instance != null)
                TrainStationController.Instance.OnParkStateChanged -= ProcessStationArrival;

            if (PlayerManager.Instance != null)
                PlayerManager.Instance.OnPlayersUpdated -= CheckForSessionEnding;
        }

        private void ProcessStationArrival(bool isParked)
        {
            if (!isParked)
                return;

            int stationIndex = StationManager.Instance.GetNearestStationIndex
                (
                    TrainStationController.Instance.CurrentTrainLocation
                );

            if (_arrivedStationIDs.Contains(stationIndex))
                return;

            _arrivedStationIDs.Add(stationIndex);
            _amountOfStationsVisited = _arrivedStationIDs.Count;
        }

        private void CheckForSessionEnding()
        {
            if(PlayerManager.Instance.PlayerCount == 0)
                GameManager.Instance.EndGame();
        }

        private protected override bool Use(Interactor interactor)
        {
            if (!base.Use(interactor))
                return false;

            PlayerManager.Instance.DespawnPlayer(interactor.NetworkObject);

            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfUseable(Interactor interactor) => IsInteractable
            && !IsOnCooldown
            && interactor.InteractingTarget == null
            && _amountOfStationsVisited > 1;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfInteractable(Interactor interactor) => false;
    }
}