using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
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

    private bool firebaseDependenciesResolved = false;
    private bool placesReady_ = false;
    //              type of place          id                 DATA
    private Dictionary<string,Dictionary<string,Dictionary<string,string>>> allPlaces_ = new Dictionary<string,Dictionary<string,Dictionary<string,string>>>();

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
        downloadAllPlaces();
    }
    
    private void CheckDependenciesFirebase(Task<DependencyStatus> task) {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Set a flag here indiciating that Firebase is ready to use by your
            // application.
            CreateFirebaseObject();
            firebaseDependenciesResolved = true;
            Debug.Log("Firebase Connected!!!");
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
            Debug.Log($"Firebase user created successfully: {newUser.DisplayName} ({ newUser.UserId})");
            GameObject.Find("registerController").GetComponent<registerScreenController>().userCreatedSuccessfully(newUser.DisplayName);
            return;
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void LoginUser(string email, string password){
        auth.SignInWithEmailAndPasswordAsync(email,password).ContinueWith(task => {
            if(task.Exception != null){
                Debug.LogError($"An exception has happened:{task.Exception}");
                GameObject.Find("emailLoginController").GetComponent<emailLoginController>().errorLoginUser($"{task.Exception}");

            }else{
                Debug.Log("Se ha iniciado sesión correctamente");
                GameObject.Find("emailLoginController").GetComponent<emailLoginController>().userLogedSuccessfully(task.Result.DisplayName);
            }
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
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            actualUser_ = new UserData(auth.CurrentUser);
            writeUserData();
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            Debug.Log("El usuario anonimo ya existia");
            actualUser_ = new UserData(auth.CurrentUser);
            writeUserData();
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
    }

    ///// DATABASE METHODS /////
    //only for debugging purpose
    public void writeNewPlaceOnDataBase(Place place, string type, int placeID){
        database.Child("places").Child(type).Child(placeID.ToString()).SetRawJsonValueAsync(JsonUtility.ToJson(place));
    }

    public void writeUserData(){
        database.Child("users").Child(actualUser_.firebaseUserData_.UserId).SetRawJsonValueAsync(actualUser_.ToJson());
    }

    public void userVisitedPlace(string type, int id){
        bool firstTime = true;
        Debug.Log(actualUser_.visitedPlaces_);
        for(int i = 0; i < actualUser_.visitedPlaces_.Count; i++ ){
            if(actualUser_.visitedPlaces_[i].type_ == type && actualUser_.visitedPlaces_[i].id_ == id){
                firstTime = false;
                actualUser_.visitedPlaces_[i].timesVisited_++;
                break;
            }
        }
        if(firstTime){
            actualUser_.visitedPlaces_.Add(new VisitedPlace(type,id,0));
        }
        writeUserData();
    }

    /*
        esto debe extraerse del menu de opciones
        en modes podemos recibir:
            - distance     -> ordenar por distancia -> si este esta activado debemos mirar la posicion del usuario si no no
            - most visited -> ordenar por mas visitados

            - seen         ->mostrar los ya vistos
            - viewpoints   ->mostrar miradores
            - beach        ->mostrar playas
            - hiking route ->mostrar senderos
            - natural pool ->mostrar charcos/piscinas naturales
            - natural park ->mostrar parques naturales
    */
    public IEnumerator<Place> askForAPlace(/*Dictionary<string,bool> modes, double latitude = 0, double longitude = 0*/){
        Debug.Log("Entrando en askForAPlace");
        /*habria que hacer deletes mirando las opciones que nos dijeron, habria que hacer otro metodo que solo se ejecute
        una vez el usuario se logea o cuando el usuario cambia las opciones para tener descargados todos los sitios, selecciona los
        que quiere el usuario, los ordena como pide el usuario y luego solo asignas cuando te preguntan
        */
        foreach(var typeOfSite in allPlaces_.Keys){
            foreach(var siteId in allPlaces_[typeOfSite].Keys){
                string convertion = "";
                foreach(var property in allPlaces_[typeOfSite][siteId].Keys){
                    convertion += allPlaces_[typeOfSite][siteId][property] + ";";
                }
                Debug.Log(convertion);
                yield return new Place(convertion);
            }
        }
    }

    private void downloadAllPlaces(){
        List<string> typesOfSites = new List<string>();
        typesOfSites.Add("beachs");
        typesOfSites.Add("hikingRoutes");
        typesOfSites.Add("naturalParks");
        typesOfSites.Add("naturalPools");
        typesOfSites.Add("viewpoints");
        foreach(string typeSite in typesOfSites){
            //StartCoroutine es como olvidate de esto hasta que termine, 
            //cuando termina ejecuta la siguiente linea como si no hubiera pasado nada
            //es para que no se pause la app mientras se descargan los sitios
            StartCoroutine(downloadOneTypeOfSite(typeSite, typesOfSites));
        }
    }


    /*
        Hay que intentar eliminar la clase serverhandler de una vez
    */


    private IEnumerator downloadOneTypeOfSite(string typeSite, List<string> typesOfSites){
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
                if(allPlaces_.Count == typesOfSites.Count){
                    placesReady_ = true;
                    Debug.Log("ready!");
                    //applySelections()//aplica los filtros que haya elegido el usuario
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
        yield return new WaitForSeconds(0);
    }

    public bool placesAreReady(){
        return placesReady_;
    }

}

