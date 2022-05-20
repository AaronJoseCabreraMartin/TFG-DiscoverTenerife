using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controls the panels that shows the information of 
  * each stored place.
  */
public class StoredPlacesController : MonoBehaviour
{
    /**
      * @brief StoredPlace that the user has chosen. Its a static property
      * to keep the information between scenes.
      */
    public static StoredPlace choosenStoredPlace_;

    /**
      * @brief StoredPlacesController static reference to the object that is selected?????
      */
    public static StoredPlacesController StoredPlacesControllerObject_;

    /**
      * @brief Reference to the GameObject that contains the panel that shows the stored places.
      */
    [SerializeField] private GameObject storedPlacesPanel_;

    /**
      * @brief List of all the StoredPlaces that are shown by the panel. 
      */
    [SerializeField] private List<StoredPlace> storedPlaces_;

    /**
      * @brief The text that will be shown when one of the spaces for store a place is free.
      */
    [SerializeField] private string defaultText_ = "You have this empty field to store a place for visit it when you don't have internet conection.";

    /**
      * @brief This method is called before the first frame. It sets the information
      * of each stored place on the correspondent element of the panel, and add a reference
      * of the StoredPlace object that is on the index-th position to the storedPlaces_ list. 
      * If there isnt any place stored on one of the index it will add null to the
      * storedPlaces_ list.
      */
    void Awake(){        
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
        //WTF esto siempre apunta a la ultima que se haya inicializado, no lo uso mas en esta clase.
        StoredPlacesController.StoredPlacesControllerObject_ = this;
    }

    /**
      * @param int index of which place is chosen the current user.
      * @brief This method should be called when the user choose one of the stored
      * places. It selects the stored place that is on the index-th position.
      */
    public void chooseStoredPlace(int index){
        if(index >= storedPlaces_.Count || !StoredPlace.thereIsAPlaceStoredIn(index) ){
            Debug.Log($"Error, no se encuentra stored place numero {index} en chooseStoredPlace");
        }else{
            StoredPlacesController.choosenStoredPlace_ = storedPlaces_[index];
        }
    }

    /**
      * @param int index of which place the user wants to delete.
      * @brief This method should be called when the user clicks on the delete
      * one stored place button. It deletes the stored information of the
      * index-th stored place. It also updates the information of the panel
      * that shows its information. 
      */
    public void deleteStoredPlace(int index){
        if(index >= storedPlaces_.Count ){
            Debug.Log($"Error, no se encuentra stored place numero {index} en chooseStoredPlace");
        }else{
            StoredPlace.eraseStoredDataOf(index);
            //storedPlaces_.RemoveAt(index);
            storedPlacesPanel_.transform.GetChild(index).Find("EnterButton/Text").GetComponent<Text>().text = defaultText_;
        }
    }

    /**
      * @brief this method is called when the current GameObject is destroyed. It
      * makes the StoredPlacesControllerObject_ static property null again.
      */
    void OnDestroy(){
        StoredPlacesController.StoredPlacesControllerObject_ = null;
    }
}
