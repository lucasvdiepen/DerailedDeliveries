using UnityEngine;

namespace DerailedDeliveries.Framework.Buttons.MainMenu
{
    /// <summary>
    /// A <see cref="BasicButton"/> class which is responsible for closing the application.
    /// </summary>
    public class QuitButton : BasicButton
    {
        private protected override void OnButtonPressed() => Application.Quit();
    }
}