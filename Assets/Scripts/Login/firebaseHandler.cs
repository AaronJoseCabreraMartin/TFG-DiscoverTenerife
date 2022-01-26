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

public class firebaseHandler : MonoBehaviour
{

    public static firebaseHandler firebaseHandlerInstance_ = null;

    FirebaseApp firebaseApp = null;
    internal Firebase.Auth.FirebaseAuth auth = null;
    internal DatabaseReference database = null;
    //EL USUARIO ACTUAL ESTA EN auth.CurrentUser
    
    private GoogleSignInConfiguration configuration;

    public UserData actualUser_ = null;
    private bool firebaseDependenciesResolved = false;
    private bool placesReady_ = false;
    private bool userDataReady_ = false;
    //              type of place          id                 DATA
    private Dictionary<string,Dictionary<string,Dictionary<string,string>>> allPlaces_ = new Dictionary<string,Dictionary<string,Dictionary<string,string>>>();

    private requestHandler requestHandler_;

    private List<Dictionary<string, string>> placesToUpdateQueue_;
    private string userDataUploaded_;
    private List<string> friendDataDownloadQueue_;
    private List<string> newFriendDataDownloadQueue_;
    

    private void Awake() {
        //GameObject[] objs = GameObject.FindGameObjectsWithTag("firebaseHandler");
        //if (objs.Length > 1) //si ya existe una firebaseHandler no crees otra
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
    }

    void Update(){
        if(!firebaseDependenciesResolved){
            // cuando termine           A                           ejecuta         B                   con este contexto (para acceder a las cosas privadas)
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(CheckDependenciesFirebase,TaskScheduler.FromCurrentSynchronizationContext());
        }
        bool readyForUpdateChanges = internetConnection() && userDataReady_ && placesReady_ && firebaseDependenciesResolved;
        //Si hay conexion y no se han enviado las visitas offline, envialas.
        if(readyForUpdateChanges && StoredPlace.changesToUpdate_ ){
            StoredPlace.UpdateChanges();
        }

        if(readyForUpdateChanges && placesToUpdateQueue_ != null){
            if(placesToUpdateQueue_.Count == 0){
                placesToUpdateQueue_ = null;
            }else{
                uploadPlacesQueue();
            }
        }

        if(readyForUpdateChanges && userDataUploaded_ == "false"){
            writeUserData();
        }

        if(readyForUpdateChanges && friendDataDownloadQueue_.Count != 0){
            downloadAllFriendData();
        }

        if(readyForUpdateChanges && newFriendDataDownloadQueue_.Count != 0){
            downloadAllNewFriendInvitationsData();
        }
    }
    
    public bool internetConnection(){
        //string toShow = $"InternetConnection: ";
        //toShow+=$"CarrierDataNetwork = {Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork} ";
        //toShow+=$"LocalAreaNetwork = {Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork}";
        //Debug.Log(toShow);
        return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || 
                Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
    }

