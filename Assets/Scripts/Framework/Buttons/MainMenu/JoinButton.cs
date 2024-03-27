using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.Buttons.MainMenu
{
    /// <summary>
    /// A button that is responsible for transitioning to the join state.
    /// </summary>
    public class JoinButton : BasicButton
    {
        private string test = "test";

        private protected override void OnButtonPressed() => StateMachine.StateMachine.Instance.GoToState<JoinState>();

        private void Start()
        {
            test = "test";

            if(test == "test")
            {
                test = "test";
            }
            else
            {
                test = "test";
            }


            return;
        }
    }
}