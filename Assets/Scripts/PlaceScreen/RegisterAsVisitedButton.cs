using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterAsVisitedButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    Falta:
    - el cartel de no lo has visitado aun debe cambiar a ya lo has visitado en un tono verde
    - aumentar la barra del nivel
    - el historial se puede hacer de dos maneras:
        - visitados recientemente (por timestamp de mayor a menor, mas nuevo a mas viejo, se fija en que orden visitaste por ULTIMA vez cada sitio)
        - orden de visitados (por el vector de sitios visitados, se fija en que orden visitaste por PRIMERA vez un sitio)
    */

    public void tryToRegisterAsVisited(){
        gpsController gps = GameObject.FindGameObjectsWithTag("gpsController")[0].GetComponent<gpsController>(); 
        if(gps.CalculateDistanceToUser(PlaceHandler.choosenPlace_.getLatitude(), PlaceHandler.choosenPlace_.getLongitude()) < 0.05){//si esta a menos de 50m
            firebaseHandler firebaseHandlerObject = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
            if(firebaseHandlerObject.cooldownVisitingPlaceByNameFinished(PlaceHandler.choosenPlace_.getName())){
                firebaseHandlerObject.userVisitedPlaceByName(PlaceHandler.choosenPlace_.getName());
                Debug.Log("mostrar mensaje de ok!");
            }else{
                Debug.Log("mostrar mensaje de aun debe esperar un poco para volver a visitar este sitio!");
            }
        }else{
            Debug.Log("mostrar mensaje de demasiado lejos");
        }
    }
}
