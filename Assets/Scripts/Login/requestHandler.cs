using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief class that controls the request of places information.
  */
public class requestHandler
{
    /**
      * @brief stores the type of the next place that will be returned.
      */
    private int actualIndexType_;

    /**
      * @brief List of strings that contains the valid types of the places.
      */
    private List<string> typesOfSites_;//WTF esto deberia cogerlo de las gamerules y si
    // algun lugar tiene un tipo que no esta en la lista arrojar un error.
    
    /**
      * @brief stores the index of the next place that will be returned.
      */
    private int actualIndexPlace_;

    /**
      * Stores all the places splited by types.
      * - First index: Type.
      * - Second index: Place ID.
      */
    private List<List<Place>> listOfPlaces_;

    /**
      * Stores the sorted index of the places when the type is defined.
      */
    private List<List<int>> sortedPlaceIndex_;
    // almacena una lista de int por cada tipo
    // cada posicion de esa lista de int indica un indice
    // del lugar que debemos buscar en listOfPlaces

    /**
      * @brief true if the places should be sorted by distance, false in other case.
      */
    private bool sortedByDistance_;

    /**
      * @param Dictionary<string,Dictionary<string,Dictionary<string,string>>> dictionary string conversion
      * the data of all places.
      * @brief constructor of the requestHandler class, it initialize all the properties of the
      * object and instanciate a new Place and store it on the listOfPlaces_ property for each 
      * place of the given dictionary. 
      */
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
    /**
      * @returns Place the next place that should be showed to the user.
      * @brief this method access to the sortByLessDistance method of optionsController class and
      * it shorts the places acording to the chosen sort method calling the sortPlaces method if the
      * chosen sort method has changed since last call to this method. It returns the next place that
      * should be showed to the user acording to the sorting method and the prefferences of the current
      * user of what the current user wants to see.
      */
    public Place askForAPlace(){
        optionsController options = optionsController.optionsControllerInstance_;
        firebaseHandler firebaseHandlerObject = firebaseHandler.firebaseHandlerInstance_;
        if(sortedByDistance_ != options.sortByLessDistance()){
            sortedByDistance_ = options.sortByLessDistance();
            sortPlaces();
        }
        Dictionary<string, bool> whatToSeeOptions = options.whatToSeeOptions();
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
        //toReturn.startDownload();
        return toReturn;
    }

    /**
      * @brief updates the actualIndexPlace_ property to point the next place type that should be chosen.
      */
    private void updateTypeIndex(){
        actualIndexType_ = (actualIndexType_+1)%listOfPlaces_.Count;
        if(actualIndexPlace_ != 0){
            actualIndexPlace_ = 0;
        }
    }

    /**
      * @brief updates the actualIndexPlace_ property to point the next place id that should be chosen.
      * If it finshes all the places from that type, it calls the updateTypeIndex method.
      */
    private void updatePlaceIndex(){
        if(actualIndexPlace_ + 1 >= listOfPlaces_[actualIndexType_].Count){
            actualIndexPlace_ = 0;
            //si el indextype se pasa vuelve a 0
            updateTypeIndex();
        }else{
            actualIndexPlace_++;
        }
    }

    /**
      * @brief Sorts the places acording to the current user preferences.
      */
    public void sortPlaces(){
        optionsController options = optionsController.optionsControllerInstance_;
        gpsController gps = gpsController.gpsControllerInstance_; 
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

    /**
      * @brief Calls the method oneMoreVisit of the given instance of the class Place that
      * matches the given type and id.
      */
    public void oneMoreVisitToPlaceByTypeAndId(string type, string id){
        listOfPlaces_[typesOfSites_.IndexOf(type)][Int32.Parse(id)].oneMoreVisit();
    }

    /**
      * @return Place with the given type and id.
      * @brief It returns the place that match the given type and id, if there inst any
      * place with that index or type it will raise an exception.
      */
    public Place getPlaceByTypeAndId(string type, string id){
        return listOfPlaces_[typesOfSites_.IndexOf(type)][Int32.Parse(id)];
    }
}
