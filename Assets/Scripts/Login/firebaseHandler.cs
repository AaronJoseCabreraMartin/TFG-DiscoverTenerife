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
 */ 
public class firebaseHandler : MonoBehaviour
{
    /**
      * @public
      * This public static attribute store the unique instance of this class.
      */
    public static firebaseHandler firebaseHandlerInstance_ = null;

    /**
      * 
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
      * It stores if the user data could be uptaded when it suffered any change. The possible values it
      * can have are:
      * - "false" : this state means that there is some changes on the user data that should be uploaded
      * as soon as the internet connection is avaible.
      * - "true" : this state means that there isnt any change on the user data that should be uploaded.
      * - "inProgress" : this state means that the user data is uploading right now, this state prevent
      * this class to make several calls to the uploading information method.
      */
    private string userDataUploaded_;

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

    private List<Dictionary<string,string>> otherUserScoresToUpload_;

    private bool uploadingNotifications_;

    private List<RankingPlayerData> rankingData_;

    private bool allowDownloadRankingData_;
    private bool downloadingRankingData_;

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

            if(readyForUploadChanges && userDataUploaded_ == "false"){
                writeUserData();
            }

            if(readyForUploadChanges && friendDataDownloadQueue_.Count != 0){
                downloadAllFriendData();
            }

            if(readyForUploadChanges && newFriendDataDownloadQueue_.Count != 0){
                downloadAllNewFriendInvitationsData();
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
                //Debug.Log($"userThatHaveToBeNotified = {userThatHaveToBeNotified}");
                //Debug.Log($"getStringConversionOfNewAcceptedFriends = " + currentUser_.getFriendDataByUID(userThatHaveToBeNotified).getStringConversionOfNewAcceptedFriends() );
                updateUserAddedAFriend(userThatHaveToBeNotified, 
                                        currentUser_.getFriendDataByUID(userThatHaveToBeNotified).getStringConversionOfNewAcceptedFriends());
            }

