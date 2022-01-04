using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uniqueselectionDesplegableMenu : MonoBehaviour
{
    private bool anyChange_;//para optimizar
    private uniqueselectionToggle[] toggles_;
    [SerializeField] private string defaultText_;
    private Text textField_;

    public bool showToggles_;
    public int defaultToggleActive_ = 0;
    private optionsController optionController_;

    private bool ready_ = false;

    // Start is called before the first frame update
    void Start()
    {
        string toShow = $"Start of {gameObject.name} ";
        anyChange_ = true;
        toggles_ = gameObject.transform.Find("toggles").GetComponentsInChildren<uniqueselectionToggle>();
        textField_ = gameObject.transform.Find("Text").GetComponentsInChildren<Text>()[0];
        textField_.text = defaultText_;
        optionController_ = GameObject.FindGameObjectsWithTag("optionsController")[0].GetComponent<optionsController>();
        selectToggle(defaultToggleActive_);
        /*optionController_.copyOptions();
        foreach(var toggle in toggles_){
            toShow += $"{toggle.isOn} "; 
        }
        Debug.Log(toShow);*/
        ready_ = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(anyChange_){
            showAllToggles(showToggles_);
        }
    }

    public void OnClick(){
        if(optionsController.lastOptionClicked_ == null){//si no hay ningun menu desplegado
            optionsController.lastOptionClicked_ = this.gameObject;//este es el menu desplegado
        }else if(optionsController.lastOptionClicked_ == this.gameObject){//si ya estoy desplegado
            optionsController.lastOptionClicked_ = null;//ya no hay menu desplegado
        }else{//si hay uno desplegado y no soy yo, no hagas nada
            return;
        }
        textField_.text = (textField_.text == defaultText_) ? "Click here to save options" : defaultText_;
        showToggles_ = !showToggles_;
        anyChange_ = true;
        if(textField_.text == defaultText_){
            optionController_.saveOptions();
        }
    }

    public void showAllToggles(bool show){
        foreach(var toggle in toggles_){
            toggle.gameObject.SetActive(show); 
        }
    }

    public bool checkToggle(int index){
        return toggles_[index].isOn;
    }

    public int size(){
        return toggles_.Length;
    }

    private void desactiveAll(){
        foreach(var toggle in toggles_){
            //toggle.isOn = false;
            toggle.changeState(false);
        }
    }

    public void selectToggle(int index){
        desactiveAll();
        //toggles_[index].isOn = true;
        toggles_[index].changeState(true);
        /*string toShow = $"selectToggle with {index}: ";
        foreach(var toggle in toggles_){
            toShow += $"{toggle.isOn} ";
        }
        Debug.Log(toShow);*/
        anyChange_ = true;
    }

    public bool anyChange(){
        return anyChange_;
    }

    public bool ready(){
        return ready_;
    }
}
