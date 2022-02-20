using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief class that controls the colored messages that are shown to 
  * the user giving him information but they hide after a few seconds.
  */
public class toastMessage : MonoBehaviour
{
    /**
      * @brief GameObject that contains the text that is shown on the toast message.
      */
    [SerializeField] private GameObject text_;

    /**
      * @brief GameObject that contains the image that is shown as a background
      * during the message is not hide. You can choose its color.
      */
    [SerializeField] private GameObject image_;

    /**
      * @brief This method is called before the first frame. It calls the show
      * method with false as a parameter.
      */
    void Awake(){
        show(false);
    }

    /**
      * @param bool true if you want to show the message and the background image.
      * @brief This method hides the text and the background image of the toast
      * message if you give it a false, if you give it a true it shows the text
      * and the background image.
      */
    private void show(bool mode){
        text_.SetActive(mode);
        image_.SetActive(mode);
    }

    /**
      * @param string message that will be shown.
      * @param Color32 color of the background image.
      * @param int time in seconds that the message will be shown.
      * @brief This method start the coroutine showMessage with the given parameters.
      */
    public void makeAnimation(string message, Color32 color, int duration){
        StartCoroutine(showMessage(message,color,duration));
    }

    /**
      * @param string message that will be shown.
      * @param Color32 color of the background image.
      * @param int time in seconds that the message will be shown.
      * @brief This coroutine show the text and the background image with 
      * the given color and the given string during the given number of seconds,
      * then it hides the message and the background image.
      */
    private IEnumerator showMessage(string message, Color32 color, int duration){
        text_.GetComponent<Text>().text = message;
        image_.GetComponent<Image>().color = color;
        show(true);
        yield return new WaitForSeconds(duration);
        show(false);
    }
}
