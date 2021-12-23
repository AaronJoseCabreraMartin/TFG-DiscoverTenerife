using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHandler : MonoBehaviour
{
    private static firebaseHandler serverHandler_;
    private Place place_;
    private bool placeLoaded_;

    //almacena que sitio a escogido el usuario cuando clicka en uno
    // para cambiar de escena y consevar la informacion
    public static Place choosenPlace_ = null;

    void Awake()
    {
        if(!PlaceHandler.serverHandler_){
            PlaceHandler.serverHandler_ = GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        place_ = null;
        placeLoaded_ = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(placeLoaded_ == false){
            if(place_ == null && PlaceHandler.serverHandler_.placesAreReady()){
                //assing place
                place_ = PlaceHandler.serverHandler_.askForAPlace();
            }else if (place_ != null && place_.isReady()){
                loadPlace();
            }
        }
    }

    private void loadPlace(){
        gameObject.GetComponentInChildren<Text>().text = place_.getName();
        gameObject.GetComponent<Image>().sprite = place_.getImage();
        placeLoaded_ = true;
    }

    public void selectPlace(){
        PlaceHandler.choosenPlace_ = place_;
    }
}
