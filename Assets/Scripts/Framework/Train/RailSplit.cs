using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Class responsible for splitting rail track.
/// </summary>
public class RailSplit : MonoBehaviour
{
    [field: SerializeField]
    public SplineContainer[] PossibleTracks { get; set; }

    private void Awake()
    {
        if (PossibleTracks.Length > 2)
            Debug.LogWarning("This RailSplit contains more than two possible tracks", this);
    }
}
