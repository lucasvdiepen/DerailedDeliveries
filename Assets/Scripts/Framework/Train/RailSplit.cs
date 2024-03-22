using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Class responsible for splitting rail track.
/// </summary>
public class RailSplit : MonoBehaviour
{
    [SerializeField]
    private SplineContainer[] possibleWays = null;

    public SplineContainer GetRandomWay()
    {
        int randIndex = Random.Range(0, possibleWays.Length);
        return possibleWays[randIndex];
    }
}
