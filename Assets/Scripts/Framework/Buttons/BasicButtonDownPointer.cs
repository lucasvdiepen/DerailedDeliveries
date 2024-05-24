using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DerailedDeliveries.Framework.Buttons
{
    /// <summary>
    /// An abstract class that is responsible for handling the button click event.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class BasicButtonDownPointer : MonoBehaviour, IPointerDownHandler
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

        /// <summary>
        /// The method that is called when the button is pressed down.
        /// </summary>
        private protected abstract void OnButtonPointerDown(PointerEventData eventData);

        /// <summary>
        /// Called when the button is pressed down.
        /// </summary>
        /// <param name="eventData">Event data of the pointer down event.</param>
        public void OnPointerDown(PointerEventData eventData) => OnButtonPointerDown(eventData);
    }
}