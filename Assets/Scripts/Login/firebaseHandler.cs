using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

/**
 * @brief This class follows the Singleton patron, so it only has one instance at the same time.
 * The instance could be found on the firebaseHandlerInstance_ public static property.
 * This class handles all the firebase releated methods like validating user, creating new users,
 * downloading and uploading all the data from the database.
 * This class is a partial class it has another 3 files: downloadHandler.cs, uploadHandler.cs and
 * loginHandler.cs
 */ 
public partial class firebaseHandler : MonoBehaviour
{
    /**
      * @public
      * This public static attribute store the unique instance of this class.
      */
    public static firebaseHandler firebaseHandlerInstance_ = null;

    /**
      * @brief Reference to the FirebaseApp object
      */
    private FirebaseApp firebaseApp = null;
    
    /**
      * This attribute store the current authenticate user, it starts as null.
      */
    internal Firebase.Auth.FirebaseAuth auth = null;
    //EL USUARIO ACTUAL ESTA EN auth.CurrentUser
    
    /**
      * A reference to the firebase database, you have to use this attribute to
      * access or modify the database data.
      */
    internal DatabaseReference database = null;
    
    /**
      * This object store the configuration for accessing the Google SingIn services,
      * it stores for example the WebClientId token.
      */
    private GoogleSignInConfiguration configuration;

    /**
      * A instance of the class UserData, it stores the downloaded data from the database,
      * check the documentation of that class to get more information.
      */
    public UserData currentUser_ = null;
    
    /**
      * This flag is true when the firebase dependences are resolved and ready to use. If this
      * flag is false, this class will try to resolve the firebase dependencies each frame.
      */
    private bool firebaseDependenciesResolved = false;
    
    /**
      * This flag is true when the firebase dependences are trying to be resolved right now. This flag
      * prevent this class for making several calls to the firebase dependencies resolver method.
      */
    private bool firebaseDependenciesRunning = false;

    /**
      * This flag determinate if the information of the interesting points are already downloaded
      * from the database and its ready to use.
      */
    private bool placesReady_ = false;
    
    /**
      * If this flag is true, the information of the user are already downloaded from the
      * databse and its ready to use.
      */
    private bool userDataReady_ = false;
    /**
      * This atribute store all the data of the places downloaded from the database. The data stored
      * as strings on a dictionary so they cant be directly used, its necessary to use the Place class
      * for interpreting the information that is stored on this attribute. The data is structured as following:
      * - allPlaces contains an entry for each type of place, each entry is another dictionary.
      * - Each of those dictionaries has an entry for each place of that type, each entry is another dictionary.
      * - Each of those dictionaries are the information of each of the places but in a string form.
      */
    //              type of place          id                 DATA
    private Dictionary<string,Dictionary<string,Dictionary<string,string>>> allPlaces_ = new Dictionary<string,Dictionary<string,Dictionary<string,string>>>();
    
    /**
      * This atribute stores an instance of the requestHandler class, that class converts the information 
      * of allPlaces_ on instances of the class Place. Check that class documentation for more info.
      */
    public requestHandler requestHandler_;

    /**
      * This atribute stores the list of places that have been visited but the internet conection have failed
      * when this class tried to upload it. This class will try to upload that information since it gets
      * internet connection again.
      */
    private List<Dictionary<string, string>> placesToUpdateQueue_;

    /**
      * It stores a list of the users ID that this class needs to download some information, because they
      * are friends of the current user. As soon as the information is started to download, the uid is 
      * removed from the list.
      */
    private List<string> friendDataDownloadQueue_;

    /**
      * It stores a list of the users ID that this class needs to download some information because they
      * sended a new friend invitation. As soon as the information is started to download, the uid is 
      * removed from the list.
      */
    private List<string> newFriendDataDownloadQueue_;

    /**
      * @brief true if it is trying to download any of the
      * user permissions lists right now. false in other case.
      */
    private bool downloadingAnyOfTheUsersPermissionsLists_;
    
    /**
      * @brief true if there is changes on the social preferences that should be uploaded.
      */
    private bool hasToUploadSocialPreferences_;

    /**
      * @brief List of dictionary strings with the user id and the score of that user that needs
      * to be uploaded.
      */
    private List<Dictionary<string,string>> otherUserScoresToUpload_;

