using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHandler : MonoBehaviour
{
    private static ServerHandler serverHandler_;

    private Place place_ = null;

    private bool placeLoaded_ = false;

    public static Place choosenPlace_ = null;

    void Awake()
    {
        if(!PlaceHandler.serverHandler_){
            GameObject gameObject = new GameObject("ServerHandler");
            PlaceHandler.serverHandler_ = gameObject.AddComponent<ServerHandler>();
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!PlaceHandler.serverHandler_.isReady()){
            //imagen de cargando
        }else{
            loadPlace();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(placeLoaded_ == false && PlaceHandler.serverHandler_.isReady()){
            loadPlace();
        }
    }

    private void loadPlace(){
        place_ = PlaceHandler.serverHandler_.askForAPlace();
        gameObject.GetComponentInChildren<Text>().text = place_.getName();
        gameObject.GetComponent<Image>().sprite = place_.getImage();
        placeLoaded_ = true;
    }

    public void selectPlace(){
        PlaceHandler.choosenPlace_ = place_;
    }
}
