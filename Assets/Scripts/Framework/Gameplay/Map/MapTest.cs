using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    [Header("World Transforms")]
    [SerializeField] 
    private Transform _bottomRightWorld;

    [Header("Map Transforms")]
    [SerializeField]
    private Transform _bottomRightMap;

    [SerializeField]
    private Transform _vectorZeroMap;

    [Header("Map Settings")]
    [SerializeField]
    private Transform _mapIndicator;

    [SerializeField]
    private Transform _trackingTransform;

    [SerializeField]
    private Vector3 _offset;

    [SerializeField]
    private float _xRectScale;

    [SerializeField]
    private float _yRectScale;

    void Update()
    {
        _xRectScale = _bottomRightMap.position.x / _bottomRightWorld.position.z;
        _yRectScale = _bottomRightMap.position.y / _bottomRightWorld.position.x;

        _mapIndicator.position = _vectorZeroMap.position;

        _mapIndicator.localPosition += 
            new Vector3
                (
                    _trackingTransform.position.z * _xRectScale,
                    _trackingTransform.position.x * _yRectScale,
                    0
                ) + _offset;
    }
}
