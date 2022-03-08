using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief This class controls the panel that shows the stored places of
  * the current user when he is choosing the place for sending a challenge.
  * It inherites from adaptableSizePanel abstract class.
  */
public class chooseAPlaceToSendAsAChallengePanel : adaptableSizePanel
{
  /**
    * This method is called to instanciate the panel. It creates a prefab for each place that
    * that the current user has stored. It changes the panelFilled_ property to true.
    */
  protected override void fillPanel(){
    for(int i = 0; i < gameRules.getMaxPlacesStored(); i++){
      if(StoredPlace.thereIsAPlaceStoredIn(i)){
        GameObject placeObject = Instantiate(prefab_, new Vector3(0, 0, 0), Quaternion.identity);
        placeObject.GetComponent<storedPlaceToSend>().setData(StoredPlace.loadStoredPlace(i));
        placeObject.transform.SetParent(this.transform);
        placeObject.GetComponent<storedPlaceToSend>().setPanel(this.gameObject);
        items_.Add(placeObject);
      }
    }
    panelFilled_ = true;
  }
}
