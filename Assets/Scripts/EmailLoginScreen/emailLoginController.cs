using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class emailLoginController : MonoBehaviour
{
    [SerializeField] private GameObject user;
    [SerializeField] private GameObject password;
    [SerializeField] private GameObject toastMessageObject_;
    private bool inProgress = false; //para asegurar que solo se hace una llamada

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
            toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
            inProgress = true;
            string userText = user.transform.Find("Text").GetComponent<Text>().text;
            string passwordText = password.GetComponent<InputField>().text;

            //Debug.Log($"user = {userText}\tpassword = {passwordText}");
            if (userText.Length == 0 || passwordText.Length == 0) {
                //Debug.Log("Tienes que completar todos los campos");
                toastMessageInstance.makeAnimation("You must complete all the fields",new Color32(255,0,0,255),2);
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
                toastMessageInstance.makeAnimation("Invalid format on the email",new Color32(255,0,0,255),2);
                StartCoroutine(ChangeImageColor(GameObject.Find("/Canvas/user").GetComponent<Image>(), 2));
                inProgress = false;
                return;
            }
            
            //esto debe ser asi porque firebaseHandler no existe en esta escena
            firebaseHandler.firebaseHandlerInstance_.LoginUser(userText,passwordText);
        }
    }
    
    private IEnumerator ChangeImageColor(Image toMark,int time){
        toMark.color = Color.red;
        yield return new WaitForSeconds(time);
        toMark.color = Color.white;
    }
    
    public void userLogedSuccessfully(string name){
        inProgress = false;
        Debug.Log("userLogedSuccessfully: " + name);
        //ChangeScene.changeScene("PantallaPrincipal");
        GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal",0.5f,"");
    }

    public void errorLoginUser(string error){
        inProgress = false;
        toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
        toastMessageInstance.makeAnimation("Error: The user or the password are wrong",new Color32(255,0,0,255),2);
    }
}