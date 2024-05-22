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
        /// <summary>
        /// A method that sends players to the <see cref="GameState"/>.
        /// </summary>
        [ObserversRpc(RunLocally = true)]
        public void StartGame() => StateMachine.StateMachine.Instance.GoToState<GameState>();

        /// <summary>
        /// A method to end the game and reload the scene.
        /// </summary>
        [ObserversRpc(RunLocally = true)]
        public void EndGame() => StateMachine.StateMachine.Instance.GoToState<ScoreState>();
    }
}