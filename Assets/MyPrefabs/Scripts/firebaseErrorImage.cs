using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class firebaseErrorImage : MonoBehaviour
{
    firebaseHandler firebaseHandlerObject;
    void Awake(){
        firebaseHandlerObject = firebaseHandler.firebaseHandlerInstance_;
    }

    // Update is called once per frame
    void Update()
    {
        // muestra la imagen de error si el firebaseHandlerObject no se encuentra o si no se resolvieron las dependencias
        GetComponent<Image>().enabled = !((firebaseHandlerObject == null || firebaseHandlerObject.FirebaseDependenciesAreResolved()));
        transform.GetChild(0).gameObject.SetActive(!((firebaseHandlerObject == null || firebaseHandlerObject.FirebaseDependenciesAreResolved())));
    }
}
