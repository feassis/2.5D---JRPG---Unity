using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] PartyManager partyManager;
    [SerializeField] private EnemyManager enemyManager;

    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playersBattlers = new List<BattleEntities>();

    private void Start()
    {
        
    }

}

[Serializable]
public class BattleEntities
{
    public string Name;
    public float CurHP;
    public float MaxHP;
    public int Strength;
    public int Initiative;
    public int Level;
    public bool IsPlayer;

    public void SetEntityValues(string name, float curHP, float maxHP, 
        int strength, int initiative, int level, bool isPlayer)
    {
        Name = name;
        CurHP = curHP;
        MaxHP = maxHP;
        Strength = strength;
        Initiative = initiative;
        Level = level;
        IsPlayer = isPlayer;
    }
}