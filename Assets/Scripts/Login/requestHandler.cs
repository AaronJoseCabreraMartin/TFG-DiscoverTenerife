using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class requestHandler
{
    
    private int actualIndexType_;
    private List<string> typesOfSites_;
    
    private int actualIndexPlace_;
    private List<List<Place>> listOfPlaces_;

    public requestHandler(Dictionary<string,Dictionary<string,Dictionary<string,string>>> allPlaces){
        typesOfSites_ = new List<string>();
        actualIndexType_ = 0;
        listOfPlaces_ = new List<List<Place>>();
        foreach(var typeOfSite in allPlaces.Keys){
            typesOfSites_.Add((string) typeOfSite);
            listOfPlaces_.Add(new List<Place>());
            foreach(var siteId in allPlaces[typeOfSite].Keys){
                listOfPlaces_[actualIndexType_].Add(new Place(allPlaces[typeOfSite][siteId]));
            }
            actualIndexType_++;
        }
        actualIndexType_ = 0;
        actualIndexPlace_ = 0;
    }

    /*
        esto debe extraerse del menu de opciones
        en modes podemos recibir:
            - distance     -> ordenar por distancia -> si este esta activado debemos mirar la posicion del usuario si no no
            - most visited -> ordenar por mas visitados

            - Viewpoints      -> mostrar miradores
            - Hiking Routes   -> mostrar senderos
            - Beachs          -> mostrar playas
            - Natural Pools   -> mostrar charcos/piscinas naturales
            - Natural Parks   -> mostrar parques naturales
            - Already Visited -> mostrar los ya vistos
    */
    public Place askForAPlace(){
        optionsController options = GameObject.FindGameObjectsWithTag("optionsController")[0].GetComponent<optionsController>();
        bool sortByLessDistance = options.sortByLessDistance();
        Dictionary<string, bool> whatToSeeOptions = options.whatToSeeOptions();
        /*
        
        quizas cuando options controller detecte el cambio de orden deberia avisar
        a esta clase con algun metodo tipo sort
        elegir que sitios mostrar o no es facil solo hay que mirar que el tipo en concreto 
        este a true o que no lo haya visitado

        */
        bool placeDecided = false;
        //while(!placeDecided){
            //cambia de tipo hasta que encuentres uno que si esta
            while(!whatToSeeOptions[typesOfSites_[actualIndexType_]]){
                updateTypeIndex();            
            }
            /*if(!whatToSeeOptions["Already Visited"]){
                int previousIndexType = actualIndexType_;
                while(site visitado){
                    updatePlaceIndex();
                }
                if(previousIndexType == actualIndexType_){
                    placeDecided = true;
                }
            }else{
                placeDecided = true;
            }*/
        //}


        Place toReturn = listOfPlaces_[actualIndexType_][actualIndexPlace_];
        updatePlaceIndex();
        toReturn.startDownload();
        return toReturn;
    }

    private void updateTypeIndex(){
        actualIndexType_ = (actualIndexType_+1)%listOfPlaces_.Count;
    }

    private void updatePlaceIndex(){
        if(actualIndexPlace_ + 1 >= listOfPlaces_[actualIndexType_].Count){
            actualIndexPlace_ = 0;
            //si el indextype se pasa vuelve a 0
            updateTypeIndex();
        }else{
            actualIndexPlace_++;
        }
    }
}
