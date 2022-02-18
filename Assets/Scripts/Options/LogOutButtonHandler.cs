using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief class that controls the logout button.
  */
public class LogOutButtonHandler : MonoBehaviour
{
    /**
      * @brief this method should be called when the logout button is pressed.
      * It calls the LogOut method of firebaseHandler class and the changeScene
      * method of ChangeScene class to go to the login screen.
      */
    public void LogOut()
    {
        firebaseHandler.firebaseHandlerInstance_.LogOut();
        ChangeScene.changeScene("PantallaLogin");
    }
}
