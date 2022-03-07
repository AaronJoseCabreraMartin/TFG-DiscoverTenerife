using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
  * @brief Controls the options that the user has chosen. It follows the singleton pattern so
  * it only can be one instance of this class per execution. This class isnt destroyed
  * on load.
  */
public class optionsController : MonoBehaviour
{
    /**
      * @brief reference to the unique instance of the class that can exist on each execution.
      */
    public static optionsController optionsControllerInstance_ = null;

    /**
      * @brief true if the user has chosen to see the distance in Kilometers, false if 
      * the user has chosen to see the distance in Milles.
      */
    private bool distanceInKM_;

    /**
      * @brief This dictionary has an entry for each type of site. The key is the type of
      * the site, and the value is if the user has chosen to see that type of site or not.
      */
    private Dictionary<string, bool> whatToSee_;

    /**
      * @brief This dictionary has an entry for each social option, allow that other users
      * can find me on the ranking, allow that other user can send me friendship invitations
      * or allow that my friends can chanllege me.
      */
    private Dictionary<string, bool> socialOptions_;

    /**
      * @brief true if the user has chosen that places has to be sorted by less distance first,
      * false if the user has chosen that places has to be sorted by more visited first.
      */
    private bool sortByLessDistance_;

    /**
      * @brief true if the user has chosen that places on the story has to be sorted by 
      * first visited first, false if the user has chosen that the places on the story
      * has to be sorted by most recent visited first.
      */
    private bool sortStoryByFirstVisit_;

    /**
      * @brief contains the name of the last scene that this object was.
      */
    private string lastScene_;

    /**
      * @brief reference to the game object of the sorting places options.
      */
    private uniqueselectionDesplegableMenu sortByLessDistanceMenu_;

    /**
      * @brief reference to the game object of the what type of places
      * you want to see options.
      */
    private multiselectionDesplegableMenu whatToSeeMenu_;
    
    /**
      * @brief reference to the game object of the social options.
      */
    private multiselectionDesplegableMenu socialOptionsMenu_;

    /**
      * @brief reference to the game object of the chosing distance unit options.
      */
    private uniqueselectionDesplegableMenu distanceUnitMenu_;

    /**
      * @brief reference to the game object of the sorting places on the story options.
      */
    private uniqueselectionDesplegableMenu sortStoryMenu_;
    
    /**
      * @brief true if the value of the options were asigned to the game object 
      * that represent those options.
      */
    private bool optionsCopied_;

    /**
      * @brief reference to the last clicked option on the options screen.
      */
    static public GameObject lastOptionClicked_;


    /**
      * @brief this method is called before the first frame, it checks if exits
      * another optionsController instance, on that case destroy this gameobject.
      * It also stores the name of the current scene.
      */
    void Awake(){
        if(optionsController.optionsControllerInstance_ != null){
            Destroy(this.gameObject); //no crees otro
            return;
        }
        optionsController.optionsControllerInstance_ = this;
        DontDestroyOnLoad(this.gameObject);
        lastScene_ = SceneManager.GetActiveScene().name;
    }

    /**
      * @brief this method is called on the first frame. It check if the user has 
      * already stored some options on the current device, if that is not the case,
      * it creates the option as activated. If the user has already stored some options
      * it loads the options from the current device. It also intializes all the properties 
      * of the object.
      */
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

