using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief class that controls he panels that show the current user stats.
  */
public class statsController : MonoBehaviour
{
    /**
      * @brief Array with all the GameObjects that shows the different stats 
      * of the current user.
      */
    [SerializeField] private GameObject[] infoPanels_;

    /**
      * @brief Reference to the firebaseHandler object.
      */
    private firebaseHandler firebaseHandlerObject_;

    /**
      * @brief True if the current user's scores is already loaded, false in other
      * case.
      */
    private bool dataLoaded_;

    /**
      * @brief This method is called before the first frame, it instantiate the
      * firebaseHandlerObject_ property and checks if both placesAreReady and 
      * userDataIsReady method of firebaseHandler return true it calls loadData
      * method, in other case it sets the dataLoaded_ property to false.
      */
    void Awake(){
        firebaseHandlerObject_ = firebaseHandler.firebaseHandlerInstance_;
        if(firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            loadData();
        }else{
            dataLoaded_ = false;
        }
    }

    /**
      * @brief This method is called once on each frame. If the data isnt loaded and 
      * both placesAreReady and userDataIsReady methods of firebaseHandler class returns
      * true it calls the loadData method. 
      */
    void Update(){
        if(!dataLoaded_ && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            loadData();
        }
    }

    /**
      * @brief Update the data of all the panels that shows the current user's stats
      * using the getters of UserData class. It also changes the dataLoaded_ to true.
      */
    private void loadData(){
        infoPanels_[0].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.countOfVisitedPlaces().ToString());
        infoPanels_[1].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.countOfAccumulatedVisits().ToString());
        infoPanels_[2].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.mostVisitedPlace());
        infoPanels_[3].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.mostVisitedZone());
        infoPanels_[4].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.mostVisitedType());
        dataLoaded_ = true;
    }
}