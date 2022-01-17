using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class storedPlaceHandler : MonoBehaviour
{
    private int index_;
    void Awake(){
        index_ = Int32.Parse(Regex.Match(gameObject.name, @"\d+").Value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getIndex(){
        return index_;
    }
}
