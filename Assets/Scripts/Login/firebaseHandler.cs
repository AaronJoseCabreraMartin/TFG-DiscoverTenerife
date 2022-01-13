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
    FirebaseApp firebaseApp = null;
    internal Firebase.Auth.FirebaseAuth auth = null;
    internal DatabaseReference database = null;
    //EL USUARIO ACTUAL ESTA EN auth.CurrentUser
    
    private GoogleSignInConfiguration configuration;

    public UserData actualUser_ = null;
    private int minutesOfCooldown_ = 60;//1 hora de cooldown
    private bool firebaseDependenciesResolved = false;
    private bool placesReady_ = false;
    private bool userDataReady_ = false;
    //              type of place          id                 DATA
    private Dictionary<string,Dictionary<string,Dictionary<string,string>>> allPlaces_ = new Dictionary<string,Dictionary<string,Dictionary<string,string>>>();

    private requestHandler requestHandler_;

    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("firebaseHandler");
        if (objs.Length > 1) //si ya existe una firebaseHandler no crees otra
        {
            Destroy(this.gameObject);
            return;
        }

        configuration = new GoogleSignInConfiguration { WebClientId = "993595598765-gov25ig79svl8v52ne2rlrmi7jcl8gf8.apps.googleusercontent.com", 
                                                        RequestEmail = true,
                                                        RequestIdToken = true };
        // cuando termine           A                           ejecuta         B                   con este contexto (para acceder a las cosas privadas)
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(CheckDependenciesFirebase,TaskScheduler.FromCurrentSynchronizationContext());
        DontDestroyOnLoad(this.gameObject);
    }
    
    private void CheckDependenciesFirebase(Task<DependencyStatus> task) {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Set a flag here indiciating that Firebase is ready to use by your
            // application.
            CreateFirebaseObject();
            firebaseDependenciesResolved = true;
            Debug.Log("Firebase Connected!!!");
            //downloadAllPlaces();
        }
        else
        {
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
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
        userDataReady_ = false;
    }

    ///// DATABASE METHODS /////
    //only for debugging purpose
    public void writeNewPlaceOnDataBase(Place place, string type, int placeID){
        database.Child("places").Child(type).Child(placeID.ToString()).SetRawJsonValueAsync(place.ToJson());
    }

    public void writeUserData(){
        database.Child("users").Child(actualUser_.firebaseUserData_.UserId).SetRawJsonValueAsync(actualUser_.ToJson());
    }

    public void readUserData(){        
        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/visitedPlaces_").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+task.Exception);
            } else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                // por que si el usuario solo se registra y no visita ningun sitio en ese momento, 
                // el array de visitados sera null (que es lo que estoy pidiendo), entonces tengo que inicializar 
                // los datos como si fuera un nuevo usuario
                if(snapshot.GetRawJsonValue() == null){
                    actualUser_ = new UserData(auth.CurrentUser);
                }else{
                    //  pos             data
                    List<Dictionary<string,string>> listVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshot.GetRawJsonValue());
                    actualUser_ = new UserData(auth.CurrentUser,listVersion);
                }
                //por cualquiera de los dos caminos tiene que estar la user data lista
                userDataReady_ = true;
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void writePlaceData(string type, string id){
        requestHandler_.oneMoreVisitToPlaceByTypeAndId(type,id);
        Place place = requestHandler_.getPlaceByTypeAndId(type,id);
        database.Child("places").Child(type).Child(id).SetRawJsonValueAsync(place.ToJson());
    }

    public bool cooldownVisitingPlaceFinished(string type, int id){
        if(actualUser_.visitedPlaces_.Exists(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id)){
            VisitedPlace place = actualUser_.visitedPlaces_.Find(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id);
            //si ya lo habia visitado devolvemos true si ha cumplido el cooldown
            // hay 10.000.000 de ticks en un segundo, * 60 son minutos
            //Debug.Log($"{place.lastVisitTimestamp_} + {minutesOfCooldown_ * 10000000 * 60}\n {place.lastVisitTimestamp_ + minutesOfCooldown_ * 10000000 * 60} < {DateTime.Now.Ticks} ? ");
            return (place.lastVisitTimestamp_ + minutesOfCooldown_ * 10000000 * 60 < DateTime.Now.Ticks);
        }
        //si no lo habia visitado, el cooldown siempre se ha cumplido
        return true;
    }
    
    public bool cooldownVisitingPlaceByNameFinished(string name){
        Dictionary<string,string> typeAndId = findPlaceByName(name);
        return cooldownVisitingPlaceFinished(typeAndId["type"],Int32.Parse(typeAndId["id"]));
    }

    public void userVisitedPlace(string type, int id){
        bool firstTime = true;
        for(int i = 0; i < actualUser_.visitedPlaces_.Count; i++ ){
            //busco en los ya visitados y si es uno de esos, contabilizo la visita
            if(actualUser_.visitedPlaces_[i].type_ == type && actualUser_.visitedPlaces_[i].id_ == id){
                firstTime = false;
                actualUser_.visitedPlaces_[i].timesVisited_++;
                break;
            }
        }
        long actualTime = DateTime.Now.Ticks;
        //si no lo habia visitado antes, registro la visita
        if(firstTime){
            actualUser_.visitedPlaces_.Add(new VisitedPlace(type,id,1,actualTime));
        }else{
            actualUser_.newVisitAt(type,id,actualTime);
        }
        allPlaces_[type][id.ToString()]["timesItHasBeenVisited_"] = (Int32.Parse(allPlaces_[type][id.ToString()]["timesItHasBeenVisited_"])+1).ToString();
        writePlaceData(type,id.ToString());
        writeUserData();
    }

    public void userVisitedPlaceByName(string name){
        Dictionary<string,string> typeAndId = findPlaceByName(name);
        userVisitedPlace(typeAndId["type"],Int32.Parse(typeAndId["id"]));
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
        FirebaseDatabase.DefaultInstance.GetReference($"places/{typeSite}/").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+task.Exception);
            } else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.GetRawJsonValue());
                //           id                 data
                List<Dictionary<string,string>> listVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshot.GetRawJsonValue());
                Dictionary<string,Dictionary<string,string>> dictionaryVersion = new Dictionary<string,Dictionary<string,string>>();
                for(int i = 0; i <listVersion.Count; i++ ){
                    dictionaryVersion[i.ToString()] = listVersion[i];
                }
                /*foreach(var key in dictionaryVersion.Keys){
                    Debug.Log($"\"{key}\"");
                    foreach(var key2 in dictionaryVersion[key].Keys){
                        Debug.Log($"\t\"{key2}\" : \"{dictionaryVersion[key][key2]}\"");
                    }
                }*/
                allPlaces_[typeSite] = dictionaryVersion;
                if(allPlaces_.Count == mapRulesHandler.getTypesOfSites().Count){
                    placesReady_ = true;
                    Debug.Log("ready!");
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
    }

}

