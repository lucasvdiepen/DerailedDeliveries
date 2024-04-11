using Cinemachine;
using DerailedDeliveries.Framework.Utils;
using UnityEngine;

/// <summary>
/// Class responsible for controlling and switching between virtual cameras.
/// </summary>
public class CameraController : AbstractSingleton<CameraController>
{
    [SerializeField]
    private CinemachineVirtualCamera[] _cinemachineVirtualCameras;

    /// <summary>
    /// Method to change priority to a specific camera, disables all other cameras.
    /// </summary>
    /// <param name="targetCamera"></param>
    public void ChangeActiveCamera(CinemachineVirtualCamera targetCamera)
    {
        int cameras = _cinemachineVirtualCameras.Length;

        for (int i = 0; i < cameras; i++)
        {
            _cinemachineVirtualCameras[i].Priority = 0;
        }

        targetCamera.Priority = 1;
    }
}
