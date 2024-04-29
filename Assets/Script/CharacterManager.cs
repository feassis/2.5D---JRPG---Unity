using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject joinPopup;
    [SerializeField] private TextMeshProUGUI joinPopupText;


    private bool infrontOfPartyMember;
    private GameObject joinableMember;
    private PlayerControls playerControls;
    private List<GameObject> overworldCharacters = new List<GameObject>();

    private const string NPC_JOINABLE_TAG = "NPCJoinable";
    private const string PARTY_JOINED_MESSAGE = " Joined The Party!";

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    void Start()
    {
        playerControls.Player.Interact.performed += _ => Interact();
        SpawnOverworldMembers();
    }


    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Interact()
    {
        if (infrontOfPartyMember == true && joinableMember != null)
        {
            MemberJoined(joinableMember.GetComponent<JoinableCharacterScript>().MemberToJoin);//add member
            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(false);
            infrontOfPartyMember = false;
            joinableMember = null;
        }
    }

    private void MemberJoined(PartyMemberInfo partyMember)
    {
        GameObject.FindFirstObjectByType<PartyManager>().AddMemberToPartyByName(partyMember.MemberName);// add party member
        joinableMember.GetComponent<JoinableCharacterScript>().CheckIfJoined();// disable joinable member

        joinPopup.SetActive(true);
        joinPopupText.text = partyMember.MemberName + PARTY_JOINED_MESSAGE;

        SpawnOverworldMembers();// adding an overworld member
    }

    private void SpawnOverworldMembers()
    {
        for (int i = 0; i < overworldCharacters.Count; i++)
        {
            Destroy(overworldCharacters[i]);
        }
        overworldCharacters.Clear();

        List<PartyMember> currentParty = GameObject.FindFirstObjectByType<PartyManager>().GetCurrentParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            if (i == 0) // first member will be the player
            {
                GameObject player = gameObject; // get the player

                GameObject playerVisual = Instantiate(currentParty[i].MemberOverworldVisualPrefab,
                transform.position, Quaternion.identity); // spawn the member visual

                playerVisual.transform.SetParent(player.transform); // settting the parent to the player

                player.GetComponent<PlayerController>().SetOverworldVisuals(playerVisual.GetComponent<Animator>(),
                playerVisual.GetComponent<SpriteRenderer>(),
                playerVisual.transform.localScale); // assign the player controller values

                playerVisual.GetComponent<MemberFollowAI>().enabled = false;
                overworldCharacters.Add(playerVisual); // add the overworld character visual to the list
            }
            else // any other will be a follower
            {
                Vector3 positionToSpawn = transform.position;// get the followers spawn position
                positionToSpawn.x -= i;

                GameObject tempFollower = Instantiate(currentParty[i].MemberOverworldVisualPrefab,
                positionToSpawn, Quaternion.identity);// spawn the follower

                // set follow ai settings
                tempFollower.GetComponent<MemberFollowAI>().SetFollowDistance(i); // set follow ai settings
                overworldCharacters.Add(tempFollower); // add the follower visual to our list
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            //enable our prompt
            infrontOfPartyMember = true;
            joinableMember = other.gameObject;
            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            //disable our prompt
            infrontOfPartyMember = false;

            if(joinableMember == null)
            {
                return;
            }

            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(false);
            joinableMember = null;
        }
    }

}
