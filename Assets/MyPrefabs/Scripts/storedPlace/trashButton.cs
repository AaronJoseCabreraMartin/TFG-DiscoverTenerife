using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class trashButton : MonoBehaviour
{
    [SerializeField] private GameObject toastMessageObject_;
    [SerializeField] private GameObject numberOfNotUpdatedVisits_;

    private bool shouldAdviseUser_;
    // Start is called before the first frame update
    void Start()
    {
        shouldAdviseUser_ = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClick(){
        //si el usuario no ha sido advertido y tiene visitas por subir, avisale
        //si no tiene visitas por subir o ya fue avisado, no lo avises más
        if(numberOfNotUpdatedVisits_.GetComponent<Text>().text != "0" && shouldAdviseUser_){
            shouldAdviseUser_ = false;
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("If you delete the stored place without updating your visits, those visits will not count.\nClick again on delete button to confirm.", new Color32(255,145,15,255), 5);
        }else{
            StoredPlacesController.StoredPlacesControllerObject_.deleteStoredPlace(transform.parent.GetComponent<storedPlaceHandler>().getIndex());
            numberOfNotUpdatedVisits_.GetComponent<Text>().text = "0";
        }
    }

}
