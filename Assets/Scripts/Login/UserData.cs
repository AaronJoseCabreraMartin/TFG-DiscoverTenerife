using UnityEngine;
using System;
using System.Collections.Generic;

/**
  * @brief Class that stores the current user data.
  */
public class UserData{

    /**
      * @brief Reference to the current user firebase data.
      */
    public Firebase.Auth.FirebaseUser firebaseUserData_;

    /**
      * @brief List of the places that the current user has visited. 
      */
    [SerializeField] public List<VisitedPlace> visitedPlaces_;//type of the place, id, veces visitado
    //WTF porque es serialize??? no deberia serlo.

    /**
      * @brief String that contains the display name of the current user.
      */
    private string displayName_;
    
    /**
      * @brief double that contains the latitude of the base of the current user.
      */
    private double baseLatitude_;

    /**
      * @brief double that contains the longitude of the base of the current user.
      */
    private double baseLongitude_;
    
    /**
      * @brief bool true if the user has already stablished his base, false in other case.
      */
    private bool baseEstablished_;
    //si el usario se registra sin tener la ubicacion activada puede que la base no sea establecida

    /**
      * @brief list of strings that contains all the user ids of the friends of the current user.
      */
    private List<string> friendList_;
    //almacena el UID de los usuarios amigos

    /**
      * @brief list that contains all the data of the friends of the current user.
      */
    private List<FriendData> friendDataList_;

    /**
      * @brief list that contains strings with the user ids of all the user that has sended a friendship
      * invitation to the current user and the current user hasnt accepted or denied.
      */
    private List<string> friendInvitationsList_;
    //almacena el UID de los usuarios que te han enviado una peticion de amistad

    /**
      * @brief list that stores all the necessary information of the users those that the current 
      * user has a new friendship invitation.
      */
    private List<newFriendData> newFriendDataList_;

    private List<challengeData> challenges_;

