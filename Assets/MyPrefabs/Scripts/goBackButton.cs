using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goBackButton : MonoBehaviour
{
    [SerializeField] private string NombrePantalla_ = "PantallaLogin";
    public void OnClick(){
        ChangeScene.changeScene(NombrePantalla_);
    }
}
