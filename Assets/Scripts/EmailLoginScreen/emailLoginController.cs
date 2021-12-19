using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class emailLoginController : MonoBehaviour
{
    public GameObject user;
    public GameObject password;
    public GameObject errorImage;
    private bool inProgress = false; //para asegurar que solo se hace una llamada

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

    public void tryToLoginUser(){
        if(!inProgress){
            inProgress = true;
            string userText = user.transform.Find("Text").GetComponent<Text>().text;
            string passwordText = password.GetComponent<InputField>().text;

            //Debug.Log($"user = {userText}\tpassword = {passwordText}");
            if (userText.Length == 0 || passwordText.Length == 0) {
                //Debug.Log("Tienes que completar todos los campos");
                StartCoroutine(ShowError("You must complete all the fields",2));
                if(userText.Length == 0){
                    StartCoroutine(ChangeImageColor(GameObject.Find("/Canvas/user").GetComponent<Image>(), 2));
                }
                if (passwordText.Length == 0){
                    StartCoroutine(ChangeImageColor(GameObject.Find("/Canvas/password").GetComponent<Image>(), 2));
                }
                inProgress = false;
                return;
            }

            if(!isEmail(userText)){
                StartCoroutine(ShowError("Invalid format on the email",2));
                StartCoroutine(ChangeImageColor(GameObject.Find("/Canvas/user").GetComponent<Image>(), 2));
                inProgress = false;
                return;
            }


            //esto debe ser asi porque firebaseHandler no existe en esta escena
            GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>().LoginUser(userText,passwordText);
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
    
    public void userLogedSuccessfully(string name){
        inProgress = false;
        Debug.Log("userCreatedSuccessfully: " + name);
        SceneManager.LoadScene("PantallaPrincipal");
    }

    public void errorLoginUser(string error){
        inProgress = false;
        //Debug.Log("errorLoginUser: " + error);
        StartCoroutine(ShowError("Error: The user or the password are wrong",2));
    }
}