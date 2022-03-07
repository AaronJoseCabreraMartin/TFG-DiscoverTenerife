using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief class that controls the request of places information.
  */
public class requestHandler
{
    /**
      * @brief stores the index of the next place that will be returned.
      */
    private int currentIndex_;

    /**
      * @brief List of strings that contains the valid types of the places.
      */
    private List<string> typesOfSites_;//WTF esto deberia cogerlo de las gamerules y si
    // algun lugar tiene un tipo que no esta en la lista arrojar un error.

    /**
      * Stores all the places splited by types.
      * - First index: Type.
      * - Second index: Place ID.
      */
    private List<List<Place>> listOfPlaces_;

    /**
      * Stores the sorted index of the places when the type is defined.
      */
    private List<Dictionary<string,int>> sortedPlaceIndex_;
    // los diccionarios estan ordenados dentro de la lista
    // cada diccionario tiene dos claves type and id, en type
    // esta el indice de la primera de las listas de listOfPlaces_
    // y en id esta el indice de la segunda de las listas de
    // listOfPlaces_ 

    /**
      * @brief true if the places should be sorted by distance, false in other case.
      */
    private bool sortedByDistance_;

    /**
      * @brief It stores the extracted options of which types of places
      * the user wants to see. It is stored for comparing and noticing
      * where they change and for accesing it for sorting the places.
      */
    private Dictionary<string,bool> whatToSeeOptions_;

    /**
      * @brief the index on which we start to count to extract new places.
      * This index is releated to the sortedPlace list.
      */
    private int startIndex_ = 0;

    /**
      * @param Dictionary<string,Dictionary<string,Dictionary<string,string>>> dictionary string conversion
      * the data of all places.
      * @brief constructor of the requestHandler class, it initialize all the properties of the
      * object and instanciate a new Place and store it on the listOfPlaces_ property for each 
      * place of the given dictionary. 
      */
    public requestHandler(Dictionary<string,Dictionary<string,Dictionary<string,string>>> allPlaces){
        typesOfSites_ = new List<string>();
        currentIndex_ = 0;
        listOfPlaces_ = new List<List<Place>>();
        sortedPlaceIndex_ = new List<Dictionary<string,int>>();
        foreach(var typeOfSite in allPlaces.Keys){
            typesOfSites_.Add((string) typeOfSite);
            listOfPlaces_.Add(new List<Place>());
            //sortedPlaceIndex_.Add(new Dictionary<int>());
            foreach(var siteId in allPlaces[typeOfSite].Keys){
                listOfPlaces_[currentIndex_].Add(new Place(allPlaces[typeOfSite][siteId]));
                Dictionary<string,int> pairOfIndex = new Dictionary<string,int>();
                pairOfIndex["type"] = typesOfSites_.IndexOf(typeOfSite);
                pairOfIndex["id"] = Int32.Parse(siteId);
                sortedPlaceIndex_.Add(pairOfIndex);
            }
            currentIndex_++;
        }
        currentIndex_ = startIndex_;
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
        Dictionary<string, bool> whatToSeeOptions = optionsController.optionsControllerInstance_.whatToSeeOptions();
        if(sortedByDistance_ != options.sortByLessDistance() || thereIsAnyChangeInOptions()){
            sortedByDistance_ = options.sortByLessDistance();
            whatToSeeOptions_ = whatToSeeOptions;
            sortPlaces();
            //si hubo un cambio en las opciones, pon el startIndex_ en el principio
            startIndex_ = 0;
            currentIndex_ = 0;
            Debug.Log("Cambio en las opciones!");
        }

        Place toReturn = listOfPlaces_[sortedPlaceIndex_[currentIndex_]["type"]][sortedPlaceIndex_[currentIndex_]["id"]];
        updateIndex();
        //toReturn.startDownload();
        return toReturn;
    }

    /**
      * @brief updates the currentIndex_ property to point the next place type that should be chosen.
      */
    private void updateIndex(){
        currentIndex_ = (currentIndex_+1)%sortedPlaceIndex_.Count;
    }

