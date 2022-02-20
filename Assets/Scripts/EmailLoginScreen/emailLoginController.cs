using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

/**
  * @brief Class that controls the email login button. It also makes information messages for the user.
  */
public class emailLoginController : MonoBehaviour
{
    /**
      * @brief GameObject this reference contains the input box for the user email field.
      */
    [SerializeField] private GameObject user;

    /**
      * @brief GameObject this reference contains the input box for the user password field.
      */
    [SerializeField] private GameObject password;

    /**
      * @brief GameObject this reference contains the toast message panel that show the user
      * the errors that are happening.
      */
    [SerializeField] private GameObject toastMessageObject_;

    /**
      * @brief bool this flag is true when the logging process is working, and false in otherwise.
      */
    private bool inProgress = false; //para asegurar que solo se hace una llamada

    /**
      * @param string to check if it contains a valid format email
      * @return bool true if the given string is a valid format email, false in otherwise.
      * @brief this method returns if the given string contains a email that matches the
      * well formated email regular expression. 
      */
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

    /**
      * If the inProgress flag is true this method wont do nothing. This is for preventing
      * several parallel calls to the login user method.
      * If the inProgress flag is false this method checks what the user has wrote on 
      * the input fields. It makes a toast error animation on the following situations:
      * - The email input field is empty.
      * - The password input field is empty.
      * - The email input field not contains a well formated email.
      *
      * When the toast message are generated it also start the ChangeImageColor coroutine
      * that changes the color of the input field to show the user where he did the mistake.
      * If everything is OK it calls the LoginUser method of the firebaseHandler class and 
      * set the inProgress flag to true.
      */
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
            firebaseHandler.firebaseHandlerInstance_.LoginUser(userText,passwordText);
        }
    }
    
    /**
      * @param Image the image of the input field that you want to change the color.
      * @param int the time in seconds that you want to let the color changed.
      * @brief This coroutine change the color of the given image to red, then wait
      * the given number of seconds and then change the color of the given image to white
      * again.
      */
    private IEnumerator ChangeImageColor(Image toMark,int time){
        toMark.color = Color.red;
        yield return new WaitForSeconds(time);
        toMark.color = Color.white;
    }
    
    /**
      * @param string that contains the name of the user that logged in
      * @brief This method should be called when the login process worked successfully,
      * it sets the inProgress flag to false and call the changeSceneWithAnimation method
      * of the class ChangeScene.
      */
    public void userLogedSuccessfully(string name){
        inProgress = false;
        Debug.Log("userLogedSuccessfully: " + name);
        //ChangeScene.changeScene("PantallaPrincipal");
        GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal",0.5f,"");
    }

    /**
      * @param string that contains the error that happened during the logging process
      * @brief This method should be called when the login process failed. It makes a 
      * toast message that shows the given error.
      */
    public void errorLoginUser(string error){
        inProgress = false;
        toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
        toastMessageInstance.makeAnimation("Error: The user or the password are wrong",new Color32(255,0,0,255),2);
    }
}