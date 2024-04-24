using UnityEngine;

using DerailedDeliveries.Framework.UI.TextUpdaters;
using DerailedDeliveries.Framework.Gameplay.Timer;

/// <summary>
/// A TextUpdater class that is responsible for updating the <see cref="TimerUpdater"/>'s text.
/// </summary>
public class TimerTextUpdater : MonoBehaviour
{
    [SerializeField]
    private TextUpdater _milliSecondsText;

    [SerializeField]
    private TextUpdater _secondsText;

    [SerializeField]
    private TextUpdater _minutesText;

    private void OnEnable()
    {
        TimerUpdater.Instance.OnTimerUpdated += UpdateText;

        UpdateText(TimerUpdater.Instance.TimeRemaining);
    }

    private void OnDisable() => TimerUpdater.Instance.OnTimerUpdated -= UpdateText;

    private void UpdateText(float newTime)
    {
        if (!_milliSecondsText || !_secondsText || !_minutesText)
            return;

        int minutes = (int)(newTime / 60);
        int seconds = (int)(newTime % 60);
        int milliseconds = (int)(100 * (newTime % 1));

        _minutesText.ReplaceTag(GetIntString(minutes));
        _secondsText.ReplaceTag(GetIntString(seconds));
        _milliSecondsText.ReplaceTag(GetIntString(milliseconds));
    }

    private string GetIntString(int number)
    {
        return number < 10
            ? "0" + number
            : number.ToString();
    }
}