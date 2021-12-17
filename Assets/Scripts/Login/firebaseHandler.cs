using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
//using Firebase.Database;
using Google;

public class firebaseHandler : MonoBehaviour
{
    FirebaseApp firebaseApp = null;
    internal Firebase.Auth.FirebaseAuth auth = null;
    internal Firebase.Auth.FirebaseUser user = null;
    //internal DatabaseReference database = null;
    
    private GoogleSignInConfiguration configuration;

    private bool firebaseDependenciesResolved = false;

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
            Debug.Log("Firebase Connected!!!");
            CreateFirebaseObject();
            firebaseDependenciesResolved = true;
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
        // con esto recogemos si tiene algun usuario registrado
        user = auth.CurrentUser;
        // con esto recogemos la referencia a la base de datos, para poder hacer operaciones de escritura o lectura.
        //database = FirebaseDatabase.GetInstance(firebaseApp).RootReference;
    }

    public bool FirebaseDependenciesAreResolved(){
        return firebaseDependenciesResolved;
    }

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
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            // despues de este log format parece que no se esta ejecutando lo demas

            Debug.Log(GameObject.Find("registerController").GetComponent<registerScreenController>());
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
        //Si el current user es nulo, es que nunca se habia logeado
        if(auth.CurrentUser == null){
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
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            Debug.Log("El usuario anonimo ya existia");
        }
        GameObject.Find("anonymousButtonHandler").GetComponent<anonymousButtonHandler>().anonymousUserLoginSucessfully();
    }

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
            Debug.Log("Email = " + task.Result.Email);
            SignInWithGoogleOnFirebase(task.Result.IdToken);
        }

        if(errors.Length != 0){
            Debug.Log("llamada a funcion algo salio mal(errors)");
        }
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
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
                Debug.Log("se inicio sesion correctamente! llamada a TODO OK.");
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /*
    Falta leer mas el codigo de ejemplo
    Hacer funcional el boton de Google, habra que poner dos 
    botones uno de registrarse y otro de iniciar sesion
    */

    public void LogOut()
    {
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
    }
}
