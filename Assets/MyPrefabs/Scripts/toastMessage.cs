using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief class that controls the colored messages that are shown to 
  * the user giving him information but they hide after a few seconds.
  * It has a system of queues because if you start several pararell
  * coroutines at the same time of showing the toast message only the
  * last one will be shown, but I solved this problem using the queue
  * system. Each message will wait until its the first one of the queue
  * and there inst any other message showing itself at that moment to show
  * the first one.
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
      * @brief true if a toastMessage is shown, false if there isnt any toastMessage being
      * shown.
      */
    private bool inProcess_;

    /**
      * @brief This list stores strings of the messages of the nexts toastMessages that 
      * will be shown.
      */
    private List<string> messagesQueue_;

    /**
      * @brief This list stores the colors of the nexts toastMessages that will be shown.
      */
    private List<Color32> colorsQueue_;

    /**
      * @brief This list stores the duration of the nexts toastMessages that will be shown.
      */
    private List<int> durationQueue_;

    /**
      * @brief This method is called before the first frame. It calls the show
      * method with false as a parameter and it initialize all the queues and 
      * the inProcess_ property as false.
      */
    void Awake(){
        inProcess_ = false;
        messagesQueue_ = new List<string>();
        colorsQueue_ = new List<Color32>();
        durationQueue_ = new List<int>();
        show(false);
    }

    /**
      * @brief This method is called each frame. It checks if the inProcess_ property
      * is false and if the three queues arent empty, if that is the case it calls the
      * showFirstElementOfTheQueue method in other case it just do nothing.
      */
    void Update(){
      if(!inProcess_ && messagesQueue_.Count != 0 && colorsQueue_.Count != 0 && durationQueue_.Count != 0){
        showFirstElementOfTheQueue();
      }
    }

    /**
      * @brief This method remove the first element of the three queues and 
      * start the showMessage coroutine with those elements.
      */
    void showFirstElementOfTheQueue(){
      string message = messagesQueue_[0];
      messagesQueue_.RemoveAt(0);
      Color32 color = colorsQueue_[0];
      colorsQueue_.RemoveAt(0);
      int duration = durationQueue_[0];
      durationQueue_.RemoveAt(0);
      StartCoroutine(showMessage(message,color,duration));
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
      * @brief This method puts on the queue the message to be shown.
      */
    public void makeAnimation(string message, Color32 color, int duration){
      messagesQueue_.Add(message);
      colorsQueue_.Add(color);
      durationQueue_.Add(duration);
    }

    /**
      * @param string message that will be shown.
      * @param Color32 color of the background image.
      * @param int time in seconds that the message will be shown.
      * @brief This coroutine show the text and the background image with 
      * the given color and the given string during the given number of seconds,
      * then it hides the message and the background image. When it starts, it 
      * puts the inProcess_ property to true and just before endind it sets the
      * inProcess_ property as false.
      */
    private IEnumerator showMessage(string message, Color32 color, int duration){
        inProcess_ = true;
        text_.GetComponent<Text>().text = message;
        image_.GetComponent<Image>().color = color;
        show(true);
        yield return new WaitForSeconds(duration);
        show(false);
        inProcess_ = false;
    }
}
