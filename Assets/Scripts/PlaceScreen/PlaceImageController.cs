using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceImageController : MonoBehaviour
{
    private bool loaded_ = false;
    // Start is called before the first frame update
    void Start()
    {
        if(PlaceHandler.choosenPlace_ != null){
          FillFields();
        }
    }

    void Update()
    {
        if(PlaceHandler.choosenPlace_ != null && !loaded_){
          FillFields();
        }
    }

    void FillFields(){
      /* TODO
      - mostrar si se ha visitado o no un lugar
      - mostrar distancia al usuario
      - mostrar descripcion
      */
      gameObject.transform.Find("Name").gameObject.GetComponent<Text>().text = PlaceHandler.choosenPlace_.getName();
      gameObject.transform.Find("Address").gameObject.GetComponent<Text>().text = PlaceHandler.choosenPlace_.getAddress();
      gameObject.GetComponent<Image>().sprite = PlaceHandler.choosenPlace_.getImage();
      loaded_ = true;
    }
}
