using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controls the image that is showed when there isnt any friend
  * on the friend list of the current user.
  */
public class noFriendsMessage : MonoBehaviour
{
    /**
      * @brief GameObject that reference the panel that shows the friends of the current user.
      */
    [SerializeField] private GameObject friendsPanel_;

    /**
      * @brief GameObject that contains the background image that is showed when there isnt any friend
      * on the friend list of the current user. 
      */
    [SerializeField] private GameObject image_;

    /**
      * @brief GameObject that contains the text that is showed when there isnt any friend
      * on the friend list of the current user. 
      */
    [SerializeField] private GameObject text_;

    /**
      * This method is called each frame, if the getInvitationsCount method of the class friendsPanel_
      * returns a zero it shows the image_ GameObject and the text_ GameObject. It hides them in other case.
      */
    void Update()
    {
        bool show = friendsPanel_.GetComponent<friendsPanel>().getInvitationsCount() == 0;
        image_.GetComponent<Image>().enabled = show;
        text_.SetActive(show);
    }
}
