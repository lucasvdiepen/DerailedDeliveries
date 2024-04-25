using UnityEngine;

using DerailedDeliveries.Framework.UI.TextUpdaters;
using DerailedDeliveries.Framework.Gameplay.Timer;

/// <summary>
/// A TextUpdater class that is responsible for updating the <see cref="TimerUpdater"/>'s text.
/// </summary>
public class TimerTextUpdater : TextUpdater
{
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
        int minutes = (int)(newTime / 60);
        int seconds = (int)(newTime % 60);
        int milliseconds = (int)(100 * (newTime % 1));

        string timerText 
            = GetIntString(minutes) 
            + " : "
            + GetIntString(seconds) 
            + " : "
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