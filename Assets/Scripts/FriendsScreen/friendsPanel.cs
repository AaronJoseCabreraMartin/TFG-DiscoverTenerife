using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows the friends of the current user.
  * It inherites from adaptableSizePanel abstract class.
  */  
public class friendsPanel : adaptableSizePanel
{
  /**
    * This method instantiate a new prefab_ for each friend of the current user and it stores
    * it on the friends_ list. It also change the value of the property panelFilled_ to true. 
    */
  protected override void fillPanel(){
    for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.currentUser_.countOfFriendData(); i++){
      GameObject friendObject = Instantiate(prefab_, new Vector3(0, 0, 0), Quaternion.identity);
      friendObject.GetComponent<Friend>().setData(firebaseHandler.firebaseHandlerInstance_.currentUser_.getFriendData(i));
      friendObject.transform.SetParent(this.transform);
      friendObject.GetComponent<Friend>().setPanel(this.gameObject);
      items_.Add(friendObject);
    }
    panelFilled_ = true;
  }
}
