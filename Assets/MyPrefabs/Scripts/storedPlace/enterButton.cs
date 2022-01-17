using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enterButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(){
        if(StoredPlace.thereIsAPlaceStoredIn(transform.parent.GetComponent<storedPlaceHandler>().getIndex())){
            StoredPlacesController.StoredPlacesControllerObject_.chooseStoredPlace(transform.parent.GetComponent<storedPlaceHandler>().getIndex());
            ChangeScene.changeScene("PantallaSitioGuardado");
        }
    }
}
