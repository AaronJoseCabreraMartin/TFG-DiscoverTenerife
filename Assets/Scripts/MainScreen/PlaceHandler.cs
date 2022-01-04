using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHandler : MonoBehaviour
{
    [SerializeField] private Animator transition_;

    private static firebaseHandler serverHandler_;
    private Place place_;
    private bool placeLoaded_;

    //almacena que sitio a escogido el usuario cuando clicka en uno
    // para cambiar de escena y consevar la informacion
    public static Place choosenPlace_ = null;
    private static int turn_ = 0;

    void Awake()
    {
        PlaceHandler.turn_ = 0;
        if(!PlaceHandler.serverHandler_){//es el primero
            PlaceHandler.serverHandler_ = GameObject.Find("firebaseHandler").GetComponent<firebaseHandler>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        place_ = null;
        placeLoaded_ = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(placeLoaded_ == false){
            /*
            Esto esta limitando a un askForAPlace en cada frame seria mejor que askForAPlace recibiera que posicion quieres
            por ejemplo teniendo una list<int> con los places que tiene que devolver en cada orden
            */
            if(place_ == null && PlaceHandler.serverHandler_.placesAreReady() && isMyTurn()){
                //assing place
                place_ = PlaceHandler.serverHandler_.askForAPlace();
                PlaceHandler.turn_++;
            }else if (place_ != null && place_.isReady()){
                loadPlace();
            }
        }
    }

    private void loadPlace(){
        transition_.SetTrigger("Success");
        transition_.enabled = false;
        gameObject.GetComponentInChildren<Text>().text = place_.getName();
        gameObject.GetComponent<Image>().sprite = place_.getImage();
        placeLoaded_ = true;
    }

    public void selectPlace(){
        PlaceHandler.choosenPlace_ = place_;
    }

    private bool isMyTurn(){
        return Int32.Parse(Regex.Match(gameObject.name, @"\d+").Value) == PlaceHandler.turn_;
    }
}
