using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows the new friendships invitations.
  * It inherites from adaptableSizePanel abstract class.
  */
public class newFriendInvitationPanel : adaptableSizePanel
{
    /**
      * This method is called to instanciate the panel. It creates a prefab for each new friendship invitation
      * that the current user has. It changes the panelFilled_ property to true.
      */
    protected override void fillPanel(){
      for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.currentUser_.countOfNewFriendData(); i++){
        GameObject newInvitationObject = Instantiate(prefab_, new Vector3(0, 0, 0), Quaternion.identity);
        newInvitationObject.GetComponent<newFriendInvitation>().setData(firebaseHandler.firebaseHandlerInstance_.currentUser_.getNewFriendData(i));
        newInvitationObject.transform.SetParent(this.transform);
        newInvitationObject.GetComponent<newFriendInvitation>().setPanel(this.gameObject);
        items_.Add(newInvitationObject);
      }
      panelFilled_ = true;
    }
}
