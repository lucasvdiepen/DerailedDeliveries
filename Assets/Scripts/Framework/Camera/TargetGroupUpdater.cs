using Cinemachine;
using UnityEngine;
using FishNet;

using DerailedDeliveries.Framework.Train;
using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.Camera
{
    /// <summary>
    /// Class responsible for updating the cinemachine target group.
    /// </summary>
    public class TargetGroupUpdater : MonoBehaviour
    {
        [SerializeField]
        private CinemachineTargetGroup _targetGroup;

        private TrainController _trainController;

        private void Awake() => _trainController = TrainEngine.Instance.GetComponent<TrainController>();

        private void OnEnable()
        {
            PlayerManager.Instance.OnPlayerJoined += HandlePlayerJoined;
            InstanceFinder.TimeManager.OnPostTick += OnPostTick;
        }

        private void OnDisable()  
        {
            PlayerManager.Instance.OnPlayerJoined -= HandlePlayerJoined;
            InstanceFinder.TimeManager.OnPostTick -= OnPostTick;
        }

        private void HandlePlayerJoined(PlayerId playerId) => _targetGroup.AddMember(playerId.transform, 1, 1);

        private void OnPostTick() => _targetGroup.transform.rotation = _trainController.MiddleWagon.rotation;
    }
}

