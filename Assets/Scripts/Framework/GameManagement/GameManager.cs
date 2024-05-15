using FishNet.Object;

using DerailedDeliveries.Framework.StateMachine.States;
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
    }
}