    /**
      * @param Firebase.Auth.FirebaseUser firebase user data
      * @param List<Dictionary<string,string>> (Optional) Information of the sites that the user has already visited
      * @param Dictionary<string,string> (Optional) Information of the geographical coords of the user's base.
      * @param List<string> (Optional) List of user ids of the current user's friends.
      * @param List<string> (Optional) List of user ids of the current user's new friendship invitations.
      * @param List<string> (Optional) List of user ids of the users that has accepted current user's friendships invitations.
      * @param List<string> (Optional) List of user ids of the users that has deleted current user's friendship.
      * @brief Constructor. It initialices all the properties taking aware if they have been given with a real 
      * value or they are null. 
      * - If the baseCordsData parameter isnt null, it sets baseEstablished_ attribute to true, it
      * sets it as false in other case.
      * - If the friendInvitationsAcceptedList parameter isnt null, it adds each element of the list to the
      * friendList_ attribute.
      * - If the deletedFriendsList parameter isnt null, it deletes each element of that list from the friendList_
      * attribute.
      */
    public UserData(Firebase.Auth.FirebaseUser newFireBaseUserData, List<Dictionary<string,string>> oldVisitedPlaces = null, 
                    Dictionary<string,string> baseCordsData = null, List<string> friendList = null, 
                    List<string> friendInvitationsList = null, List<string> friendInvitationsAcceptedList = null,
                    List<string> deletedFriendsList = null, List<Dictionary<string,string>> challengesData = null){
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

        foreach(Dictionary<string,string> challenge in challengesData){
          if(challenge["startTimestamp_"] != null && !gameRules.challengeHasExpired(Int64.Parse(challenge["startTimestamp_"]))){
            //si el reto no se ha caducado lo guardo
            challenges_.Add(new challengeData(challenge));
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

    /**
      * @return string that contains the JSON formated conversion of the current object.
      * @brief This method converts all the properties of the current object in a string that 
      * follows the JSON format.
      */
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
        
        if(challenges_.Count != 0){
          conversion += "\"challenges_\" : [";
          for(int i = 0; i < challenges_.Count; i++){
              conversion += challenges_[i].ToJson();
              if(i+1 < challenges_.Count){
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

    /**
      * @return bool true if the current user has visited the place that has the given type and id. 
      * @param string type of the searched place.
      * @param int id of the searched place.
      * @brief This method return true if the current user has visited the place that has the given type
      * and the given id. It returns false in any other case.
      */
    public bool hasVisitPlace(string type, int id){
        return visitedPlaces_.Exists(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id);
    }

    /**
      * @return int Quantity of places that the current user has visited.
      */
    public int countOfVisitedPlaces(){
        return visitedPlaces_.Count;
    }

    /**
      * @return int Quantity of places of the given type that the user has visited.
      * @brief returns the quantity of places of the given type that the user has visited.
      * Zones of the map are defined on mapRulesHandler class. 
      */
    public int countOfVisitedPlacesOfZone(string zone){
    //devuelve la cantidad de sitios de una zona dada que ha visitado el usuario
        firebaseHandler firebaseHandlerObject = firebaseHandler.firebaseHandlerInstance_;
        int count = 0;
        foreach(var visitedPlace in visitedPlaces_){
            if(firebaseHandlerObject.getPlaceData(visitedPlace.type_, visitedPlace.id_.ToString())["zone_"] == zone){
                count++;
            }
        }
        return count;
    }

    /**
      * @return int Accumulated sum of number of visit of all the places that the current user has visited.
      * @brief returns the accumulated sum of number of visit of all the places that the current user has visited.
      */
    public int countOfAccumulatedVisits(){
        int count = 0;
        foreach(var visitedPlace in visitedPlaces_){
            count += visitedPlace.timesVisited_;
        }
        return count;
    }

    /**
      * @return string that contains the name of the place that the current user has visited more times.
      * @brief it returns a string that contains the name of the place that the current user has 
      * visited more times.
      */
    public string mostVisitedPlace(){
        //WTF mala practica porque no estoy revisando que no tengan nombres repetidos... 
        // deberia retornar un dictionario con el id y el tipo que son las claves...
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

    /**
      * @return string that contains the most visited zone of the current user.
      * @brief it returns a string that contains the most visited zone of the current user.
      * Zones of the map are defined on mapRulesHandler class. 
      */
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

    /**
      * @return string that contains the most visited zone of the current user.
      * @brief it returns a string that contains the most visited zone of the current user.
      * Type of the sites are defined on mapRulesHandler class. 
      */
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

    /**
      * @param string Zone that you want to know how many visit the current user has done.
      * @return int Accumulated sum of all visit to places that are on the given zone.
      * @brief it returns the sum of all visits to places that are on the given zone.
      * Zones of the map are defined on mapRulesHandler class. 
      */
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

    /**
      * @param string type of the site that the current user has visit.
      * @param int id of the site that the current user has visit.
      * @param long timestamp of the visit.
      * @brief This method register a visit of the current user to the place that has 
      * the given type and id. It registers the visit at the given timestamp. If the user
      * havent visit a place with the given type and id this method print an message
      * on the console.
      */
    public void newVisitAt(string type, int id, long visitTime){
        // WTF en firebaseHandler en el metodo de userVisitedPlace NO deberia comprobar
        // si es una nueva visita o no, deberia hacerlo todo aqui, es decir, siempre en firebase
        // llamar a este metodo y aquí en vez del debug hacer el visitedPlaces_.add(new VisitedPlace())
        VisitedPlace place = visitedPlaces_.Find(visitedPlace => visitedPlace.type_ == type && visitedPlace.id_ == id);
        if(place == null){
            Debug.Log($"No se ha encontrado el sitio con tipo {type} y con id {id} en newVisitAt");
        }else{
            place.newVisitAt(visitTime);
        }
    }

    /**
      * @param double latitude of the base
      * @param double longitude of the base
      * @brief this method should be called once per user. It stablished the user base
      * on the given coords and set the baseEstablished_ attribute to true.
      */
    public void setBase(double latitude,double longitude){
        baseLongitude_ = longitude;
        baseLatitude_ = latitude;
        baseEstablished_ = true;
    }

    /**
      * @return double the baseLongitude_ attribute.
      * @brief Getter of the baseLongitude_ attribute.
      */
    public double getBaseLongitude(){
        return baseLongitude_;
    }

    /**
      * @return double the baseLatitude_ attribute.
      * @brief Getter of the baseLatitude_ attribute.
      */
    public double getBaseLatitude(){
        return baseLatitude_;
    }

    /**
      * @return bool the baseEstablished_ attribute.
      * @brief Getter of the baseEstablished_ attribute, true if the base of the current
      * user has been already established, false in other case.
      */
    public bool baseEstablished(){
        return baseEstablished_;
    }

    /**
      * @param string name of the visited place.
      * @return long the timestamp of the last visit to the place that has the given name.
      * @brief It returns the timestamp of the last visit to the place that has the given name,
      * if the user hadn't visited a place with that name it returns 0.
      */
    public long getTimestampByName(string name){
        Dictionary<string,string> placeKeys = firebaseHandler.firebaseHandlerInstance_.findPlaceByName(name);
        VisitedPlace place = visitedPlaces_.Find(visitedPlace => visitedPlace.type_ == placeKeys["type"] && visitedPlace.id_.ToString() == placeKeys["id"]);
        if(place == null){
            return (long)0;
        }else{
            return place.lastVisitTimestamp_;
        }
    }

    /**
      * @return int how many new friendship invitations the current user has.
      * @brief It returns an int with how many new friendship invitations the current user has.
      */
    public int countOfFriendInvitations(){
        return friendInvitationsList_.Count;
    }

    /**
      * @param int the index for choose the friendship invitation. 
      * @return the user id of the index-th friendship invitation.
      * @brief It returns the user id of the friendship invitation that is on the given position
      * on the list of all friendship invitations. If the index is bigger than the list or the index
      * is negative it will raise an exception.
      */
    public string getFriendInvitation(int index){
        return friendInvitationsList_[index];
    }

    /**
      * @string user id to delete the friendship invitation.
      * @brief this method removes the frienship invitation of the user
      * that has the given user id.
      */
    public void deleteInvitationByName(string name){
        //WTF no deberia llamarse deleteInvitationByUID ???
        friendInvitationsList_.Remove(name);
    }

    /**
      * @return int how many friends has the current user.
      */
    public int countOfFriends(){
        return friendList_.Count;
    }

    /**
      * @param int position of the user id that you want to get.
      * @return string that contains the user id that is on the index-th position of the
      * current user friend list.
      * @brief Returns the user id of the friend index-th of the current user's friend list.
      * If the given index is bigger than the size of the current user's friend list or its 
      * negative it will raise an exception. 
      */
    public string getFriend(int index){
        return friendList_[index];
    }

    /**
      * @return int with the friendDataList_ size.
      */
    public int countOfFriendData(){
        return friendDataList_.Count;
    }

    /**
      * @param int position of the friendData that you want to get.
      * @return FriendData of the friend that is on the index-th position of
      * the current user friendDataList_.
      * @brief Returns the friendData of the friend that is on the index-th position.
      * If the given index is bigger than the size of the friendDataList_ list or
      * its negative it will raise an exception.
      */
    public FriendData getFriendData(int index){
        return friendDataList_[index];
    }

    /**
      * @param FriendData to add to the friendDataList_ attribute.
      */
    public void addFriendData(FriendData friendData){
        friendDataList_.Add(friendData);
    }

    /**
      * @return bool true if all the friendData was downloaded, false in other case.
      */
    public bool friendDataIsComplete(){
        return friendDataList_.Count == friendList_.Count;
    }

    /**
      * @return Count of the size of the friendInvitationsList_ attribute.
      */
    public int countOfNewFriends(){
        return friendInvitationsList_.Count;
    }

    /**
      * @param int the position of the invitation that you want to get.
      * @return string the user id of the invitation that is on the index-th position
      * of the friendInvitationsList_ attribute.
      * @brief It returns the user id of the index-th invitation of the friendInvitationsList_ 
      * attribute. If the index is bigger than the list's size or its negative it will raise an exception.
      */
    public string getNewFriend(int index){
        return friendInvitationsList_[index];
    }

    /**
      * @return int of the newFriendDataList_ attribute size.
      */
    public int countOfNewFriendData(){
        return newFriendDataList_.Count;
    }

    /**
      * @param int the position of the friendship invitation data that you want to get.
      * @return string the user id of the friendship invitation data that is on 
      * the index-th position of the friendDataList_ attribute.
      * @brief It returns the user id of the index-th friendship invitation data of the friendDataList_ 
      * attribute. If the index is bigger than the list's size or its negative it will raise an exception.
      */
    public newFriendData getNewFriendData(int index){
        return newFriendDataList_[index];
    }

    /**
      * @param newFriendData the data that you want to add to the newFriendDataList_ attribute.
      */
    public void addNewFriendData(newFriendData friendData){
        newFriendDataList_.Add(friendData);
    }

    /**
      * @return bool True if all frienship invitation data is downloaded, false in other case. 
      */
    public bool newFriendDataIsComplete(){
        return friendInvitationsList_.Count == newFriendDataList_.Count;
    }

    /**
      * @param string that contains the user id of the user that the current user
      * wants to add as a friend.
      * @brief This method add the given user id to the friend list and removes
      * the friendship invitation and the friendship invitation data of the attributes
      * of the current user.
      */
    public void acceptFriend(string uid){
        friendList_.Add(uid);
        friendInvitationsList_.Remove(uid);
        newFriendDataList_.Remove(newFriendDataList_.Find(newFriendData => newFriendData.getUid() == uid));
    }

    /**
      * @param string that contains the user id of the user that the current user
      * wants to delete friendship.
      * @brief This method remove either the given user id of the friend list and the 
      * correspondent data from the properties of the current user.
      */
    public void deleteFriend(string uid){
        friendList_.Remove(uid);
        friendDataList_.Remove(friendDataList_.Find(friendData => friendData.getUid() == uid));
        //Debug.Log("deleteFriend: "+ToJson());
    }

    /**
      * @param int the position of the visited place that you want to get.
      * @return VisitedPlace that is on the index-th position of the visitedPlaces_ list.
      * @brief This method returns the VisitedPlace that is stored on the given position.
      * If the given position is bigger than the visitedPlaces_ size or the given index is 
      * negative it will raise an exception.
      */
    public VisitedPlace getStoryPlaceData(int index){
        return visitedPlaces_[index];
    }

    /**
      * @return string with the user id of the current user.
      * @brief getter of the user id.
      */
    public string getUid(){
      return firebaseUserData_.UserId;
    }

    /**
      * @param string that contains the user's display name.
      * @return true if the current user has a friend with the given display name.
      * @brief This method checks if on the current user's friend list exists any user
      * with the given display name.
      */
    public bool isAFriendByDisplayName(string displayName){
      return friendDataList_.Exists(friendData => friendData.getDisplayName() == displayName);
    }
}