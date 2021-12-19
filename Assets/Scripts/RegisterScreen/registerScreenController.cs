using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class registerScreenController : MonoBehaviour
{
    public GameObject user;
    public GameObject password;
    public GameObject confirmPassword;
    public GameObject errorImage;
    private bool inProgress = false;

    void Awake(){
        errorImage.SetActive(false);
    }

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
                StartCoroutine(ShowError("You must complete all the fields",2));
                return;
            }

            //comprobar que el correo introducido hace match con la expresion regular de los correos
            if(!isEmail(userText)){
                inProgress = false;
                StartCoroutine(ShowError("Invalid format on the email",2));
                StartCoroutine(ChangeImageColor(user.GetComponent<Image>(), 2));
                return;
            }

            if(passwordText.Length < 6){
                inProgress = false;
                StartCoroutine(ShowError("The password must contain at least 6 characters",2));
                StartCoroutine(ChangeImageColor(password.GetComponent<Image>(), 2));
                return;
            }

            if(passwordText != confirmPasswordText){
                inProgress = false;
                StartCoroutine(ShowError("The password must be the same on the two password fields",2));
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

    private IEnumerator ShowError(string error,int time){
        errorImage.transform.Find("errorMessage").GetComponent<Text>().text = error;
        errorImage.SetActive(true);
        yield return new WaitForSeconds(time);
        errorImage.SetActive(false);
    }

    public void userCreatedSuccessfully(string name){
        inProgress = false;
        Debug.Log("userCreatedSuccessfully: " + name);
        SceneManager.LoadScene("PantallaPrincipal");
    }

    public void errorCreatingUser(string error){
        inProgress = false;
        Debug.Log("errorCreatingUser: " + error);
    }
}
