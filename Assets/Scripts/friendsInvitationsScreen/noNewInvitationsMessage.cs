using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noNewInvitationsMessage : MonoBehaviour
{
    [SerializeField] private GameObject newFriendInvitationPanel_;
    [SerializeField] private GameObject image_;
    [SerializeField] private GameObject text_;

    // Update is called once per frame
    void Update()
    {
        bool show = newFriendInvitationPanel_.GetComponent<newFriendInvitationPanel>().getInvitationsCount() == 0;
        image_.GetComponent<Image>().enabled = show;
        text_.SetActive(show);
    }
}
