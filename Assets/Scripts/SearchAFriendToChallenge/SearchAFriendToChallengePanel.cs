using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
  * @brief This class handles the panel that shows the current user's
  * friends that allow be challenged.  
  * It inherites from adaptableSizePanel abstract class.
  */
public class SearchAFriendToChallengePanel : adaptableSizePanel
{
    /**
      * @brief This method is called to instanciate the panel. It creates a prefab for each challengeable friend
      * that the current user has. It changes the panelFilled_ property to true.
      */
    protected override void fillPanel(){
        if(FriendData.usersThatAllowBeChallenged_ != null){
          for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.currentUser_.countOfFriendData(); i++){
            FriendData friendData = firebaseHandler.firebaseHandlerInstance_.currentUser_.getFriendData(i);

            // si el usuario esta en la lista de usuarios que admiten ser retados y no tiene un reto del usuario actual
            if(FriendData.usersThatAllowBeChallenged_.Exists(uid => uid == friendData.getUid()) &&
                !friendData.hasAChallengeOfThisUser(firebaseHandler.firebaseHandlerInstance_.currentUser_.getUid()) ){
                  GameObject newChanllegeAFriendObject = Instantiate(prefab_, new Vector3(0, 0, 0), Quaternion.identity);
                  newChanllegeAFriendObject.GetComponent<Friend>().setData(friendData);
                  newChanllegeAFriendObject.transform.SetParent(this.transform);
                  newChanllegeAFriendObject.GetComponent<Friend>().setPanel(this.gameObject);
                  items_.Add(newChanllegeAFriendObject);
            }
          }
          //si la lista no se habia descargado el panel sigue sin haberse llenado
          panelFilled_ = true;
        }
    }
}
