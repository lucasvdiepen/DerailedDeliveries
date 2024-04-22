using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Map
{
    /// <summary>
    /// A class that is responsible for updating the visuals of the map.
    /// </summary>
    public class MapUpdater : MonoBehaviour
    {
        [Header("World Transforms")]
        [SerializeField]
        private Transform _bottomRightWorldTransform;

        [Header("Map Settings")]
        [SerializeField]
        private RectTransform _mapIndicator;

        [SerializeField]
        private Transform _trackingTransform;

        private float _xRectScale;
        private float _yRectScale;

        private void Awake()
        {
            RectTransform _mapTransform = GetComponent<RectTransform>();

            _xRectScale = _mapTransform.rect.width / _bottomRightWorldTransform.position.z;
            _yRectScale = _mapTransform.rect.height / _bottomRightWorldTransform.position.x;
        }

        private void LateUpdate()
        {
            _mapIndicator.anchoredPosition =
                new Vector2
                    (
                        _trackingTransform.position.z * _xRectScale,
                        _trackingTransform.position.x * _yRectScale * -1
                    );
        }
    }
}