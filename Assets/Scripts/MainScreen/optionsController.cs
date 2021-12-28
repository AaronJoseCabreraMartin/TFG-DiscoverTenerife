using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class optionsController : MonoBehaviour
{
    private bool distanceInKM_;
    private Dictionary<string, bool> whatToSee_;
    private bool sortByLessDistance_;
    private string lastScene_;

    private uniqueselectionDesplegableMenu sortByLessDistanceMenu_;
    private multiselectionDesplegableMenu whatToSeeMenu_;
    private uniqueselectionDesplegableMenu distanceUnitMenu_;

    static public GameObject lastOptionClicked_;

    void Awake(){
        GameObject[] objs = GameObject.FindGameObjectsWithTag("optionsController");
        if (objs.Length > 1){ //si ya existe una optionsController 
            Destroy(this.gameObject); //no crees otro
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        lastScene_ = SceneManager.GetActiveScene().name;
    }

    void Start(){
        distanceInKM_ = true;
        whatToSee_ = new Dictionary<string, bool>();
        whatToSee_["Viewpoints"] = true;
        whatToSee_["Hiking Routes"] = true;
        whatToSee_["Beachs"] = true;
        whatToSee_["Natural Pools"] = true;
        whatToSee_["Natural Parks"] = true;
        whatToSee_["Already Visited"] = true;
        sortByLessDistance_ = true;
    }

    void Update(){
        if(lastScene_ != SceneManager.GetActiveScene().name){
            lastScene_ = SceneManager.GetActiveScene().name;
            if(SceneManager.GetActiveScene().name == "PantallaOpciones"){
                copyOptions();//si existian opciones previas, deben mostrarse
                sortByLessDistanceMenu_ = GameObject.Find("/Canvas/Short results by").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
                whatToSeeMenu_ = GameObject.Find("/Canvas/Choose what to see").gameObject.GetComponent<multiselectionDesplegableMenu>();
                distanceUnitMenu_ = GameObject.Find("/Canvas/Choose Distance Unit").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
                
            }else if(SceneManager.GetActiveScene().name == "PantallaPrincipal" || 
                        SceneManager.GetActiveScene().name == "PantallaLugar"){
                sortByLessDistanceMenu_ = null;
                whatToSeeMenu_ = null;
                distanceUnitMenu_ = null;
            }else{// si no es ninguna de esas opciones destruye este gameobject
                Destroy(this.gameObject);
                return;
            }
        }
        if(sortByLessDistanceMenu_ && whatToSeeMenu_ && distanceUnitMenu_){
            if(sortByLessDistanceMenu_.anyChange()){
                sortByLessDistance_ = sortByLessDistanceMenu_.checkToggle(0);
                //Debug.Log($"sortByLessDistance_ = {sortByLessDistance_}");
            }
            if(whatToSeeMenu_.anyChange()){
                Dictionary<string,bool> changes = new Dictionary<string,bool>();//no puedes editar un diccionario mientras lo recorres
                foreach(KeyValuePair<string, bool> option in whatToSee_){
                    changes[option.Key] = whatToSeeMenu_.checkToggleByText(option.Key);
                }
                foreach(KeyValuePair<string, bool> option in changes){
                    whatToSee_[option.Key] = changes[option.Key];
                    //Debug.Log($"whatToSee_[{option.Key}] = {whatToSee_[option.Key]}");
                }
            }
            if(distanceUnitMenu_.anyChange()){
                distanceInKM_ = distanceUnitMenu_.checkToggle(0);
                //Debug.Log($"distanceInKM_ = {distanceInKM_}");
            }
        }
    }

    public bool distanceInKM(){
        return distanceInKM_;
    }

    public bool sortByLessDistance(){
        return sortByLessDistance_;
    }

    public Dictionary<string, bool> whatToSeeOptions(){
        bool areAllFalse = true;
        foreach(var key in whatToSee_.Keys){
            if(key.ToString() != "Already Visited" && whatToSee_[key]){
                areAllFalse = false;
            }
        }
        Dictionary<string, bool> whatToSeeOptions = new Dictionary<string, bool>();
        if(areAllFalse){
            whatToSeeOptions["viewpoints"] = true;
            whatToSeeOptions["hikingRoutes"] = true;
            whatToSeeOptions["beachs"] = true;
            whatToSeeOptions["naturalPools"] = true;
            whatToSeeOptions["naturalParks"] = true;
        }else{
            whatToSeeOptions["viewpoints"] = whatToSee_["Viewpoints"];
            whatToSeeOptions["hikingRoutes"] = whatToSee_["Hiking Routes"];
            whatToSeeOptions["beachs"] = whatToSee_["Beachs"];
            whatToSeeOptions["naturalPools"] = whatToSee_["Natural Pools"];
            whatToSeeOptions["naturalParks"] = whatToSee_["Natural Parks"];
        }
        whatToSeeOptions["Already Visited"] = whatToSee_["Already Visited"];
        return whatToSeeOptions;
    }

    private void copyOptions(){
        uniqueselectionDesplegableMenu newSortByLessDistanceMenu_ = GameObject.Find("/Canvas/Short results by").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
        multiselectionDesplegableMenu newWhatToSeeMenu_ = GameObject.Find("/Canvas/Choose what to see").gameObject.GetComponent<multiselectionDesplegableMenu>();
        uniqueselectionDesplegableMenu newDistanceUnitMenu_ = GameObject.Find("/Canvas/Choose Distance Unit").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
        
        newSortByLessDistanceMenu_.selectToggle(distanceInKM_ ? 0 : 1);
        newDistanceUnitMenu_.selectToggle(sortByLessDistance_ ? 0 : 1);

        newWhatToSeeMenu_.setToggleStateByText("Viewpoints", whatToSee_["Viewpoints"]);
        newWhatToSeeMenu_.setToggleStateByText("Hiking Routes", whatToSee_["Hiking Routes"]);
        newWhatToSeeMenu_.setToggleStateByText("Beachs", whatToSee_["Beachs"]);
        newWhatToSeeMenu_.setToggleStateByText("Natural Pools", whatToSee_["Natural Pools"]);
        newWhatToSeeMenu_.setToggleStateByText("Natural Parks", whatToSee_["Natural Parks"]);
        newWhatToSeeMenu_.setToggleStateByText("Already Visited", whatToSee_["Already Visited"]);
    }
}