    private void CheckDependenciesFirebase(Task<DependencyStatus> task) {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Set a flag here indiciating that Firebase is ready to use by your
            // application.
            CreateFirebaseObject();
            firebaseDependenciesResolved = true;
            Debug.Log("Firebase Connected!!!");
            //ya habia una sesion iniciada antes!
            if(auth != null){
                Debug.Log("YA HABIA UNA SESION INICIADA!! "+ (auth.CurrentUser.IsAnonymous ? "Anonymous" : auth.CurrentUser.DisplayName));
                ChangeScene.changeScene("PantallaPrincipal");
                downloadAllPlaces();
                readUserData();
            }
        }else{
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
            firebaseDependenciesResolved = false;
        }
    }

    private void CreateFirebaseObject(){
        // Esto es para crear el objeto FirebaseApp, necesario para manejar los distintos metodos de firebase.
        firebaseApp = FirebaseApp.DefaultInstance;
        // para obtener el objeto Firebase.Auth.FirebaseAuth
        auth = Firebase.Auth.FirebaseAuth.GetAuth(firebaseApp);
        // con esto recogemos la referencia a la base de datos, para poder hacer operaciones de escritura o lectura.
        //database = FirebaseDatabase.GetInstance(firebaseApp).RootReference;
        //database = FirebaseDatabase.DefaultInstance.SetEditorDatabaseUrl("https://discovertenerife-fd031-default-rtdb.europe-west1.firebasedatabase.app/");
        database = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public bool FirebaseDependenciesAreResolved(){
        return firebaseDependenciesResolved;
    }

    ///// USER ACCOUNTS METHODS /////
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
                actualUser_ = new UserData(auth.CurrentUser);
                writeUserData();
                GameObject.Find("registerController").GetComponent<registerScreenController>().userCreatedSuccessfully(newUser.DisplayName);
                downloadAllPlaces();
                //no hay que hacer read por lo tanto, ya esta ready
                userDataReady_ = true;
                return;
            },TaskScheduler.FromCurrentSynchronizationContext());
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void LoginUser(string email, string password){
        auth.SignInWithEmailAndPasswordAsync(email,password).ContinueWith(task => {
            if(task.Exception != null){
                Debug.LogError($"An exception has happened:{task.Exception}");
                GameObject.Find("emailLoginController").GetComponent<emailLoginController>().errorLoginUser($"{task.Exception}");

            }else{
                Debug.Log($"Se ha iniciado sesión correctamente: {task.Result.DisplayName} ({ task.Result.UserId})");
                //actualUser_ = new UserData(auth.CurrentUser);
                readUserData();
                GameObject.Find("emailLoginController").GetComponent<emailLoginController>().userLogedSuccessfully(task.Result.DisplayName);
                downloadAllPlaces();
            }
            return;
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

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
                actualUser_ = new UserData(auth.CurrentUser);
                userDataReady_ = true;
                writeUserData();
                //no hay que hacer read por lo tanto, ya esta ready
                downloadAllPlaces();
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            Debug.Log($"El usuario anonimo ya existia {auth.CurrentUser.DisplayName} {auth.CurrentUser.UserId}");
            readUserData();
            //actualUser_ = new UserData(auth.CurrentUser);
            downloadAllPlaces();
        }
        GameObject.Find("anonymousButtonHandler").GetComponent<anonymousButtonHandler>().anonymousUserLoginSucessfully();
    }

    //button function
    public void SignInWithGoogle()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }
    
    // internal es que solo tienen acceso este fichero!? algo asi lei
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        string errors = "";
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    errors += "Got Error: " + error.Status + " " + error.Message + "\n";
                }
                else
                {
                    errors += "Got Unexpected Exception?!?" + task.Exception + "\n";
                }
            }
        }
        else if (task.IsCanceled)
        {
            errors += "Canceled";
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");
            Debug.Log("Email = " + task.Result.Email);
            Debug.Log("Google ID Token = " + task.Result.IdToken);
            SignInWithGoogleOnFirebase(task.Result.IdToken,task.Result);
        }

        if(errors.Length != 0){
            GameObject.Find("googleLoginController").GetComponent<googleLoginController>().errorLoginUser(errors);
            Debug.Log("llamada a funcion algo salio mal(errors)");
        }
    }

    private void SignInWithGoogleOnFirebase(string idToken, GoogleSignInUser user)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        Debug.Log("Para buscar: hasta aqui todo bien");

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            //peta en este punto
            // warn system ignoring header x-firebase-locale because its value was null. 
            Debug.Log("Pabuscar: he llegado aqui 2");
            string errors = "";
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0)){
                    errors += "\nError code = " + inner.ErrorCode + " Message = " + inner.Message;
                }
            }
            else
            {
                GameObject.Find("Login button Google").GetComponent<googleLoginController>().userLogedSuccessfully(user.DisplayName);
                Debug.Log("se inicio sesion correctamente! llamada a TODO OK.");
                downloadAllPlaces();
            }
            if(errors.Length != 0){
                GameObject.Find("Login button Google").GetComponent<googleLoginController>().errorLoginUser(errors);
                Debug.Log("llamada a funcion algo salio mal(errors)");
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

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

    public void writeUserData(){
        Debug.Log("writeUserData..."+actualUser_.ToJson());
        //database.Child("users").Child(actualUser_.firebaseUserData_.UserId).SetRawJsonValueAsync(actualUser_.ToJson());
        userDataUploaded_ = "inProgress";
        database.Child("users").Child(actualUser_.firebaseUserData_.UserId).SetRawJsonValueAsync(actualUser_.ToJson()).ContinueWith(taskUploadUserData =>{
            Debug.Log("data wrote! " + (taskUploadUserData.IsCompleted ? "true" : "false") );
            userDataUploaded_ = taskUploadUserData.IsCompleted ? "true" : "false";
        },TaskScheduler.FromCurrentSynchronizationContext());

        /*
        FirebaseDatabase.DefaultInstance.GetReference($"users/GzHvoKfRGLdmO0ouAtJmDTiy6zV2/baseCords_").GetValueAsync().ContinueWith(testTask => {
            if (testTask.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+testTask.Exception);
            } else if (testTask.IsCompleted) {
                DataSnapshot snapshotTestTask = testTask.Result;
                Dictionary<string,string> baseCords;
                if(snapshotTestTask.GetRawJsonValue() == null){
                    baseCords = null;
                }else{
                    baseCords = JsonConvert.DeserializeObject<Dictionary<string,string>>(snapshotTestTask.GetRawJsonValue());
                    Debug.Log(baseCords);
                    foreach(var key in baseCords.Keys){
                        Debug.Log($"{key} -> {baseCords[key]}");
                    }
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
        FirebaseDatabase.DefaultInstance.GetReference($"users/GzHvoKfRGLdmO0ouAtJmDTiy6zV2/friendsInvitations_").GetValueAsync().ContinueWith(testTask => {
            if (testTask.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+testTask.Exception);
            } else if (testTask.IsCompleted) {
                DataSnapshot snapshotTestTask = testTask.Result;
                List<string> friendsInvitations;
                if(snapshotTestTask.GetRawJsonValue() == null){
                    friendsInvitations = null;
                }else{
                    friendsInvitations = JsonConvert.DeserializeObject<List<string>>(snapshotTestTask.GetRawJsonValue());
                    Debug.Log(friendsInvitations);
                    foreach(string friend in friendsInvitations){
                        Debug.Log(friend);
                    }
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
        */
    }

    public void readUserData(){ 
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
                Debug.Log($"snapshotVisitedPlaces = {snapshotVisitedPlaces.GetRawJsonValue()}");
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
                        Debug.Log($"snapshotBaseCords = {snapshotBaseCords.GetRawJsonValue()}");
                        // si la base data es null quiere decir que el usuario nunca llego a activar su servicio GPS
                        if(snapshotBaseCords.GetRawJsonValue() == null){
                            baseCordsData = null;
                        }else{
                            //                                                    name, number
                            baseCordsData = JsonConvert.DeserializeObject<Dictionary<string,string>>(snapshotBaseCords.GetRawJsonValue());
                        }
                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/friends_").GetValueAsync().ContinueWith(taskFriends => {
                            if (taskFriends.IsFaulted) {
                                // Handle the error...
                                Debug.Log("Error: "+taskFriends.Exception);
                            } else if (taskFriends.IsCompleted) {
                                DataSnapshot snapshotFriends = taskFriends.Result;
                                List<string> friendsList;
                                Debug.Log($"snapshotFriends = {snapshotFriends.GetRawJsonValue()}"); 
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
                                        Debug.Log($"snapshotFriendsInvitations = {snapshotFriendsInvitations.GetRawJsonValue()}"); 
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
                                                Debug.Log($"snapshotacceptedFriendsInvitations = {snapshotacceptedFriendsInvitations.GetRawJsonValue()}");
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
                                                actualUser_ = new UserData(auth.CurrentUser, visitedPlacesListVersion, baseCordsData, friendsList, friendsInvitationsList, acceptedFriendsInvitationsList );
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

    public void writePlaceData(string type, string id){
        Place place = requestHandler_.getPlaceByTypeAndId(type,id);
        //database.Child("places").Child(type).Child(id).SetRawJsonValueAsync(place.ToJson());
        database.Child("places").Child(type).Child(id).SetRawJsonValueAsync(place.ToJson()).ContinueWith(uploadPlaceDataTask => {
            if(uploadPlaceDataTask.IsFaulted){
                if(placesToUpdateQueue_ == null){
                    placesToUpdateQueue_ = new List<Dictionary<string, string>>();
                }
                placesToUpdateQueue_.Add(new Dictionary<string,string>{{type,id}});
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    public bool cooldownVisitingPlaceFinished(string type, int id){
        /*
        
        PARA DEBUGEAR RAPIDO
        
        */
        return true;
        /*
        
        PARA DEBUGEAR RAPIDO
        
        */
        if(actualUser_.visitedPlaces_.Exists(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id)){
            VisitedPlace place = actualUser_.visitedPlaces_.Find(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id);
            //si ya lo habia visitado devolvemos true si ha cumplido el cooldown
            // hay 10.000.000 de ticks en un segundo, * 60 son minutos
            //Debug.Log($"{place.lastVisitTimestamp_} + {gameRules.getMinutesOfCooldown() * 10000000 * 60}\n {place.lastVisitTimestamp_ + gameRules.getMinutesOfCooldown() * 10000000 * 60} < {DateTime.Now.Ticks} ? ");
            return (place.lastVisitTimestamp_ + gameRules.getMinutesOfCooldown() * 10000000 * 60 < DateTime.Now.Ticks);
        }
        //si no lo habia visitado, el cooldown siempre se ha cumplido
        return true;
    }

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
            //Debug.Log($"{storedPlace.lastVisitTimestamp()} + {gameRules.getMinutesOfCooldown() * 10000000 * 60}\n {storedPlace.lastVisitTimestamp() + gameRules.getMinutesOfCooldown() * 10000000 * 60} < {DateTime.Now.Ticks} ? ");
            return (storedPlace.lastVisitTimestamp() + gameRules.getMinutesOfCooldown() * 10000000 * 60 < DateTime.Now.Ticks);
        }
        //si no lo habia visitado, el cooldown siempre se ha cumplido
        return true;
    }
    
    
    public bool cooldownVisitingPlaceByNameFinished(string name){
        Dictionary<string,string> typeAndId = findPlaceByName(name);
        return cooldownVisitingPlaceFinished(typeAndId["type"],Int32.Parse(typeAndId["id"]));
    }

    public void userVisitedPlace(string type, int id, long timeOfTheVisit = -1){
        bool firstTime = true;
        for(int i = 0; i < actualUser_.visitedPlaces_.Count; i++ ){
            //busco en los ya visitados y si es uno de esos, contabilizo la visita
            if(actualUser_.visitedPlaces_[i].type_ == type && actualUser_.visitedPlaces_[i].id_ == id){
                firstTime = false;
                //actualUser_.visitedPlaces_[i].timesVisited_++;
                break;
            }
        }
        long actualTime = timeOfTheVisit <= 0 ? DateTime.Now.Ticks : timeOfTheVisit;
        //si no lo habia visitado antes, registro la visita
        if(firstTime){
            actualUser_.visitedPlaces_.Add(new VisitedPlace(type,id,1,actualTime));
        }else{
            actualUser_.newVisitAt(type,id,actualTime);
        }
        allPlaces_[type][id.ToString()]["timesItHasBeenVisited_"] = (Int32.Parse(allPlaces_[type][id.ToString()]["timesItHasBeenVisited_"])+1).ToString();
        //requestHandler_.oneMoreVisitToPlaceByTypeAndId(type,id.ToString());
        writePlaceData(type,id.ToString());
        writeUserData();
    }

    public void userVisitedPlaceByName(string name, long timeOfTheVisit = -1){
        Dictionary<string,string> typeAndId = findPlaceByName(name);
        userVisitedPlace(typeAndId["type"],Int32.Parse(typeAndId["id"]),timeOfTheVisit);
    }

    public bool hasUserVisitPlaceByName(string name){
        Dictionary<string,string> typeAndId = findPlaceByName(name);
        return actualUser_.hasVisitPlace(typeAndId["type"],Int32.Parse(typeAndId["id"]));
    }

    public Place askForAPlace(){
        if(requestHandler_ == null){
            requestHandler_ = new requestHandler(allPlaces_);
        }
        return requestHandler_.askForAPlace();
    }

    private void downloadAllPlaces(){
        foreach(string typeSite in mapRulesHandler.getTypesOfSites()){
            //StartCoroutine es como olvidate de esto hasta que termine, 
            //cuando termina ejecuta la siguiente linea como si no hubiera pasado nada
            //es para que no se pause la app mientras se descargan los sitios
            StartCoroutine(downloadOneTypeOfSite(typeSite));
        }
        
    }

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

    public bool placesAreReady(){
        return placesReady_;
    }

    public bool userDataIsReady(){
        return userDataReady_;
    }

    public void sortPlaces(){
        requestHandler_.sortPlaces();
    }

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

    public Dictionary<string,string> getPlaceData(string type, string id){
        return allPlaces_[type][id];
    }

    void OnApplicationQuit(){
        FirebaseApp.DefaultInstance.Dispose();
        StoredPlace.saveAll();
    }

    void uploadPlacesQueue(){
        foreach(Dictionary<string,string> placeToUpdate in placesToUpdateQueue_){
            foreach(string key in placeToUpdate.Keys){
                //creo que esto va a petar porque no puedes modificar un diccionario mientras lo recorres.
                placesToUpdateQueue_.Remove(placeToUpdate);
                writePlaceData(key, placeToUpdate[key]);
            }
        }
    }

/*
        cuando aceptas la peticion updateas la lista de amigos aceptados
        si eliminas un amigo updateas la lista de amigos de este usuario, crear otra lista de amigos eliminados

POR ALGUN MOTIVO USERDATA NUNCA LLEGA A ESTAR READY!
SI NO COMPRUEBO SI ESTA READY PETA POR QUE DICE QUE HAY NULL REFERENCE EN USERDATA friendDataIsComplete() LINEA 284

*/
    private void downloadAllFriendData(){
        for(int index = 0; index < actualUser_.countOfFriends(); index++){
            string uid = actualUser_.getFriend(index);
            if(friendDataDownloadQueue_.Exists(friendDataForDownload => friendDataForDownload == uid)){
                friendDataDownloadQueue_.Remove(uid);
                downloadFriendData(uid);
            }
        }
    }

    private void downloadFriendData(string uid){
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
                        actualUser_.addFriendData(new FriendData(uid, displayName, deletedFriends));
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    //llamar cuando se agrege a un amigo
    public void addFriendDataToDownload(string uid){
        friendDataDownloadQueue_.Add(uid);
    }

    private void downloadAllNewFriendInvitationsData(){
        for(int index = 0; index < actualUser_.countOfNewFriends(); index++){
            string uid = actualUser_.getNewFriend(index);
            if(newFriendDataDownloadQueue_.Exists(newFriendDataForDownload => newFriendDataForDownload == uid)){
                newFriendDataDownloadQueue_.Remove(uid);
                downloadNewFriendInvitationData(uid);
            }
        }
    }

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
                        actualUser_.addNewFriendData(new newFriendData(uid, displayName, newFriends));
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }
}

