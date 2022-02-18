using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/**
  * @brief This class follows the singleton pattern, the unique instance of the class
  * can be found on an object with the tag "sceneManager". It has to be instanciate because
  * the coroutines cant be used in static methods, so if you need to use the changeSceneWithAnimation
  * method you have to have an instance of the class. It also has the public static method 
  * changeScene that allow change the active scene to other but without any transition.
  */
public class ChangeScene : MonoBehaviour
{
    /**
      * this SerializeField Animator contains a reference to an Animator object
      * that has the fade in and fade out animations
      */
    [SerializeField] private Animator transition_;

    /**
      * this SerializeField GameObject contains a GameObject with the text that will be showed
      * during the fade in animation.
      */
    [SerializeField] private GameObject text_;

    /**
      * @brief This method is called just before the first frame. It search for other sceneManager
      * objects and if there is other one, it will destroy this instance. It also hide the text
      * that will be showed during the fade in animation.
      */
    void Awake(){
        GameObject[] objs = GameObject.FindGameObjectsWithTag("sceneManager");
        if (objs.Length > 1){ //si ya existe una sceneManager no crees otra
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        text_.SetActive(false);//para evitar que reciba el los clicks
        //DontDestroyOnLoad(transform.root.gameObject);
    }

    /**
      * @param string that contains the name of the scene that it will change.
      * @brief This method tries to load the scene that have the given name, if there
      * isnt any scene with the given name it will produce an exception.
      */
    public static void changeScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

    /**
      * @param string the name of the scene to change
      * @param float the duration time of the transition in seconds
      * @param string the text that will be showed during the transition
      * @brief this method starts a coroutine that execute the makeFadeInFadeOutAnimation method.
      */
    public void changeSceneWithAnimation(string sceneName, float transitionTime = 1f, string text = "Loading..."){
        StartCoroutine(makeFadeInFadeOutAnimation(sceneName, transitionTime, text));
    }

    /**
      * @param string the name of the scene to change
      * @param float the duration time of the transition in seconds
      * @param string the text that will be showed during the transition
      * @brief this coroutine shows the given text, makes a fade in animation, change the scene, and then
      * it does a fade out animation to hide both the text and the black image. All the process takes 
      * the given number of seconds.
      */
    private IEnumerator makeFadeInFadeOutAnimation(string sceneName, float transitionTime, string text = "Loading..."){
        transition_.ResetTrigger("Reset");
        text_.SetActive(true);
        text_.GetComponent<Text>().text = text;
        transition_.SetTrigger("FadeIn");
        yield return new WaitForSeconds(transitionTime/2);
        SceneManager.LoadScene(sceneName);
        transition_.ResetTrigger("FadeIn");
        transition_.SetTrigger("FadeOut");
        yield return new WaitForSeconds(transitionTime);
        transition_.ResetTrigger("FadeOut");
        transition_.SetTrigger("Reset");
        text_.SetActive(false);//para evitar que reciba el los clicks
    }
}
