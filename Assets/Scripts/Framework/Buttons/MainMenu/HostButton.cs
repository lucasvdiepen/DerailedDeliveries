using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.Buttons.MainMenu
{
    /// <summary>
    /// A button that is responsible for transitioning to the host state.
    /// </summary>
    public class HostButton : BasicButton
    {
        private protected override void OnButtonPressed() => StateMachine.StateMachine.Instance.GoToState<HostState>();
    }
}