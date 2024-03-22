using UnityEngine;
using UnityEngine.InputSystem;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Test class responsible for debugging train engine functionality.
    /// </summary>
    [RequireComponent(typeof(TrainEngine))]
    public class TestEngineController : MonoBehaviour
    {
        private TrainEngine _trainEngine;

        private void Awake() => _trainEngine = GetComponent<TrainEngine>();

        private void Update()
        {
            if (Keyboard.current.wKey.wasPressedThisFrame) _trainEngine.AdjustSpeed(true);
            if (Keyboard.current.sKey.wasPressedThisFrame) _trainEngine.AdjustSpeed(false);
        }
    }
}
