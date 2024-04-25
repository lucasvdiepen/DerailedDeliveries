using UnityEngine;
using DG.Tweening;

using DerailedDeliveries.Framework.UI.TextUpdaters;
using DerailedDeliveries.Framework.Gameplay.Timer;

/// <summary>
/// A TextUpdater class that is responsible for updating the <see cref="TimerUpdater"/>'s text.
/// </summary>
public class TimerTextUpdater : TextUpdater
{
    [SerializeField]
    private Color _chaosColor;

    [SerializeField]
    private Color _baseColor;

    [SerializeField]
    private float _chaosColorFadeDuration = 1f;

    private bool _switchedColor;

    private void OnEnable()
    {
        TimerUpdater.Instance.OnTimerUpdated += UpdateText;

        UpdateText(TimerUpdater.Instance.TimeRemaining);
    }

    private void OnDisable()
    {
        if(TimerUpdater.Instance != null)
            TimerUpdater.Instance.OnTimerUpdated -= UpdateText;
    }

    private void UpdateText(float newTime)
    {
        if(newTime < TimerUpdater.Instance.ChaosSpeedMultiplierThreshold && !_switchedColor)
        {
            DOTween.To
            (
                () => _baseColor,
                (Color newColor) => { Text.color = newColor; },
                _chaosColor,
                _chaosColorFadeDuration
            );

            _switchedColor = true;
        }
        else if(newTime > TimerUpdater.Instance.ChaosSpeedMultiplierThreshold)
        {
            Text.color = _baseColor;

            _switchedColor = false;
        }

        int minutes = (int)(newTime / 60);
        int seconds = (int)(newTime % 60);
        int milliseconds = (int)(100 * (newTime % 1));

        string timerText 
            = GetIntString(minutes) 
            + "\u00A0 : \u00A0"
            + GetIntString(seconds) 
            + "\u00A0 : \u00A0"
            + GetIntString(milliseconds);

        ReplaceTag(timerText);
    }

    private string GetIntString(int number)
    {
        return number < 10
            ? "0" + number
            : number.ToString();
    }
}