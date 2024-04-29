using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinableCharacterScript : MonoBehaviour
{
    public PartyMemberInfo MemberToJoin;
    [SerializeField] private GameObject interactPrompt;

    // Start is called before the first frame update
    void Start()
    {
        CheckIfJoined();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowInteractPrompt(bool showPrompt)
    {
        interactPrompt.SetActive(showPrompt);
    }

    public void CheckIfJoined()
    {
        List<PartyMember> currParty = GameObject.FindFirstObjectByType<PartyManager>().GetCurrentParty();

        for (int i = 0; i < currParty.Count; i++)
        {
            if (currParty[i].MemberName == MemberToJoin.MemberName)
            {
                gameObject.SetActive(false);
            }
        }
    }


}
