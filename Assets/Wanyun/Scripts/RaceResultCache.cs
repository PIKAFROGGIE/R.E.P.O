using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public static class RaceResultCache
{
    // Final ranking for the current round
    public static List<Player> FinalRanking;

    // Current round number
    public static int CurrentRound = 1;
}
