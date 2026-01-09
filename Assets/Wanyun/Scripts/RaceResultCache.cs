using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public static class RaceResultCache
{
    // 当前轮的最终排名
    public static List<Player> FinalRanking;

    // 当前是第几轮
    public static int CurrentRound = 1;
}
