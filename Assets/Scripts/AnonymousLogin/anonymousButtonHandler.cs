using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anonymousButtonHandler : MonoBehaviour{
    private bool inProgress = false;

    public void tryToLoginAnoymous(){
        // para evitar que clickee varias veces, quizas se podria cambiar el color del boton o deshabilitarlo
        if(!inProgress){
            inProgress = true;
            GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>().AnonymousUser();
        }
    }

    public void anonymousUserLoginSucessfully(){
        inProgress = false;
        ChangeScene.changeScene("PantallaPrincipal");
    }

    public void errorLoginAnonymousUser(string error){
        inProgress = false;
        Debug.Log("errorLoginUser: " + error);
    }
}
