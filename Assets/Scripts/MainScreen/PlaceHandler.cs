using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This class handles an element of the site panel. It shows a loading gif animation
  * when its downloading the image of the showed place.
  */
public class PlaceHandler : MonoBehaviour
{
    /**
      * @brief Animator that stores the loading gif.
      */
    [SerializeField] private Animator transition_;

    /**
      * @brief Reference to the firebaseHandler object.
      */
    private static firebaseHandler serverHandler_; //WTF no deberia hacer falta.
    
    /**
      * @brief Place object that stores the place's data.
      */
    private Place place_;

    /**
      * @brief bool true if the place_ has already downloaded the place's photo
      */
    private bool placeLoaded_;

    /**
      * @brief stores a reference to the place chose by the user. It is a static property
      * to allow conserve the information even when the user changes the scene.
      */
    public static Place choosenPlace_ = null;
    //almacena que sitio a escogido el usuario cuando clicka en uno
    // para cambiar de escena y consevar la informacion

    /** 
      * @brief stores the index of the place that has to download the image at this moment.
      */
    private static int turn_ = 0;

    /**
      * @brief This method is called before the first frame, it initialices the static properties 
      * turn_ as 0, and serverHandler_ as the firebase instance.
      */
    void Awake()
    {
        PlaceHandler.turn_ = 0;
        if(!PlaceHandler.serverHandler_){//es el primero
            PlaceHandler.serverHandler_ = firebaseHandler.firebaseHandlerInstance_;
        }
    }

    /**
      * @brief This method is called on the first frame, it initialices the place_
      * property as null and the placeLoaded_ as false. 
      */
    void Start()
    {
        place_ = null;
        placeLoaded_ = false;
    }

    /**
      * @brief This method is called once per frame.
      * - It checks if the serverHandler_ static property is null, it that is the case it tries
      * to get the firebaseHandlerInstance_ static propert of firebaseHandler.
      * - If the serverHandler static property isnt null, and the place didnt load al ready,
      * if its the turn of the current place, it starts the download and add 1 to the turn. And
      * if the place has been selected but they are ready, it calls the loadPlace method.
      */
    void Update()
    {
        if(!PlaceHandler.serverHandler_){//es el primero
            PlaceHandler.serverHandler_ = firebaseHandler.firebaseHandlerInstance_;
        }else{
            if(placeLoaded_ == false){
                /*
                Esto esta limitando a un askForAPlace en cada frame seria mejor que askForAPlace recibiera que posicion quieres
                por ejemplo teniendo una list<int> con los places que tiene que devolver en cada orden
                */
                if(place_ == null && PlaceHandler.serverHandler_.placesAreReady() && isMyTurn() && (Place.webClient_ == null || !Place.webClient_.IsBusy)){
                    //assing place
                    place_ = PlaceHandler.serverHandler_.askForAPlace();
                    place_.startDownload();
                    PlaceHandler.turn_++;
                }else if (place_ != null && place_.isReady()){
                    loadPlace();
                }
            }
        }
    }

    /**
      * This method should be called when the place is ready to be shown. It hides the loading
      * gif and set the name and the image on this gameobject. It sets the placeLoaded_ to true.
      */
    private void loadPlace(){
        transition_.SetTrigger("Success");
        transition_.enabled = false;
        gameObject.GetComponentInChildren<Text>().text = place_.getName();
        gameObject.GetComponent<Image>().sprite = place_.getImage();
        placeLoaded_ = true;
    }

    /**
      * @brief this method should be called when the user chose this place. It sets the 
      * choosenPlace_ static property as a reference to the place_ property.
      */
    public void selectPlace(){
        PlaceHandler.choosenPlace_ = place_;
    }

    /**
      * @return bool, true if its the turn of this place false in other case.
      * @brief This method checks if its the turn of this place using this gameobject
      * name and the turn_ static property of PlaceHandler.
      */
    private bool isMyTurn(){
        return Int32.Parse(Regex.Match(gameObject.name, @"\d+").Value) == PlaceHandler.turn_;
    }
}
