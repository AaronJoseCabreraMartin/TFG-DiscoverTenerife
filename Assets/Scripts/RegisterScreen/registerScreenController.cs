using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/**
  * @brief Class that controls all the inputs and the buttons of the
  * register screen. It also controls the error messages that are show
  * for giving information to the user.
  */
public class registerScreenController : MonoBehaviour
{
    /**
      * @brief GameObject that contains the input field where the user writes his email.
      */
    [SerializeField] private GameObject user;

    /**
      * @brief GameObject that contains the input field where the user writes his password.
      */
    [SerializeField] private GameObject password;

    /**
      * @brief GameObject that contains the input field where the user writes again
      * his password.
      */
    [SerializeField] private GameObject confirmPassword;

    /**
      * @brief GameObject that contains the toast message that shows the errors and
      * the information to the user.
      */
    [SerializeField] private GameObject toastMessageObject_;


    /**
      * @brief boolean that control if the register process is 
      * currently working. This helps to avoid several parallel calls
      * to the register process.
      */
    private bool inProgress = false;

    /**
      * @param string that contains the email that the user wrote.
      * @return bool true if the given email match the regular expression of 
      * an email, false in other case.
      * @brief This method returns true if the given string contains a email
      * and false in other case.
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
      * This method should be called when the user press the register button.
      * If the inProgress attribute is true, it doesnt do nothing.
      * It checks if the user has wrote something on each field and if 
      * the email is well formated and the both passwords are the same and they
      * had a length of more than 6 characters. 
      * If any of the previous errors happen, it shows a toast message with the error.
      * If every thing was ok, it calls the CreateNewUser firebaseHandler method with
      * the given user and password.
      */
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
            firebaseHandler.firebaseHandlerInstance_.CreateNewUser(userText,passwordText);
        }
    }

    /**
      * @param Image image to change the color.
      * @param int seconds that this coroutine waits to make the color change
      * again.
      * @brief Coroutine that change the color of the given image to red,
      * wait the given number of seconds and then, change the color to
      * white again.
      */
    private IEnumerator ChangeImageColor(Image toMark,int time){
        toMark.color = Color.red;
        yield return new WaitForSeconds(time);
        toMark.color = Color.white;
    }

    /**
      * @param string that contains the new user's name.
      * @brief This method should be called when the register process finished
      * successfully. It changes the inProgress attribute to false and calls
      * the changeSceneWithAnimation method of ChangeScene for transit
      * between the current scene to the main screen.
      */
    public void userCreatedSuccessfully(string name){
        inProgress = false;
        Debug.Log("userCreatedSuccessfully: " + name);
        //ChangeScene.changeScene("PantallaPrincipal");
        GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal",0.5f,"");
    }

    /**
      * @param string that contains information about the error that 
      * happened during the registration process.
      * @brief This method should be called when the register process failed.
      * It changes the inProgress attribute to false and creates
      * a red toast message showing the error.
      */
    public void errorCreatingUser(string error){
        inProgress = false;
        Debug.Log("errorCreatingUser: " + error);
        toastMessageObject_.GetComponent<toastMessage>().makeAnimation(error, new Color32(255,0,0,255),2);
    }
}
