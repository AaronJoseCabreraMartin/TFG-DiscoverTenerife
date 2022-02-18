using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controlls the google login button.
  */
public class googleLoginController : MonoBehaviour
{
    /**
      * @brief True if the sing in method is on progress. False in other case.
      * Its initial value is false.
      */
    private bool inProgress = false; //para asegurar que solo se hace una llamada

    /**
      * If the inProgress property is false, it sets the inProgress property to true
      * and it calls the SignInWithGoogle method of the firebaseHandler class.
      * If the inProgress property is true. It doenst do nothing, thanks to that
      * you cant make several parallel calls to the sing in method of goole.
      */
    public void tryToLoginUser(){
        if(!inProgress){
            inProgress = true;
            firebaseHandler.firebaseHandlerInstance_.SignInWithGoogle();
        }
    }

    /**
      * This method sould be called if the login with google process finished successfully.
      * It calls the changeScene method of the ChangeScene class, in order to change the 
      * scene to the main one.
      * It also set the inProgress property to false.
      */
    public void userLogedSuccessfully(string name){
        inProgress = false;
        Debug.Log("userCreatedSuccessfully: " + name);
        ChangeScene.changeScene("PantallaPrincipal");
        //GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal");
    }

    /**
      * This method should be called if the login with google process coudnt finish.
      * It shows the error on the console.
      * It also set the inProgress property to false.
      */
    public void errorLoginUser(string error){
        inProgress = false;
        Debug.Log("errorLoginUser: " + error);
    }
}