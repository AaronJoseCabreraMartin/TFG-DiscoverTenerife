using UnityEngine;
using System;
using System.Collections.Generic;

public class UserData{
    public Firebase.Auth.FirebaseUser firebaseUserData_;

    [SerializeField] public List<VisitedPlace> visitedPlaces_;//type of the place, id, veces visitado
    
    private string displayName_;
    // coordenadas de la "casa" del usuario
    private double baseLatitude_;
    private double baseLongitude_;
    //si el usario se registra sin tener la ubicacion activada puede que la base no sea establecida
    private bool baseEstablished_;

    //almacena el UID de los usuarios amigos
    private List<string> friendList_;
    private List<FriendData> friendDataList_;

    //almacena el UID de los usuarios que te han enviado una peticion de amistad
    private List<string> friendInvitationsList_;
    private List<newFriendData> newFriendDataList_;

    //almacena el UID de los usuarios que han aceptado tu solicitud de amistad
    //private List<string> acceptedFriendInvitationsList_;//darle a los usuarios permiso para añadirse en la lista de amigos de los otros???

    public UserData(Firebase.Auth.FirebaseUser newFireBaseUserData, List<Dictionary<string,string>> oldVisitedPlaces = null, Dictionary<string,string> baseCordsData = null, 
                                                                    List<string> friendList = null, List<string> friendInvitationsList = null, List<string> friendInvitationsAcceptedList = null,
                                                                    List<string> deletedFriendsList = null){
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
        //si null, false
        baseEstablished_ = baseCordsData != null;
        if(baseCordsData != null){
            baseLatitude_ = Convert.ToDouble(baseCordsData["baseLatitude_"]);
            baseLongitude_ = Convert.ToDouble(baseCordsData["baseLongitude_"]);
        }
        if(firebaseUserData_.IsAnonymous){
            displayName_ = "Anonymous";
        }else{
            displayName_ = firebaseUserData_.DisplayName;
        }

        friendList_ = friendList == null ? new List<string>() : friendList;
        friendDataList_ = new List<FriendData>();

        friendInvitationsList_ = friendInvitationsList == null ? new List<string>() : friendInvitationsList;
        newFriendDataList_ = new List<newFriendData>();

        if(friendInvitationsAcceptedList != null){
            //si se ha aceptado alguna petición de amistad, aniadelo a la lista de amigos
            foreach(string uid in friendInvitationsAcceptedList){
                friendList_.Add(uid);
            }
        }

        if(deletedFriendsList !=null){
            foreach(string uid in deletedFriendsList){
                friendList_.Remove(uid);
            }
        }
        /*
        for(int index = 0; index < 30; index++){
            friendInvitationsList_.Add(index.ToString());
        }
        */
        /*for(int index = 0; index < 30; index++){
            friendList_.Add(index.ToString());
        }*/
        
    }

