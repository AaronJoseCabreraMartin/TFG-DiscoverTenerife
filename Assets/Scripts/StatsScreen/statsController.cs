using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class statsController : MonoBehaviour
{
    [SerializeField] private GameObject[] infoPanels_;

    private firebaseHandler firebaseHandlerObject_;
    private bool dataLoaded_;

    void Awake(){
        firebaseHandlerObject_ = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        if(firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            loadData();
        }else{
            dataLoaded_ = false;
        }
    }

    // Update is called once per frame
    void Update(){
        if(!dataLoaded_ && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            loadData();
        }
    }

    private void loadData(){
        infoPanels_[0].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.countOfVisitedPlaces().ToString());
        infoPanels_[1].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.countOfAccumulatedVisits().ToString());
        infoPanels_[2].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.mostVisitedPlace());
        infoPanels_[3].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.mostVisitedZone());
        infoPanels_[4].GetComponent<informationPosterController>().updateData(firebaseHandlerObject_.actualUser_.mostVisitedType());
        dataLoaded_ = true;
    }
}