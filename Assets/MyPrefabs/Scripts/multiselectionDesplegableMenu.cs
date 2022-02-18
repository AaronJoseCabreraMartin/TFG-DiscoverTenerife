using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This method controlls a multiselection desplegable menu.
  */
public class multiselectionDesplegableMenu : MonoBehaviour
{
    /**
      * @brief True if it has to show the toggles, false if it has to hide the toggles.
      */ 
    public bool showToggles_;

    /**
      * @brief True if there is any change on any of the toggles, false in other case.
      */
    private bool anyChange_;//para optimizar

    /**
      * @brief Array with a reference to all the toggles that this menu has.
      */
    private Toggle[] toggles_;

    /**
      * @brief String that contains the text that the button of the menu shows as 
      * default text.
      */
    [SerializeField] private string defaultText_;
    
    /**
      * @brief Text that is showed on the menu's button.
      */
    private Text textField_;

    /**
      * @brief True if all the toggles have to been selected by default, false in other case.
      */
    public bool defaultToggleValue_ = true;

    /**
      * @brief Reference to the optionsController instance.
      */
    private optionsController optionController_;

    /**
      * @brief This method is called on the first frame. It search all the toggles that are
      * childs of this gameobject, then it applies the default value to all of them. It also
      * changes the text of the menu's button to the default text and intialize the anyChange_
      * property to true and the optionController_ property to a reference of the optionsController.
      */
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

    /**
      * @brief This method is called on each frame. It checks if anyChange_ property is true, 
      * it calls the showAllToggles method.
      */
    void Update()
    {
        if(anyChange_){
            showAllToggles(showToggles_);
        }
    }

    /**
      * @brief This method should be called when the user clicks the menu's button.
      * First it checks if the lastOptionClicked_ static property of optionsController is
      * null it sets that property as this GameObject, if that property is this GameObject,
      * it sets that property to null, in other case this method dont do nothing.
      * When the lastOptionClicked_ static property of optionsController is null or it is 
      * this GameObject it changes the button's text to the default text or the saving text.
      * It also changes the showToggles_ property to the opposite value, the anyChange
      * to true and then calls the saveOptions method of optionsController class.
      */
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

    /**
      * @param bool true if you want to show all the toggles, false if you
      * want to hide them.
      * @brief This method hide all the menu's toggles if you give as a parameter a false, 
      * if you give a true it shows all of them. It puts the anyChange_ property as false.
      */
    public void showAllToggles(bool show){
        foreach(var toggle in toggles_){
            toggle.gameObject.SetActive(show); 
        }
        anyChange_ = false;
    }

    /**
      * @param int position of the toggle that you want to check his value. 
      * @return bool true if that toggle is selected, false in other case.
      * @brief This method returns if the toggle that is on the index-th position
      * is selected or not. If the index is bigger than the toggles_ array or negative
      * it will raise an exception.
      */
    public bool checkToggle(int index){
        return toggles_[index].isOn;
    }

    /**
      * @param int postition of the toggle that will change its value. 
      * @param bool state that you want to put to that toggle.
      * @brief This method select or deselect depending on the boolean that you 
      * pass as parameter the index-th toggle. It also puts the anyChange_ property
      * to true.
      */
    public void setStateToggle(int index, bool state){
        toggles_[index].isOn = state;
        anyChange_ = true;
    }

    /**
      * @return int Size of the toggles array.
      * @brief Getter of the toggle's array length.
      */
    public int size(){
        return toggles_.Length;
    }

    /**
      * @return bool anyChange_'s property value.
      * @brief Getter of the anyChange property.
      */
    public bool anyChange(){
        return anyChange_;
    }

    /**
      * @param string that contains the text of the toggle that you want to check.
      * @return bool with the value of the toggle that has the given text.
      * @brief This method returns the state of the toggle that has the given text,
      * if there isnt any toggle with the given text it returns false.
      */
    public bool checkToggleByText(string name){
        foreach(var toggle in toggles_){
            if(toggle.transform.Find("Label").gameObject.GetComponent<Text>().text == name){
                return toggle.isOn;
            }
        }
        Debug.Log($"Error, no se encontro el toggle con nombre {name}");
        return false;
    }

    /**
      * @param string that contains the text of the toggle that you want to check.
      * @param bool state that you want to set on the toggle that has that text.
      * @brief This method sets the given state on the toggle that has the given text.
      */
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
