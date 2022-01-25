using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const float timePerMove = 1.0f;
    public float timeSinceEndTurn { get; private set; } = -1f;

    private int maxActions = 0;

    public void EndTurn()
    {
        BroadcastMessage("OnEndTurn", null, SendMessageOptions.DontRequireReceiver);
        StartCoroutine(EndTurnCounter(timePerMove * maxActions));
    }


    /// <summary>
    /// Sets the max actions for the next turn. This will linearly increase how long the endphase will be.
    /// </summary>
    /// <param name="nActions">Max number of actions taken at end of turn</param>
    public void SetMaxActions(int nActions)
    {
        maxActions = nActions > maxActions ? nActions : maxActions;
    }


    /// <summary>
    /// Helper function to broadcast "OnStartTurn" after endphase duration.
    /// </summary>
    /// <param name="duration">Duration of endphase</param>
    /// <returns>generator for coroutine</returns>
    private IEnumerator EndTurnCounter(float duration)
    {
        timeSinceEndTurn = 0;
        while (timeSinceEndTurn <= duration)
        {
            timeSinceEndTurn += Time.deltaTime;
            yield return null;
        }
        BroadcastMessage("OnStartTurn", null, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Receiver to OnStartTurn message. Resets max actions for the next turn.
    /// </summary>
    void OnStartTurn()
    {
        maxActions = 0;
    }
}
