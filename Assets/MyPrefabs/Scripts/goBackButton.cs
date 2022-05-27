using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
  * @brief Class that controls the goBackButton prefab
  */
public class goBackButton : MonoBehaviour{
    /**
      * @brief string that contains the name of the scene that
      * you will go if you call either the Onclick or the changeSceneWithAnimation
      * methods.
      */
    [SerializeField] private string NombrePantalla_ = "PantallaLogin";

    /**
      * @brief This method should be called when the user clicks on the button.
      * It calls the changeScene method of ChangeScene class with the NombrePantalla_
      * property string.
      */
    public void OnClick(){
        //ChangeScene.changeScene(NombrePantalla_);
        changeSceneWithAnimation();
    }

    /**
      * @brief This method should be called when the user clicks on the button.
      * It calls the changeSceneWithAnimation method of ChangeScene class with the 
      * NombrePantalla_ property string.
      */
    public void changeSceneWithAnimation(){
        GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation(NombrePantalla_, 0.5f, "");
    }

    /**
      * @brief This method is called onces per frame, it checks if the user is pressing the escape button on an android
      * device, if that is the case, it calls the OnClick method.
      */
    void Update(){
      if (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape)){
        //OnClick();
        changeSceneWithAnimation();
      }
    }
}
