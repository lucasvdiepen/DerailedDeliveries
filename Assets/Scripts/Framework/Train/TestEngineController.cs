using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Test class responsible for debugging train engine functionality.
    /// </summary>
    public class TestEngineController : NetworkBehaviour
    {
        private void Update()
        {
            if (!IsServer)
                return;

            if (Keyboard.current.wKey.wasPressedThisFrame) TrainEngine.Instance.AdjustSpeed(true);
            if (Keyboard.current.sKey.wasPressedThisFrame) TrainEngine.Instance.AdjustSpeed(false);

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) TrainEngine.Instance.CurrentSplitDirection = false;
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) TrainEngine.Instance.CurrentSplitDirection = true;
        }
    }
}
