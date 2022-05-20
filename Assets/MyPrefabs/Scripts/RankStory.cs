using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that represents data that will be displayed on each element of the rank story
  */
public class RankStory : MonoBehaviour
{
    /**
      * @brief GameObject that shows the range that the player reached that time
      */
    [SerializeField] private GameObject rank_;

    /**
      * @brief GameObject that shows the date where the player reached that time
      */
    [SerializeField] private GameObject date_;

    /**
      * @param Dictonary<string,string> with two entries, range_ with the range name and date_ with the exact date
      * where the player reached that range
      * @brief This method simply sets the text component of rank_ and date_ GameObject to the correspond one.
      */
    public void setData(Dictionary<string,string> information){
        rank_.GetComponent<Text>().text = information["range_"];
        //date_.GetComponent<Text>().text = (new DateTime(Int64.Parse(information["date_"]))).ToString();
        //por algun motivo el timestamp se queda como una fecha normal de tipo 17/04/2022 al subirlo a firebase
        date_.GetComponent<Text>().text = information["date_"];
    }
}
