using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class informationPosterController : MonoBehaviour
{
    [SerializeField] private GameObject data_;
    
    public void updateData(string newData){
        data_.GetComponent<Text>().text = newData;
    }
}