    /**
      * @brief Boolean that is used as a flag that indicates if the uploading of the acceptedFriendsInvitations
      * property of a friend is uploading right now
      */
    private bool uploadingNotifications_;

    /**
      * @brief List with all the data that is showed on the ranking screen. It has a RankingPlayerData object
      * for each player that appear on the ranking.
      */
    private List<RankingPlayerData> rankingData_;

    /**
      * @brief Boolean that is used as a flag that indicates if its time to download all the ranking data.
      */
    private bool allowDownloadRankingData_;

    /**
      * @brief Boolean that is used as a flag that indicates if the ranking data is downloaded right now.
      */
    private bool downloadingRankingData_;

    //                       uid, property
    private List<Dictionary<string,string>> otherUsersPropertiesToUpload_;
    //es el current user, el uid lo sabemos solo necesitamos saber la property NO DEBERIAN HABER REPETIDOS
    private List<string> currentUserPropertiesToUpload_;

    /**
     * The awake method is called before the first frame, it checks if other 
     * firebaseHandler was instanciated before, if that is the case, it destroy this object.
     */
    private void Awake() {
        //tengo que subscribirme siempre porque sino, cuando haga el destroy
        //no tiene a qué desubscribirse.
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(firebaseHandler.firebaseHandlerInstance_ != null){
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        firebaseHandlerInstance_ = this;
        configuration = new GoogleSignInConfiguration { WebClientId = "993595598765-gov25ig79svl8v52ne2rlrmi7jcl8gf8.apps.googleusercontent.com", 
                                                        RequestEmail = true,
                                                        RequestIdToken = true };
        // cuando termine           A                           ejecuta         B                   con este contexto (para acceder a las cosas privadas)
        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(CheckDependenciesFirebase,TaskScheduler.FromCurrentSynchronizationContext());
        friendDataDownloadQueue_ = new List<string>();
        newFriendDataDownloadQueue_ = new List<string>();
        otherUserScoresToUpload_ = new List<Dictionary<string,string>>();
        otherUsersPropertiesToUpload_ = new List<Dictionary<string,string>>();
        currentUserPropertiesToUpload_ = new List<string>();
        rankingData_ = new List<RankingPlayerData>();
        allowDownloadRankingData_ = false;
        downloadingRankingData_ = false;
    }

    /**
      * The Update method is called every frame. On this method this class constantly check:
      * - If it has to resolve the firebase dependencies, on that case try to solve them first.
      * - If the firebase dependencies has already been resolved, check if it has some information
      * waiting for being uploaded or downloaded try if there is internet connection.
      */
    void Update(){
        if(!firebaseDependenciesResolved && !firebaseDependenciesRunning){
            firebaseDependenciesRunning = true;
            // cuando termine           A                           ejecuta         B                   con este contexto (para acceder a las cosas privadas)
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(CheckDependenciesFirebase,TaskScheduler.FromCurrentSynchronizationContext());
        }else if(firebaseDependenciesResolved && !firebaseDependenciesRunning){
            bool readyForUploadChanges = internetConnection() && userDataReady_ && placesReady_ && firebaseDependenciesResolved;
            //Si hay conexion y no se han enviado las visitas offline, envialas.
            if(readyForUploadChanges && StoredPlace.changesToUpdate_ ){
                StoredPlace.UploadChanges();
            }

            if(readyForUploadChanges && placesToUpdateQueue_ != null){
                if(placesToUpdateQueue_.Count == 0){
                    placesToUpdateQueue_ = null;
                }else{
                    uploadPlacesQueue();
                }
            }

            if(readyForUploadChanges && friendDataDownloadQueue_.Count != 0){
                downloadAllFriendData();
            }

            if(readyForUploadChanges && newFriendDataDownloadQueue_.Count != 0){
                downloadAllNewfriendsInvitationsData();
            }

            if(internetConnection() && firebaseDependenciesResolved && !downloadingAnyOfTheUsersPermissionsLists_ &&
                ( FriendData.usersThatAllowFriendshipInvitations_ == null || 
                  FriendData.usersThatAllowBeChallenged_ == null || 
                  FriendData.usersThatAllowAppearedOnRanking_ == null )){
                    downloadUsersPermissionsLists();
            }

            if(readyForUploadChanges && hasToUploadSocialPreferences_){
                uploadSocialPreferences();
            }

            if(readyForUploadChanges && otherUserScoresToUpload_.Count != 0){
                uploadOtherUserScores();
            }

            if(readyForUploadChanges && currentUser_.friendDataIsComplete() && currentUser_.anyUserHasToBeNotified() && !uploadingNotifications_){
                string userThatHaveToBeNotified = currentUser_.nextFriendToBeNotified();
                updateUserAddedAFriend(userThatHaveToBeNotified, 
                                        currentUser_.getFriendDataByUID(userThatHaveToBeNotified).getStringConversionOfNewAcceptedFriends());
            }

            if( allowDownloadRankingData_ && internetConnection() && firebaseDependenciesResolved && 
                    FriendData.usersThatAllowAppearedOnRanking_ != null && !downloadingRankingData_ && !isRankingDataComplete()){
                downloadRankingPlayerData();
            }

            if(readyForUploadChanges && otherUsersPropertiesToUpload_.Count != 0){
                writeAllFriendProperties();
            }

            if(readyForUploadChanges && currentUserPropertiesToUpload_.Count != 0){
                writeQueuedUserProperties();
            }
        }
    }
    
    /**
      * This method returns true only if there is internet connection, otherwise it will return false.
      */
    public bool internetConnection(){
        return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || 
                Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
    }

    /**
      * @param Task<DependencyStatus> task to wait and take its results
      * This method creates the firebase object and check if there is a previous session, it there is
      * login that sesion. It also calls the method that download the places information and the user data.
      */
    private void CheckDependenciesFirebase(Task<DependencyStatus> task) {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Esto es para crear el objeto FirebaseApp, necesario para manejar los distintos metodos de firebase.
            firebaseApp = FirebaseApp.DefaultInstance;
            // para obtener el objeto Firebase.Auth.FirebaseAuth
            auth = Firebase.Auth.FirebaseAuth.GetAuth(firebaseApp);
            // con esto recogemos la referencia a la base de datos, para poder hacer operaciones de escritura o lectura.
            database = FirebaseDatabase.DefaultInstance.RootReference;
            firebaseDependenciesResolved = true;
            firebaseDependenciesRunning = false;
            //Debug.Log("Firebase Connected!!!");
            //ya habia una sesion iniciada antes!
            if(auth != null){
                Debug.Log("YA HABIA UNA SESION INICIADA!! "+ (auth.CurrentUser.IsAnonymous ? "Anonymous" : auth.CurrentUser.DisplayName));
                //ChangeScene.changeScene("PantallaPrincipal");
                GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal");
                downloadAllPlaces();
                readUserData();
            }
        }else{
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
            firebaseDependenciesResolved = false;
            firebaseDependenciesRunning = false;
        }
    }

