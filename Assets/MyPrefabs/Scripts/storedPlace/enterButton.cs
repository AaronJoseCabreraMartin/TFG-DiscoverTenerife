using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that handles the enter button of a stored place. It chooses
  * the storedPlaceHandler that is on the parent of this button GameObject.
  */
public class enterButton : MonoBehaviour
{
    /**
      * @brief This class should be called when the user clicks on a stored place.
      * If there is a place stored on the index that has the storedPlaceHandler that
      * is on the parent of this GameObject, it calls the chooseStoredPlace
      * method of StoredPlacesController class with that index. Then, it changes
      * the scene.
      */
    public void OnClick(){
        if(StoredPlace.thereIsAPlaceStoredIn(transform.parent.GetComponent<storedPlaceHandler>().getIndex())){
            StoredPlacesController.StoredPlacesControllerObject_.chooseStoredPlace(transform.parent.GetComponent<storedPlaceHandler>().getIndex());
            ChangeScene.changeScene("PantallaSitioGuardado");
        }
    }
}
