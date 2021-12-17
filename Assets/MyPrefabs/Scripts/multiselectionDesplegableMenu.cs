using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class multiselectionDesplegableMenu : MonoBehaviour
{
    public bool showToggles_;
    private bool anyChange_;//para optimizar
    private Toggle[] toggles_;

    public bool defaultToggleValue_ = true;
    

    // Start is called before the first frame update
    void Start()
    {
        anyChange_ = true;
        toggles_ = gameObject.transform.Find("toggles").GetComponentsInChildren<Toggle>();
        foreach(var toggle in toggles_){
            toggle.isOn = defaultToggleValue_;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(anyChange_){
            showAllToggles(showToggles_);
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
        anyChange_ = false;
    }

    public bool checkToggle(int index){
        return toggles_[index];
    }

    public int size(){
        return toggles_.Length;
    }
}
