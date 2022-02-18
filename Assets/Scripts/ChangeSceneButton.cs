using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Simple class that gives an easily callable function for changing scene without
  * animations and without instanciate new objects. It is made for buttons that change 
  * between scene quickily.
  */
public class ChangeSceneButton : MonoBehaviour
{
    /**
      * @param string name of the scene that will be loaded
      * @brief it calls the changeScene public static method of ChangeScene class with the given
      * scene name.
      */
    public void onClick(string sceneName){
        ChangeScene.changeScene(sceneName);
    }
}