    /**
      * @brief Sorts the places acording to the current user preferences.
      */
    public void sortPlaces(){
        optionsController options = optionsController.optionsControllerInstance_;
        gpsController gps = gpsController.gpsControllerInstance_;
        if(options.sortByLessDistance()){
            sortedPlaceIndex_.Sort(delegate(Dictionary<string,int> placeIndexA, Dictionary<string,int> placeIndexB){
                if(placeIndexA == placeIndexB){
                    return 0;
                }else{
                    int comparationResult = compareTwoPlacesTakingAwareOfOptions(placeIndexA, placeIndexB);
                    if(comparationResult != 0){
                        return comparationResult;
                    }

                    Place placeA = listOfPlaces_[placeIndexA["type"]][placeIndexA["id"]];
                    Place placeB = listOfPlaces_[placeIndexB["type"]][placeIndexB["id"]];

                    // si ambos son de tipos que quiero ver o ambos no son de tipos que quiero ver 
                    // o ambos han sido visitados o ninguno ha sido visitado, ordena por distancia.
                    double distanceA = gps.CalculateDistanceToUser(placeA.getLatitude(),placeA.getLongitude());
                    double distanceB = gps.CalculateDistanceToUser(placeB.getLatitude(),placeB.getLongitude());
                    
                    return (distanceA < distanceB) ? -1 : 1;
                }
            });
        }else{
            sortedPlaceIndex_.Sort(delegate(Dictionary<string,int> placeIndexA, Dictionary<string,int> placeIndexB){
                if( placeIndexA == placeIndexB){
                    return 0;
                }else{
                    int comparationResult = compareTwoPlacesTakingAwareOfOptions(placeIndexA, placeIndexB);
                    if(comparationResult != 0){
                        return comparationResult;
                    }

                    Place placeA = listOfPlaces_[placeIndexA["type"]][placeIndexA["id"]];
                    Place placeB = listOfPlaces_[placeIndexB["type"]][placeIndexB["id"]];
                    
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

    /**
      * @param Dictionary<string,int> type and id of the place A
      * @param Dictionary<string,int> type and id of the place B
      * @return int the comparation result, it could be -1, 0 or 1.
      * @brief This method compares two given places (dictionaries contains id
      * and type of each one) the possible results are:
      * - -1 if placeA should be first than placeB
      * - 1 if placeB should be first than placeA
      * - 0 if placeA and placeB they are equivalent if we just look the user preferences
      * of place's type and visited or not visited. 
      */
    private int compareTwoPlacesTakingAwareOfOptions(Dictionary<string,int> placeIndexA, Dictionary<string,int> placeIndexB){
        Dictionary<string, bool> whatToSeeOptions = optionsController.optionsControllerInstance_.whatToSeeOptions();
        //si es de un tipo que no quiero y el otro si es de un tipo que quiero, es mejor el otro
        if(!whatToSeeOptions[typesOfSites_[placeIndexA["type"]]] && 
            whatToSeeOptions[typesOfSites_[placeIndexB["type"]]] ){
                return 1;
        }
        //si es de un tipo que no quiero y el otro si es de un tipo que quiero, es mejor el otro
        if(whatToSeeOptions[typesOfSites_[placeIndexA["type"]]] && 
            !whatToSeeOptions[typesOfSites_[placeIndexB["type"]]] ){
                return -1;
        }

        if(!whatToSeeOptions["Already Visited"]){//no quiero visitados
            bool placeAWasVisited = firebaseHandler.firebaseHandlerInstance_.currentUser_.hasVisitPlace(typesOfSites_[placeIndexA["type"]], placeIndexA["id"]);
            bool placeBWasVisited = firebaseHandler.firebaseHandlerInstance_.currentUser_.hasVisitPlace(typesOfSites_[placeIndexB["type"]], placeIndexB["id"]);
            
            //si a ha sido visitado y b no, b es mejor
            if(placeAWasVisited && !placeBWasVisited){
                return 1;
            }

            //si b ha sido visitado y a no, a es mejor
            if(!placeAWasVisited && placeBWasVisited){
                return -1;
            }
        }
        return 0;
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

    /**
      * @brief This method sets the currentIndex_ at the startIndex_ to start
      * from the begining of the sorted list of places.
      */
    public void useStartIndex(){
        currentIndex_ = startIndex_;
    }

    /**
      * @brief This method updates the startIndex_ to take new places.
      */
    public void updateStartIndex(){
        startIndex_ = currentIndex_;
    }

    /**
      * @return true if there is some changes on the whatToSeeOptions, false if they isnt
      * any change.
      * @brief This method checks if the user has changed his type of places preferences
      * or not.
      */
    private bool thereIsAnyChangeInOptions(){
        //true si son diferentes
        Dictionary<string, bool> whatToSeeOptions = optionsController.optionsControllerInstance_.whatToSeeOptions();
        if(whatToSeeOptions_ == null || whatToSeeOptions.Count != whatToSeeOptions_.Count){
            return true;
        }

        foreach(var key in whatToSeeOptions_.Keys){
            if( !whatToSeeOptions.ContainsKey(key) || whatToSeeOptions[key] != whatToSeeOptions_[key]){
                return true;
            }
        }

        return false;
    }

    /**
      * @return int the maximum number of visits between all places.
      * @brief This method calculates the maximum number of visits between each
      * type of place and then, it calculates the maximum number of all types.
      * Basically, it returns the maximum number of visits between all places.
      */
    public int visitsOfMostVisitedPlace(){
      return listOfPlaces_.Max(typeOfPlaces => typeOfPlaces.Max(place => place.getTimesItHasBeenVisited()));
    }
}
