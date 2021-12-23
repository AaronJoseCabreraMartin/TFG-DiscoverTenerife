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

            - seen         ->mostrar los ya vistos
            - viewpoints   ->mostrar miradores
            - beach        ->mostrar playas
            - hiking route ->mostrar senderos
            - natural pool ->mostrar charcos/piscinas naturales
            - natural park ->mostrar parques naturales
    */
    public Place askForAPlace(/*Dictionary<string,bool> modes, double latitude = 0, double longitude = 0*/){
        Place toReturn = listOfPlaces_[actualIndexType_][actualIndexPlace_];
        if(actualIndexPlace_ + 1 >= listOfPlaces_[actualIndexType_].Count){
            actualIndexPlace_ = 0;
            //si el indextype se pasa vuelve a 0
            actualIndexType_ = (actualIndexType_+1)%listOfPlaces_.Count;
        }else{
            actualIndexPlace_++;
        }
        toReturn.startDownload();
        return toReturn;
    }
}
