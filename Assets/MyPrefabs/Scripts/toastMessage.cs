using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class toastMessage : MonoBehaviour
{
    [SerializeField] private GameObject text_;
    [SerializeField] private GameObject image_;

    void Awake(){
        show(false);
    }

    private void show(bool mode){
        text_.SetActive(mode);
        image_.SetActive(mode);
    }

    public void makeAnimation(string message, Color32 color, int duration){
        StartCoroutine(showMessage(message,color,duration));
    }

    private IEnumerator showMessage(string message, Color32 color, int duration){
        text_.GetComponent<Text>().text = message;
        image_.GetComponent<Image>().color = color;
        show(true);
        yield return new WaitForSeconds(duration);
        show(false);
    }
}
