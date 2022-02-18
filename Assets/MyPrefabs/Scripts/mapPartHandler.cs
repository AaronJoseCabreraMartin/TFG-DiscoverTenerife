using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that control one of the parts of the map that shown
  * the proportion of places visited by the current user.
  */
public class mapPartHandler : MonoBehaviour
{
    /**
      * @brief GameObject that has the text that shows he percentage of
      * how many place of that zone the current user has visited already.
      */
    [SerializeField] private GameObject percentage_;

    /**
      * @brief GameObject that has the image that shows visually the proportion
      * of places visited.
      */
    [SerializeField] private GameObject image_;

    /**
      * @brief Reference to the firebaseHandler instance.
      */
    private firebaseHandler firebaseHandlerObject_;

    /**
      * @brief True if the data was already loaded, false in other case.
      */
    private bool dataLoaded_;

    /**
      * @brief This method is called before the first frame. It initialice the firebaseHandlerObject_
      * property and if both placesAreReady and userDataIsReady return true, it calls loadData method,
      * it sets the dataLoaded_ to false in other case.
      */
    void Awake(){
        firebaseHandlerObject_ = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        if(firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            loadData();
        }else{
            dataLoaded_ = false;
        }
    }

    /**
      * @brief This method is called each frame. If dataLoaded_ is false and both placesAreReady and 
      * userDataIsReady return true, it calls loadData method,
      */
    void Update(){
        if(!dataLoaded_ && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            loadData();
        }
    }

    /**
      * @brief This method calculate the proportion of visited places of the determined zone.
      * Then it changes the text that is shown on the part of the map and the proportion
      * of the background image that is filled. It also puts the dataLoaded_ property as true.
      * <b>IMPORTANT NOTE: THE ZONE IS DETERMINED BY THE NAME OF THE GAMEOBJECT THAT THIS 
      * SCRIPT IS ATTACHED.</b> 
      */
    private void loadData(){        
        float countVisitedPlacesOfZone = (float) firebaseHandlerObject_.actualUser_.countVisitedPlacesOfZone(gameObject.name);
        float totalPlacesOfZone = (float) firebaseHandlerObject_.totalOfPlacesOfZone(gameObject.name);
        float value = countVisitedPlacesOfZone/totalPlacesOfZone;
        percentage_.GetComponent<Text>().text = Math.Round(value*100,2).ToString()+"%";
        image_.GetComponent<Image>().fillAmount = value;
        dataLoaded_ = true;
    }
}
