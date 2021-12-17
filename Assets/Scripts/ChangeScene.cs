using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    static public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
