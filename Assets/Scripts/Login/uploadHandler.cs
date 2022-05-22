using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

/**
  * @brief the part of the class firebaseHandler that controls the uploading operations
  */
public partial class firebaseHandler{

    //only for debugging purpose
    public void writeNewPlaceOnDataBase(Place place, string type, int placeID){
        database.Child($"places/{type}/{placeID.ToString()}").SetRawJsonValueAsync(place.ToJson());
    }

    /**
      * @param type string that contains the type of the place that we want to upload.
      * @param id string that contains the id of the place that we want to upload.
      * @brief This method uploads the data from the place that matches the given type and id. If the
      * upload fails it adds the information of the place to placesToUploadQueue_
      */
    public void writePlaceData(string type, string id){
        Place place = requestHandler_.getPlaceByTypeAndId(type,id);
        FirebaseDatabase.DefaultInstance.GetReference($"places/{type}/{id}/timesItHasBeenVisited_").GetValueAsync().ContinueWith(visitsTask => {
             if (visitsTask.IsCompleted) {
                DataSnapshot visitsSnapshot = visitsTask.Result;
                string cloudVersion = visitsSnapshot.GetRawJsonValue() == null ?
                                                "0" : JsonConvert.DeserializeObject<string>(visitsSnapshot.GetRawJsonValue());
                //tengo que subir, el numero de visitas que hay en la nube + el numero de visitas que haya hecho en local y que no haya subido
                int total = Int32.Parse(cloudVersion) + place.getNewVisits();
                database.Child($"places/{type}/{id}/timesItHasBeenVisited_").SetRawJsonValueAsync(total.ToString()).ContinueWith(uploadPlaceDataTask => {
                    if(uploadPlaceDataTask.IsCompleted){
                        //si las visitas locales se suben, debo empezar a contar de 0 las visitas que tengo que subir
                        place.resetNewVisits();
                    }else{
                        Debug.LogError("writePlaceData fallo!");
                        if(placesToUpdateQueue_ == null){
                            placesToUpdateQueue_ = new List<Dictionary<string, string>>();
                        }
                        placesToUpdateQueue_.Add(new Dictionary<string,string>{{type,id}});
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
             }else{
                Debug.LogError("writePlaceData fallo!");
                if(placesToUpdateQueue_ == null){
                    placesToUpdateQueue_ = new List<Dictionary<string, string>>();
                }
                placesToUpdateQueue_.Add(new Dictionary<string,string>{{type,id}});
             }

        },TaskScheduler.FromCurrentSynchronizationContext());
    }


  /**
    * @brief this method tries to upload all the information that is on the placesToUpdateQueue_
    * property. If it success, it erase the place from the list, but if its fails the writePlaceData
    * will add again the place to the queue.
    */
  void uploadPlacesQueue(){
      List<Dictionary<string,string>> toRemove = new List<Dictionary<string,string>>();
      foreach(Dictionary<string,string> placeToUpdate in placesToUpdateQueue_){
          foreach(string key in placeToUpdate.Keys){
              toRemove.Add(placeToUpdate);
              writePlaceData(key, placeToUpdate[key]);
          }
      }
      foreach(Dictionary<string,string> placeToRemove in toRemove){
              placesToUpdateQueue_.Remove(placeToRemove);
      }
  }

    /**
      * @param string that contains the user that will has erased the friendship with the current user.
      * @param string that contains the deleted friends list of the user that has the given id in
      * the JSON format.
      * @brief This method must be called when there is internet connection, otherwise it wont 
      * to nothing. This method upload the deletedFriends property of the user that has the given id.
      */
    public void updateUserDeleteAFriend(string noFriendUid,string deletedFriendsListInJSON){
        //uso los metodos de friend data y luego llamo aqui solo con la info que hay para subir
        FirebaseDatabase.DefaultInstance.GetReference($"users/{noFriendUid}/deletedFriends_").GetValueAsync().ContinueWith(propertyTask => {
            if(propertyTask.IsCompleted){
                DataSnapshot propertySnapshot = propertyTask.Result;
                List<string> cloudVersion = propertySnapshot.GetRawJsonValue() == null ?
                                                new List<string>() : JsonConvert.DeserializeObject<List<string>>(propertySnapshot.GetRawJsonValue());
                
                //simplemente añado el uid del current user si no lo tiene en la lista, si lo tiene 
                if(cloudVersion.FindIndex(uid => uid == currentUser_.getUid()) == -1){
                    cloudVersion.Add(currentUser_.getUid());
                    //TIENE que haber internet, sino no dejo hacer nada en friends
                    database.Child($"users/{noFriendUid}/deletedFriends_").SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion));
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }


    /**
      * @param FriendData object that contains all the information releated
      * to the user that own the challenges we want to upload.
      * @brief this method upload the challengue list of the given user. 
      */
    public void uploadFriendChallengesOf(FriendData friendDataToUpload){
        FirebaseDatabase.DefaultInstance.GetReference($"users/{friendDataToUpload.getUid()}/deletedFriends_").GetValueAsync().ContinueWith(propertyTask => {
            if(propertyTask.IsCompleted){
                DataSnapshot propertySnapshot = propertyTask.Result;
                List<Dictionary<string,string>> cloudVersion = propertySnapshot.GetRawJsonValue() == null ?
                                                new List<Dictionary<string,string>>() : 
                                                JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(propertySnapshot.GetRawJsonValue());

                //si no tenia un challenge de este usuario, le añado el nuevo challenge y lo subo
                if(cloudVersion.FindIndex(challenge => challenge["challengerId_"] == currentUser_.getUid()) == -1){
                    cloudVersion.Add(friendDataToUpload.getChallengeOfUser(currentUser_.getUid()).toDictionaryVersion());
                    database.Child($"users/{friendDataToUpload.getUid()}/challenges_")
                            .SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion));
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @brief this method should be called either when there is changes on the current user's social preferences or
      * the last changes couldnt been uploaded on the past. It checks if there is any change on the three permision
      * and upload to the firebase database the changes on the preferences. It checks if the three lists are uploaded,
      * if one or more of them fails on the uploading, it puts the hasToUploadSocialPreferences_ property to true, so
      * this method will be called again when the internet connection is ready.
      */
    public void uploadSocialPreferences(){
        bool changesToUpload = false;
        int countOfDone = 0;
        string currentUserId = currentUser_.firebaseUserData_.UserId.ToString();
        if(optionsController.optionsControllerInstance_.socialOptions("addMe") && !FriendData.usersThatAllowFriendshipInvitations_.Exists(uid => uid == currentUserId)){
            //el usuario permite recibir peticiones de amistad y no está en la lista
            FriendData.usersThatAllowFriendshipInvitations_.Add(currentUserId);
            changesToUpload = true;
        }else if(!optionsController.optionsControllerInstance_.socialOptions("addMe") && FriendData.usersThatAllowFriendshipInvitations_.Exists(uid => uid == currentUserId)){
            //el usuario NO permite recibir peticiones de amistad y está en la lista
            FriendData.usersThatAllowFriendshipInvitations_.Remove(currentUserId);
            changesToUpload = true;
        }

        if(changesToUpload){
            string stringConversion = JsonConvert.SerializeObject(FriendData.usersThatAllowFriendshipInvitations_);
            database.Child("users/usersThatAllowFriendshipInvitations").SetRawJsonValueAsync(stringConversion).ContinueWith(uploadListTask => {
                if(uploadListTask.IsCompleted){
                    countOfDone++;
                    if(countOfDone == 3){
                        hasToUploadSocialPreferences_ = false;
                    }
                }else{
                    hasToUploadSocialPreferences_ = true;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfDone++;
            if(countOfDone == 3){
                hasToUploadSocialPreferences_ = false;
            }
        }

        changesToUpload = false;
        if(optionsController.optionsControllerInstance_.socialOptions("challengeMe") && !FriendData.usersThatAllowBeChallenged_.Exists(uid => uid == currentUserId)){
            //el usuario permite recibir retos y no está en la lista
            FriendData.usersThatAllowBeChallenged_.Add(currentUserId);
            changesToUpload = true;
        }else if(!optionsController.optionsControllerInstance_.socialOptions("challengeMe") && FriendData.usersThatAllowBeChallenged_.Exists(uid => uid == currentUserId)){
            //el usuario NO permite recibir retos y está en la lista
            FriendData.usersThatAllowBeChallenged_.Remove(currentUserId);
            changesToUpload = true;
        }

        if(changesToUpload){
            string stringConversion = JsonConvert.SerializeObject(FriendData.usersThatAllowBeChallenged_);
            database.Child("users/usersThatAllowBeChallenged").SetRawJsonValueAsync(stringConversion).ContinueWith(uploadListTask => {
                if(uploadListTask.IsCompleted){
                    countOfDone++;
                    if(countOfDone == 3){
                        hasToUploadSocialPreferences_ = false;
                    }
                }else{
                    hasToUploadSocialPreferences_ = true;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfDone++;
            if(countOfDone == 3){
                hasToUploadSocialPreferences_ = false;
            }
        }

        changesToUpload = false;
        if(optionsController.optionsControllerInstance_.socialOptions("ranking") && !FriendData.usersThatAllowAppearedOnRanking_.Exists(uid => uid == currentUserId)){
            //el usuario permite aparecer en el ranking y no está en la lista
            FriendData.usersThatAllowAppearedOnRanking_.Add(currentUserId);
            changesToUpload = true;
        }else if(!optionsController.optionsControllerInstance_.socialOptions("ranking") && FriendData.usersThatAllowAppearedOnRanking_.Exists(uid => uid == currentUserId)){
            //el usuario NO permite aparecer en el ranking y está en la lista
            FriendData.usersThatAllowAppearedOnRanking_.Remove(currentUserId);
            changesToUpload = true;
        }

        if(changesToUpload){
            string stringConversion = JsonConvert.SerializeObject(FriendData.usersThatAllowAppearedOnRanking_);
            database.Child("users/usersThatAllowAppearedOnRanking").SetRawJsonValueAsync(stringConversion).ContinueWith(uploadListTask => {
                if(uploadListTask.IsCompleted){
                    countOfDone++;
                    if(countOfDone == 3){
                        hasToUploadSocialPreferences_ = false;
                    }
                }else{
                    hasToUploadSocialPreferences_ = true;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfDone++;
            if(countOfDone == 3){
                hasToUploadSocialPreferences_ = false;
            }
        }
    }

    /**
      * @brief This method tries to upload the information of the first element of
      * otherUserScoresToUpload_ list, it removes the first element just before
      * of trying to upload it. If it fails on the uploading, it adds again the 
      * dictionary with the information to the otherUserScoresToUpload_ list.
      * The information it uploads is the the earnedScore_ to the user id.
      */
    void uploadOtherUserScores(){
        string uid = otherUserScoresToUpload_[0]["uid_"];
        string score = "\"" + otherUserScoresToUpload_[0]["score_"] + "\"";
        otherUserScoresToUpload_.RemoveAt(0);
        
        FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/earnedScore_").GetValueAsync().ContinueWith(earnedScoreTask => {
             if (earnedScoreTask.IsFaulted) {
                // Handle the error...
                Debug.LogError("Error: "+earnedScoreTask.Exception);
                Dictionary<string,string> toUpload = new Dictionary<string,string>();
                toUpload["uid_"] = uid;
                toUpload["score_"] = score;
                otherUserScoresToUpload_.Add(toUpload);
            } else if (earnedScoreTask.IsCompleted) {
                DataSnapshot earnedScoreSnapshot = earnedScoreTask.Result;
                if(earnedScoreSnapshot.GetRawJsonValue() != null){
                    score = (Int32.Parse(score) + Int32.Parse(earnedScoreSnapshot.GetRawJsonValue())).ToString();
                }
                database.Child($"users/{uid}/earnedScore_").SetRawJsonValueAsync(score).ContinueWith(uploadScoreTask => {
                    if(!uploadScoreTask.IsCompleted){
                        Dictionary<string,string> toUpload = new Dictionary<string,string>();
                        toUpload["uid_"] = uid;
                        toUpload["score_"] = score;
                        otherUserScoresToUpload_.Add(toUpload);
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @param string user id of the friend that has been acepted
      * @param string with the conversion of the accepted friendships invitations in JSON 
      * format.
      * @brief This method tries to upload the accepted friendships invitations list of the 
      * given user. If it complete it succesfully, it removes the friendUid of the list of the 
      * user that need to be notified with the hasBeenNotified method. It also sets the
      * uploadingNotifications_ property as true but when the try of updating finish, in
      * any way, it sets the uploadingNotifications_ property as false.
      */
    public void updateUserAddedAFriend(string friendUid, string acceptedFriendsInvitationsListInJSON){
        uploadingNotifications_ = true;
        //uso los metodos de friend data y luego llamo aqui solo con la info que hay para subir
        //TIENE que haber internet, sino no dejo hacer nada en friends
        database.Child($"users/{friendUid}/acceptedFriendsInvitations_").SetRawJsonValueAsync(acceptedFriendsInvitationsListInJSON).ContinueWith(uploadScoreTask => {
            if(uploadScoreTask.IsCompleted){
                currentUser_.hasBeenNotified(friendUid);
            }
            uploadingNotifications_ = false;
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @brief This method calls the writeFriendProperty with each element of the
      * otherUsersPropertiesToUpload_ list and it removes the each of those elements
      * taking care of that each of those elements could enter again on the list. That 
      * could happen if the upload goes wrong.
      */
    public void writeAllFriendProperties(){
        int initialCount = otherUsersPropertiesToUpload_.Count;
        for(int index = 0; index < otherUsersPropertiesToUpload_.Count; index++){
            Dictionary<string,string> toUpload = otherUsersPropertiesToUpload_[index];
            writeFriendProperty(toUpload["uid_"], 
                                toUpload["property_"], 
                                currentUser_.getFriendDataByUID(toUpload["uid_"]).getJSONof(toUpload["property_"]) 
                                );
            
        }
        for(int index = 0; index < initialCount; index++){
            otherUserScoresToUpload_.RemoveAt(0);
        }
    }

    /**
      * @param string with the user id of the friend that will be uploaded
      * @param string with the name of the property that needs to be uploaded
      * @param string with the json conversion of the property that needs to be uploaded
      * @brief This method downloads the current information of the given property of
      * the given friend and then calls the correct method for taking care of 
      * differences between the cloud information and the local information
      */
    public void writeFriendProperty(string userId, string property, string json){
        /*

        Propiedades que podrían dar problemas:
            acceptedFriendsInvitations_ -> si dos usuarios aceptan la invitacion de ese usuario a la vez
            deletedFriends -> si dos usuarios eliminan a ese usuario a la vez
            earnedScore -> si dos usuarios cumplen retos de esa persona a la vez
            challenges -> si dos usuarios retan a ese usuario a la vez
        
        Si termino esto probablemente otherUserScoresToUpload_ no me haga falta
        */
        FirebaseDatabase.DefaultInstance.GetReference($"users/{userId}/{property}").GetValueAsync().ContinueWith(propertyTask => {    
            if (propertyTask.IsFaulted || propertyTask.IsCanceled) {
                Debug.LogError($"Fallo al descargar la informacion del atributo {property} del usuario {userId} : "+propertyTask.Exception);
            }else if(propertyTask.IsCompleted){
                DataSnapshot snapshotProperty = propertyTask.Result;
                if(property == "challenges_"){
                    writeFriendChallenges(userId, json, snapshotProperty.GetRawJsonValue());
                }else if(property == "deletedFriends_"){
                    writeFriendDeletedFriends(userId, json, snapshotProperty.GetRawJsonValue());
                }else if(property == "acceptedFriendsInvitations_"){
                    writeFriendAcceptedFriendsInvitations(userId, json, snapshotProperty.GetRawJsonValue());
                //}else if(property == "earnedScore_"){
                //    Debug.LogError("Para earnedScore_ tenemos el metodo uploadOtherUserScores que tiene en cuenta todo!");
                //}else if(property == "friendsInvitations_"){
                //    Debug.LogError("Para friendsInvitations tenemos el metodo sendFriendshipInvitation que tiene en cuenta todo!");
                }else{
                    Debug.LogError("Property desconocida en writeFriendProperty de uploadHandler: " + property);
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @param string with the user id of the friend that will be uploaded
      * @param string with the json conversion of the challenges of that friend with the local information
      * @param string with the json conversion of the challenges of that friend with the cloud information
      * @brief This method compares the challenge list of the given user to upload correctly the information
      * taking care of the differences between the local version and the cloud version. If something goes
      * wrong it adds again the challenge property to the otherUsersPropertiesToUpload_ list.
      */
    private void writeFriendChallenges(string userId, string jsonLocal, string jsonCloud){
        //este usuario no tiene permiso para quitarle retos a su amigo pero si para añadirselos (solo los de él)
        List<Dictionary<string,string>> localVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(jsonLocal);
        List<Dictionary<string,string>> cloudVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(jsonCloud);
        //lo que hago es que simplemente si este usuario ha añadido un challenge al otro usuario, añado la entrada del nuevo
        //challenge a lo que ya esta en la nube. No añado a lo que ya tenia descargado y luego lo subo, asi evito el problema.
        int index = cloudVersion.FindIndex(localChallenge => localChallenge["challengerId_"] == currentUser_.getUid());
        if(index != -1){
            cloudVersion.Add(localVersion[index]);
            database.Child($"users/{userId}/challenges_").SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion)).ContinueWith(taskUploadUserData =>{
                //control if the information have been uploaded or not
                if(!taskUploadUserData.IsCompleted){
                    Dictionary<string,string> uidAndProperty = new Dictionary<string,string>();
                    uidAndProperty["uid_"] = userId;
                    uidAndProperty["property_"] = "challenges_";
                    otherUsersPropertiesToUpload_.Add(uidAndProperty);
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    /**
      * @param string with the user id of the friend that will be uploaded
      * @param string with the json conversion of the deletedFriend list of that friend with the local information
      * @param string with the json conversion of the deletedFriend list of that friend with the cloud information
      * @brief This method compares the deletedFriend list of the given user to upload correctly the information
      * taking care of the differences between the local version and the cloud version. If something goes
      * wrong it adds again the deletedFriend property to the otherUsersPropertiesToUpload_ list.
      */
    private void writeFriendDeletedFriends(string userId, string jsonLocal, string jsonCloud){
        //este es especial porque el unico uid que este usuario puede escribir en esa propiedad es el suyo propio
        //por lo tanto no necesita el jsonLocal

        List<string> cloudVersion = JsonConvert.DeserializeObject<List<string>>(jsonCloud);
        int index = cloudVersion.FindIndex(uid => uid == currentUser_.getUid());
        if(index != -1){
            cloudVersion.Add(currentUser_.getUid());
            database.Child($"users/{userId}/deletedFriends_").SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion)).ContinueWith(taskUploadUserData =>{
                //control if the information have been uploaded or not
                if(!taskUploadUserData.IsCompleted){
                    Dictionary<string,string> uidAndProperty = new Dictionary<string,string>();
                    uidAndProperty["uid_"] = userId;
                    uidAndProperty["property_"] = "deletedFriends_";
                    otherUsersPropertiesToUpload_.Add(uidAndProperty);
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    /**
      * @param string with the user id of the friend that will be uploaded
      * @param string with the json conversion of the friendInvitations list of that friend with the local information
      * @param string with the json conversion of the friendInvitations list of that friend with the cloud information
      * @brief This method compares the friendInvitations list of the given user to upload correctly the information
      * taking care of the differences between the local version and the cloud version. If something goes
      * wrong it adds again the friendInvitations property to the otherUsersPropertiesToUpload_ list.
      */
    private void writeFriendAcceptedFriendsInvitations(string userId, string jsonLocal, string jsonCloud){
        //este es especial porque el unico uid que este usuario puede escribir en esa propiedad es el suyo propio
        //por lo tanto no necesita el jsonLocal

        List<string> cloudVersion = JsonConvert.DeserializeObject<List<string>>(jsonCloud);
        //lo que hago es que simplemente si este usuario ha añadido un challenge al otro usuario, añado la entrada del nuevo
        //challenge a lo que ya esta en la nube. No añado a lo que ya tenia descargado y luego lo subo, asi evito el problema.
        int index = cloudVersion.FindIndex(uid => uid == currentUser_.getUid());
        if(index != -1){
            cloudVersion.Add(currentUser_.getUid());
            database.Child($"users/{userId}/acceptedFriendsInvitations_").SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion)).ContinueWith(taskUploadUserData =>{
                //control if the information have been uploaded or not
                if(!taskUploadUserData.IsCompleted){
                    Dictionary<string,string> uidAndProperty = new Dictionary<string,string>();
                    uidAndProperty["uid_"] = userId;
                    uidAndProperty["property_"] = "acceptedFriendsInvitations_";
                    otherUsersPropertiesToUpload_.Add(uidAndProperty);
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    /**
      * @brief This method calls writeSafeProperties with each safe property and writeUnsafeProperty with 
      * each unsafe property of the current user. It uses the getJSONof method of UserData class.
      */
    public void writeAllUserProperties(){

        foreach(string safeProperty in UserData.safeProperties_){
            writeSafeProperties(safeProperty, currentUser_.getJSONof(safeProperty));
        }
        //earnedScore es peligrosa porque tienes que tener en cuenta si el usuario ya se sumo o no la earnedScore porque
        //el usuario nada mas leerlo lo pone a 0 (quizas por eso es safe porque segun lo lee lo pone a 0)
        
        foreach(string unsafeProperty in UserData.unsafeProperties_){
            writeUnsafeProperty(unsafeProperty, currentUser_.getJSONof(unsafeProperty));
        }
    }
    
    /**
      * @brief this method calls the writeSafeProperties or writeUnsafeProperty depending of the type
      * of each element that is stored in currentUserPropertiesToUpload_ list. It removes the elements
      * wich it calls those methods taking care of the fact that those properties could enter again
      * on the currentUserPropertiesToUpload_ list if they fail on the upload.
      */
    public void writeQueuedUserProperties(){
        int initialCount = currentUserPropertiesToUpload_.Count;
        for(int index = 0; index < currentUserPropertiesToUpload_.Count; index++){
            string propertyToUpload = currentUserPropertiesToUpload_[index];

            if(UserData.safeProperties_.FindIndex(property => property == propertyToUpload) != -1){
                writeSafeProperties(propertyToUpload, 
                                    currentUser_.getJSONof(propertyToUpload));
            }else{
                writeUnsafeProperty(propertyToUpload, 
                                    currentUser_.getJSONof(propertyToUpload));
            }
        }
        for(int index = 0; index < initialCount; index++){
            otherUserScoresToUpload_.RemoveAt(0);
        }
    }

    /**
      * @param string that contains the name of the property
      * @param string that contains the json conversion of that property
      * @brief this method writes on the given property on the database the given
      * json value. It doesnt take care of the version that is on the database. It simply
      * overwrite it. If something goes wrong during the upload, it adds the property to
      * currentUserPropertiesToUpload_ again.
      */
    public void writeSafeProperties(string property, string json){
        database.Child($"users/{currentUser_.getUid()}/{property}").SetRawJsonValueAsync(json).ContinueWith(taskUploadUserData =>{
            //si la subida falla y la propiedad NO esta ya en la cola, ponla de nuevo en la cola
            if(!taskUploadUserData.IsCompleted && currentUserPropertiesToUpload_.FindIndex(prop => prop == property) == -1){
                currentUserPropertiesToUpload_.Add(property);
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }
    /**
      * @param string that contains the name of the property
      * @param string that contains the json conversion of that property
      * @brief this method writes on the given property on the database the given
      * json value. It takes care of the information that is currently on the database and
      * tries to avoid incongruencies of the database. If something goes wrong during the
      * uploading it adds again the property to the currentUserPropertiesToUpload_ list.
      */
    public void writeUnsafeProperty(string property, string json){
        /*
        Propiedades que podrían dar problemas:
            "friendsInvitations_", 
            "deletedFriends_", 
            "challenges_", 
            "earnedScore_", //Nada mas sumarlo lo ponemos a 0 puede que esto no de problemas
            "acceptedFriendsInvitations_"

        currentUser_.getJSONof(toUpload["property_"])
        */
        //el usuario deberia tener almacenado que tenia cuando descargo la info para comparar

        FirebaseDatabase.DefaultInstance.GetReference($"users/{currentUser_.getUid()}/{property}").GetValueAsync().ContinueWith(propertyTask => {
            if(!propertyTask.IsCompleted && currentUserPropertiesToUpload_.FindIndex(prop => prop == property) == -1){
                currentUserPropertiesToUpload_.Add(property);
            }else if(propertyTask.IsCompleted){
                DataSnapshot propertySnapshot = propertyTask.Result;
                if(property == "friendsInvitations_"){
                    List<string> cloudVersion = JsonConvert.DeserializeObject<List<string>>(propertySnapshot.GetRawJsonValue());
                    if(cloudVersion == null){
                        cloudVersion = new List<string>();
                    }
                    //tengo que eliminar de la version online todas las invitaciones que haya aceptado o rechazado
                    foreach(string toDelete in currentUser_.friendsInvitationsDeletedList_){
                        cloudVersion.Remove(toDelete);
                    }

                    database.Child($"users/{currentUser_.getUid()}/{property}").SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion)).ContinueWith(taskUploadUserData =>{
                        //si la subida falla y la propiedad NO esta ya en la cola, ponla de nuevo en la cola
                        if(!taskUploadUserData.IsCompleted && currentUserPropertiesToUpload_.FindIndex(prop => prop == property) == -1){
                            currentUserPropertiesToUpload_.Add(property);
                        }else if(taskUploadUserData.IsCompleted){
                            currentUser_.friendsInvitationsDeletedList_.Clear();
                        }
                    },TaskScheduler.FromCurrentSynchronizationContext());

                }else if(property == "deletedFriends_"){
                    //deleted friends contiene los usuarios que me han eliminado como amigo
                    //cada vez que me logeo lo primero que hago es vaciar esa lista... por lo que en local siempre sera vacia
                    //pero puede ocurrir que despues de yo haber vaciado la local alguien me elimine y entonces tengo que 
                    //respetar la que esta en la bbdd... pero si no vacio la de la bbdd nunca, alguien que me elimine no me
                    //podra aceptar nunca mas... tengo que almacenar los que ya he eliminado y dejar en la bbdd aquellos deletedfriends
                    //que aun no haya eliminado en local

                    List<string> cloudVersion = JsonConvert.DeserializeObject<List<string>>(propertySnapshot.GetRawJsonValue());
                    if(cloudVersion == null){
                        cloudVersion = new List<string>();
                    }
                    //tengo que eliminar de la version online todas las amistades que ya haya eliminado
                    foreach(string toDelete in currentUser_.deletedFriends_){
                        cloudVersion.Remove(toDelete);
                    }

                    database.Child($"users/{currentUser_.getUid()}/{property}").SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion)).ContinueWith(taskUploadUserData =>{
                        //si la subida falla y la propiedad NO esta ya en la cola, ponla de nuevo en la cola
                        if(!taskUploadUserData.IsCompleted && currentUserPropertiesToUpload_.FindIndex(prop => prop == property) == -1){
                            currentUserPropertiesToUpload_.Add(property);
                        }else if(taskUploadUserData.IsCompleted){
                            currentUser_.deletedFriends_.Clear();
                        }
                    },TaskScheduler.FromCurrentSynchronizationContext());

                }else if(property == "challenges_"){
                    List<Dictionary<string,string>> cloudVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(propertySnapshot.GetRawJsonValue());
                     if(cloudVersion == null){
                        cloudVersion = new List<Dictionary<string,string>>();
                    }
                    bool somethingToUpdate = false;
                    //tengo que eliminar de la version online todas las amistades que ya haya eliminado
                    foreach(challengeData toDelete in currentUser_.deletedChallenges_){
                        Dictionary<string,string> toDeleteDictionary = cloudVersion.Find(challenge => challenge["challengerId_"] == toDelete.getChallengerId());
                        if(toDeleteDictionary != null){
                            cloudVersion.Remove(toDeleteDictionary);
                            somethingToUpdate = true;
                        }
                    }
                    if(somethingToUpdate){
                        database.Child($"users/{currentUser_.getUid()}/{property}").SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion)).ContinueWith(taskUploadUserData =>{
                            //si la subida falla y la propiedad NO esta ya en la cola, ponla de nuevo en la cola
                            if(!taskUploadUserData.IsCompleted && currentUserPropertiesToUpload_.FindIndex(prop => prop == property) == -1){
                                currentUserPropertiesToUpload_.Add(property);
                            }else if(taskUploadUserData.IsCompleted){
                                currentUser_.deletedChallenges_.Clear();
                            }
                        },TaskScheduler.FromCurrentSynchronizationContext());
                    }

                }else if(property == "acceptedFriendsInvitations_"){
                    //mismo motivo que deletedFriends
                    //tengo que tener cuidado con las accepted friends invitations porque nada mas descargado
                    //vacia la lista entonces tengo que tener apuntado cuales he aceptado ya para eliminarlos
                    //por que puede que me acepten una invitacion mientras que esta conectado por lo que hay
                    //que respetar los que no tenga en local porque sino siempre lo dejo como una lista vacia

                    List<string> cloudVersion = JsonConvert.DeserializeObject<List<string>>(propertySnapshot.GetRawJsonValue());
                    if(cloudVersion == null){
                        cloudVersion = new List<string>();
                    }
                    //tengo que eliminar de la version online todas las amistades que ya haya eliminado
                    foreach(string toDelete in currentUser_.acceptedFriends_){
                        cloudVersion.Remove(toDelete);
                    }
                    database.Child($"users/{currentUser_.getUid()}/{property}").SetRawJsonValueAsync(JsonConvert.SerializeObject(cloudVersion)).ContinueWith(taskUploadUserData =>{
                        //si la subida falla y la propiedad NO esta ya en la cola, ponla de nuevo en la cola
                        if(!taskUploadUserData.IsCompleted && currentUserPropertiesToUpload_.FindIndex(prop => prop == property) == -1){
                            currentUserPropertiesToUpload_.Add(property);
                        }else if(taskUploadUserData.IsCompleted){
                            currentUser_.acceptedFriends_.Clear();
                        }
                    },TaskScheduler.FromCurrentSynchronizationContext());
                }else if(property == "earnedScore_"){
                    //creo que la score que tiene que quedar subida es, si en la nube NO es 0 y 
                    //antes de hacer la descarga, el mio tampoco era 0 
                    string stringVersion = JsonConvert.DeserializeObject<string>(propertySnapshot.GetRawJsonValue());
                    int cloudVersion = Int32.Parse(stringVersion == null ? "0" : stringVersion);
                    if(cloudVersion != 0 && currentUser_.earnedScoreBeforeAdding_ != 0){
                        //debo restar el que esta en la nube con lo que yo habia conseguido antes
                        //por ejemplo yo me descago 10 puntos pero justo en ese momento alquien escribe que he ganado 2 puntos mas
                        //por lo que la otra persona pondra 12 entonces aqui yo dejaria 12-10=2 puntos
                        //luego aun que otra persona me sume 3 por ejemplo pondria 5 online pero yo ya no puedeo sobreescribir mas esa propiedad
                        //simplemente la dejaria la que ponen online y cuando me vuelva a conectar pillaria esos nuevos 5 puntos
                        int newCloudVersion = cloudVersion - currentUser_.earnedScoreBeforeAdding_;

                        database.Child($"users/{currentUser_.getUid()}/{property}").SetRawJsonValueAsync(JsonConvert.SerializeObject(newCloudVersion)).ContinueWith(taskUploadUserData =>{
                            //si la subida falla y la propiedad NO esta ya en la cola, ponla de nuevo en la cola
                            if(!taskUploadUserData.IsCompleted && currentUserPropertiesToUpload_.FindIndex(prop => prop == property) == -1){
                                currentUserPropertiesToUpload_.Add(property);
                            }else if(taskUploadUserData.IsCompleted){
                                currentUser_.earnedScoreBeforeAdding_ = 0; 
                            }
                        },TaskScheduler.FromCurrentSynchronizationContext());
                    }

                }else{
                    Debug.LogError($"property desconocida en writeUnsafeProperty: {property}");
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }
}