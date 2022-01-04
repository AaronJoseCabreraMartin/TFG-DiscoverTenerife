using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uniqueselectionToggle : MonoBehaviour
{
    private bool lastState_;
    [SerializeField] private GameObject checkMark_;

    public bool isOn = false;
    // Start is called before the first frame update
    void Start()
    {
        lastState_ = !isOn;
    }

    // Update is called once per frame
    void Update()
    {
        //checkMark_.SetActive(isOn);
        if(isOn != lastState_){
            showCheckMark(isOn);
            lastState_ = isOn;
        }
    }

    public void changeState(bool newState){
        isOn = newState;
        lastState_ = !isOn;
    }

    private void showCheckMark(bool show){
        checkMark_.SetActive(show);
    }
}
