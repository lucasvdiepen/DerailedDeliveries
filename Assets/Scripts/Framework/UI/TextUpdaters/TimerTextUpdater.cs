using UnityEngine;
using DG.Tweening;

using DerailedDeliveries.Framework.Gameplay.Timer;

namespace DerailedDeliveries.Framework.UI.TextUpdaters
{
    /// <summary>
    /// A TextUpdater class that is responsible for updating the <see cref="TimerUpdater"/>'s text.
    /// </summary>
    public class TimerTextUpdater : TextUpdater
    {
        [Header("Text Updaters")]
        [SerializeField]
        private TextUpdater _minutesText;

        [SerializeField]
        private TextUpdater _secondsText;

        [SerializeField]
        private TextUpdater _millisecondsText;

        [Header("Timer settings")]
        [SerializeField]
        private Color _chaosColor;

        [SerializeField]
        private Color _baseColor;

        [SerializeField]
        private float _colorFadeDuration = 1f;

        private bool _isChaosColor;
        private bool _isSwitchingColor;

        private void OnEnable()
        {
            TimerUpdater.Instance.OnTimerUpdated += UpdateText;

            UpdateText(TimerUpdater.Instance.TimeRemaining);
        }

        private void OnDisable()
        {
            if (TimerUpdater.Instance != null)
                TimerUpdater.Instance.OnTimerUpdated -= UpdateText;
        }

        private void UpdateText(float newTime)
        {
            if (newTime < TimerUpdater.Instance.ChaosSpeedMultiplierThreshold && !_isChaosColor)
            {
                SwitchColor(Text.color, _chaosColor);

                _isChaosColor = true;
            }
            else if (newTime > TimerUpdater.Instance.ChaosSpeedMultiplierThreshold && _isChaosColor)
            {
                SwitchColor(Text.color, _baseColor);

                _isChaosColor = false;
            }

            int minutes = (int)(newTime / 60);
            int seconds = (int)(newTime % 60);
            int milliseconds = (int)(100 * (newTime % 1));

            _minutesText.ReplaceTag(GetIntString(minutes));
            _secondsText.ReplaceTag(GetIntString(seconds));
            _millisecondsText.ReplaceTag(GetIntString(milliseconds));
        }

        private void SwitchColor(Color baseColor, Color newColor)
        {
            if (_isSwitchingColor)
                return;

            _isSwitchingColor = true;

            DOTween.To
                (
                    () => baseColor,
                    (Color tweenColor) => 
                    { 
                        Text.color = tweenColor;
                        _secondsText.Text.color = tweenColor;
                        _minutesText.Text.color = tweenColor;
                        _millisecondsText.Text.color = tweenColor;
                    },
                    newColor,
                    _colorFadeDuration
                ).OnComplete(() => { _isSwitchingColor = false; });
        }

        private string GetIntString(int number)
        {
            return number < 10
                ? "0" + number
                : number.ToString();
        }
    }
}