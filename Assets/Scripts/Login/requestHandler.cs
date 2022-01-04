using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class requestHandler
{
    
    private int actualIndexType_;
    private List<string> typesOfSites_;
    
    private int actualIndexPlace_;
    private List<List<Place>> listOfPlaces_;
    // almacena una lista de int por cada tipo
    // cada posicion de esa lista de int indica un indice
    // del lugar que debemos buscar en listOfPlaces
    private List<List<int>> sortedPlaceIndex_;

    private bool sortedByDistance_;

    public requestHandler(Dictionary<string,Dictionary<string,Dictionary<string,string>>> allPlaces){
        typesOfSites_ = new List<string>();
        actualIndexType_ = 0;
        listOfPlaces_ = new List<List<Place>>();
        sortedPlaceIndex_ = new List<List<int>>();
        foreach(var typeOfSite in allPlaces.Keys){
            typesOfSites_.Add((string) typeOfSite);
            listOfPlaces_.Add(new List<Place>());
            sortedPlaceIndex_.Add(new List<int>());
            foreach(var siteId in allPlaces[typeOfSite].Keys){
                listOfPlaces_[actualIndexType_].Add(new Place(allPlaces[typeOfSite][siteId]));
                sortedPlaceIndex_[actualIndexType_].Add(Int32.Parse(siteId));
            }
            actualIndexType_++;
        }
        actualIndexType_ = 0;
        actualIndexPlace_ = 0;
        sortPlaces();
        sortedByDistance_ = true;
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
        firebaseHandler firebaseHandlerObject = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        if(sortedByDistance_ != options.sortByLessDistance()){
            sortedByDistance_ = options.sortByLessDistance();
            sortPlaces();
        }
        Dictionary<string, bool> whatToSeeOptions = options.whatToSeeOptions();
        
        /*
        
        quizas cuando options controller detecte el cambio de orden deberia avisar
        a esta clase con algun metodo tipo sort
        elegir que sitios mostrar o no es facil solo hay que mirar que el tipo en concreto 
        este a true o que no lo haya visitado

        */
        bool placeDecided = false;
        while(!placeDecided){
            //cambia de tipo hasta que encuentres uno que si esta
            while(!whatToSeeOptions[typesOfSites_[actualIndexType_]]){
                updateTypeIndex();            
            }
            if(!whatToSeeOptions["Already Visited"]){
                int previousIndexType = actualIndexType_;
                while(firebaseHandlerObject.actualUser_.hasVisitPlace(typesOfSites_[actualIndexType_],sortedPlaceIndex_[actualIndexType_][actualIndexPlace_])){
                    updatePlaceIndex();
                }
                //si cambia de tipo debemos asegurarnos de que el tipo actual lo quiere ver
                if(previousIndexType == actualIndexType_){
                    placeDecided = true;
                }
            }else{
                placeDecided = true;
            }
        }

        Place toReturn = listOfPlaces_[actualIndexType_][sortedPlaceIndex_[actualIndexType_][actualIndexPlace_]];
        updatePlaceIndex();
        toReturn.startDownload();
        return toReturn;
    }

    private void updateTypeIndex(){
        actualIndexType_ = (actualIndexType_+1)%listOfPlaces_.Count;
        if(actualIndexPlace_ != 0){
            actualIndexPlace_ = 0;
        }
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

    public void sortPlaces(){
        optionsController options = GameObject.FindGameObjectsWithTag("optionsController")[0].GetComponent<optionsController>();
        gpsController gps = GameObject.FindGameObjectsWithTag("gpsController")[0].GetComponent<gpsController>(); 
        if(options.sortByLessDistance()){
            for(int typeIndex = 0; typeIndex < sortedPlaceIndex_.Count; typeIndex++){
                sortedPlaceIndex_[typeIndex].Sort(delegate(int placeIndexA, int placeIndexB){
                    if(/*(placeIndexA == null && placeIndexB == null) ||*/ placeIndexA == placeIndexB){
                        return 0;
                    /*}else if(placeIndexA == null){//A < B
                        return -1;
                    }else if(placeIndexB == null){//A > B
                        return 1;*/
                    }else{
                        Place placeA = listOfPlaces_[typeIndex][placeIndexA];
                        double distanceA = gps.CalculateDistanceToUser(placeA.getLatitude(),placeA.getLongitude());
                        Place placeB = listOfPlaces_[typeIndex][placeIndexB];
                        double distanceB = gps.CalculateDistanceToUser(placeB.getLatitude(),placeB.getLongitude());
                        return (distanceA < distanceB) ? -1 : 1;
                    }
                });
            }
        }else{
            /*
            Comprobar que funciona bien la playa 126 le cambie el numero de visitas y creo que a las dos ultimas tambien
            Si hay empate podriamos ordenar por distancia
            */
            for(int typeIndex = 0; typeIndex < sortedPlaceIndex_.Count; typeIndex++){
                sortedPlaceIndex_[typeIndex].Sort(delegate(int placeIndexA, int placeIndexB){
                    if(/*(placeIndexA == null && placeIndexB == null) ||*/ placeIndexA == placeIndexB){
                        return 0;
                    /*}else if(placeIndexA == null){//A < B
                        return -1;
                    }else if(placeIndexB == null){//A > B
                        return 1;*/
                    }else{
                        Place placeA = listOfPlaces_[typeIndex][placeIndexA];
                        Place placeB = listOfPlaces_[typeIndex][placeIndexB];
                        //Si tienen las mismas visitas ordenamos de menos distancia a mayor
                        if(placeA.getTimesItHasBeenVisited() == placeB.getTimesItHasBeenVisited()){
                            double distanceA = gps.CalculateDistanceToUser(placeA.getLatitude(),placeA.getLongitude());
                            double distanceB = gps.CalculateDistanceToUser(placeB.getLatitude(),placeB.getLongitude());
                            return (distanceA < distanceB) ? -1 : 1;
                        }else{
                            //esto ordena de menor a mayor pero nosotros queremos que sea el mas visitado el primero
                            return (placeA.getTimesItHasBeenVisited() > placeB.getTimesItHasBeenVisited() ? -1 : 1);
                        }
                    }
                });
            }
        }
    }

    public void oneMoreVisitToPlaceByTypeAndId(string type, string id){
        listOfPlaces_[typesOfSites_.IndexOf(type)][Int32.Parse(id)].oneMoreVisit();
    }
    public Place getPlaceByTypeAndId(string type, string id){
        return listOfPlaces_[typesOfSites_.IndexOf(type)][Int32.Parse(id)];
    }
}
