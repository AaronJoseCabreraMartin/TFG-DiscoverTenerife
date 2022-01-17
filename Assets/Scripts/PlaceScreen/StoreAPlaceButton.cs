using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreAPlaceButton : MonoBehaviour
{
    [SerializeField] private GameObject toastMessageObject_;

    public void OnClick(){
        toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
        if(StoredPlace.isPlaceStoredByName(PlaceHandler.choosenPlace_.getName())){
            toastMessageInstance.makeAnimation("You already have this place stored!", new Color32(255,145,15,255), 5);
        }else if(StoredPlace.thereIsSpaceForOtherPlaceStored()){
            StoredPlace.storePlace(PlaceHandler.choosenPlace_);
            toastMessageInstance.makeAnimation("You stored this place successfully", new Color32(76,175,80,255), 5);
        }else{
            toastMessageInstance.makeAnimation("Error, you cant store more places!", new Color32(255,0,0,255), 5);   
        }
    }
}
