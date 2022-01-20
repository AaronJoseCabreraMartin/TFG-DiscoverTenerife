using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uniqueselectionDesplegableMenu : MonoBehaviour
{
    private bool anyChange_;//para optimizar
    //private uniqueselectionToggle[] toggles_;
    private Toggle[] toggles_;
    [SerializeField] private string defaultText_;
    private Text textField_;

    public bool showToggles_;
    private optionsController optionController_;

    // Start is called before the first frame update
    void Awake()
    {
        anyChange_ = true;
        //toggles_ = gameObject.transform.Find("toggles").GetComponentsInChildren<uniqueselectionToggle>();
        toggles_ = gameObject.transform.Find("toggles").GetComponentsInChildren<Toggle>();
        textField_ = gameObject.transform.Find("Text").GetComponentsInChildren<Text>()[0];
        textField_.text = defaultText_;
        optionController_ = optionsController.optionsControllerInstance_;
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
        //if(textField_.text == defaultText_){
            optionController_.saveOptions();
        //}
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
            toggle.isOn = false;
        }
    }

    public void selectToggle(int index){
        desactiveAll();
        toggles_[index].isOn = true;
        anyChange_ = true;
    }

    public void saveOptions(){
        optionController_.saveOptions();
    }
}
/*





private Toggle[] toggles_;
    private Text textField_;
    public bool showToggles_;
    [SerializeField] private string defaultText_;
    private optionsController optionController_;
    void Awake()
    {   
        toggles_ = gameObject.transform.Find("toggles").GetComponentsInChildren<Toggle>();
        foreach(Toggle toggle in toggles_){
            toggle.onValueChanged.AddListener((t) => OnToggleValueChanged(toggle, t));
        }
 
        optionController_ = optionsController.optionsControllerInstance_;
        textField_ = gameObject.transform.Find("Text").GetComponentsInChildren<Text>()[0];
        textField_.text = defaultText_;
    }

    private void OnToggleValueChanged(Toggle toggle, bool newValue)
    {
        if (newValue)
        {
            for (int i = 0; i < toggles_.Length; i++)
            {
                if (toggles_[i] != toggle)
                    toggles_[i].isOn = false;
            }
        }
    }

    void Update(){
        showAllToggles(showToggles_);
    }

    public void showAllToggles(bool show){
        foreach(var toggle in toggles_){
            toggle.gameObject.SetActive(show); 
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
    }

    public bool checkToggle(int index){
        return toggles_[index].isOn;
    }

    public void selectToggle(int index){
        toggles_[index].isOn = true;
    }

*/