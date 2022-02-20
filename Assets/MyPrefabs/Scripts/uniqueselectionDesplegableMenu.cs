using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This class controls the unique selection desplegable menu.
  */
public class uniqueselectionDesplegableMenu : MonoBehaviour
{
    /**
      * @brief Array of GameObjects that each of them contains a toggle.
      */
    [SerializeField] private GameObject[] toggles_;

    /**
      * @brief The default text that will be shown when the user didnt 
      * have clicked on the menu.
      */
    [SerializeField] private string defaultText_;

    /**
      * @brief The default text that will be shown when the user
      * have clicked on the menu.
      */
    [SerializeField] private string saveText_ = "Click here to save options";

    /**
      * @brief Text that shows the state of the menu, waiting to be clicked for showing itself or
      * waiting to be clicked for hiding itself.
      */
    [SerializeField] private Text textField_;

    /**
      * @brief true if there is some change that could reconfigure 
      * the menu or the toggle. 
      */
    private bool anyChange_;//para optimizar

    /**
      * @brief int that store the position of the current active toggle.
      */
    private int activeToggle_;

    /**
      * @brief true if the toggles should be shown, false if the toggles
      * should be hidden.
      */
    public bool showToggles_;

    /**
      * @brief This method is called before the first frame, it initialices
      * anyChange_ as true, showToggles_ as false, the shown text as the default text
      * and the active toggle as zero. 
      */
    void Awake()
    {
        anyChange_ = true;
        showToggles_ = false;
        textField_.text = defaultText_;
        activeToggle_ = 0;
    }

    /**
      * @brief This method is called once per frame. If anychange_ is false, it doesnt do
      * nothing but if its true, it calls showAllToggles and updateState methods.
      */
    void Update()
    {
        if(anyChange_){
            showAllToggles(showToggles_);
            updateState();
        }
    }

    /**
      * @brief This method should be called when the main button of the menu is clicked
      * by the user. It checks if lastOptionClicked_ static property of optionsController
      * class:
      * - if it is null, it sets lastOptionClicked_ as this gameObject.
      * - if it is this gameObject, it sets it as null.
      * - if it is another gameObject, it doesnt do nothing.
      *
      * That is the way to control that only one of the desplegables menus are
      * opened at the same time.
      * It also sets the correspondent text to the text field. Hide or shown
      * the toggles, and it calls the saveOptions method of optionsController class.
      */
    public void OnClick(){
        if(optionsController.lastOptionClicked_ == null){//si no hay ningun menu desplegado
            optionsController.lastOptionClicked_ = this.gameObject;//este es el menu desplegado
        }else if(optionsController.lastOptionClicked_ == this.gameObject){//si ya estoy desplegado
            optionsController.lastOptionClicked_ = null;//ya no hay menu desplegado
        }else{//si hay uno desplegado y no soy yo, no hagas nada
            return;
        }
        textField_.text = (textField_.text == defaultText_) ? saveText_ : defaultText_;
        showToggles_ = !showToggles_;
        anyChange_ = true;
        //si es el default text es porque estaba el savetext y le hizo click
        //if(textField_.text == defaultText_){
            optionsController.optionsControllerInstance_.saveOptions();
        //}
    }

    /**
      * @param bool true if you want to show all the toggles, false if you want to hide them.
      */
    public void showAllToggles(bool show){
        foreach(var toggle in toggles_){
            toggle.SetActive(show); 
        }
    }

    /**
      * @param int the toggle's number that you want to check.
      * @return bool, true if the activeToggle_ is the same as the given number,
      * false in other case.
      * @brief This method returns true if the toggle that is on the index-th position
      * is on, in other case its false.
      */
    public bool checkToggle(int index){
        return index == activeToggle_;
    }

    /**
      * @return int Length of the toggles_ property.
      * @brief getter of the length of the toggles_ property.
      */
    public int size(){
        return toggles_.Length;
    }

    /**
      * @param int the position of the toggle that you want to select.
      * @brief This method active the toggle that is on the index-th position.
      * it also puts the anyChange_ property to true.
      */
    public void selectToggle(int index){
        activeToggle_ = index;
        anyChange_ = true;
    }

    /**
      * @brief This method calls the saveOptions method of the optionsController class.
      */
    public void saveOptions(){
        optionsController.optionsControllerInstance_.saveOptions();
    }

    /**
      * @brief This method put off all the toggles except the active one.
      */
    private void updateState(){
        for(int index = 0; index < toggles_.Length; index++){
            toggles_[index].transform.Find("Background/Checkmark").gameObject.SetActive(index==activeToggle_);
        }
    }
}