        socialOptions_ = new Dictionary<string, bool>();
        List<string> socialOptions = new List<string> { "addMe", "challengeMe", "ranking" };
        foreach(string option in socialOptions){
            if(PlayerPrefs.HasKey(option)){
                socialOptions_[option] = PlayerPrefs.GetInt(option) == 1;//1 activado
            }else{
                socialOptions_[option] = true;
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

        if(PlayerPrefs.HasKey("sortStoryByFirstVisit_")){
            sortStoryByFirstVisit_ = PlayerPrefs.GetInt("sortStoryByFirstVisit_") == 1;
        }else{
            sortStoryByFirstVisit_ = true;
            PlayerPrefs.SetInt("sortStoryByFirstVisit_",1);
        }
        optionsCopied_ = false;
    }

    /**
      * @brief this method is called each frame. 
      * First it checks if the options isnt copied and youre on the options screen or in the story screen
      * and calls the copyOptions or the copyStoryOptions respectively.
      * It also check if youre exiting the options screen or the story screen and if that is the
      * case it calls the storeOptions method.
      * If youre changing scene and youre on the options screen it finds and initialize the 
      * sortByLessDistanceMenu_, whatToSeeMenu_, distanceUnitMenu_, socialOptionsMenu_ properties and sets
      * the optionsCopied_ as false. 
      * If youre changing scene and youre on the story screen it finds and initialize the
      * sortStoryMenu_ property and sets the optionsCopied_ as false.
      * If youre changing scene and youre neither on the story screen nor the options screen
      * it sets the gameobject of the options as null.
      */
    void Update(){
        if(!optionsCopied_ && sortByLessDistanceMenu_ != null && distanceUnitMenu_ != null && whatToSeeMenu_ != null){
            copyOptions();//si existian opciones previas, deben mostrarse
        }else if(!optionsCopied_ && sortStoryMenu_ != null){
            copyStoryOptions();
        }
        if(lastScene_ != SceneManager.GetActiveScene().name){
            if(lastScene_ == "PantallaOpciones" || lastScene_ == "PantallaHistorial"){//si salimos de opciones, guarda los ajustes
                storeOptions();
            }
            lastScene_ = SceneManager.GetActiveScene().name;
            if(SceneManager.GetActiveScene().name == "PantallaOpciones"){
                sortByLessDistanceMenu_ = GameObject.Find("/Canvas/Short results by").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
                whatToSeeMenu_ = GameObject.Find("/Canvas/Choose what to see").gameObject.GetComponent<multiselectionDesplegableMenu>();
                distanceUnitMenu_ = GameObject.Find("/Canvas/Choose Distance Unit").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
                socialOptionsMenu_ = GameObject.Find("/Canvas/Social Options").gameObject.GetComponent<multiselectionDesplegableMenu>();
                
                optionsCopied_ = false;
            }else if(SceneManager.GetActiveScene().name == "PantallaHistorial"){
                sortStoryMenu_ = GameObject.Find("/Canvas/Choose Order").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
                optionsCopied_ = false;
            }else{
                sortByLessDistanceMenu_ = null;
                whatToSeeMenu_ = null;
                distanceUnitMenu_ = null;
                socialOptionsMenu_ = null;
                sortStoryMenu_ = null;
            }
        }
    }

    /**
      * @return true if the user has chosen the distance unit as kilometers, false
      * if the user has chosen the distance unit as milles.
      * @brief getter of the distanceInKM_ property.
      */
    public bool distanceInKM(){
        return distanceInKM_;
    }

    /**
      * @return true if the user has chosen to sort the places as less distance first,
      * false if the user has chosen to sort the places as more visited first.
      * @brief getter of the sortByLessDistance_ property.
      */
    public bool sortByLessDistance(){
        return sortByLessDistance_;
    }

    /**
      * @return dictionary with one entry for each type of site, the value mean if the
      * user want to see that type or not.
      * @brief return a dictionary that contains an entry for each type of place, each entry has
      * a boolean value that is true if the user chose he wants to see it and false if he dont
      * want to see it. There is an aditional entry for choosing if the user wants to see places
      * that he already had visited.
      */
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

    /**
      * @brief this method make that the gameobjects of the options screen show the same
      * choices that this class has stored on his attributes.
      */
    public void copyOptions(){
        // esto se deberia poder quitar
        uniqueselectionDesplegableMenu newSortByLessDistanceMenu_ = GameObject.Find("/Canvas/Short results by").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
        multiselectionDesplegableMenu newWhatToSeeMenu_ = GameObject.Find("/Canvas/Choose what to see").gameObject.GetComponent<multiselectionDesplegableMenu>();
        uniqueselectionDesplegableMenu newDistanceUnitMenu_ = GameObject.Find("/Canvas/Choose Distance Unit").gameObject.GetComponent<uniqueselectionDesplegableMenu>();
        multiselectionDesplegableMenu  newSocialOptionsMenu_ = GameObject.Find("/Canvas/Social Options").gameObject.GetComponent<multiselectionDesplegableMenu>();
        

        newWhatToSeeMenu_.setToggleStateByText("Viewpoints", whatToSee_["Viewpoints"]);
        newWhatToSeeMenu_.setToggleStateByText("Hiking Routes", whatToSee_["Hiking Routes"]);
        newWhatToSeeMenu_.setToggleStateByText("Beaches", whatToSee_["Beaches"]);
        newWhatToSeeMenu_.setToggleStateByText("Natural Pools", whatToSee_["Natural Pools"]);
        newWhatToSeeMenu_.setToggleStateByText("Natural Parks", whatToSee_["Natural Parks"]);
        newWhatToSeeMenu_.setToggleStateByText("Already Visited", whatToSee_["Already Visited"]);
        
        newSortByLessDistanceMenu_.selectToggle(( sortByLessDistance_ ? 0 : 1));
        
        newDistanceUnitMenu_.selectToggle((distanceInKM_ ? 0 : 1));

        newSocialOptionsMenu_.setToggleStateByText("Other users can send me friendship invitations", socialOptions_["addMe"]);
        newSocialOptionsMenu_.setToggleStateByText("Friends can challenge me", socialOptions_["challengeMe"]);
        newSocialOptionsMenu_.setToggleStateByText("Other users can see me on the ranking", socialOptions_["ranking"]);
        
        //string toShow = "On Copy Options ";
        //toShow += $"Viewpoints -> {whatToSee_["Viewpoints"]} ";
        //toShow += $"Hiking Routes -> {whatToSee_["Hiking Routes"]} ";
        //toShow += $"Beaches -> {whatToSee_["Beaches"]} ";
        //toShow += $"Natural Pools -> {whatToSee_["Natural Pools"]} ";
        //toShow += $"Natural Parks -> {whatToSee_["Natural Parks"]} ";
        //toShow += $"Already Visited -> {whatToSee_["Already Visited"]} ";
        //toShow += $"Other users can send me friendship invitations -> {socialOptions_["addMe"]} ";
        //toShow += $"Friends can challenge me -> {socialOptions_["challengeMe"]} ";
        //toShow += $"Other users can see me on the ranking -> {socialOptions_["ranking"]} ";
        //toShow += $"distanceInKM_ = {distanceInKM_}, {(distanceInKM_ ? 0 : 1)} ";
        //toShow += $"sortByLessDistance_ = {sortByLessDistance_}, {(sortByLessDistance_ ? 0 : 1)}";
        //Debug.Log(toShow);

        optionsCopied_ = true;
    }

    /**
      * @brief this method make that the gameobject of the story options show the same
      * choices that this class has stored on his attributes.
      */
    public void copyStoryOptions(){
        //Debug.Log($"On copyStoryOptions con {sortStoryByFirstVisit_}");
        sortStoryMenu_.selectToggle((sortStoryByFirstVisit_ ? 0 : 1));
        optionsCopied_ = true;
    }

    /**
      * @brief this method store on this class attributes the user choices of the 
      * options screen and sets the optionsCopied_ property to true.
      */
    public void saveOptions(){
        //string toShow = "On save Options ";
        if(sortByLessDistanceMenu_ && whatToSeeMenu_ && distanceUnitMenu_){
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

            if(socialOptionsMenu_.anyChange()){
                Dictionary<string,bool> changes = new Dictionary<string,bool>();//no puedes editar un diccionario mientras lo recorres
                List<string> listaOpciones = new List<string> {"Other users can send me friendship invitations", "Friends can challenge me", "Other users can see me on the ranking"};
                foreach(string option in listaOpciones){
                    changes[option] = socialOptionsMenu_.checkToggleByText(option);
                    //toShow += $" {option} -> {socialOptionsMenu_.checkToggleByText(option)} \n";
                }
                socialOptions_["addMe"] = changes["Other users can send me friendship invitations"];
                socialOptions_["challengeMe"] = changes["Friends can challenge me"];
                socialOptions_["ranking"] = changes["Other users can see me on the ranking"];
                firebaseHandler.firebaseHandlerInstance_.uploadSocialPreferences();
                
            }
            distanceInKM_ = distanceUnitMenu_.checkToggle(0);
            sortByLessDistance_ = sortByLessDistanceMenu_.checkToggle(0);
            //toShow += $"distanceInKM_ = {distanceInKM_} ";
            //toShow += $"sortByLessDistance_ = {sortByLessDistance_}";
            storeOptions();
        }else if(sortStoryMenu_){
            sortStoryByFirstVisit_ = sortStoryMenu_.checkToggle(0);
            //toShow += $"sortStoryByFirstVisit_ = {sortStoryByFirstVisit_}";
            storeOptions();
        }
        //Debug.Log(toShow);
    }

    /**
      * @brief This method stores the options choices of the current user on the current
      * device.
      */
    private void storeOptions(){
        //string toShow ="On storeOptions ";
        List<string> options = new List<string> { "Viewpoints", "Hiking Routes", "Beaches", "Natural Pools","Natural Parks", "Already Visited"};
        foreach(string option in options){
            PlayerPrefs.SetInt(option, whatToSee_[option] ? 1 : 0 );
            //toShow += $"{option} = {(whatToSee_[option] ? 1 : 0)}";
        }

        List<string> socialOptions = new List<string> { "addMe", "challengeMe", "ranking"};
        foreach(string option in socialOptions){
            PlayerPrefs.SetInt(option, socialOptions_[option] ? 1 : 0 );
            //toShow += $"{option} = {(socialOptions_[option] ? 1 : 0)}";
        }

        PlayerPrefs.SetInt("distanceInKM_", distanceInKM_ ? 1 : 0);
        PlayerPrefs.SetInt("sortByLessDistance_", sortByLessDistance_ ? 1 : 0);
        PlayerPrefs.SetInt("sortStoryByFirstVisit_", sortStoryByFirstVisit_ ? 1 : 0);
        PlayerPrefs.Save();
        //toShow += $"{distanceInKM_} = {(distanceInKM_ ? 1 : 0)}";
        //toShow += $"{sortByLessDistance_} = {(sortByLessDistance_ ? 1 : 0)}";
        //toShow += $"sortStoryByFirstVisit_ = {sortStoryByFirstVisit_}";
        //Debug.Log(toShow);
    }

    /**
      * @return bool true if the user has chose to sort first visit first on the
      * story screen, false if the user has chose to sort the last visit first on
      * the story screen.
      * @brief getter of the sortStoryByFirstVisit_ attribute.
      */
    public bool sortStoryByFirstVisit(){
        return sortStoryByFirstVisit_;
    }

    /**
      * @param string social option that you want to access.
      * @return bool true if the social option is active, false in other case.
      * @brief getter of the given social option.
      */
    public bool socialOptions(string option){
        return socialOptions_[option];
    }
}