            if( allowDownloadRankingData_ && internetConnection() && firebaseDependenciesResolved && 
                    FriendData.usersThatAllowAppearedOnRanking_ != null && !downloadingRankingData_ && !isRankingDataComplete()){
                downloadRankingPlayerData();
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
            // Set a flag here indiciating that Firebase is ready to use by your
            // application.
            //CreateFirebaseObject();
            // Esto es para crear el objeto FirebaseApp, necesario para manejar los distintos metodos de firebase.
            firebaseApp = FirebaseApp.DefaultInstance;
            // para obtener el objeto Firebase.Auth.FirebaseAuth
            auth = Firebase.Auth.FirebaseAuth.GetAuth(firebaseApp);
            // con esto recogemos la referencia a la base de datos, para poder hacer operaciones de escritura o lectura.
            //database = FirebaseDatabase.GetInstance(firebaseApp).RootReference;
            //database = FirebaseDatabase.DefaultInstance.SetEditorDatabaseUrl("https://discovertenerife-fd031-default-rtdb.europe-west1.firebasedatabase.app/");
            database = FirebaseDatabase.DefaultInstance.RootReference;
            firebaseDependenciesResolved = true;
            firebaseDependenciesRunning = false;
            Debug.Log("Firebase Connected!!!");
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
                Debug.Log("User profile updated successfully.");
                
                Debug.Log($"Firebase user created successfully: {newUser.DisplayName} ({ newUser.UserId})");
                currentUser_ = new UserData(auth.CurrentUser);
                writeUserData();
                GameObject.Find("registerController").GetComponent<registerScreenController>().userCreatedSuccessfully(newUser.DisplayName);
                downloadAllPlaces();
                //no hay que hacer read por lo tanto, ya esta ready
                userDataReady_ = true;
                return;
            },TaskScheduler.FromCurrentSynchronizationContext());
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @param string that contains the user email that will try to login with.
      * @param string that contains the user password that will try to login with.
      * This method tries to start a sesion with the given email and password, if it finish successfully it will
      * call the downloading information methods and also call the emailLoginController userLogedSuccessfully method.
      */
    public void LoginUser(string email, string password){
        auth.SignInWithEmailAndPasswordAsync(email,password).ContinueWith(task => {
            if(task.Exception != null){
                Debug.LogError($"An exception has happened:{task.Exception}");
                GameObject.Find("emailLoginController").GetComponent<emailLoginController>().errorLoginUser($"{task.Exception}");

            }else{
                Debug.Log($"Se ha iniciado sesión correctamente: {task.Result.DisplayName} ({ task.Result.UserId})");
                //currentUser_ = new UserData(auth.CurrentUser);
                readUserData();
                GameObject.Find("emailLoginController").GetComponent<emailLoginController>().userLogedSuccessfully(task.Result.DisplayName);
                downloadAllPlaces();
            }
            return;
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * This method tries to starts a session as an anonymous user, if it finish successfully it calls the 
      * downloading information methods and it also calls the anonymousUserLoginSucessfully method of the 
      * anonymousButtonHandler class.
      */
    public void AnonymousUser(){
        //Si el current user es nulo, es que nunca se habia logeado o si con el que se ha loggeado tiene anonymous a false
        if(auth.CurrentUser == null || !auth.CurrentUser.IsAnonymous){
            auth.SignInAnonymouslyAsync().ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    GameObject.Find("anonymousButtonHandler").GetComponent<anonymousButtonHandler>().errorLoginAnonymousUser("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    GameObject.Find("anonymousButtonHandler").GetComponent<anonymousButtonHandler>().errorLoginAnonymousUser("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.Log($"Nuevo usuario anonimo registrado correctamente: {newUser.DisplayName} ({newUser.UserId})");
                currentUser_ = new UserData(auth.CurrentUser);
                userDataReady_ = true;
                writeUserData();
                //no hay que hacer read por lo tanto, ya esta ready
                downloadAllPlaces();
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            Debug.Log($"El usuario anonimo ya existia {auth.CurrentUser.DisplayName} {auth.CurrentUser.UserId}");
            readUserData();
            //currentUser_ = new UserData(auth.CurrentUser);
            downloadAllPlaces();
        }
        GameObject.Find("anonymousButtonHandler").GetComponent<anonymousButtonHandler>().anonymousUserLoginSucessfully();
    }

    /**
      * This method will be called when the google signin button is pressed, it start the google authentication
      * process calling the method OnAuthenticationFinished when it finish.
      */
    public void SignInWithGoogle()
    {
        if(!firebaseDependenciesResolved || firebaseDependenciesRunning){
            return;
        }
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished,TaskScheduler.FromCurrentSynchronizationContext());
    }
    
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn ();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser> ();
        signIn.ContinueWith (task => {
            if (task.IsCanceled) {
                Debug.Log("Google SingIn is cancelled!");
                signInCompleted.SetCanceled ();
            } else if (task.IsFaulted) {
                Debug.Log($"Google SingIn has an exception! {task.Exception}");
                signInCompleted.SetException (task.Exception);
            } else {
                Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential (((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask => {
                    if (authTask.IsCanceled) {
                        Debug.Log("Google SingIn on authentication is cancelled!");
                        signInCompleted.SetCanceled();
                    } else if (authTask.IsFaulted) {
                        Debug.Log($"Google SingIn on authentication has an exception! {authTask.Exception}");
                        signInCompleted.SetException(authTask.Exception);
                    } else {
        Debug.Log($"firebaseDependenciesResolved = {firebaseDependenciesResolved}");
                        signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);
                        GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal");
                        Debug.Log($"task.Result = {authTask.Result}");
                        Debug.Log("Welcome: " + authTask.Result.DisplayName + "!");
                        Debug.Log("Email = " + authTask.Result.Email);
                        Debug.Log($"auth.CurrentUser = {auth.CurrentUser}");
                        downloadAllPlaces();
                        readUserData();
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * This method end the current firebase user sesion.
      */
    public void LogOut()
    {
        auth.SignOut();
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
        userDataReady_ = false;
        StoredPlace.eraseStoredData();
    }

    ///// DATABASE METHODS /////
    //only for debugging purpose
    public void writeNewPlaceOnDataBase(Place place, string type, int placeID){
        database.Child("places").Child(type).Child(placeID.ToString()).SetRawJsonValueAsync(place.ToJson());
    }

    /**
      * This method uploads to the database all the current user releated information. It also upload 
      * the changes on the user social preferences. 
      */
    public void writeUserData(){
        Debug.Log("writeUserData..."+currentUser_.ToJson());
        //database.Child("users").Child(currentUser_.firebaseUserData_.UserId).SetRawJsonValueAsync(currentUser_.ToJson());
        userDataUploaded_ = "inProgress";
        database.Child("users").Child(currentUser_.firebaseUserData_.UserId).SetRawJsonValueAsync(currentUser_.ToJson()).ContinueWith(taskUploadUserData =>{
            Debug.Log("data wrote? " + (taskUploadUserData.IsCompleted ? "true" : "false") );
            uploadSocialPreferences();
            userDataUploaded_ = taskUploadUserData.IsCompleted ? "true" : "false";
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * This method download all the information that is releated to the user from the firebase database.
      */
    public void readUserData(){
        Debug.Log($"readUserData: {auth.CurrentUser.UserId}");
        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/visitedPlaces_").GetValueAsync().ContinueWith(taskPlaces => {
            if (taskPlaces.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+taskPlaces.Exception);
            } else if (taskPlaces.IsCompleted) {
                DataSnapshot snapshotVisitedPlaces = taskPlaces.Result;
                // por que si el usuario solo se registra y no visita ningun sitio en ese momento, 
                // el array de visitados sera null (que es lo que estoy pidiendo), entonces tengo que inicializar 
                // los datos como si fuera un nuevo usuario
                //              pos    data
                List<Dictionary<string,string>> visitedPlacesListVersion;
                //Debug.Log($"snapshotVisitedPlaces = {snapshotVisitedPlaces.GetRawJsonValue()}");
                if(snapshotVisitedPlaces.GetRawJsonValue() == null){
                    visitedPlacesListVersion = null;
                }else{
                    visitedPlacesListVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshotVisitedPlaces.GetRawJsonValue());
                }

                Dictionary<string,string> baseCordsData;
                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/baseCords_").GetValueAsync().ContinueWith(taskBaseCords => {
                    if (taskBaseCords.IsFaulted) {
                        // Handle the error...
                        Debug.Log("Error: "+taskBaseCords.Exception);
                    } else if (taskBaseCords.IsCompleted) {
                        DataSnapshot snapshotBaseCords = taskBaseCords.Result;
                        //Debug.Log($"snapshotBaseCords = {snapshotBaseCords.GetRawJsonValue()}");
                        // si la base data es null quiere decir que el usuario nunca llego a activar su servicio GPS
                        if(snapshotBaseCords.GetRawJsonValue() == null){
                            baseCordsData = null;
                        }else{
                            //                                                       name, number
                            baseCordsData = JsonConvert.DeserializeObject<Dictionary<string,string>>(snapshotBaseCords.GetRawJsonValue());
                        }
                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/friends_").GetValueAsync().ContinueWith(taskFriends => {
                            if (taskFriends.IsFaulted) {
                                // Handle the error...
                                Debug.Log("Error: "+taskFriends.Exception);
                            } else if (taskFriends.IsCompleted) {
                                DataSnapshot snapshotFriends = taskFriends.Result;
                                List<string> friendsList;
                                //Debug.Log($"snapshotFriends = {snapshotFriends.GetRawJsonValue()}"); 
                                if(snapshotFriends.GetRawJsonValue() == null){
                                    friendsList = null;
                                }else{
                                    //                                                UIDs
                                    friendsList = JsonConvert.DeserializeObject<List<string>>(snapshotFriends.GetRawJsonValue());
                                    foreach(string uid in friendsList){
                                        friendDataDownloadQueue_.Add(uid);
                                    }
                                }
                                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/friendsInvitations_").GetValueAsync().ContinueWith(taskFriendsInvitations => {
                                     if (taskFriendsInvitations.IsFaulted) {
                                        // Handle the error...
                                        Debug.Log("Error: "+taskFriendsInvitations.Exception);
                                    } else if (taskFriendsInvitations.IsCompleted) {
                                        DataSnapshot snapshotFriendsInvitations = taskFriendsInvitations.Result;
                                        List<string> friendsInvitationsList;
                                        //Debug.Log($"snapshotFriendsInvitations = {snapshotFriendsInvitations.GetRawJsonValue()}"); 
                                        if(snapshotFriendsInvitations.GetRawJsonValue() == null){
                                            friendsInvitationsList = null;
                                        }else{
                                            //                                                          UIDs
                                            friendsInvitationsList = JsonConvert.DeserializeObject<List<string>>(snapshotFriendsInvitations.GetRawJsonValue());
                                            foreach(string uid in friendsInvitationsList){
                                                newFriendDataDownloadQueue_.Add(uid);
                                            }
                                        }
                                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/acceptedFriendsInvitations_").GetValueAsync().ContinueWith(taskacceptedFriendsInvitations => {
                                            if (taskacceptedFriendsInvitations.IsFaulted) {
                                                // Handle the error...
                                                Debug.Log("Error: "+taskacceptedFriendsInvitations.Exception);
                                            } else if (taskacceptedFriendsInvitations.IsCompleted) {
                                                DataSnapshot snapshotacceptedFriendsInvitations = taskacceptedFriendsInvitations.Result;
                                                List<string> acceptedFriendsInvitationsList;
                                                //Debug.Log($"snapshotacceptedFriendsInvitations = {snapshotacceptedFriendsInvitations.GetRawJsonValue()}");
                                                bool haveToUploadData = false;
                                                if(snapshotacceptedFriendsInvitations.GetRawJsonValue() == null){
                                                    acceptedFriendsInvitationsList = null;
                                                }else{
                                                    //                                                                  UIDs
                                                    acceptedFriendsInvitationsList = JsonConvert.DeserializeObject<List<string>>(snapshotacceptedFriendsInvitations.GetRawJsonValue());
                                                    foreach(string uid in acceptedFriendsInvitationsList){
                                                        friendDataDownloadQueue_.Add(uid);
                                                    }
                                                    haveToUploadData = true;
                                                }
                                                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/deletedFriends_").GetValueAsync().ContinueWith(taskDeletedFriends => {
                                                    if (taskDeletedFriends.IsFaulted) {
                                                        // Handle the error...
                                                        Debug.Log("Error: "+taskDeletedFriends.Exception);
                                                    } else if (taskDeletedFriends.IsCompleted) {
                                                        DataSnapshot snapshotDeletedFriends = taskDeletedFriends.Result;
                                                        List<string> deletedFriendsList;
                                                        //Debug.Log($"snapshotDeletedFriends = {snapshotDeletedFriends.GetRawJsonValue()}");
                                                        if(snapshotDeletedFriends.GetRawJsonValue() == null){
                                                            deletedFriendsList = null;
                                                        }else{
                                                            deletedFriendsList = JsonConvert.DeserializeObject<List<string>>(snapshotDeletedFriends.GetRawJsonValue());
                                                            haveToUploadData = true; 
                                                        }
                                                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/challenges_").GetValueAsync().ContinueWith(taskChallenges => {
                                                            if (taskChallenges.IsFaulted) {
                                                                // Handle the error...
                                                                Debug.Log("Error: "+taskChallenges.Exception);
                                                            } else if (taskChallenges.IsCompleted) {
                                                                DataSnapshot snapshotChallenges = taskChallenges.Result;
                                                                List<Dictionary<string,string>> challengesList;
                                                                //Debug.Log($"snapshotChallenges = {snapshotChallenges.GetRawJsonValue()}");
                                                                if(snapshotChallenges.GetRawJsonValue() == null){
                                                                    challengesList = null;
                                                                }else{
                                                                    challengesList = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshotChallenges.GetRawJsonValue());
                                                                    haveToUploadData = true; 
                                                                }
                                                        
                                                                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/score_").GetValueAsync().ContinueWith(taskScore => {
                                                                    if (taskScore.IsFaulted) {
                                                                        // Handle the error...
                                                                        Debug.Log("Error: "+taskScore.Exception);
                                                                    } else if (taskScore.IsCompleted) {
                                                                        DataSnapshot snapshotScore = taskScore.Result;
                                                                        string userScore;
                                                                        //Debug.Log($"snapshotScore = {snapshotScore.GetRawJsonValue()}");
                                                                        if(snapshotScore.GetRawJsonValue() == null){
                                                                            userScore = null;
                                                                            haveToUploadData = true; 
                                                                        }else{
                                                                            userScore = JsonConvert.DeserializeObject<string>(snapshotScore.GetRawJsonValue());
                                                                        }
                                                                        
                                                                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/earnedScore_").GetValueAsync().ContinueWith(taskEarnedScore => {
                                                                            if (taskEarnedScore.IsFaulted) {
                                                                                // Handle the error...
                                                                                Debug.Log("Error: "+taskEarnedScore.Exception);
                                                                            } else if (taskEarnedScore.IsCompleted) {
                                                                                DataSnapshot snapshotEarnedScore = taskEarnedScore.Result;
                                                                                string userEarnedScore;
                                                                                //Debug.Log($"snapshotEarnedScore = {snapshotEarnedScore.GetRawJsonValue()}");
                                                                                if(snapshotEarnedScore.GetRawJsonValue() == null){
                                                                                    userEarnedScore = null;
                                                                                }else{
                                                                                    userEarnedScore = JsonConvert.DeserializeObject<string>(snapshotEarnedScore.GetRawJsonValue());
                                                                                    haveToUploadData = true;
                                                                                }

                                                                                currentUser_ = new UserData(auth.CurrentUser, visitedPlacesListVersion, baseCordsData, 
                                                                                                            friendsList, friendsInvitationsList, acceptedFriendsInvitationsList, 
                                                                                                            deletedFriendsList, challengesList, userScore, userEarnedScore);

                                                                                //por cualquiera de los caminos tiene que estar la user data lista
                                                                                userDataReady_ = true;
                                                                                if(haveToUploadData){
                                                                                    writeUserData();
                                                                                }
                                                                            
                                                                            }
                                                                        },TaskScheduler.FromCurrentSynchronizationContext());
                                                                    }
                                                                },TaskScheduler.FromCurrentSynchronizationContext());
                                                            }
                                                        },TaskScheduler.FromCurrentSynchronizationContext());
                                                    }
                                                },TaskScheduler.FromCurrentSynchronizationContext());
                                            }
                                        },TaskScheduler.FromCurrentSynchronizationContext());
                                    }
                                },TaskScheduler.FromCurrentSynchronizationContext());
                            }
                        },TaskScheduler.FromCurrentSynchronizationContext());
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());       
    }

    /**
      * @param type string that contains the type of the place that we want to upload.
      * @param id string that contains the id of the place that we want to upload.
      * @brief This method uploads the data from the place that matches the given type and id. If the
      * upload fails it adds the information of the place to placesToUploadQueue_
      */
    public void writePlaceData(string type, string id){
        Place place = requestHandler_.getPlaceByTypeAndId(type,id);
        database.Child("places").Child(type).Child(id).Child("timesItHasBeenVisited_").SetRawJsonValueAsync(place.getTimesItHasBeenVisited().ToString()).ContinueWith(uploadPlaceDataTask => {
            if(uploadPlaceDataTask.IsFaulted){
                Debug.Log("writePlaceData fallo!");
                if(placesToUpdateQueue_ == null){
                    placesToUpdateQueue_ = new List<Dictionary<string, string>>();
                }
                placesToUpdateQueue_.Add(new Dictionary<string,string>{{type,id}});
            }
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
        /*
        
        PARA DEBUGEAR RAPIDO
        
        */
        return true;
        /*
        
        PARA DEBUGEAR RAPIDO
        
        */
        if(currentUser_.visitedPlaces_.Exists(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id)){
            VisitedPlace place = currentUser_.visitedPlaces_.Find(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id);
            //si ya lo habia visitado devolvemos true si ha cumplido el cooldown
            // hay 10.000.000 de ticks en un segundo, * 60 son minutos
            //Debug.Log($"{place.lastVisitTimestamp_} + {gameRules.getMinutesOfCooldown() * 10000000 * 60}\n {place.lastVisitTimestamp_ + gameRules.getMinutesOfCooldown() * 10000000 * 60} < {DateTime.Now.Ticks} ? ");
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
        /*
        
        PARA DEBUGEAR RAPIDO
        
        */
        return true;
        /*
        
        PARA DEBUGEAR RAPIDO
        
        */
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
        writeUserData();
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
      * @brief This method start as many coroutines as types of places are defined on mapRulesHandler.
      * Each coroutine will download the information of that type of place. If you want more information
      * check the mapRulesHandler documentation and the downloadOneTypeOfSite method of this class.
      */
    private void downloadAllPlaces(){
        foreach(string typeSite in mapRulesHandler.getTypesOfSites()){
            //StartCoroutine es como olvidate de esto hasta que termine, 
            //cuando termina ejecuta la siguiente linea como si no hubiera pasado nada
            //es para que no se pause la app mientras se descargan los sitios
            StartCoroutine(downloadOneTypeOfSite(typeSite));
        }
        
    }

    /**
      * @param string that contains the type of the sites you want to download the information.
      * @brief This coroutine tries to download all the places of the given type. If there is no
      * internet connection it waits until there is avaible again. When all the places of all types
      * has already downloaded it sets ON the placesReady_ flag.
      */
    private IEnumerator downloadOneTypeOfSite(string typeSite){
        while(!internetConnection()){//debemos esperar a tener conexion
            yield return new WaitForSeconds(0.5f);
        }
        FirebaseDatabase.DefaultInstance.GetReference($"places/{typeSite}/").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+task.Exception);
            } else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                //           id                 data
                List<Dictionary<string,string>> listVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshot.GetRawJsonValue());
                Dictionary<string,Dictionary<string,string>> dictionaryVersion = new Dictionary<string,Dictionary<string,string>>();
                for(int i = 0; i <listVersion.Count; i++ ){
                    dictionaryVersion[i.ToString()] = listVersion[i];
                }
                allPlaces_[typeSite] = dictionaryVersion;
                if(allPlaces_.Count == mapRulesHandler.getTypesOfSites().Count){
                    placesReady_ = true;
                    Debug.Log("Places ready!");
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
        yield return new WaitForSeconds(0);
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
      * @brief this method tries to upload all the information that is on the placesToUpdateQueue_
      * property. If it success, it erase the place from the list, but if its fails the writePlaceData
      * will add again the place to the queue.
      */
    void uploadPlacesQueue(){
        List<Dictionary<string,string>> toRemove = new List<Dictionary<string,string>>();
        foreach(Dictionary<string,string> placeToUpdate in placesToUpdateQueue_){
            foreach(string key in placeToUpdate.Keys){
                toRemove.Add(placeToUpdate);
                writePlaceData(key, placeToUpdate[key]);
            }
        }
        foreach(Dictionary<string,string> placeToRemove in toRemove){
                placesToUpdateQueue_.Remove(placeToRemove);
        }
    }

    /**
      * @brief this method calls the downloadFriendData method with each user id that is 
      * on the friendDataDownloadQueue_.
      */
    private void downloadAllFriendData(){
        for(int index = 0; index < currentUser_.countOfFriends(); index++){
            string uid = currentUser_.getFriend(index);
            if(friendDataDownloadQueue_.Exists(friendDataForDownload => friendDataForDownload == uid)){
                friendDataDownloadQueue_.Remove(uid);
                downloadFriendData(uid);
            }
        }
    }

    /**
      * @param string that contains the user id of the friend of the current user that you want to
      * obtain data.
      * @brief this method tries to download the necesary information of the given user id. If it fails
      * on the process it adds the user id to the friendDataDownloadQueue_ list. If it success it calls
      * the addFriendData method of UserData class creating a new FriendData object.
      */
    public void downloadFriendData(string uid){
        FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/displayName_").GetValueAsync().ContinueWith(displayNameTask => {
            if (displayNameTask.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+displayNameTask.Exception);
                friendDataDownloadQueue_.Add(uid);
            } else if (displayNameTask.IsCompleted) {
                DataSnapshot snapshotDisplayName = displayNameTask.Result;
                string displayName;
                if(snapshotDisplayName.GetRawJsonValue() == null){
                    displayName = "null";
                }else{
                    displayName = JsonConvert.DeserializeObject<string>(snapshotDisplayName.GetRawJsonValue());
                }
                FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/deletedFriends_").GetValueAsync().ContinueWith(deletedFriendsTask => {
                    if (deletedFriendsTask.IsFaulted) {
                        // Handle the error...
                        Debug.Log("Error: "+deletedFriendsTask.Exception);
                        friendDataDownloadQueue_.Add(uid);
                    } else if (deletedFriendsTask.IsCompleted) {
                        DataSnapshot deletedFriendsSnapshot = deletedFriendsTask.Result;
                        List<string> deletedFriends;
                        if(deletedFriendsSnapshot.GetRawJsonValue() == null){
                            deletedFriends = new List<string>();
                        }else{
                            deletedFriends = JsonConvert.DeserializeObject<List<string>>(deletedFriendsSnapshot.GetRawJsonValue());
                        }
                        FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/challenges_").GetValueAsync().ContinueWith(challengesTask => {
                            if (challengesTask.IsFaulted) {
                                // Handle the error...
                                Debug.Log("Error: "+challengesTask.Exception);
                                friendDataDownloadQueue_.Add(uid);
                            } else if (challengesTask.IsCompleted) {
                                DataSnapshot challengesSnapshot = challengesTask.Result;
                                List<Dictionary<string,string>> challenges;
                                if(challengesSnapshot.GetRawJsonValue() == null){
                                    challenges = new List<Dictionary<string,string>>();
                                }else{
                                    challenges = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(challengesSnapshot.GetRawJsonValue());
                                }
                                FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/acceptedFriendsInvitations_").GetValueAsync().ContinueWith(acceptedNewFriendsTask => {
                                    if (acceptedNewFriendsTask.IsFaulted) {
                                        // Handle the error...
                                        Debug.Log("Error: "+acceptedNewFriendsTask.Exception);
                                        friendDataDownloadQueue_.Add(uid);
                                    } else if (acceptedNewFriendsTask.IsCompleted) {
                                        DataSnapshot acceptedNewFriendsSnapshot = acceptedNewFriendsTask.Result;
                                        List<string> acceptedNewFriends;
                                        if(acceptedNewFriendsSnapshot.GetRawJsonValue() == null){
                                            acceptedNewFriends = new List<string>();
                                        }else{
                                            acceptedNewFriends = JsonConvert.DeserializeObject<List<string>>(acceptedNewFriendsSnapshot.GetRawJsonValue());
                                        }
                                        
                                        if(currentUser_.hasToBeNotified(uid)){
                                            acceptedNewFriends.Add(currentUser_.getUid());
                                        }
                                        currentUser_.addFriendData(new FriendData(uid, displayName, deletedFriends, challenges, acceptedNewFriends));
                                    }
                                },TaskScheduler.FromCurrentSynchronizationContext());
                            }
                        },TaskScheduler.FromCurrentSynchronizationContext());
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @brief This method calls downloadNewFriendInvitationData method with each of the 
      * user ids that are on the new friends invitations list of the current user.
      */
    private void downloadAllNewFriendInvitationsData(){
        for(int index = 0; index < currentUser_.countOfNewFriends(); index++){
            string uid = currentUser_.getNewFriend(index);
            if(newFriendDataDownloadQueue_.Exists(newFriendDataForDownload => newFriendDataForDownload == uid)){
                newFriendDataDownloadQueue_.Remove(uid);
                downloadNewFriendInvitationData(uid);
            }
        }
    }

    /**
      * @param string that contains the user id that you want to download the enough information
      * for create the newFriendInvitation object.
      * @brief this method tries to download the display name and the list of newFriendsInvitations 
      * of the user that matches the given user id. If its fails it adds the user id to the 
      * newFriendDataDownloadQueue_ list and if its success, its calls the addNewFriendData method of
      * UserData.
      */
    private void downloadNewFriendInvitationData(string uid){
        FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/displayName_").GetValueAsync().ContinueWith(displayNameTask => {
            if (displayNameTask.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+displayNameTask.Exception);
                newFriendDataDownloadQueue_.Add(uid);
            } else if (displayNameTask.IsCompleted) {
                DataSnapshot snapshotDisplayName = displayNameTask.Result;
                string displayName;
                if(snapshotDisplayName.GetRawJsonValue() == null){
                    displayName = "null";
                }else{
                    displayName = JsonConvert.DeserializeObject<string>(snapshotDisplayName.GetRawJsonValue());
                }
                FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/acceptedFriendsInvitations_").GetValueAsync().ContinueWith(acceptedNewFriendsTask => {
                    if (acceptedNewFriendsTask.IsFaulted) {
                        // Handle the error...
                        Debug.Log("Error: "+acceptedNewFriendsTask.Exception);
                        newFriendDataDownloadQueue_.Add(uid);
                    } else if (acceptedNewFriendsTask.IsCompleted) {
                        DataSnapshot newFriendsSnapshot = acceptedNewFriendsTask.Result;
                        List<string> newFriends;
                        if(newFriendsSnapshot.GetRawJsonValue() == null){
                            newFriends = new List<string>();
                        }else{
                            newFriends = JsonConvert.DeserializeObject<List<string>>(newFriendsSnapshot.GetRawJsonValue());
                        }
                        currentUser_.addNewFriendData(new newFriendData(uid, displayName, newFriends));
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @param string that contains the user that will has erased the friendship with the current user.
      * @param string that contains the deleted friends list of the user that has the given id in
      * the JSON format.
      * @brief This method must be called when there is internet connection, otherwise it wont 
      * to nothing. This method upload the deletedFriends property of the user that has the given id.
      */
    public void updateUserDeleteAFriend(string noFriendUid,string deletedFriendsListInJSON){
        //uso los metodos de friend data y luego llamo aqui solo con la info que hay para subir
        //TIENE que haber internet, sino no dejo hacer nada en friends
        database.Child("users").Child(noFriendUid).Child("deletedFriends_").SetRawJsonValueAsync(deletedFriendsListInJSON);
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
    //                                                displayName a buscar, friend/challenge o ranking
        Dictionary<string,string> toReturn = new Dictionary<string,string>();
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
        bool searching = false;
        if(listWhereSearch != null ){
            searching = true;
            int finishedCounter = 0;
            bool encontrado = false;
            for(int index = 0; index < listWhereSearch.Count; index++){
                string uid = listWhereSearch[index]; 
                FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/displayName_").GetValueAsync().ContinueWith(displayNameTask => {
                    finishedCounter++;
                    if(displayNameTask.IsCompleted){
                        DataSnapshot snapshotDisplayName = displayNameTask.Result;
                        string displayNameConverted = JsonConvert.DeserializeObject<string>(snapshotDisplayName.GetRawJsonValue());
                        Debug.Log($"displayNameConverted = {displayNameConverted}, toSearch = {toSearch}, finishedCounter = {finishedCounter}");
                        if(displayNameConverted == toSearch){
                            encontrado = true;
                            toReturn["uid"] = uid;
                            toReturn["name"] = toSearch;
                            if(toAdvise != null){
                                toAdvise.resultsOfTheSearch(toReturn);
                            }
                        }
                    }else{
                        Debug.Log("Error: "+displayNameTask.Exception);
                    }
                    if(finishedCounter == listWhereSearch.Count && toAdvise != null && !encontrado){
                        toAdvise.resultsOfTheSearch(toReturn);
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        if(!searching && toAdvise != null){
            toAdvise.resultsOfTheSearch(toReturn);
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

        if(scene.name == "PantallaRanking"){
            allowDownloadRankingData_ = true;
        }else{
            allowDownloadRankingData_ = false;
            rankingData_.Clear();
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
      * @brief This method download the lists of the user social permissions.
      * They are usersThatAllowFriendshipInvitations, usersThatAllowBeChallenged and
      * usersThatAllowAppearedOnRanking. The downloadingAnyOfTheUsersPermissionsLists_
      * is set as a true. The social permissions are downloaded in a parallel way for
      * searching more efficiency if some of them failed the downloadingAnyOfTheUsersPermissionsLists_
      * property will be set as false again.
      */
    private void downloadUsersPermissionsLists(){
        int countOfListsDownloaded = 0;
        downloadingAnyOfTheUsersPermissionsLists_ = true;
        if(FriendData.usersThatAllowFriendshipInvitations_ == null){
            FirebaseDatabase.DefaultInstance.GetReference($"users/usersThatAllowFriendshipInvitations").GetValueAsync().ContinueWith(searchTask => {
                if (searchTask.IsFaulted || searchTask.IsCanceled) {
                    Debug.Log("Fallo al descargar la lista de usersThatAllowFriendshipInvitations "+searchTask.Exception);
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }else if(searchTask.IsCompleted){
                    DataSnapshot snapshotSearch = searchTask.Result;
                    if(snapshotSearch.GetRawJsonValue() != null){
                        FriendData.usersThatAllowFriendshipInvitations_ = JsonConvert.DeserializeObject<List<string>>(snapshotSearch.GetRawJsonValue());
                    }
                }
                if(countOfListsDownloaded == 3){
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfListsDownloaded++;
        }

        if(FriendData.usersThatAllowBeChallenged_ == null){
            FirebaseDatabase.DefaultInstance.GetReference($"users/usersThatAllowBeChallenged").GetValueAsync().ContinueWith(searchTask => {
                if (searchTask.IsFaulted || searchTask.IsCanceled) {
                    Debug.Log("Fallo al descargar la lista de usersThatAllowBeChallenged "+searchTask.Exception);
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }else if(searchTask.IsCompleted){
                    DataSnapshot snapshotSearch = searchTask.Result;
                    if(snapshotSearch.GetRawJsonValue() != null){
                        FriendData.usersThatAllowBeChallenged_ = JsonConvert.DeserializeObject<List<string>>(snapshotSearch.GetRawJsonValue());
                    }
                }
                if(countOfListsDownloaded == 3){
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfListsDownloaded++;
        }

        if(FriendData.usersThatAllowAppearedOnRanking_ == null){
            FirebaseDatabase.DefaultInstance.GetReference($"users/usersThatAllowAppearedOnRanking").GetValueAsync().ContinueWith(searchTask => {
                if (searchTask.IsFaulted || searchTask.IsCanceled) {
                    Debug.Log("Fallo al descargar la lista de usersThatAllowAppearedOnRanking "+searchTask.Exception);
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }else if(searchTask.IsCompleted){
                    DataSnapshot snapshotSearch = searchTask.Result;
                    if(snapshotSearch.GetRawJsonValue() != null){
                        FriendData.usersThatAllowAppearedOnRanking_ = JsonConvert.DeserializeObject<List<string>>(snapshotSearch.GetRawJsonValue());
                    }
                }
                if(countOfListsDownloaded == 3){
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfListsDownloaded++;
        }
    }

    /**
      * @param FriendData object that contains all the information releated
      * to the user that own the challenges we want to upload.
      * @brief this method upload the challengue list of the given user. 
      */
    public void uploadFriendChallengesOf(FriendData friendDataToUpload){
        //TIENE que haber internet
        database.Child("users").Child(friendDataToUpload.getUid()).Child("challenges_").SetRawJsonValueAsync(friendDataToUpload.getStringConversionOfChallenges());
    }

    /**
      * @brief this method should be called either when there is changes on the current user's social preferences or
      * the last changes couldnt been uploaded on the past. It checks if there is any change on the three permision
      * and upload to the firebase database the changes on the preferences. It checks if the three lists are uploaded,
      * if one or more of them fails on the uploading, it puts the hasToUploadSocialPreferences_ property to true, so
      * this method will be called again when the internet connection is ready.
      */
    public void uploadSocialPreferences(){
        bool changesToUpload = false;
        int countOfDone = 0;
        string currentUserId = currentUser_.firebaseUserData_.UserId.ToString();
        if(optionsController.optionsControllerInstance_.socialOptions("addMe") && !FriendData.usersThatAllowFriendshipInvitations_.Exists(uid => uid == currentUserId)){
            //el usuario permite recibir peticiones de amistad y no está en la lista
            FriendData.usersThatAllowFriendshipInvitations_.Add(currentUserId);
            changesToUpload = true;
        }else if(!optionsController.optionsControllerInstance_.socialOptions("addMe") && FriendData.usersThatAllowFriendshipInvitations_.Exists(uid => uid == currentUserId)){
            //el usuario NO permite recibir peticiones de amistad y está en la lista
            FriendData.usersThatAllowFriendshipInvitations_.Remove(currentUserId);
            changesToUpload = true;
        }

        if(changesToUpload){
            string stringConversion = JsonConvert.SerializeObject(FriendData.usersThatAllowFriendshipInvitations_);
            database.Child("users").Child("usersThatAllowFriendshipInvitations").SetRawJsonValueAsync(stringConversion).ContinueWith(uploadListTask => {
                if(uploadListTask.IsCompleted){
                    countOfDone++;
                    if(countOfDone == 3){
                        hasToUploadSocialPreferences_ = false;
                    }
                }else{
                    hasToUploadSocialPreferences_ = true;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfDone++;
            if(countOfDone == 3){
                hasToUploadSocialPreferences_ = false;
            }
        }

        changesToUpload = false;
        if(optionsController.optionsControllerInstance_.socialOptions("challengeMe") && !FriendData.usersThatAllowBeChallenged_.Exists(uid => uid == currentUserId)){
            //el usuario permite recibir retos y no está en la lista
            FriendData.usersThatAllowBeChallenged_.Add(currentUserId);
            changesToUpload = true;
        }else if(!optionsController.optionsControllerInstance_.socialOptions("challengeMe") && FriendData.usersThatAllowBeChallenged_.Exists(uid => uid == currentUserId)){
            //el usuario NO permite recibir retos y está en la lista
            FriendData.usersThatAllowBeChallenged_.Remove(currentUserId);
            changesToUpload = true;
        }

        if(changesToUpload){
            string stringConversion = JsonConvert.SerializeObject(FriendData.usersThatAllowBeChallenged_);
            database.Child("users").Child("usersThatAllowBeChallenged").SetRawJsonValueAsync(stringConversion).ContinueWith(uploadListTask => {
                if(uploadListTask.IsCompleted){
                    countOfDone++;
                    if(countOfDone == 3){
                        hasToUploadSocialPreferences_ = false;
                    }
                }else{
                    hasToUploadSocialPreferences_ = true;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfDone++;
            if(countOfDone == 3){
                hasToUploadSocialPreferences_ = false;
            }
        }

        changesToUpload = false;
        if(optionsController.optionsControllerInstance_.socialOptions("ranking") && !FriendData.usersThatAllowAppearedOnRanking_.Exists(uid => uid == currentUserId)){
            //el usuario permite aparecer en el ranking y no está en la lista
            FriendData.usersThatAllowAppearedOnRanking_.Add(currentUserId);
            changesToUpload = true;
        }else if(!optionsController.optionsControllerInstance_.socialOptions("ranking") && FriendData.usersThatAllowAppearedOnRanking_.Exists(uid => uid == currentUserId)){
            //el usuario NO permite aparecer en el ranking y está en la lista
            FriendData.usersThatAllowAppearedOnRanking_.Remove(currentUserId);
            changesToUpload = true;
        }

        if(changesToUpload){
            string stringConversion = JsonConvert.SerializeObject(FriendData.usersThatAllowAppearedOnRanking_);
            database.Child("users").Child("usersThatAllowAppearedOnRanking").SetRawJsonValueAsync(stringConversion).ContinueWith(uploadListTask => {
                if(uploadListTask.IsCompleted){
                    countOfDone++;
                    if(countOfDone == 3){
                        hasToUploadSocialPreferences_ = false;
                    }
                }else{
                    hasToUploadSocialPreferences_ = true;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfDone++;
            if(countOfDone == 3){
                hasToUploadSocialPreferences_ = false;
            }
        }
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
      * @brief This method tries to upload the information of the first element of
      * otherUserScoresToUpload_ list, it removes the first element just before
      * of trying to upload it. If it fails on the uploading, it adds again the 
      * dictionary with the information to the otherUserScoresToUpload_ list.
      * The information it uploads is the the earnedScore_ to the user id.
      */
    void uploadOtherUserScores(){
        string uid = otherUserScoresToUpload_[0]["uid_"];
        string score = "\"" + otherUserScoresToUpload_[0]["score_"] + "\"";
        otherUserScoresToUpload_.RemoveAt(0);
        database.Child("users").Child(uid).Child("earnedScore_").SetRawJsonValueAsync(score).ContinueWith(uploadScoreTask => {
            if(!uploadScoreTask.IsCompleted){
                Dictionary<string,string> toUpload = new Dictionary<string,string>();
                toUpload["uid_"] = uid;
                toUpload["score_"] = score;
                otherUserScoresToUpload_.Add(toUpload);
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @param string user id of the friend that has been acepted
      * @param string with the conversion of the accepted friendships invitations in JSON 
      * format.
      * @brief This method tries to upload the accepted friendships invitations list of the 
      * given user. If it complete it succesfully, it removes the friendUid of the list of the 
      * user that need to be notified with the hasBeenNotified method. It also sets the
      * uploadingNotifications_ property as true but when the try of updating finish, in
      * any way, it sets the uploadingNotifications_ property as false.
      */
    public void updateUserAddedAFriend(string friendUid, string acceptedFriendsInvitationsListInJSON){
        uploadingNotifications_ = true;
        //uso los metodos de friend data y luego llamo aqui solo con la info que hay para subir
        //TIENE que haber internet, sino no dejo hacer nada en friends
        database.Child("users").Child(friendUid).Child("acceptedFriendsInvitations_").SetRawJsonValueAsync(acceptedFriendsInvitationsListInJSON).ContinueWith(uploadScoreTask => {
            if(uploadScoreTask.IsCompleted){
                currentUser_.hasBeenNotified(friendUid);
            }
            uploadingNotifications_ = false;
        },TaskScheduler.FromCurrentSynchronizationContext());;
    }

    /**
      * @brief This method downloads all the data what will be shown on the players ranking
      * and store it as a RankingPlayerData object on the rankingData_ list. It changes the
      * downloadingRankingData_ attribute to reflect the current state of the download.
      */
    public void downloadRankingPlayerData(){
        //Solo necesito descargarme el nombre y el score, con el score puedo calcular el top y el rango.
        downloadingRankingData_ = true;
        foreach(string userId in FriendData.usersThatAllowAppearedOnRanking_){
            FirebaseDatabase.DefaultInstance.GetReference($"users/{userId}/displayName_").GetValueAsync().ContinueWith(displayNameTask => {    
                if (displayNameTask.IsFaulted || displayNameTask.IsCanceled) {
                    downloadingRankingData_ = false;
                    rankingData_.Clear();
                    Debug.Log($"Fallo al descargar la informacion de ranking de {userId} :"+displayNameTask.Exception);
                }else if(displayNameTask.IsCompleted){
                    DataSnapshot snapshotDisplayName = displayNameTask.Result;
                    if(snapshotDisplayName.GetRawJsonValue() != null){
                        string displayName = JsonConvert.DeserializeObject<string>(snapshotDisplayName.GetRawJsonValue());
                        
                        FirebaseDatabase.DefaultInstance.GetReference($"users/{userId}/score_").GetValueAsync().ContinueWith(scoreTask => {    
                            if (scoreTask.IsFaulted || scoreTask.IsCanceled) {
                                downloadingRankingData_ = false;
                                rankingData_.Clear();
                                Debug.Log($"Fallo al descargar la informacion de ranking de {userId} :"+scoreTask.Exception);
                            }else if(scoreTask.IsCompleted){
                                DataSnapshot snapshotScore = scoreTask.Result;
                                int score;
                                if(snapshotScore.GetRawJsonValue() != null){
                                    score = Int32.Parse(JsonConvert.DeserializeObject<string>(snapshotScore.GetRawJsonValue()));

                                }else{
                                    score = 0;
                                    Debug.Log($"users/{userId}/score_ es null");
                                }
                                rankingData_.Add(new RankingPlayerData(displayName, score, userId));

                                if(isRankingDataComplete()){
                                    downloadingRankingData_ = false;
                                }
                            }
                        },TaskScheduler.FromCurrentSynchronizationContext());
                    
                    }else{
                        Debug.Log($"users/{userId}/displayName_ es null");
                    }
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }
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