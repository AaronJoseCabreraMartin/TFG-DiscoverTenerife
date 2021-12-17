using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uniqueselectionDesplegableMenu : MonoBehaviour
{
    private bool anyChange_;//para optimizar
    private uniqueselectionToggle[] toggles_;

    public bool showToggles_;
    public int defaultToggleActive_ = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        anyChange_ = true;
        toggles_ = gameObject.transform.Find("toggles").GetComponentsInChildren<uniqueselectionToggle>();
        desactiveAll();
        toggles_[defaultToggleActive_].isOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(anyChange_){
            showAllToggles(showToggles_);
            anyChange_ = false;
        }
    }

    public void OnClick(){
        showToggles_ = !showToggles_;
        anyChange_ = true;
    }

    private void showAllToggles(bool show){
        foreach(var toggle in toggles_){
            toggle.gameObject.SetActive(show); 
        }
    }

    public bool checkToggle(int index){
        return toggles_[index];
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
    }
}
