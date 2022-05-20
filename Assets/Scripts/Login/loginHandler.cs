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
  * @brief the part of the class firebaseHandler that controls the login user, create user, and logout operations
  */
public partial class firebaseHandler{
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
                //writeUserData();
                firebaseHandler.firebaseHandlerInstance_.writeAllUserProperties();
                
                //no hay que hacer read por lo tanto, ya esta ready
                downloadAllPlaces();
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            Debug.Log($"El usuario anonimo ya existia {auth.CurrentUser.DisplayName} {auth.CurrentUser.UserId}");
            readUserData();
            downloadAllPlaces();
        }
        GameObject.Find("anonymousButtonHandler").GetComponent<anonymousButtonHandler>().anonymousUserLoginSucessfully();
    }

    /**
      * @brief This method will be called when the google signin button is pressed, it start the google authentication
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
    
    /**
      * @param Task<GoogleSignInUser> with the task of sing in the user with google
      * @brief This method tries to sing in the user with google api. If something goes
      * worng it cancels the sing in process and print some console messages. If everything
      * goes fine it changes the scene and calls the downloadAllPlaces and readUserData methods.
      */
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
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
                        //Debug.Log($"firebaseDependenciesResolved = {firebaseDependenciesResolved}");
                        signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);
                        GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal");
                        //Debug.Log($"task.Result = {authTask.Result}");
                        //Debug.Log("Welcome: " + authTask.Result.DisplayName + "!");
                        //Debug.Log("Email = " + authTask.Result.Email);
                        //Debug.Log($"auth.CurrentUser = {auth.CurrentUser}");
                        downloadAllPlaces();
                        //cuando inicio sesion con google no creo un nuevo usuario porque el metodo de google
                        //ya me lo inicializa y lo que hago es leer sus datos directamente? WTF probar
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
}