using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneButton : MonoBehaviour
{
    public void onClick(string sceneName){
        ChangeScene.changeScene(sceneName);
    }
}
