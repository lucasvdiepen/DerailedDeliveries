using Cinemachine;
using UnityEngine;

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

        private void OnEnable() => PlayerManager.Instance.OnPlayerJoined += HandlePlayerJoined;

        private void OnDisable() => PlayerManager.Instance.OnPlayerJoined -= HandlePlayerJoined;

        private void HandlePlayerJoined(PlayerId playerId) => _targetGroup.AddMember(playerId.transform, 1, 1);

        private void Update() => _targetGroup.transform.rotation = _trainController.MiddleWagon.rotation;
    }
}

