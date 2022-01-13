using UnityEngine;
using System;
using System.Collections.Generic;

public class UserData{
    public Firebase.Auth.FirebaseUser firebaseUserData_;

    [SerializeField] public List<VisitedPlace> visitedPlaces_;//type of the place, id, veces visitado
    
    private string displayName_;


    public UserData(Firebase.Auth.FirebaseUser newFireBaseUserData, List<Dictionary<string,string>> oldVisitedPlaces = null){
        firebaseUserData_ = newFireBaseUserData;
        visitedPlaces_ = new List<VisitedPlace>();
        if(oldVisitedPlaces != null){
            for(int index = 0; index < oldVisitedPlaces.Count; index++){
                visitedPlaces_.Add(new VisitedPlace(oldVisitedPlaces[index]["type_"],
                                                    Int32.Parse(oldVisitedPlaces[index]["id_"]),
                                                    Int32.Parse(oldVisitedPlaces[index]["timesVisited_"]),
                                                    Int64.Parse(oldVisitedPlaces[index]["lastVisitTimestamp_"])
                                                    ));
            }   
        }
        if(firebaseUserData_.IsAnonymous){
            displayName_ = "Anonymous";
        }else{
            displayName_ = firebaseUserData_.DisplayName;
        }
    }

    public string ToJson(){
        string conversion = "{";
        conversion += $"\"displayName_\" : \"{displayName_}\",";
        conversion += "\"visitedPlaces_\" : [";
        for(int i = 0; i < visitedPlaces_.Count; i++){
            conversion += visitedPlaces_[i].ToJson();
            if(i+1 < visitedPlaces_.Count){
                conversion += ",";
            }
        }
        conversion += "] }";
        return conversion;
    }

    public bool hasVisitPlace(string type, int id){
        return visitedPlaces_.Exists(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id);
    }

    public int countOfVisitedPlaces(){
        return visitedPlaces_.Count;
    }

    //devuelve la cantidad de sitios de una zona dada que ha visitado el usuario
    public int countOfVisitedPlacesOfZone(string zone){
        firebaseHandler firebaseHandlerObject = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        int count = 0;
        foreach(var visitedPlace in visitedPlaces_){
            if(firebaseHandlerObject.getPlaceData(visitedPlace.type_, visitedPlace.id_.ToString())["zone_"] == zone){
                count++;
            }
        }
        return count;
    }

    public int countOfAccumulatedVisits(){
        int count = 0;
        foreach(var visitedPlace in visitedPlaces_){
            count += visitedPlace.timesVisited_;
        }
        return count;
    }

    public string mostVisitedPlace(){
        int maxVisits = 0;
        string maxName = "No visits already";
        firebaseHandler firebaseHandlerObject = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        foreach(var visitedPlace in visitedPlaces_){
            if(maxVisits < visitedPlace.timesVisited_){
                maxVisits = visitedPlace.timesVisited_;
                maxName = firebaseHandlerObject.getPlaceData(visitedPlace.type_, visitedPlace.id_.ToString())["name_"];
            }
        }
        //return maxName + " with " + maxVisits.ToString() + " times";
        return maxName;
    }

    public string mostVisitedZone(){
        int maxVisits = 0;
        Dictionary<string, int> countOfEachZone = new Dictionary<string, int>();
        foreach(string zoneName in mapRulesHandler.getZoneNames()){
            countOfEachZone[zoneName] = 0;
        }
        firebaseHandler firebaseHandlerObject = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        foreach(var visitedPlace in visitedPlaces_){
            countOfEachZone[firebaseHandlerObject.getPlaceData(visitedPlace.type_, visitedPlace.id_.ToString())["zone_"]]++;
        }
        string maxZone = "No visits already";
        foreach(var zone in countOfEachZone.Keys){
            if(maxVisits < countOfEachZone[zone]){
                maxVisits = countOfEachZone[zone];
                maxZone = zone;
            }
        }
        //return maxZone + " with " + maxVisits.ToString() + " times";
        return maxZone;
    }


    public string mostVisitedType(){
        int maxVisits = 0;
        Dictionary<string, int> countOfEachType = new Dictionary<string, int>();
        foreach(string type in mapRulesHandler.getTypesOfSites()){
            countOfEachType[type] = 0;
        }
        foreach(var visitedPlace in visitedPlaces_){
            countOfEachType[visitedPlace.type_]++;
        }
        string maxType = "No visits already";
        foreach(var zone in countOfEachType.Keys){
            if(maxVisits < countOfEachType[zone]){
                maxVisits = countOfEachType[zone];
                maxType = zone;
            }
        }
        //return maxType + " with " + maxVisits.ToString() + " times";
        return maxType;
    }

    public int countVisitedPlacesOfZone(string zone){
        int count = 0;
        firebaseHandler firebaseHandlerObject = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        foreach(var visitedPlace in visitedPlaces_){
            if(firebaseHandlerObject.getPlaceData(visitedPlace.type_, visitedPlace.id_.ToString())["zone_"] == zone){
                count++;
            }
        }
        return count;
    }

    public void newVisitAt(string type, int id, long visitTime){
        VisitedPlace place = visitedPlaces_.Find(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id);
        if(place == null){
            Debug.Log($"No se ha encontrado el sitio con tipo {type} y con id {id} en newVisitAt");
        }else{
            place.newVisitAt(visitTime);
        }
    }
}