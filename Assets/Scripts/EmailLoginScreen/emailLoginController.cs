using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class emailLoginController : MonoBehaviour
{
    private GameObject user;
    private GameObject password;

    void Awake()
    {
        user = GameObject.Find("/Canvas/user/Text");
        password = GameObject.Find("/Canvas/password");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tryToLoginUser(){
        string userText = user.GetComponent<Text>().text;
        string passwordText = password.GetComponent<InputField>().text;


        Debug.Log($"user = {userText}\tpassword = {passwordText}");
        if (userText.Length == 0 || passwordText.Length == 0) {
            Debug.Log("Tienes que completar todos los campos");
            return;
        }

        //comprobar que el correo introducido hace match con la expresion regular de los correos

        GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>().LoginUser(userText,passwordText);
    }

    public void userLogedSuccessfully(string name){
        Debug.Log("userCreatedSuccessfully: " + name);
        SceneManager.LoadScene("PantallaPrincipal");
    }

    public void errorLoginUser(string error){
        Debug.Log("errorLoginUser: " + error);
    }
}