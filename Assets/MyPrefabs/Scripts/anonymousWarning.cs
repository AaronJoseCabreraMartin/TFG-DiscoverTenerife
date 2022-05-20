using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief This class controls the panel that is shown when the current user is an anonymous user.
  * The sign prevent the anonymous user use the social functionalities of the application
  */
public class anonymousWarning : MonoBehaviour
{   
    /**
      * @brief GameObject that contains the background image of the panel
      */
    [SerializeField] private GameObject image_;

    /**
      * @brief GameObject that contains the text of the panel
      */
    [SerializeField] private GameObject text_;

    /**
      * @brief This method is called before the first frame. It hides or shows the text and the image of the panel
      * if the current user is anonymous.
      */
    void Awake()
    {
        image_.SetActive(firebaseHandler.firebaseHandlerInstance_.currentUser_.IsAnonymous());
        text_.SetActive(firebaseHandler.firebaseHandlerInstance_.currentUser_.IsAnonymous());
    }
}
