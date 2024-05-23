using Cinemachine;
using DG.Tweening;
using UnityEngine;

using DerailedDeliveries.Framework.PopupManagement;

namespace DerailedDeliveries.Framework.Gameplay.Map
{
    /// <summary>
    /// Class responsible for updating the weight of the map background in the <see cref="CinemachineTargetGroup"/>.
    /// </summary>
    public class MapTargetGroupUpdater : MonoBehaviour
    {
        [SerializeField]
        private CinemachineTargetGroup _targetGroup;

        [SerializeField]
        private Popup _mapPopup;

        [SerializeField]
        private float _endTargetWeight = 1.25f;

        [SerializeField]
        private float _lerpWeightDuration = 1.25f;

        private void OnEnable()
        {
            _mapPopup.OnShowPopup += HandleShowPopup;
            _mapPopup.OnClosePopup += HandleClosePopup;
        }

        private void OnDisable()
        {
            if (_mapPopup == null)
                return;

            _mapPopup.OnShowPopup -= HandleShowPopup;
            _mapPopup.OnClosePopup -= HandleClosePopup;
        }

        private void HandleClosePopup()
        {
            DOTween.To(()
                => _targetGroup.m_Targets[0].weight, x
                => _targetGroup.m_Targets[0].weight = x, 0, _lerpWeightDuration);

            DOTween.To(()
                => _targetGroup.m_Targets[0].radius, x
                => _targetGroup.m_Targets[0].radius = x, 0, _lerpWeightDuration);
        }

        private void HandleShowPopup()
        {
            DOTween.To(() 
                => _targetGroup.m_Targets[0].weight, x 
                => _targetGroup.m_Targets[0].weight = x, _endTargetWeight, _lerpWeightDuration);

            DOTween.To(()
                => _targetGroup.m_Targets[0].radius, x
                => _targetGroup.m_Targets[0].radius = x, 1.15f, _lerpWeightDuration);
        }
    }
}
