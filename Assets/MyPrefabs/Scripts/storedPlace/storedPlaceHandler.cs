using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/**
  * @brief Class that controls the element of a panel that
  * represent a stored place. It extracts the index from the
  * name of this GameObject.
  */
public class storedPlaceHandler : MonoBehaviour
{
    /**
      * @brief index of this stored place.
      */
    private int index_;

    /**
      * @brief This method is called on the first frame, it initialices
      * the index_ property as the number that this GameObject has on
      * its name.
      */
    void Awake(){
        index_ = Int32.Parse(Regex.Match(gameObject.name, @"\d+").Value);
    }

    /**
      * @return int with the index of this GameObject.
      * @brief Getter of the index_ property.
      */
    public int getIndex(){
        return index_;
    }
}
