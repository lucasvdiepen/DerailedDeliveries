using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    [SerializeField]
    private RectTransform _mapIndicator;

    [SerializeField]
    private RectTransform _parentTransform;

    [SerializeField]
    private Transform _trackingTransform;

    [SerializeField]
    private float _scale = 10;

    private Vector3 _positionTranslateOffset;

    private void Awake()
    {
        _positionTranslateOffset = _parentTransform.position;
    }


    void Update()
    {
        _mapIndicator.position = _positionTranslateOffset;

        if (_trackingTransform == null)
            return;

        Vector3 translatedTrackingLocation = 
            new Vector3(_trackingTransform.position.x, _trackingTransform.position.z, 0) / _scale;

        _mapIndicator.position += translatedTrackingLocation;
    }
}
