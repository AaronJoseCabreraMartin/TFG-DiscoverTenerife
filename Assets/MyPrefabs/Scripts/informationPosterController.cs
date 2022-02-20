using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This class controls the informative panel
  * prefab that is used for example on the stats screen.
  */
public class informationPosterController : MonoBehaviour
{
    /**
      * @brief GameObject that contains the text that will be showed.
      */
    [SerializeField] private GameObject data_;
    
    /** 
      * @param string
      * @brief This method sets the given string as the text that is
      * showed on the data_ GameObject.
      */
    public void updateData(string newData){
        data_.GetComponent<Text>().text = newData;
    }
}