    public string ToJson(){
        string conversion = "{";
        conversion += $"\"displayName_\" : \"{displayName_}\",";
        if(baseEstablished_){
            conversion += "\"baseCords_\" :{";
            conversion += $"\"baseLatitude_\" : \"{baseLatitude_}\",";
            conversion += $"\"baseLongitude_\" : \"{baseLongitude_}\"";
            conversion += "},";
        }
        if(friendList_.Count != 0){
            conversion += "\"friends_\" :[";
            for(int i = 0; i < friendList_.Count; i++){
                conversion +=  "\"" + friendList_[i] + "\"";
                if(i+1 != friendList_.Count){
                   conversion += ",";
                }
            }
            conversion += "],";
        }
        if(friendInvitationsList_.Count != 0){
            conversion += "\"friendsInvitations_\" :[";
            for(int i = 0; i < friendInvitationsList_.Count; i++){
                conversion += "\"" + friendInvitationsList_[i] + "\"";
                if(i+1 != friendInvitationsList_.Count){
                   conversion += ",";
                }
            }
            conversion += "],";
        }
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
        firebaseHandler firebaseHandlerObject = firebaseHandler.firebaseHandlerInstance_;
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
        firebaseHandler firebaseHandlerObject = firebaseHandler.firebaseHandlerInstance_;
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
        firebaseHandler firebaseHandlerObject = firebaseHandler.firebaseHandlerInstance_;
        foreach(var visitedPlace in visitedPlaces_){
            Dictionary<string, string> placeData = firebaseHandlerObject.getPlaceData(visitedPlace.type_, visitedPlace.id_.ToString());
            //Debe contar todas las visitas de todos los sitios          
            countOfEachZone[placeData["zone_"]]+=visitedPlace.timesVisited_;
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
        firebaseHandler firebaseHandlerObject = firebaseHandler.firebaseHandlerInstance_;
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

    public void setBase(double latitude,double longitude){
        baseLongitude_ = longitude;
        baseLatitude_ = latitude;
        baseEstablished_ = true;
    }

    public double getBaseLongitude(){
        return baseLongitude_;
    }

    public double getBaseLatitude(){
        return baseLatitude_;
    }

    public bool baseEstablished(){
        return baseEstablished_;
    }

    public long getTimestampByName(string name){
        Dictionary<string,string> placeKeys = firebaseHandler.firebaseHandlerInstance_.findPlaceByName(name);
        VisitedPlace place = visitedPlaces_.Find(visitedPlace => visitedPlace.type_ == placeKeys["type"] && visitedPlace.id_.ToString() == placeKeys["id"]);
        if(place == null){
            return (long)0;
        }else{
            return place.lastVisitTimestamp_;
        }
    }

    public int countOfFriendInvitations(){
        return friendInvitationsList_.Count;
    }

    public string getFriendInvitation(int index){
        return friendInvitationsList_[index];
    }

    public void deleteInvitationByName(string name){
        friendInvitationsList_.Remove(name);
    }

    public int countOfFriends(){
        return friendList_.Count;
    }

    public string getFriend(int index){
        return friendList_[index];
    }

    public int countOfFriendData(){
        return friendDataList_.Count;
    }

    public FriendData getFriendData(int index){
        return friendDataList_[index];
    }

    public void addFriendData(FriendData friendData){
        friendDataList_.Add(friendData);
    }

    public bool friendDataIsComplete(){
        return friendDataList_.Count == friendList_.Count;
    }

    public int countOfNewFriends(){
        return friendInvitationsList_.Count;
    }

    public string getNewFriend(int index){
        return friendInvitationsList_[index];
    }

    public int countOfNewFriendData(){
        return newFriendDataList_.Count;
    }

    public newFriendData getNewFriendData(int index){
        return newFriendDataList_[index];
    }

    public void addNewFriendData(newFriendData friendData){
        newFriendDataList_.Add(friendData);
    }

    public bool newFriendDataIsComplete(){
        return friendInvitationsList_.Count == newFriendDataList_.Count;
    }

    public void acceptFriend(string uid){
        friendList_.Add(uid);
        friendInvitationsList_.Remove(uid);
        newFriendDataList_.Remove(newFriendDataList_.Find(newFriendData => newFriendData.getUid() == uid));
    }

    public void deleteFriend(string uid){
        friendList_.Remove(uid);
        friendDataList_.Remove(friendDataList_.Find(friendData => friendData.getUid() == uid));
        Debug.Log("deleteFriend: "+ToJson());
    }
/*
los usuarios tienen permiso para 
    escribir en una propiedad de los demas usuarios, la propiedad es friendsInvitations
    leer los nombres de los usuarios, los displayName
    escribir en una propiedad acceptedFriendInvitations

    entonces tu te escribes a ti mismo en la lista friends invitations de tu amigo, el recibe la peticion
    si acepta se escriben mutuamente en la lista de amigos, si se rechaza se borra de la lista friendsInvitations
    para que A sepa que B acepto su peticion de amistad deberia haber un campo que tambien lo puedan editar todos los usuarios
    que sea acceptedFriendInvitations
*/
}