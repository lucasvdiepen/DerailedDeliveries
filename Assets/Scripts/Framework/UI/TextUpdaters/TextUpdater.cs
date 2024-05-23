using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DerailedDeliveries.Framework.UI.TextUpdaters
{
    /// <summary>
    /// A class responsible for updating text with tags.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextUpdater : MonoBehaviour
    {
        [SerializeField]
        private string _tag;

        private TextMeshProUGUI _text;
        private string _defaultText;
        private Dictionary<string, string> _tags = new();

        /// <summary>
        /// A getter that retrieves this <see cref="TextUpdater"/>'s <see cref="TextMeshProUGUI"/> component.
        /// </summary>
        public TextMeshProUGUI Text => _text;

        private protected virtual void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _defaultText = _text.text;
        }

        /// <summary>
        /// Replaces a tag with the given text.
        /// </summary>
        /// <param name="tag">The tag to replace.</param>
        /// <param name="text">The text to replace the tag with.</param>
        public void ReplaceTag(string tag, string text)
        {
            if (_text == null)
                return;

            _tags[tag] = text;

            string defaultText = _defaultText;

            foreach ((string key, string value) in _tags)
                defaultText = defaultText.Replace(key, value);

            _text.text = defaultText;
        }

        /// <summary>
        /// Replaces the text of the default tag set in the inspector.
        /// </summary>
        /// <param name="text">The text to replace the tag with.</param>
        public void ReplaceTag(string text)
        {
            if (string.IsNullOrEmpty(_tag))
            {
                Debug.LogError("No tag set for TextUpdater");
                return;
            }

            ReplaceTag(_tag, text);
        }
    }
}