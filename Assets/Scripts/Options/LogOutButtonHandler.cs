using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOutButtonHandler : MonoBehaviour
{
    public void LogOut()
    {
        GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>().LogOut();
    }
}
