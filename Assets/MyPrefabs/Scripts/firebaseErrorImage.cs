using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class firebaseErrorImage : MonoBehaviour
{

    void Awake(){
        checkNewState();
    }

    // Update is called once per frame
    void Update()
    {
        checkNewState();
    }

    void checkNewState(){
        // muestra la imagen de error si el firebaseHandler.firebaseHandlerInstance_ no se encuentra si no hay internet o si no se resolvieron las dependencias
        bool show = firebaseHandler.firebaseHandlerInstance_ == null || 
                                            !firebaseHandler.firebaseHandlerInstance_.internetConnection() || 
                                            !firebaseHandler.firebaseHandlerInstance_.FirebaseDependenciesAreResolved();
        GetComponent<Image>().enabled = show;
        transform.GetChild(0).gameObject.SetActive(show);
    }
}
