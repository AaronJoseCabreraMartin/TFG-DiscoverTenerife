using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief class that controls the button that shows the user id.
  */
public class UIDbutton : MonoBehaviour
{
    /**
      * @brief gameobject that shows the current user id.
      */
    [SerializeField] private GameObject TextBox;

    /**
      * @brief string that contains the current user id.
      */
    private string UserId_;
    
    /**
      * @brief This method is called before the first frame, it sets the UserId_ property
      * as the user id and show it on the TextBox gameobject.
      */
    void Awake()
    {
        UserId_ = firebaseHandler.firebaseHandlerInstance_.auth.CurrentUser.UserId;
        TextBox.GetComponent<Text>().text += UserId_;
    }

    /**
      * @brief this method should be called when the user press the UID button. It copies
      * the user id on the current device clipboard.
      */
    void PastleUIDOnTheClipBoard(){
        /*
        Clipboard necesita
        using System.Windows;
        entonces no funciona en android
        Clipboard.SetText(UserId_);
        */
    }
}
