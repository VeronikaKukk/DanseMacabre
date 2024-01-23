using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public static event Action<string, int> OnTurnStart;
    public static void TurnStart(string nextPlayerName, int turnStage) => OnTurnStart?.Invoke(nextPlayerName, turnStage);

    public static event Action<string> OnEndGame;
    public static void EndGame(string winner) => OnEndGame?.Invoke(winner);

    public static event Action OnStartCountingPoints;
    public static void StartCountingPoints() => OnStartCountingPoints?.Invoke();
}
