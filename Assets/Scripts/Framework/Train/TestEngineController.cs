using FishNet.Object;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Test class responsible for debugging train engine functionality.
    /// </summary>
    public class TestEngineController : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera CinemachineVirtualCamera;
        [SerializeField] private Animator animator;

        private void Update()
        {
            if (Keyboard.current.backspaceKey.wasPressedThisFrame) TrainEngine.Instance.ToggleTrainDirection();
            if (Keyboard.current.homeKey.wasPressedThisFrame) TrainEngine.Instance.ToggleEngineState();

            if (Keyboard.current.equalsKey.wasPressedThisFrame) TrainEngine.Instance.AdjustSpeed(true);
            if (Keyboard.current.minusKey.wasPressedThisFrame) TrainEngine.Instance.AdjustSpeed(false);

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                CameraController.Instance.ChangeActiveCamera(CinemachineVirtualCamera);
                animator.SetTrigger("Enter");
            }
        }
    }
}
