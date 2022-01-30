using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class multiselectionDesplegableMenu : MonoBehaviour
{
    public bool showToggles_;
    private bool anyChange_;//para optimizar
    private Toggle[] toggles_;
    [SerializeField] private string defaultText_;
    private Text textField_;

    public bool defaultToggleValue_ = true;
    private optionsController optionController_;

    // Start is called before the first frame update
    void Start()
    {
        anyChange_ = true;
        toggles_ = gameObject.transform.Find("toggles").GetComponentsInChildren<Toggle>();
        foreach(var toggle in toggles_){
            toggle.isOn = defaultToggleValue_;
        }
        textField_ = gameObject.transform.Find("Text").GetComponentsInChildren<Text>()[0];
        textField_.text = defaultText_;
        optionController_ = GameObject.FindGameObjectsWithTag("optionsController")[0].GetComponent<optionsController>();
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
        optionController_.saveOptions();
    }

    public void showAllToggles(bool show){
        foreach(var toggle in toggles_){
            toggle.gameObject.SetActive(show); 
        }
        anyChange_ = false;
    }

    public bool checkToggle(int index){
        return toggles_[index].isOn;
    }

    public void setStateToggle(int index, bool state){
        toggles_[index].isOn = state;
        anyChange_ = true;
    }

    public int size(){
        return toggles_.Length;
    }

    public bool anyChange(){
        return anyChange_;
    }

    public bool checkToggleByText(string name){
        foreach(var toggle in toggles_){
            if(toggle.transform.Find("Label").gameObject.GetComponent<Text>().text == name){
                return toggle.isOn;
            }
        }
        Debug.Log($"Error, no se encontro el toggle con nombre {name}");
        return false;
    }

    public void setToggleStateByText(string name, bool state){
        foreach(var toggle in toggles_){
            //buscando Already Visited, encontrado Already Visited
            //Debug.Log($"{name} == {toggle.transform.Find("Label").gameObject.GetComponent<Text>().text} ? {toggle.transform.Find("Label").gameObject.GetComponent<Text>().text.ToString() == name}");
            if(toggle.transform.Find("Label").gameObject.GetComponent<Text>().text == name){
                toggle.isOn = state;
                anyChange_ = true;
                return;
            }
        }
        Debug.Log($"Error, no se encontro el toggle con nombre {name}");
    }
}
