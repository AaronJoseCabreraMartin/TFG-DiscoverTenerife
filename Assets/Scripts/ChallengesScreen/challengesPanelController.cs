using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows all the challenges
  * that the current user has, it also controls the image that will be
  * shown when the user doesnt have any challenge. It inherites from 
  * adaptableSizePanel abstract class.
  */
public class challengesPanelController : adaptableSizePanel
{
    /**
      * This method is called to instanciate the panel. It creates a prefab for each new challenge invitation
      * that the current user has. It changes the panelFilled_ property to true.
      */
    protected override void fillPanel(){
      for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.currentUser_.getQuantityOfChallenges(); i++){
        GameObject challengeObject = Instantiate(prefab_, new Vector3(0, 0, 0), Quaternion.identity);
        challengeObject.GetComponent<challengePrefabController>().setChallengeData(firebaseHandler.firebaseHandlerInstance_.currentUser_.getChallenge(i));
        challengeObject.transform.SetParent(this.transform);
        challengeObject.GetComponent<challengePrefabController>().setPanel(this.gameObject);
        items_.Add(challengeObject);
      }
      panelFilled_ = true;
    }
}
