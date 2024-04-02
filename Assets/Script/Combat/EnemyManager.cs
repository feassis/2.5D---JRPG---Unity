using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemyInfo> allEnemies = new List<EnemyInfo>();
    [SerializeField] private List<Enemy> currentEnemies = new List<Enemy>();

    private static GameObject instance;
    private const float LEVEL_MODIFIER = 0.5f;

    public List<Enemy> GetCurrentEnemies() => currentEnemies;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
        }

        DontDestroyOnLoad(gameObject);

    }

    public void GenerateEnemiesByEncounter(Encounter[] encounters, int maxNumEnemies)
    {
        currentEnemies.Clear();
        int numEnemies = Random.Range(1, maxNumEnemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            Encounter tempEncounter = encounters[Random.Range(0, encounters.Length)];
            int level = Random.Range(tempEncounter.LevelMin, tempEncounter.LevelMax + 1);
            GenerateEnemyByName(tempEncounter.Enemy.EnemyName, level);
        }
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
