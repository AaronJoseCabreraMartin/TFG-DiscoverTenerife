using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDbutton : MonoBehaviour
{
    [SerializeField] private GameObject TextBox;
    private string UserId_;
    
    void Awake()
    {
        UserId_ = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>().auth.CurrentUser.UserId;
        TextBox.GetComponent<Text>().text += UserId_;
    }

    void PastleUIDOnTheClipBoard(){
        /*
        Clipboard necesita
        using System.Windows;
        entonces no funciona en android
        Clipboard.SetText(UserId_);
        */
    }
}
