using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Simply class that controls the no new invitations message of the friend invitations screen.
  */
public class noNewInvitationsMessage : MonoBehaviour
{
    /**
      * @brief GameObject that references the panel that handles the new friendships invitations.
      */
    [SerializeField] private GameObject newFriendInvitationPanel_;

    /**
      * @brief GameObject that contains the background image that will be showed when there isnt
      * any new friendship invitations.
      */
    [SerializeField] private GameObject image_;
    
    /**
      * @brief GameObject that contains the text that will be showed when there isnt
      * any new friendship invitations.
      */
    [SerializeField] private GameObject text_;

    /**
      * This method is called every frame, it shows the background image and the text if the 
      * getInvitationsCount method of newFriendInvitationPanel class returns a zero. It hide
      * them in other case.
      */
    void Update()
    {
        bool show = newFriendInvitationPanel_.GetComponent<newFriendInvitationPanel>().getInvitationsCount() == 0;
        image_.GetComponent<Image>().enabled = show;
        text_.SetActive(show);
    }
}
