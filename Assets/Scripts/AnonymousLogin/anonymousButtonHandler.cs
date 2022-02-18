using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief class that controls the button of anonymous login.
  */
public class anonymousButtonHandler : MonoBehaviour{
    /**
      * @brief if this flag is true, the logging process is working right now, if the logging process
      * didnt started or it finished this flag will be false.
      */
    private bool inProgress = false;

    /**
      * @brief if the inProgress flag is false, this method calls the AnonymousUser 
      * method of firebaseHandler class. It also set the inProgress flag to true.
      * If it is called when the inProcess flag is on this method wont do nothing, this is
      * like this for avoid more than one parallel call to the logging process. 
      */
    public void tryToLoginAnoymous(){
        // para evitar que clickee varias veces, quizas se podria cambiar el color del boton o deshabilitarlo
        if(!inProgress){
            inProgress = true;
            firebaseHandler.firebaseHandlerInstance_.AnonymousUser();
        }
    }

    /**
      * @brief This method should be called when the logging process worked sucessfully,
      * it makes the transition to the main screen calling the changeSceneWithAnimation method
      * of the ChangeScene class. This method also put the inProgress flag to false.
      */
    public void anonymousUserLoginSucessfully(){
        inProgress = false;
        //ChangeScene.changeScene("PantallaPrincipal");
        GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal",0.5f,"");
    }

    /**
      * @param string That contains the error that happened during the logging process.
      * @brief This method sould be called when the logging process failed. It shows
      * the given error on the console. This method also put the inProgress flag to false.
      */
    public void errorLoginAnonymousUser(string error){
        inProgress = false;
        Debug.Log("errorLoginUser: " + error);
    }
}
