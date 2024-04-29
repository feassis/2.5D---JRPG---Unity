using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private enum BattleState { Start, Selection, Battle, Won, Lost, Run }

    [Header("Battle State")]
    [SerializeField] private BattleState state;


    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playersBattlers = new List<BattleEntities>();

    [Header("References")]
    private PartyManager partyManager;
    private EnemyManager enemyManager;

    [Header("UI")]
    [SerializeField] private Button[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button runButton;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;

    private int currentPlayer;
    private const string ACTION_TEXT = "'s Action";
    private const string WIN_MESSAGE = "Your party won the battle";
    private const string LOSE_MESSAGE = "Your party has been defeated";
    private const string OVERWORLD_SCENE = "OverworldScene";
    private const string SUCCESFULLY_RAN_MESSAGE = "Your party ran away";
    private const string UNSUCCESFULLY_RAN_MESSAGE = "Your party failed to run";
    private const int RUN_CHANCE = 50;
    private const int TURN_DURATION = 2;


    private void Start()
    {
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        CreatePartyEntities();
        CreateEnemyEnetities();

        ShowBattleMenu();
        DetermineBattleOrder();

        attackButton.onClick.AddListener(ShowEnemySelectionMenu);
        runButton.onClick.AddListener(SelectRunAction);
    }

    private void CreatePartyEntities()
    {
        var partyMembers = partyManager.GetAliveParty();

        int index = 0;
        foreach (var member in partyMembers)
        {
            BattleEntities battleEntity = new BattleEntities();

            battleEntity.SetEntityValues(member.MemberName, member.CurrentHealth, member.MaxHealth, member.Strength,
                member.Initiative, member.Level, true);

            BattleVisuals memberVisuals = Instantiate(member.MemberBattleVisualPrefab, 
                partySpawnPoints[index].position, Quaternion.identity).GetComponent<BattleVisuals>();

            memberVisuals.SetStartingValues(battleEntity.CurHP, battleEntity.MaxHP, battleEntity.Level);
            battleEntity.BattleVisuals = memberVisuals;

            allBattlers.Add(battleEntity);
            playersBattlers.Add(battleEntity);
            index++;
        }
    }

    private void CreateEnemyEnetities()
    {
        var enemies = enemyManager.GetCurrentEnemies();

        int index = 0;
        foreach (var enemy in enemies)
        {
            BattleEntities battleEntity = new BattleEntities();
            battleEntity.SetEntityValues(enemy.EnemyName, enemy.CurrentHealth, enemy.MaxHealth, enemy.Strength,
            enemy.Initiative, enemy.Level, false);

            BattleVisuals enemyVisuals = Instantiate(enemy.EnemyBattleVisualPrefab,
                enemySpawnPoints[index].position, Quaternion.identity).GetComponent<BattleVisuals>();

            enemyVisuals.SetStartingValues(battleEntity.CurHP, battleEntity.MaxHP, battleEntity.Level);
            battleEntity.BattleVisuals = enemyVisuals;

            allBattlers.Add(battleEntity);
            enemyBattlers.Add(battleEntity);
            index++;
        }
    }

    public void ShowBattleMenu()
    {
        actionText.text = playersBattlers[currentPlayer].Name + ACTION_TEXT;
        battleMenu.SetActive(true);
        enemySelectionMenu.SetActive(false);
    }

    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);

    }

    private void SetEnemySelectionButtons()
    {
        for (int i = 0; i < enemySelectionButtons.Length; i++)
        {
            enemySelectionButtons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            enemySelectionButtons[i].gameObject.SetActive(true);
            enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].Name;
        }
    }

    public void SelectEnemy(int currentEnemy)
    {
        BattleEntities currentPlayerEntity = playersBattlers[currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

        currentPlayerEntity.BattleAction = BattleEntities.Action.Attack;
        currentPlayer++;

        if (currentPlayer >= playersBattlers.Count) //if all players have selected an action
        {
            //Start The Battle
            StartCoroutine(BattleRoutine());
        }
        else
        {
            enemySelectionMenu.SetActive(false);  // show the battle menu for the next player
            ShowBattleMenu();
        }
    }

    private void AttackAction(BattleEntities currAttacker, BattleEntities currTarget)
    {
        int damage = currAttacker.Strength; //get damage (can use an algorithm)
        currAttacker.BattleVisuals.PlayAttackAnim(); // play the attack animation
        currTarget.CurHP -= damage; // dealing the damage
        currTarget.BattleVisuals.PlayIsHitAnim(); // play their hit anim
        currTarget.UpdateUI(); // update the UI
        bottomText.text = string.Format("{0} attacks {1} for {2} damage", currAttacker.Name, currTarget.Name, damage);

        SaveHealth();
    }


    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false); // enemy selection menu disabled
        state = BattleState.Battle; // change our state to the battle state
        bottomTextPopUp.SetActive(true); //enable our bottom text

        //loop through all our battlers
        //-> do their approriate action

        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (state == BattleState.Battle && allBattlers[i].CurHP > 0)
            {
                switch (allBattlers[i].BattleAction)
                {
                    case BattleEntities.Action.Attack:
                        // do the attack
                        yield return StartCoroutine(AttackRoutine(i));
                        break;
                    case BattleEntities.Action.Run:
                        yield return RunRoutine();
                        break;
                    default:
                        Debug.Log("Error - incorrect battle action");
                        break;
                }
            }
            

        }

        RemoveDeadBattlers();

        if (state == BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
            currentPlayer = 0;
            ShowBattleMenu();

        }

        yield return null;
        // if we havent won or lost, repeat the loop by opening the battle menu
    }

    private IEnumerator AttackRoutine(int i)
    {

        // players turn
        if (allBattlers[i].IsPlayer == true)
        {
            BattleEntities currAttacker = allBattlers[i];
            if (allBattlers[currAttacker.Target].IsPlayer == true || currAttacker.Target >= allBattlers.Count)
            {
                currAttacker.SetTarget(GetRandomEnemy());
            }

            BattleEntities currTarget = allBattlers[currAttacker.Target];
            AttackAction(currAttacker, currTarget); // attack selected enemy (attack action)
            yield return new WaitForSeconds(TURN_DURATION);// wait a few seconds

            // kill the enemy
            if (currTarget.CurHP <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);// wait a few seconds
                enemyBattlers.Remove(currTarget);;

                if (enemyBattlers.Count <= 0)
                {
                    state = BattleState.Won;
                    bottomText.text = WIN_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);// wait a few seconds
                    SceneManager.LoadScene(OVERWORLD_SCENE);
                }
            }
            // if no enemies remain
            // -> we won the battle
        }



        //enemies turn
        if (i < allBattlers.Count && allBattlers[i].IsPlayer == false)
        {
            BattleEntities currAttacker = allBattlers[i];
            currAttacker.SetTarget(GetRandomPartyMember());// get random party member (target)
            BattleEntities currTarget = allBattlers[currAttacker.Target];

            AttackAction(currAttacker, currTarget);// attack selected party member (attack action)
            yield return new WaitForSeconds(TURN_DURATION);// wait a few seconds

            if (currTarget.CurHP <= 0)
            {
                // kill the party member
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);// wait a few seconds
                playersBattlers.Remove(currTarget);

                if (playersBattlers.Count <= 0) // if no party members remain
                {
                    // -> we lost the battle
                    state = BattleState.Lost;
                    bottomText.text = LOSE_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);// wait a few seconds
                    Debug.Log("Game Over");
                }

            }
        }
    }

    private IEnumerator RunRoutine()
    {
        if (state == BattleState.Battle)
        {
            if (UnityEngine.Random.Range(1, 101) >= RUN_CHANCE)
            {
                // we have ran away

                bottomText.text = SUCCESFULLY_RAN_MESSAGE;// set our bottom text to tell us we ran away
                state = BattleState.Run;// set our battle state to run
                allBattlers.Clear();// clear our all battlers list
                yield return new WaitForSeconds(TURN_DURATION);// wait a few seconds
                SceneManager.LoadScene(OVERWORLD_SCENE);// load the overworld scene
                yield break;
            }
            else
            {
                // we failed to run away
                bottomText.text = UNSUCCESFULLY_RAN_MESSAGE; // set our bottom text to say we failed
                yield return new WaitForSeconds(TURN_DURATION); // wait a few seconds
            }

        }
    }

    private void RemoveDeadBattlers()
    {
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].CurHP <= 0)
            {
                allBattlers.RemoveAt(i);
            }
        }
    }


    public void SelectRunAction()
    {
        state = BattleState.Selection;
        // setting the current members target
        BattleEntities currentPlayerEntity = playersBattlers[currentPlayer];

        //tell the battle system we intend to run
        currentPlayerEntity.BattleAction = BattleEntities.Action.Run;

        battleMenu.SetActive(false);
        // increment through our party members
        currentPlayer++;

        if (currentPlayer >= playersBattlers.Count) //if all players have selected an action
        {
            //Start The Battle
            StartCoroutine(BattleRoutine());

        }
        else
        {
            enemySelectionMenu.SetActive(false);  // show the battle menu for the next player
            ShowBattleMenu();
        }


    }


    private int GetRandomPartyMember()
    {
        List<int> partyMembers = new List<int>(); // create a temporary list of type int (index)
        // find all the party members -> add them to our list
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer == true && allBattlers[i].CurHP > 0) // we have a party member
            {
                partyMembers.Add(i);
            }
        }
        return partyMembers[UnityEngine.Random.Range(0, partyMembers.Count)];// return a random party member
    }

    private int GetRandomEnemy()
    {
        List<int> enemies = new List<int>();
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer == false && allBattlers[i].CurHP > 0)
            {
                enemies.Add(i);
            }
        }
        return enemies[UnityEngine.Random.Range(0, enemies.Count)];
    }

    private void SaveHealth()
    {
        for (int i = 0; i < playersBattlers.Count; i++)
        {
            partyManager.SaveHealth(i, playersBattlers[i].CurHP);
        }
    }

    private void DetermineBattleOrder()
    {
        allBattlers.Sort((bi1, bi2) => -bi1.Initiative.CompareTo(bi2.Initiative)); // sorts list by initiative in ascending order.
    }

}

[Serializable]
public class BattleEntities
{
    public enum Action
    {
        Attack = 0,
        Run = 1
    }

    public Action BattleAction;

    public string Name;
    public float CurHP;
    public float MaxHP;
    public int Strength;
    public int Initiative;
    public int Level;
    public bool IsPlayer;
    public int Target;

    public BattleVisuals BattleVisuals;

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

    public void SetTarget(int target)
    {
        Target = target;
    }

    public void UpdateUI()
    {
        BattleVisuals.ChangeHealth(CurHP);
    }
}