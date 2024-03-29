using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemyInfo> allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;

    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        GenerateEnemyByName("Slime", 1);
    }

    private void GenerateEnemyByName(string enemyName, int level)
    {
        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i].EnemyName == enemyName)
            {
                Enemy enemy = new Enemy();
                enemy.EnemyName = allEnemies[i].EnemyName;
                enemy.Level = level;
                float levelModifier = (LEVEL_MODIFIER * level);

                enemy.MaxHealth = allEnemies[i].BaseHealth*(levelModifier + 1);
                enemy.CurrentHealth = enemy.MaxHealth;

                enemy.Strength = Mathf.RoundToInt(allEnemies[i].BaseStr * (levelModifier + 1));
                enemy.Initiative = Mathf.RoundToInt(allEnemies[i].BaseInitiative * (levelModifier + 1));

                enemy.EnemyBattleVisualPrefab = allEnemies[i].EnemyBattleVisualPrefab;

                currentEnemies.Add(enemy);
            }
        }
    }
}

public class Enemy
{
    public string EnemyName;
    public float MaxHealth;
    public float CurrentHealth;
    public int Level;
    public int Strength;
    public int Initiative;
    public GameObject EnemyBattleVisualPrefab;
}
