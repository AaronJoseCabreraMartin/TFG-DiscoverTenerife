using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class registerScreenController : MonoBehaviour
{
    [SerializeField] private GameObject user;
    [SerializeField] private GameObject password;
    [SerializeField] private GameObject confirmPassword;
    [SerializeField] private GameObject toastMessageObject_;
    private bool inProgress = false;

    private bool isEmail(string email){
        try{
            return Regex.IsMatch(email,
            /* cualquier cosa que no sea @ o espacio seguido de un @ seguido de cosa que no sea @ o espacio seguido de . 
            y termina con cualquier cosa que no sea @ o espacio*/
                                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        catch (RegexMatchTimeoutException){
            return false;
        }
    }

    public void tryToRegisterUser(){
        if(!inProgress){
            toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
            inProgress = true;
            string userText = user.transform.Find("Text").GetComponent<Text>().text;
            string passwordText = password.GetComponent<InputField>().text;
            string confirmPasswordText = confirmPassword.GetComponent<InputField>().text;

            if(userText.Length == 0 || passwordText.Length == 0 || confirmPasswordText.Length == 0){
                inProgress = false;
                if(userText.Length == 0){
                    StartCoroutine(ChangeImageColor(user.GetComponent<Image>(), 2));
                }
                if(passwordText.Length == 0){
                    StartCoroutine(ChangeImageColor(password.GetComponent<Image>(), 2));
                }
                if(confirmPasswordText.Length == 0){
                    StartCoroutine(ChangeImageColor(confirmPassword.GetComponent<Image>(), 2));
                }
                toastMessageInstance.makeAnimation("You must complete all the fields", new Color32(255,0,0,255),2);
                return;
            }

            //comprobar que el correo introducido hace match con la expresion regular de los correos
            if(!isEmail(userText)){
                inProgress = false;
                toastMessageInstance.makeAnimation("Invalid format on the email", new Color32(255,0,0,255),2);
                StartCoroutine(ChangeImageColor(user.GetComponent<Image>(), 2));
                return;
            }

            if(passwordText.Length < 6){
                inProgress = false;
                toastMessageInstance.makeAnimation("The password must contain at least 6 characters", new Color32(255,0,0,255),2);
                StartCoroutine(ChangeImageColor(password.GetComponent<Image>(), 2));
                return;
            }

            if(passwordText != confirmPasswordText){
                inProgress = false;
                toastMessageInstance.makeAnimation("The password must be the same on the two password fields", new Color32(255,0,0,255),2);
                StartCoroutine(ChangeImageColor(confirmPassword.GetComponent<Image>(), 2));
                return;
            }  
            GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>().CreateNewUser(userText,passwordText);
        }
    }

    private IEnumerator ChangeImageColor(Image toMark,int time){
        toMark.color = Color.red;
        yield return new WaitForSeconds(time);
        toMark.color = Color.white;
    }

    public void userCreatedSuccessfully(string name){
        inProgress = false;
        Debug.Log("userCreatedSuccessfully: " + name);
        //ChangeScene.changeScene("PantallaPrincipal");
        GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal",0.5f,"");
    }

    public void errorCreatingUser(string error){
        inProgress = false;
        Debug.Log("errorCreatingUser: " + error);
    }
}
