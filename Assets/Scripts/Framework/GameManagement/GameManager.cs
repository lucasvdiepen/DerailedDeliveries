using FishNet.Object;

using DerailedDeliveries.Framework.StateMachine.States;
using DerailedDeliveries.Framework.StateMachine;
using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.GameManagement
{
    /// <summary>
    /// A class responsible for managing the game.
    /// </summary>
    public class GameManager : NetworkAbstractSingleton<GameManager>
    {
        [ObserversRpc(RunLocally = true)]
        public void StartGame() => StateMachine.StateMachine.Instance.GoToState<GameState>();

        [ObserversRpc(RunLocally = true)]
        public void EndGame()
        {
            UnityEngine.SceneManagement.SceneManager.
                LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}