    /**
      * Getter of the flag that indicate that firebase dependencies are resolved or not.
      */
    public bool FirebaseDependenciesAreResolved(){
        return firebaseDependenciesResolved;
    }

    ///// USER ACCOUNTS METHODS /////

    /**
      * @param string that contains the new user email.
      * @param string that contains the new user password.
      * This method tries to create a new user on the firebase backend with the given email and password.
      */
    public void CreateNewUser(string email, string password){
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                GameObject.Find("registerScreenController").GetComponent<registerScreenController>().errorCreatingUser("The creatation of a new user with email and password was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                GameObject.Find("registerScreenController").GetComponent<registerScreenController>().errorCreatingUser("The creation of a new user with email and password encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;

            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile {
                DisplayName = email
            };
            auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }
                
                currentUser_ = new UserData(auth.CurrentUser);
                
                //writeUserData();
                writeAllUserProperties();
                GameObject.Find("registerController").GetComponent<registerScreenController>().userCreatedSuccessfully(newUser.DisplayName);
                downloadAllPlaces();
                //no hay que hacer read por lo tanto, ya esta ready
                userDataReady_ = true;
                return;
            },TaskScheduler.FromCurrentSynchronizationContext());
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @param string with the type of the place we want to check
      * @param int with the id of the place we want to check
      * @return bool with true if the cooldown for visiting the given place is finished, in other case, false.
      * @brief This method returns true if the place with the given type and id is ready for a new
      * visit. The cooldown time is defined on the class gameRules, check that class for more information.
      */
    public bool cooldownVisitingPlaceFinished(string type, int id){
        if(currentUser_.visitedPlaces_.Exists(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id)){
            VisitedPlace place = currentUser_.visitedPlaces_.Find(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id);
            //si ya lo habia visitado devolvemos true si ha cumplido el cooldown
            // hay 10.000.000 de ticks en un segundo, * 60 son minutos
            return (place.lastVisitTimestamp_ + gameRules.getMinutesOfCooldown() * 600000000 < DateTime.Now.Ticks);
        }
        //si no lo habia visitado, el cooldown siempre se ha cumplido
        return true;
    }

    /**
      * @param StoredPlace the StoredPlace object that you want to check if you can visit it again.
      * @return bool with true if the cooldown for visiting the given place is finished, in other case, false.
      * @brief This method returns true if the place with the given type and id is ready for a new
      * visit. The cooldown time is defined on the class gameRules, check that class for more information.
      */
    public bool cooldownVisitingStoredPlaceFinished(StoredPlace storedPlace){
        if(storedPlace.visited()){
            //si ya lo habia visitado devolvemos true si ha cumplido el cooldown
            // hay 10.000.000 de ticks en un segundo, * 60 son minutos
            return (storedPlace.lastVisitTimestamp() + gameRules.getMinutesOfCooldown() * 600000000 < DateTime.Now.Ticks);
        }
        //si no lo habia visitado, el cooldown siempre se ha cumplido
        return true;
    }
    
    /**
      * @param string that contains the name of the place that you want to check if its ready to visit
      * it again
      * @return bool with true if the cooldown for visiting the given place is finished, in other case, false.
      * @brief This method returns true if the place with the given name is ready for a new
      * visit. The cooldown time is defined on the class gameRules, check that class for more information. You
      * have to be aware that its possible that two places has the same name, on that case, it will check
      * only the first one.
      */
    public bool cooldownVisitingPlaceByNameFinished(string name){
        Dictionary<string,string> typeAndId = findPlaceByName(name);
        return cooldownVisitingPlaceFinished(typeAndId["type"],Int32.Parse(typeAndId["id"]));
    }

    /**
      * @param string that contains the type of the place that current user has visited and you 
      * want to register that visit.
      * @param int that contains the id of the place that current user has visited and you 
      * want to register that visit.
      * @param long that contains the timestamp of the visit, the default value is -1. A negative
      * value means that you want to use the current timestamp not the given one.
      * @brief This method register a new visit for the place with the given type and id
      * and the given timestamp, if the given timestamp is negative it will use the current timestamp.
      * It also calls the writePlaceData and the writeUserData methods to upload the new visit 
      * information to the database.
      */
    public void userVisitedPlace(string type, int id, long timeOfTheVisit = -1){
        long currentTime = timeOfTheVisit <= 0 ? DateTime.Now.Ticks : timeOfTheVisit;
        currentUser_.newVisitAt(type,id,currentTime);

        requestHandler_.oneMoreVisitToPlaceByTypeAndId(type,id.ToString());
        writePlaceData(type,id.ToString());
        //writeUserData();
        writeAllUserProperties();
    }

    /**
      * @param string that contains the name of the place that current user has visited and you 
      * want to register that visit.
      * @param long that contains the timestamp of the visit, the default value is -1. A negative
      * value means that you want to use the current timestamp not the given one.
      * @brief This method search on all the places the first one who match the given name, and then,
      * it calls the userVisitedPlace method, check that method for more information. You have to be
      * aware that if two places has the same name, the new visit will be added only to the first one.
      */
    public void userVisitedPlaceByName(string name, long timeOfTheVisit = -1){
        Dictionary<string,string> typeAndId = findPlaceByName(name);
        userVisitedPlace(typeAndId["type"],Int32.Parse(typeAndId["id"]),timeOfTheVisit);
    }

    /**
      * @param string that contains the name of the place that you want to check if current
      * user has visit or not.
      * @return bool true if user has visit a place with that name, false in other case.
      * @brief This method search on the places that the current user has visited and check
      * if there is at least one place with that name, on that case it returns true, it returns false
      * in other case.
      */
    public bool hasUserVisitPlaceByName(string name){
        Dictionary<string,string> typeAndId = findPlaceByName(name);
        return currentUser_.hasVisitPlace(typeAndId["type"],Int32.Parse(typeAndId["id"]));
    }

    /**
      * @return Place place that should be used next.
      * @brief This method calls askForAPlace method of requestHandler and returns the place
      * that that method returns. If you want more information check the requestHandler documentation.
      * This method also instanciate a requestHandler if it doesnt exists when its called.
      */
    public Place askForAPlace(){
        if(requestHandler_ == null){
            requestHandler_ = new requestHandler(allPlaces_);
        }
        return requestHandler_.askForAPlace();
    }

    /**
      * @return bool true if all the places are downloaded and ready to use.
      */
    public bool placesAreReady(){
        return placesReady_;
    }

    /**
      * @return bool true if the user data is downloaded and ready to use.
      */
    public bool userDataIsReady(){
        return userDataReady_;
    }

    /**
      * @brief this method calls the sortPlaces method of the requestHandler class. Check
      * that class documentation to get more information.
      */
    public void sortPlaces(){
        requestHandler_.sortPlaces();
    }
    /**
      * @param string that contains the name of the searched place
      * @return Dictionary<string,string> it contains two entries, id and type, they contains
      * the id and the type of the place that matches the given name
      * @brief this method returns a dictionary that contains the id and the type of the place
      * that has the given name. If there isn't any place with that name, it will return a 
      * dictionary that contains two entries id with the string "-1" and type with the string
      * "noType". If there is more than one place with the given name, only the first one will
      * be returned.
      */
    public Dictionary<string,string> findPlaceByName(string name){
        Dictionary<string,string> toReturn = new Dictionary<string,string>();
        foreach(var type in allPlaces_.Keys){
            foreach(var id in allPlaces_[type].Keys){
                if(allPlaces_[type][id]["name_"] == name){
                    toReturn["id"] = id.ToString();
                    toReturn["type"] = type.ToString();
                    return toReturn;
                }
            }
        }
        Debug.Log($"Error en findPlaceByName, {name} not found");
        toReturn["id"] = "-1";
        toReturn["type"] = "noType";
        return toReturn;
    }

    /**
      * @return int with the total of places of types
      * @brief getter of the quantity of places
      */
    public int totalOfPlaces(){
        int count = 0;
        foreach(var type in allPlaces_.Keys){
            count += allPlaces_[type].Count;
        }
        return count;
    }

    /**
      * @param string that contains the name of a zone. Valid zones are determinated on 
      * mapRulesHandler class.
      * @return int the number of places that are on the given zone.
      * @brief this method counts all the places that have the given zone name on their
      * zone_ property. If there is no places on the given zone, it will show a message
      * on the console and return a 0. The valid zones are determinated on the mapRulesHandler
      * class, check its documentation for more information.
      */
    public int totalOfPlacesOfZone(string zone){
        int count = 0;
        foreach(var type in allPlaces_.Keys){
            foreach(var id in allPlaces_[type].Keys){
                if(allPlaces_[type][id]["zone_"] == zone){
                    count++;
                }
            }
        }
        if(count == 0){
            Debug.Log($"No se ha encontrado ningun lugar con el tipo {zone} en totalOfPlacesOfZone");
        }
        return count;
    }

    /**
      * @param string that contains the type of the searched place.
      * @param string that contains the id of the searched place.
      * @return Dictionary<string, string> dictionary of strings that contains a string
      * version of all the information of the searched place. 
      * @brief This method search on the property allPlaces_ if there is a place with
      * the given type and id. If there isn't any place that match both the given id 
      * and the given type, it will return null.
      */
    public Dictionary<string,string> getPlaceData(string type, string id){
        return allPlaces_[type][id];
    }

    /**
      * @brief This method is called when the application is closed, it saves all the offline 
      * information and stop the firebase online services.
      */
    void OnApplicationQuit(){
        FirebaseApp.DefaultInstance.Dispose();
        StoredPlace.saveAll();
    }

    /**
      * @param string that contains the displayName that you want to search
      * @param string that contains the reason of the search, it could be friend, chanllege or ranking. 
      * This reasons for the search has to be differenciated because users can allow or not be found by
      * other users.
      * @param SearchBar object that you want to advise when the search ends. This parameter is optional.
      * @brief This method tries to find other user on the list of users that allow be found by other users
      * on the different types of searchs. If a SearchBar object is given it will call the resultsOfTheSearch 
      * method of that class with a dictionary version of the found user.
      */
    public void SearchOtherUserByName(string toSearch, string type, SearchBar toAdvise = null){
    //                           displayName a buscar, friend/challenge o ranking
        List<string> listWhereSearch;
        if(type == "usersThatAllowFriendshipInvitations"){
            listWhereSearch = FriendData.usersThatAllowFriendshipInvitations_;
        }else if(type == "usersThatAllowBeChallenged"){
            listWhereSearch = FriendData.usersThatAllowBeChallenged_;
        }else if(type == "usersThatAllowAppearedOnRanking"){
            listWhereSearch = FriendData.usersThatAllowAppearedOnRanking_;
        }else{
            listWhereSearch = null;
        }
        
        if(listWhereSearch != null ){
            bool encontrado = false;
            for(int index = 0; index < listWhereSearch.Count; index++){
                string uid = listWhereSearch[index];
                FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/displayName_").GetValueAsync().ContinueWith(displayNameTask => {
                    Dictionary<string,string> toReturn = new Dictionary<string,string>();
                    if(displayNameTask.IsCompleted){
                        DataSnapshot snapshotDisplayName = displayNameTask.Result;
                        string displayNameConverted = JsonConvert.DeserializeObject<string>(snapshotDisplayName.GetRawJsonValue());
                        //como esta dentro de un for, segun vaya encontrando los mandara a results of the search uno a uno
                        if(displayNameConverted.Contains(toSearch)){
                            encontrado = true;
                            toReturn["uid"] = uid;
                            toReturn["name"] = displayNameConverted;
                            if(toAdvise != null){
                                toAdvise.resultsOfTheSearch(toReturn);
                            }
                        }
                    }else{
                        Debug.Log("Error: "+displayNameTask.Exception);
                    }
                    if(index == listWhereSearch.Count && toAdvise != null && !encontrado){
                        toAdvise.resultsOfTheSearch(toReturn);
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }

    /**
      * @param string that contains the user id of the player that the current user want to 
      * send a new friendship invitation.
      * @param SearchedPlayer object that handles the result of the sending
      * @brief This method tries to send a new friendship invitation of the current user to the
      * user that has the given id. The sending can produce three states:
      * - sended: all was OK and the invitation was sended successfully.
      * - failed: the sending failed because there is no internet connection or other error.
      * - repeated: the sending failed because that user already have an invitation of the current user.
      */
    public void sendFriendshipInvitation(string uidToInvite, SearchedPlayer searchedPlayerToAdvice){
    // envia una peticion de amistad del usuario current al uid que le digas
    // si le pasas un SearchedPlayer avisará con el resultado
    //  - sended, failed, repeated (ese usuario ya tenía una peticion de amistad tuya)
        FirebaseDatabase.DefaultInstance.GetReference($"users/{uidToInvite}/friendsInvitations_").GetValueAsync().ContinueWith(friendsInvitationsTask => {
            if (friendsInvitationsTask.IsFaulted) {
                Debug.Log($"Fallo al acceder a la lista de solicitudes de amistad de {uidToInvite}: "+friendsInvitationsTask.Exception);
                searchedPlayerToAdvice.resultOfTheSending("failed");
            }else if(friendsInvitationsTask.IsCompleted){
                DataSnapshot snapshotFriendsInvitations = friendsInvitationsTask.Result;
                List<string> friendsInvitationsConverted;
                if(snapshotFriendsInvitations.GetRawJsonValue() == null){
                    friendsInvitationsConverted = new List<string>();
                }else{
                    friendsInvitationsConverted = JsonConvert.DeserializeObject<List<string>>(snapshotFriendsInvitations.GetRawJsonValue());
                }
                if(friendsInvitationsConverted.IndexOf(auth.CurrentUser.UserId) != -1){
                    searchedPlayerToAdvice.resultOfTheSending("repeated");
                }else{
                    friendsInvitationsConverted.Add(auth.CurrentUser.UserId);
                    //subir la lista de amigos y notificar a searchedPlayerToAdvice
                    database.Child($"users/{uidToInvite}/friendsInvitations_").SetRawJsonValueAsync(JsonConvert.SerializeObject(friendsInvitationsConverted)).ContinueWith(uploadInvitationsTask => {
                        if(uploadInvitationsTask.IsFaulted){
                            Debug.Log("Error al subir la lista de amigos tras añadirle la nueva entrada: "+uploadInvitationsTask.Exception);
                            searchedPlayerToAdvice.resultOfTheSending("failed");
                        }else if(uploadInvitationsTask.IsCompleted){
                            searchedPlayerToAdvice.resultOfTheSending("sended");
                        }
                    },TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @brief This method is called when the firebaseHandler object
      * is destroyed. 
      */
    void OnDestroy(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /**
      * @param Scene the information of the scene it just has loaded.
      * @param LoadSceneMode object that contains the information of the
      * way that the scene was loaded.
      * @brief This method is called each time a scene is loaded, if placesReady_
      * variable is true, it calls the useStartIndex method of the requestHandler
      * object, to allow keeping with same places.
      */
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if(placesReady_){
            if(requestHandler_ == null){
                requestHandler_ = new requestHandler(allPlaces_);
            }
            requestHandler_.useStartIndex();
        }

        allowDownloadRankingData_ = scene.name == "PantallaRanking";
        if(scene.name != "PantallaRanking"){
            // en algunos casos se puede llamar a cambiar de escena incluso antes
            // de que se inicie sesión etc... como rankingData_ es lo ultimo que inicializamos
            // si esto no esta en inicializado esta clase deberia ser destruida o esperar a que
            // carge
            if(rankingData_ != null){
                rankingData_.Clear();
            }
        }
    }

    /**
      * @brief This method calls the updateStartIndex method of the 
      * requestHandler class to make the requestHandler choose new 
      * places.
      */
    public void askForNewPlaces(){
        requestHandler_.updateStartIndex();
    }

    /**
      * @param Dictionary<string,string> with the information of the score and the user id
      * that need to be uploaded.
      * @brief This method adds the given dictionary to the otherUserScoresToUpload_ list.
      */
    public void addOtherUserScoreToUpload(Dictionary<string,string> uidAndScore){
        otherUserScoresToUpload_.Add(uidAndScore);
    }

    /**
      * @return bool true if all the data of the users that appeard on the ranking
      * is already downloaded, false in other case.
      * @brief This method returns true if all the data of the users that appeard on
      * the ranking is already downloaded, false in other case.
      */
    public bool isRankingDataComplete(){
        //si usersThatAllowAppearedOnRanking_ es null, false
        //si no es null, mira si mide lo mismo que la lista de usuarios que permiten aparecer en el ranking
        return FriendData.usersThatAllowAppearedOnRanking_ != null && rankingData_.Count == FriendData.usersThatAllowAppearedOnRanking_.Count;
    }

    /**
      * @return bool with the value of downloadingRankingData_ property.
      * @brief getter of the downloadingRankingData_ property.
      */
    public bool isDownloadingRankingDataNow(){
        return downloadingRankingData_;
    }

    /**
      * @param string the user id of the user that you want to obtain the 
      * ranking data.
      * @return RankingPlayerData object with the data of the player who has
      * the given user id. 
      * @brief This method returns the RankingPlayerData object with the data of the
      * player who has the given user id. It would return null if there isnt any player
      * with that id.
      */
    public RankingPlayerData getRankingPlayerDataById(string uid){
        return rankingData_.Find(rankingPlayerData => rankingPlayerData.getUid() == uid);
    }

    /**
      * @return List<RankingPlayerData> the whole rankingData_ property.
      * @brief Getter of the rankingData_ propery.
      */
    public List<RankingPlayerData> getRankingPlayerData(){
        return rankingData_;
    }

    /**
      * @brief This method sorts the users that appeard on the ranking. It sorts
      * them setting on the firsts positions users with the highers scores.
      * It also updates the top_ property of the RankingPlayerData objects of the
      * lists using the setTop method.
      */
    public void sortRankingDataList(){
        rankingData_.Sort(delegate(RankingPlayerData a, RankingPlayerData b){
            if(a.getScore() == b.getScore()){
                return 0;
            }else if(a.getScore() > b.getScore()){
                //queremos ordenarlo al reves, el de mayor score primero
                return -1;
            }
            //debe ser <
            return 1;
        });
        for(int index = 0; index < rankingData_.Count; index++){
            //el 0 es el 1
            rankingData_[index].setTop(index+1);
        }
    }
}