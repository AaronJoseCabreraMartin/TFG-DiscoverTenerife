using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that handles the delete button of the stored
  * places panel element.
  */
public class trashButton : MonoBehaviour
{
    /**
      * @brief GameObject with the reference to the toastmessage
      * where the messages will be shown.
      */
    [SerializeField] private GameObject toastMessageObject_;

    /**
      * @brief GameObject that will shown the number of not
      * uploaded visits of the stored place.
      */
    [SerializeField] private GameObject numberOfNotUploadedVisits_;

    /**
      * @brief True if it has to notify the user before deleting
      * the stored place. False if it has not to notify the user.
      */
    private bool shouldAdviseUser_;

    /**
      * @brief This method is called on the first frame. It initialices
      * the shouldAdviseUser_ property as true.
      */
    void Start()
    {
        shouldAdviseUser_ = true;
    }

    /**
      * @brief This method should be called when the delete button is clicked
      * by the user. If the user has not uploaded visits on this stored place,
      * it notify the user to make it aware of what is he doing. If the user
      * hasnt any visit for upload, it just call deleteStoredPlace method
      * of StoredPlacesController class.
      */
    public void OnClick(){
        //si el usuario no ha sido advertido y tiene visitas por subir, avisale
        //si no tiene visitas por subir o ya fue avisado, no lo avises más
        if(numberOfNotUploadedVisits_.GetComponent<Text>().text != "0" && shouldAdviseUser_){
            shouldAdviseUser_ = false;
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("If you delete the stored place without updating your visits, those visits will not count.\nClick again on delete button to confirm.", new Color32(255,145,15,255), 5);
        }else{
            StoredPlacesController.StoredPlacesControllerObject_.deleteStoredPlace(transform.parent.GetComponent<storedPlaceHandler>().getIndex());
            numberOfNotUploadedVisits_.GetComponent<Text>().text = "0";
        }
    }

}
