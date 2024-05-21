using UnityEngine;
using System;
using System.Collections.Generic;
public class PlayerData : ScriptableObject
{
    public List<PlayerDataCore> PlayerDataCoreList = new List<PlayerDataCore>();
}
[System.Serializable]
public class PlayerDataCore
{
    public int player_health;
    [Header("玩家最大生命值")]
    public int player_max_health;
    public int player_magicpoint;
    [Header("玩家最大意能")]
    public int player_max_magicpoint;
    [Header("玩家最大跳跃力")]
    public float player_jump_force_value;
    [Header("玩家最大移动速度")]
    public float player_runspeed_value;
    [Header("玩家等级")]
    public int player_level;
}