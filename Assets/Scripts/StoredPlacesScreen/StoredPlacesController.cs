using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoredPlacesController : MonoBehaviour
{
    public static StoredPlace choosenStoredPlace_;
    public static StoredPlacesController StoredPlacesControllerObject_;

    [SerializeField] private GameObject storedPlacesPanel_;

    private List<StoredPlace> storedPlaces_;
    private string defaultText_ = "You have this empty field to store a place for visit it when you don't have internet conection.";

    void Awake()
    {        
        storedPlaces_ = new List<StoredPlace>();
        int index = 0;
        foreach(Transform storedPlaceObject in storedPlacesPanel_.transform){
            if(StoredPlace.thereIsAPlaceStoredIn(index)){
                storedPlaces_.Add(StoredPlace.loadStoredPlace(index));
                storedPlaceObject.Find("EnterButton/Text").GetComponent<Text>().text = storedPlaces_[index].getName();
                storedPlaceObject.Find("StoredVisits/Number").GetComponent<Text>().text = storedPlaces_[index].newVisitsForThisPlace().ToString();
                //storedPlaceObject.Find("EnterButton/Text").transform.position = new Vector3(0.0f,-265.0f,0.0f);
            }else{
                storedPlaces_.Add(null);
            }
            index++;
        }
        StoredPlacesController.StoredPlacesControllerObject_ = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void chooseStoredPlace(int index){
        if(index >= storedPlaces_.Count || !StoredPlace.thereIsAPlaceStoredIn(index) ){
            Debug.Log($"Error, no se encuentra stored place numero {index} en chooseStoredPlace");
        }else{
            StoredPlacesController.choosenStoredPlace_ = storedPlaces_[index];
        }
    }

    public void deleteStoredPlace(int index){
        if(index >= storedPlaces_.Count ){
            Debug.Log($"Error, no se encuentra stored place numero {index} en chooseStoredPlace");
        }else{
            StoredPlace.eraseStoredDataOf(index);
            //storedPlaces_.RemoveAt(index);
            storedPlacesPanel_.transform.GetChild(index).Find("EnterButton/Text").GetComponent<Text>().text = defaultText_;
        }
    }

    void OnDestroy(){
        StoredPlacesController.StoredPlacesControllerObject_ = null;
    }
}
