using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class optionsController : MonoBehaviour
{
    public static optionsController optionsControllerInstance_ = null;

    private bool distanceInKM_;
    private Dictionary<string, bool> whatToSee_;
    private bool sortByLessDistance_;
    private string lastScene_;

    private uniqueselectionDesplegableMenu sortByLessDistanceMenu_;
    private multiselectionDesplegableMenu whatToSeeMenu_;
    private uniqueselectionDesplegableMenu distanceUnitMenu_;
    private bool optionsCopied_;
    static public GameObject lastOptionClicked_;

    void Awake(){
        if(optionsController.optionsControllerInstance_ != null){
            Destroy(this.gameObject); //no crees otro
            return;
        }
        optionsController.optionsControllerInstance_ = this;
        DontDestroyOnLoad(this.gameObject);
        lastScene_ = SceneManager.GetActiveScene().name;
    }

    void Start(){
        whatToSee_ = new Dictionary<string, bool>();
        List<string> options = new List<string> { "Viewpoints", "Hiking Routes", "Beaches", "Natural Pools","Natural Parks", "Already Visited"};
        foreach(string option in options){
            if(PlayerPrefs.HasKey(option)){
                whatToSee_[option] = PlayerPrefs.GetInt(option) == 1;//1 activado
            }else{
                whatToSee_[option] = true;
                PlayerPrefs.SetInt(option,1);
            }
        }

        if(PlayerPrefs.HasKey("distanceInKM_")){
            distanceInKM_ = PlayerPrefs.GetInt("distanceInKM_") == 1;//1 activado;
        }else{
            distanceInKM_ = true;
            PlayerPrefs.SetInt("distanceInKM_",1);

        }

        if(PlayerPrefs.HasKey("sortByLessDistance_")){
            sortByLessDistance_ = PlayerPrefs.GetInt("sortByLessDistance_") == 1;//1 activado;
        }else{
            sortByLessDistance_ = true;
            PlayerPrefs.SetInt("sortByLessDistance_",1);
        }
        optionsCopied_ = false;
    }

    void Update(){
        if(!optionsCopied_ && sortByLessDistanceMenu_ != null && distanceUnitMenu_ != null){
            copyOptions();//si existian opciones previas, deben mostrarse
        }
        if(lastScene_ != SceneManager.GetActiveScene().name){
            if(lastScene_ == "PantallaOpciones"){//si salimos de opciones, guarda los ajustes
                storeOptions();
            }
            lastScene_ = SceneManager.GetActiveScene().name;
            if(SceneManager.GetActiveScene().name == "PantallaOpciones"){
                sortByLessDistanceMenu_ = GameObject.Find("/Canvas/Short results by").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
                whatToSeeMenu_ = GameObject.Find("/Canvas/Choose what to see").gameObject.GetComponent<multiselectionDesplegableMenu>();
                distanceUnitMenu_ = GameObject.Find("/Canvas/Choose Distance Unit").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
                /*if(sortByLessDistanceMenu_ != null && distanceUnitMenu_ != null){
                    copyOptions();//si existian opciones previas, deben mostrarse
                }*/
                optionsCopied_ = false;
            }else{
                sortByLessDistanceMenu_ = null;
                whatToSeeMenu_ = null;
                distanceUnitMenu_ = null;
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
            whatToSeeOptions["beaches"] = true;
            whatToSeeOptions["naturalPools"] = true;
            whatToSeeOptions["naturalParks"] = true;
        }else{
            whatToSeeOptions["viewpoints"] = whatToSee_["Viewpoints"];
            whatToSeeOptions["hikingRoutes"] = whatToSee_["Hiking Routes"];
            whatToSeeOptions["beaches"] = whatToSee_["Beaches"];
            whatToSeeOptions["naturalPools"] = whatToSee_["Natural Pools"];
            whatToSeeOptions["naturalParks"] = whatToSee_["Natural Parks"];
        }
        whatToSeeOptions["Already Visited"] = whatToSee_["Already Visited"];
        return whatToSeeOptions;
    }

    public void copyOptions(){
        uniqueselectionDesplegableMenu newSortByLessDistanceMenu_ = GameObject.Find("/Canvas/Short results by").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
        multiselectionDesplegableMenu newWhatToSeeMenu_ = GameObject.Find("/Canvas/Choose what to see").gameObject.GetComponent<multiselectionDesplegableMenu>();
        uniqueselectionDesplegableMenu newDistanceUnitMenu_ = GameObject.Find("/Canvas/Choose Distance Unit").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
        

        newWhatToSeeMenu_.setToggleStateByText("Viewpoints", whatToSee_["Viewpoints"]);
        newWhatToSeeMenu_.setToggleStateByText("Hiking Routes", whatToSee_["Hiking Routes"]);
        newWhatToSeeMenu_.setToggleStateByText("Beaches", whatToSee_["Beaches"]);
        newWhatToSeeMenu_.setToggleStateByText("Natural Pools", whatToSee_["Natural Pools"]);
        newWhatToSeeMenu_.setToggleStateByText("Natural Parks", whatToSee_["Natural Parks"]);
        newWhatToSeeMenu_.setToggleStateByText("Already Visited", whatToSee_["Already Visited"]);
        
        newSortByLessDistanceMenu_.selectToggle(( sortByLessDistance_ ? 0 : 1));
        
        newDistanceUnitMenu_.selectToggle((distanceInKM_ ? 0 : 1));
        
        string toShow = "On Copy Options ";
        /*toShow += $"Viewpoints -> {whatToSee_["Viewpoints"]} ";
        toShow += $"Hiking Routes -> {whatToSee_["Hiking Routes"]} ";
        toShow += $"Beaches -> {whatToSee_["Beaches"]} ";
        toShow += $"Natural Pools -> {whatToSee_["Natural Pools"]} ";
        toShow += $"Natural Parks -> {whatToSee_["Natural Parks"]} ";
        toShow += $"Already Visited -> {whatToSee_["Already Visited"]} ";*/
        toShow += $"distanceInKM_ = {distanceInKM_}, {(distanceInKM_ ? 0 : 1)} ";
        toShow += $"sortByLessDistance_ = {sortByLessDistance_}, {(sortByLessDistance_ ? 0 : 1)}";
        //Debug.Log(toShow);

        optionsCopied_ = true;
    }

    public void saveOptions(){
        if(sortByLessDistanceMenu_ && whatToSeeMenu_ && distanceUnitMenu_){
            string toShow = "On save Options ";
            if(whatToSeeMenu_.anyChange()){
                Dictionary<string,bool> changes = new Dictionary<string,bool>();//no puedes editar un diccionario mientras lo recorres
                foreach(KeyValuePair<string, bool> option in whatToSee_){
                    changes[option.Key] = whatToSeeMenu_.checkToggleByText(option.Key);
                    //toShow += $" {option.Key} -> {whatToSeeMenu_.checkToggleByText(option.Key)} \n";
                }
                foreach(KeyValuePair<string, bool> option in changes){
                    whatToSee_[option.Key] = changes[option.Key];
                }
            }
            distanceInKM_ = distanceUnitMenu_.checkToggle(0);
            sortByLessDistance_ = sortByLessDistanceMenu_.checkToggle(0);
            toShow += $"distanceInKM_ = {distanceInKM_} ";
            toShow += $"sortByLessDistance_ = {sortByLessDistance_}";
            //Debug.Log(toShow);
            storeOptions();
        }
    }

    /*
    
    no entiendo que pasa es como que se copian los cambios pero antes de que aplique los valores por defecto
    entonces queda siempre con los que tiene por defecto, pero si comento las lineas de valores por defecto
    sigue sin ir, posible solucion: 
    
        en el awake del unique selection busque al options controller y haga
        askOptionsFor(this.gameObject.name) y se haga un fill con eso
    
    */

    private void storeOptions(){
        string toShow ="On storeOptions ";
        List<string> options = new List<string> { "Viewpoints", "Hiking Routes", "Beaches", "Natural Pools","Natural Parks", "Already Visited"};
        foreach(string option in options){
            PlayerPrefs.SetInt(option, whatToSee_[option] ? 1 : 0 );
            //toShow += $"{option} = {(whatToSee_[option] ? 1 : 0)}";
        }
        PlayerPrefs.SetInt("distanceInKM_", distanceInKM_ ? 1 : 0);
        PlayerPrefs.SetInt("sortByLessDistance_", sortByLessDistance_ ? 1 : 0);
        PlayerPrefs.Save();
        toShow += $"{distanceInKM_} = {(distanceInKM_ ? 1 : 0)}";
        toShow += $"{sortByLessDistance_} = {(sortByLessDistance_ ? 1 : 0)}";
        //Debug.Log(toShow);
    }
}
