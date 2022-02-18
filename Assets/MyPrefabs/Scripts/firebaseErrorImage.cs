using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/**
  * @brief Class that controls if the error of no connection with the firebase server
  * should be shown or not.
  */
public class firebaseErrorImage : MonoBehaviour
{
    /**
      * @brief This method is called before the first frame. It calls the checkNewState
      * method.
      */
    void Awake(){
        checkNewState();
    }

    /**
      * @brief This method is called on each frame. It calls the checkNewState
      * method.
      */
    void Update(){
        checkNewState();
    }

    /**
      * @brief This method checks if the firebaseHandler instance is null or if there isnt
      * internet connection or if FirebaseDependenciesAreResolved method return false
      * it shows the Firebase Error image, in other case it hides the Firebase Error image.
      */
    void checkNewState(){
        // muestra la imagen de error si el firebaseHandler.firebaseHandlerInstance_ no se encuentra si no hay internet o si no se resolvieron las dependencias
        bool show = firebaseHandler.firebaseHandlerInstance_ == null || 
                                            !firebaseHandler.firebaseHandlerInstance_.internetConnection() || 
                                            !firebaseHandler.firebaseHandlerInstance_.FirebaseDependenciesAreResolved();
        GetComponent<Image>().enabled = show;
        transform.GetChild(0).gameObject.SetActive(show);
    }
}
