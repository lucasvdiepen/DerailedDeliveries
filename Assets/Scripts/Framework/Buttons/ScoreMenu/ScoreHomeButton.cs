using FishNet;
using UnityEngine.SceneManagement;

namespace DerailedDeliveries.Framework.Buttons.ScoreMenu
{
    /// <summary>
    /// A <see cref="BasicButton"/> that is responsible for leaving the score menu.
    /// </summary>
    public class ScoreHomeButton : BasicButton
    {
        private protected override void OnButtonPressed()
        {
            InstanceFinder.ServerManager.StopConnection(true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}