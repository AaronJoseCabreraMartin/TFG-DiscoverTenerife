using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mapPartHandler : MonoBehaviour
{
    [SerializeField] private GameObject percentage_;
    [SerializeField] private GameObject image_;

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
        float countVisitedPlacesOfZone = (float) firebaseHandlerObject_.actualUser_.countVisitedPlacesOfZone(gameObject.name);
        float totalPlacesOfZone = (float) firebaseHandlerObject_.totalOfPlacesOfZone(gameObject.name);
        float value = countVisitedPlacesOfZone/totalPlacesOfZone;
        percentage_.GetComponent<Text>().text = Math.Round(value*100,2).ToString()+"%";
        image_.GetComponent<Image>().fillAmount = value;
        dataLoaded_ = true;
    }
}
