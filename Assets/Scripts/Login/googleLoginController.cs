using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class googleLoginController : MonoBehaviour
{
    private bool inProgress = false; //para asegurar que solo se hace una llamada

    public void tryToLoginUser(){
        if(!inProgress){
            inProgress = true;
            GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>().SignInWithGoogle();
        }
    }

    public void userLogedSuccessfully(string name){
        inProgress = false;
        Debug.Log("userCreatedSuccessfully: " + name);
        ChangeScene.changeScene("PantallaPrincipal");
    }

    public void errorLoginUser(string error){
        inProgress = false;
        Debug.Log("errorLoginUser: " + error);
    }
}