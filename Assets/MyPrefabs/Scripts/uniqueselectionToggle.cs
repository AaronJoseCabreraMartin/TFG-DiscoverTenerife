using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uniqueselectionToggle : MonoBehaviour
{
    private bool lastState_;

    public bool isOn;
    // Start is called before the first frame update
    void Start()
    {
        lastState_ = !isOn;
    }

    // Update is called once per frame
    void Update()
    {
        if(isOn != lastState_){
            showCheckMark(isOn);
            lastState_ = isOn;
        }
    }

    public void changeState(){
        isOn = !isOn;
    }

    private void showCheckMark(bool show){
        gameObject.transform.Find("Background").transform.Find("Checkmark").gameObject.SetActive(show);
    }
}
