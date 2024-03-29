using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "New Enemy")]
public class EnemyInfo : ScriptableObject
{
    public string EnemyName;

    public float BaseHealth;
    public int BaseStr;
    public int BaseInitiative;
    public GameObject EnemyBattleVisualPrefab;
}
