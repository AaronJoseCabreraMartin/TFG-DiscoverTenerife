using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOutButtonHandler : MonoBehaviour
{
    public void LogOut()
    {
        firebaseHandler.firebaseHandlerInstance_.LogOut();
        ChangeScene.changeScene("PantallaLogin");
    }
}
