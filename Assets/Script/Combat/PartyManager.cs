using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField]private List<PartyMemberInfo> allMembers = new List<PartyMemberInfo>();
    private List<PartyMember> currentParty = new List<PartyMember>();

    [SerializeField] private PartyMemberInfo defaultPartyMember;

    public List<PartyMember> GetCurrentParty()
    {
        List<PartyMember> aliveParty = new List<PartyMember>();
        aliveParty = currentParty;
        for (int i = 0; i < aliveParty.Count; i++)
        {
            if (aliveParty[i].CurrentHealth <= 0)
            {
                aliveParty.RemoveAt(i);
            }
        }
        return aliveParty;

    }

    private Vector3 playerPosition;
    private static GameObject instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
            AddMemberToPartyByName(defaultPartyMember.MemberName);
            AddMemberToPartyByName(defaultPartyMember.MemberName);
        }

        DontDestroyOnLoad(gameObject);

    }

    public void SaveHealth(int partyMember, float health)
    {
        currentParty[partyMember].CurrentHealth = health;
    }

    public void SetPosition(Vector3 position)
    {
        playerPosition = position;
    }

    public Vector3 GetPosition()
    {
        return playerPosition;
    }


    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMembers.Count; i++)
        {
            if (allMembers[i].name == memberName)
            {
                PartyMember partyMember = new PartyMember();

                partyMember.MemberName = allMembers[i].MemberName;
                partyMember.Level = allMembers[i].StartingLevel;
                partyMember.CurrentHealth = allMembers[i].BaseHealth;
                partyMember.MaxHealth = allMembers[i].BaseHealth;
                partyMember.Strength = allMembers[i].BaseStr;
                partyMember.Initiative = allMembers[i].BaseInitiative;
                partyMember.MemberBattleVisualPrefab = allMembers[i].MemberBattleVisualPrefab;
                partyMember.MemberOverworldVisualPrefab = allMembers[i].MemberOverworldVisualPrefab;

                currentParty.Add(partyMember);
            }
        }
    }
}

public class PartyMember
{
    public string MemberName;
    public int Level;
    public float CurrentHealth;
    public float MaxHealth;
    public int Strength;
    public int Initiative;
    public float CurrentExp;
    public float MaxExp;
    public GameObject MemberBattleVisualPrefab;
    public GameObject MemberOverworldVisualPrefab;
}
