using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class registerScreenController : MonoBehaviour
{
    private GameObject user;
    private GameObject password;
    private GameObject confirmPassword;

    void Awake()
    {
        user = GameObject.Find("/Canvas/user/Text");
        password = GameObject.Find("/Canvas/password");
        confirmPassword = GameObject.Find("/Canvas/confirmPassword");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tryToRegisterUser(){
        string userText = user.GetComponent<Text>().text;
        string passwordText = password.GetComponent<InputField>().text;
        string confirmPasswordText = confirmPassword.GetComponent<InputField>().text;


        Debug.Log($"user = {userText}\tpassword = {passwordText}\tconfirmPassword = {confirmPasswordText}");
        if(userText.Length == 0 || passwordText.Length == 0 || confirmPasswordText.Length == 0){
            Debug.Log("Tienes que completar todos los campos");
            return;
        }

        //comprobar que el correo introducido hace match con la expresion regular de los correos

        if(passwordText.Length < 6){
            Debug.Log("La contraseña debe tener al menos 6 digitos");
            return;
        }

        if(passwordText != confirmPasswordText){
            Debug.Log("La contraseña no es la misma!");
            return;
        }  
        GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>().CreateNewUser(userText,passwordText);
    }

    public void userCreatedSuccessfully(string name){
        Debug.Log("userCreatedSuccessfully: " + name);
        SceneManager.LoadScene("PantallaPrincipal");
    }

    public void errorCreatingUser(string error){
        Debug.Log("errorCreatingUser: " + error);
    }
}
