using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anonymousButtonHandler : MonoBehaviour
{
    private bool inProcess = false;

    // Update is called once per frame
    void Update()
    {

    }

    public void tryToLoginAnoymous()
    {
        // para evitar que clickee varias veces, quizas se podria cambiar el color del boton o deshabilitarlo
        if(!inProcess){
            inProcess = true;
            GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>().AnonymousUser();
        }
    }

    public void anonymousUserLoginSucessfully()
    {
        inProcess = false;
        ChangeScene.changeScene("PantallaPrincipal");
    }

    public void errorLoginAnonymousUser(string error){
        Debug.Log("errorLoginUser: " + error);
    }
}
