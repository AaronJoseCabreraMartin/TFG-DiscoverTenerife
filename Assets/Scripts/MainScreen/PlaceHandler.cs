using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHandler : MonoBehaviour
{
    private static firebaseHandler serverHandler_;
    //private static ServerHandler serverHandler_;
    [SerializeField] private DownloadHandler downloadHandler_;

    private Place place_ = null;

    private bool placeLoaded_ = false;

    public static Place choosenPlace_ = null;

    void Awake()
    {
        if(!PlaceHandler.serverHandler_){
            //GameObject gameObject = new GameObject("ServerHandler");
            //PlaceHandler.serverHandler_ = gameObject.AddComponent<ServerHandler>();
            PlaceHandler.serverHandler_ = GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>();
            Place.downloadHandler_ = downloadHandler_;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(place_ == null && PlaceHandler.serverHandler_.isReady()){
        if(place_ == null && PlaceHandler.serverHandler_.placesAreReady()){
            assignPlace();
        }else if(place_ != null &&placeLoaded_ == false && place_.isReady()){
            loadPlace();
        }
    }

    private void assignPlace(){
        place_ = PlaceHandler.serverHandler_.askForAPlace().Current;
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
