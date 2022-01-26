using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private Animator transition_;
    [SerializeField] private GameObject text_;

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

    public static void changeScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

    public void changeSceneWithAnimation(string sceneName, float transitionTime = 1f, string text = "Loading..."){
        StartCoroutine(makeFadeInFadeOutAnimation(sceneName, transitionTime, text));
    }

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
