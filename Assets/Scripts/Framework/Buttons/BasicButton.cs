using UnityEngine;
using UnityEngine.UI;

namespace DerailedDeliveries.Framework.Buttons
{
    /// <summary>
    /// An abstract class that is responsible for handling the button click event.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class BasicButton : MonoBehaviour
    {
        private Button _button;

        private Image _image;

        private protected Button Button => _button;

        private protected Image Image => _image;

        private protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        private protected virtual void OnEnable() => _button.onClick.AddListener(OnButtonPressed);

        private protected virtual void OnDisable() => _button.onClick.RemoveListener(OnButtonPressed);

        /// <summary>
        /// The method that is called when the button is pressed.
        /// </summary>
        private protected abstract void OnButtonPressed();
    }
}