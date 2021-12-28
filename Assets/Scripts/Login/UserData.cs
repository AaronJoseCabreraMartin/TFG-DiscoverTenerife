using UnityEngine;
using System;
using System.Collections.Generic;

public class UserData{
    public Firebase.Auth.FirebaseUser firebaseUserData_;
    [SerializeField] private string displayName_;
    [SerializeField] public List<VisitedPlace> visitedPlaces_;//type of the place, id, veces visitado
    
    public UserData(Firebase.Auth.FirebaseUser newFireBaseUserData){
        firebaseUserData_ = newFireBaseUserData;
        visitedPlaces_ = new List<VisitedPlace>();
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
}