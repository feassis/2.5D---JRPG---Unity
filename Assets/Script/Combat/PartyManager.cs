using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField]private List<PartyMemberInfo> allMembers = new List<PartyMemberInfo>();
    private List<PartyMember> currentParty = new List<PartyMember>();

    [SerializeField] private PartyMemberInfo defaultPartyMember;

    private void Awake()
    {
        AddMemberToPartyByName(defaultPartyMember.MemberName);
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
    public float Strength;
    public float Initiative;
    public float CurrentExp;
    public float MaxExp;
    public GameObject MemberBattleVisualPrefab;
    public GameObject MemberOverworldVisualPrefab;
}
