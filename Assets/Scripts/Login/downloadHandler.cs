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
  * @brief the part of the class firebaseHandler that controls the downloading operations
  */
public partial class firebaseHandler{
    /**
      * @brief this method calls the downloadFriendData method with each user id that is 
      * on the friendDataDownloadQueue_.
      */
    private void downloadAllFriendData(){
        for(int index = 0; index < currentUser_.countOfFriends(); index++){
            string uid = currentUser_.getFriend(index);
            if(friendDataDownloadQueue_.Exists(friendDataForDownload => friendDataForDownload == uid)){
                friendDataDownloadQueue_.Remove(uid);
                downloadFriendData(uid);
            }
        }
    }

    /**
      * @param string that contains the user id of the friend of the current user that you want to
      * obtain data.
      * @brief this method tries to download the necesary information of the given user id. If it fails
      * on the process it adds the user id to the friendDataDownloadQueue_ list. If it success it calls
      * the addFriendData method of UserData class creating a new FriendData object.
      */
    public void downloadFriendData(string uid){
        FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/displayName_").GetValueAsync().ContinueWith(displayNameTask => {
            if (displayNameTask.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+displayNameTask.Exception);
                friendDataDownloadQueue_.Add(uid);
            } else if (displayNameTask.IsCompleted) {
                DataSnapshot snapshotDisplayName = displayNameTask.Result;
                string displayName;
                if(snapshotDisplayName.GetRawJsonValue() == null){
                    displayName = "null";
                }else{
                    displayName = JsonConvert.DeserializeObject<string>(snapshotDisplayName.GetRawJsonValue());
                }
                FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/deletedFriends_").GetValueAsync().ContinueWith(deletedFriendsTask => {
                    if (deletedFriendsTask.IsFaulted) {
                        // Handle the error...
                        Debug.Log("Error: "+deletedFriendsTask.Exception);
                        friendDataDownloadQueue_.Add(uid);
                    } else if (deletedFriendsTask.IsCompleted) {
                        DataSnapshot deletedFriendsSnapshot = deletedFriendsTask.Result;
                        List<string> deletedFriends;
                        if(deletedFriendsSnapshot.GetRawJsonValue() == null){
                            deletedFriends = new List<string>();
                        }else{
                            deletedFriends = JsonConvert.DeserializeObject<List<string>>(deletedFriendsSnapshot.GetRawJsonValue());
                        }
                        FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/challenges_").GetValueAsync().ContinueWith(challengesTask => {
                            if (challengesTask.IsFaulted) {
                                // Handle the error...
                                Debug.Log("Error: "+challengesTask.Exception);
                                friendDataDownloadQueue_.Add(uid);
                            } else if (challengesTask.IsCompleted) {
                                DataSnapshot challengesSnapshot = challengesTask.Result;
                                List<Dictionary<string,string>> challenges;
                                if(challengesSnapshot.GetRawJsonValue() == null){
                                    challenges = new List<Dictionary<string,string>>();
                                }else{
                                    challenges = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(challengesSnapshot.GetRawJsonValue());
                                }
                                FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/acceptedFriendsInvitations_").GetValueAsync().ContinueWith(acceptedNewFriendsTask => {
                                    if (acceptedNewFriendsTask.IsFaulted) {
                                        // Handle the error...
                                        Debug.Log("Error: "+acceptedNewFriendsTask.Exception);
                                        friendDataDownloadQueue_.Add(uid);
                                    } else if (acceptedNewFriendsTask.IsCompleted) {
                                        DataSnapshot acceptedNewFriendsSnapshot = acceptedNewFriendsTask.Result;
                                        List<string> acceptedNewFriends;
                                        if(acceptedNewFriendsSnapshot.GetRawJsonValue() == null){
                                            acceptedNewFriends = new List<string>();
                                        }else{
                                            acceptedNewFriends = JsonConvert.DeserializeObject<List<string>>(acceptedNewFriendsSnapshot.GetRawJsonValue());
                                        }
                                        if(currentUser_.hasToBeNotified(uid)){
                                            acceptedNewFriends.Add(currentUser_.getUid());
                                        }
                                        
                                        FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/score_").GetValueAsync().ContinueWith(scoreTask => {
                                            if (scoreTask.IsFaulted) {
                                                // Handle the error...
                                                Debug.Log("Error: "+scoreTask.Exception);
                                                friendDataDownloadQueue_.Add(uid);
                                            } else if (scoreTask.IsCompleted) {
                                                DataSnapshot scoreSnapshot = scoreTask.Result;
                                                int score = 0;
                                                if(scoreSnapshot.GetRawJsonValue() != null){
                                                    score = Int32.Parse(scoreSnapshot.GetRawJsonValue().Replace('\"',' '));   
                                                }
                                                currentUser_.addFriendData(new FriendData(uid, displayName, deletedFriends, challenges, acceptedNewFriends, score));
                                           }
                                        },TaskScheduler.FromCurrentSynchronizationContext());
                                    }
                                },TaskScheduler.FromCurrentSynchronizationContext());
                            }
                        },TaskScheduler.FromCurrentSynchronizationContext());
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @brief This method calls downloadNewFriendInvitationData method with each of the 
      * user ids that are on the new friends invitations list of the current user.
      */
    private void downloadAllNewfriendsInvitationsData(){
        for(int index = 0; index < currentUser_.countOfNewFriends(); index++){
            string uid = currentUser_.getNewFriend(index);
            if(newFriendDataDownloadQueue_.Exists(newFriendDataForDownload => newFriendDataForDownload == uid)){
                newFriendDataDownloadQueue_.Remove(uid);
                downloadNewFriendInvitationData(uid);
            }
        }
    }

    /**
      * @param string that contains the user id that you want to download the enough information
      * for create the newFriendInvitation object.
      * @brief this method tries to download the display name and the list of newFriendsInvitations 
      * of the user that matches the given user id. If its fails it adds the user id to the 
      * newFriendDataDownloadQueue_ list and if its success, its calls the addNewFriendData method of
      * UserData.
      */
    private void downloadNewFriendInvitationData(string uid){
        FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/displayName_").GetValueAsync().ContinueWith(displayNameTask => {
            if (displayNameTask.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+displayNameTask.Exception);
                newFriendDataDownloadQueue_.Add(uid);
            } else if (displayNameTask.IsCompleted) {
                DataSnapshot snapshotDisplayName = displayNameTask.Result;
                string displayName;
                if(snapshotDisplayName.GetRawJsonValue() == null){
                    displayName = "null";
                }else{
                    displayName = JsonConvert.DeserializeObject<string>(snapshotDisplayName.GetRawJsonValue());
                }
                FirebaseDatabase.DefaultInstance.GetReference($"users/{uid}/acceptedFriendsInvitations_").GetValueAsync().ContinueWith(acceptedNewFriendsTask => {
                    if (acceptedNewFriendsTask.IsFaulted) {
                        // Handle the error...
                        Debug.Log("Error: "+acceptedNewFriendsTask.Exception);
                        newFriendDataDownloadQueue_.Add(uid);
                    } else if (acceptedNewFriendsTask.IsCompleted) {
                        DataSnapshot newFriendsSnapshot = acceptedNewFriendsTask.Result;
                        List<string> newFriends;
                        if(newFriendsSnapshot.GetRawJsonValue() == null){
                            newFriends = new List<string>();
                        }else{
                            newFriends = JsonConvert.DeserializeObject<List<string>>(newFriendsSnapshot.GetRawJsonValue());
                        }
                        currentUser_.addNewFriendData(new newFriendData(uid, displayName, newFriends));
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
    }

    /**
      * @brief This method download all the information that is releated to the user from the firebase database.
      */
    public void readUserData(){
        Debug.Log($"readUserData: {auth.CurrentUser.UserId}");
        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/visitedPlaces_").GetValueAsync().ContinueWith(taskPlaces => {
            if (taskPlaces.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+taskPlaces.Exception);
            } else if (taskPlaces.IsCompleted) {
                DataSnapshot snapshotVisitedPlaces = taskPlaces.Result;
                // por que si el usuario solo se registra y no visita ningun sitio en ese momento, 
                // el array de visitados sera null (que es lo que estoy pidiendo), entonces tengo que inicializar 
                // los datos como si fuera un nuevo usuario
                //              pos    data
                List<Dictionary<string,string>> visitedPlacesListVersion;
                //Debug.Log($"snapshotVisitedPlaces = {snapshotVisitedPlaces.GetRawJsonValue()}");
                if(snapshotVisitedPlaces.GetRawJsonValue() == null){
                    visitedPlacesListVersion = null;
                }else{
                    visitedPlacesListVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshotVisitedPlaces.GetRawJsonValue());
                }

                Dictionary<string,string> baseCordsData;
                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/baseCords_").GetValueAsync().ContinueWith(taskBaseCords => {
                    if (taskBaseCords.IsFaulted) {
                        // Handle the error...
                        Debug.Log("Error: "+taskBaseCords.Exception);
                    } else if (taskBaseCords.IsCompleted) {
                        DataSnapshot snapshotBaseCords = taskBaseCords.Result;
                        //Debug.Log($"snapshotBaseCords = {snapshotBaseCords.GetRawJsonValue()}");
                        // si la base data es null quiere decir que el usuario nunca llego a activar su servicio GPS
                        if(snapshotBaseCords.GetRawJsonValue() == null){
                            baseCordsData = null;
                        }else{
                            //                                                       name, number
                            baseCordsData = JsonConvert.DeserializeObject<Dictionary<string,string>>(snapshotBaseCords.GetRawJsonValue());
                        }
                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/friends_").GetValueAsync().ContinueWith(taskFriends => {
                            if (taskFriends.IsFaulted) {
                                // Handle the error...
                                Debug.Log("Error: "+taskFriends.Exception);
                            } else if (taskFriends.IsCompleted) {
                                DataSnapshot snapshotFriends = taskFriends.Result;
                                List<string> friendsList;
                                Debug.Log($"snapshotFriends = {snapshotFriends.GetRawJsonValue()}"); 
                                if(snapshotFriends.GetRawJsonValue() == null){
                                    friendsList = null;
                                }else{
                                    //                                                UIDs
                                    friendsList = JsonConvert.DeserializeObject<List<string>>(snapshotFriends.GetRawJsonValue());
                                    foreach(string uid in friendsList){
                                        friendDataDownloadQueue_.Add(uid);
                                    }
                                }
                                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/friendsInvitations_").GetValueAsync().ContinueWith(taskFriendsInvitations => {
                                     if (taskFriendsInvitations.IsFaulted) {
                                        // Handle the error...
                                        Debug.Log("Error: "+taskFriendsInvitations.Exception);
                                    } else if (taskFriendsInvitations.IsCompleted) {
                                        DataSnapshot snapshotFriendsInvitations = taskFriendsInvitations.Result;
                                        List<string> friendsInvitationsList;
                                        //Debug.Log($"snapshotFriendsInvitations = {snapshotFriendsInvitations.GetRawJsonValue()}"); 
                                        if(snapshotFriendsInvitations.GetRawJsonValue() == null){
                                            friendsInvitationsList = null;
                                        }else{
                                            //                                                          UIDs
                                            friendsInvitationsList = JsonConvert.DeserializeObject<List<string>>(snapshotFriendsInvitations.GetRawJsonValue());
                                            foreach(string uid in friendsInvitationsList){
                                                newFriendDataDownloadQueue_.Add(uid);
                                            }
                                        }
                                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/acceptedFriendsInvitations_").GetValueAsync().ContinueWith(taskacceptedFriendsInvitations => {
                                            if (taskacceptedFriendsInvitations.IsFaulted) {
                                                // Handle the error...
                                                Debug.Log("Error: "+taskacceptedFriendsInvitations.Exception);
                                            } else if (taskacceptedFriendsInvitations.IsCompleted) {
                                                DataSnapshot snapshotacceptedFriendsInvitations = taskacceptedFriendsInvitations.Result;
                                                List<string> acceptedFriendsInvitationsList;
                                                //Debug.Log($"snapshotacceptedFriendsInvitations = {snapshotacceptedFriendsInvitations.GetRawJsonValue()}");
                                                bool haveToUploadData = false;
                                                if(snapshotacceptedFriendsInvitations.GetRawJsonValue() == null){
                                                    acceptedFriendsInvitationsList = null;
                                                }else{
                                                    //                                                                  UIDs
                                                    acceptedFriendsInvitationsList = JsonConvert.DeserializeObject<List<string>>(snapshotacceptedFriendsInvitations.GetRawJsonValue());
                                                    foreach(string uid in acceptedFriendsInvitationsList){
                                                        friendDataDownloadQueue_.Add(uid);
                                                    }
                                                    haveToUploadData = true;
                                                }
                                                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/deletedFriends_").GetValueAsync().ContinueWith(taskDeletedFriends => {
                                                    if (taskDeletedFriends.IsFaulted) {
                                                        // Handle the error...
                                                        Debug.Log("Error: "+taskDeletedFriends.Exception);
                                                    } else if (taskDeletedFriends.IsCompleted) {
                                                        DataSnapshot snapshotDeletedFriends = taskDeletedFriends.Result;
                                                        List<string> deletedFriendsList;
                                                        //Debug.Log($"snapshotDeletedFriends = {snapshotDeletedFriends.GetRawJsonValue()}");
                                                        if(snapshotDeletedFriends.GetRawJsonValue() == null){
                                                            deletedFriendsList = null;
                                                        }else{
                                                            deletedFriendsList = JsonConvert.DeserializeObject<List<string>>(snapshotDeletedFriends.GetRawJsonValue());
                                                            haveToUploadData = true; 
                                                        }
                                                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/challenges_").GetValueAsync().ContinueWith(taskChallenges => {
                                                            if (taskChallenges.IsFaulted) {
                                                                // Handle the error...
                                                                Debug.Log("Error: "+taskChallenges.Exception);
                                                            } else if (taskChallenges.IsCompleted) {
                                                                DataSnapshot snapshotChallenges = taskChallenges.Result;
                                                                List<Dictionary<string,string>> challengesList;
                                                                //Debug.Log($"snapshotChallenges = {snapshotChallenges.GetRawJsonValue()}");
                                                                if(snapshotChallenges.GetRawJsonValue() == null){
                                                                    challengesList = null;
                                                                }else{
                                                                    challengesList = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshotChallenges.GetRawJsonValue());
                                                                    haveToUploadData = true; 
                                                                }
                                                        
                                                                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/score_").GetValueAsync().ContinueWith(taskScore => {
                                                                    if (taskScore.IsFaulted) {
                                                                        // Handle the error...
                                                                        Debug.Log("Error: "+taskScore.Exception);
                                                                    } else if (taskScore.IsCompleted) {
                                                                        DataSnapshot snapshotScore = taskScore.Result;
                                                                        string userScore;
                                                                        //Debug.Log($"snapshotScore = {snapshotScore.GetRawJsonValue()}");
                                                                        if(snapshotScore.GetRawJsonValue() == null){
                                                                            userScore = null;
                                                                            haveToUploadData = true; 
                                                                        }else{
                                                                            userScore = JsonConvert.DeserializeObject<string>(snapshotScore.GetRawJsonValue());
                                                                        }
                                                                        
                                                                        FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/earnedScore_").GetValueAsync().ContinueWith(taskEarnedScore => {
                                                                            if (taskEarnedScore.IsFaulted) {
                                                                                // Handle the error...
                                                                                Debug.Log("Error: "+taskEarnedScore.Exception);
                                                                            } else if (taskEarnedScore.IsCompleted) {
                                                                                DataSnapshot snapshotEarnedScore = taskEarnedScore.Result;
                                                                                string userEarnedScore;
                                                                                //Debug.Log($"snapshotEarnedScore = {snapshotEarnedScore.GetRawJsonValue()}");
                                                                                if(snapshotEarnedScore.GetRawJsonValue() == null){
                                                                                    userEarnedScore = null;
                                                                                }else{
                                                                                    userEarnedScore = JsonConvert.DeserializeObject<string>(snapshotEarnedScore.GetRawJsonValue());
                                                                                    haveToUploadData = true;
                                                                                }
                                                                                FirebaseDatabase.DefaultInstance.GetReference($"users/{auth.CurrentUser.UserId}/rangeStory_").GetValueAsync().ContinueWith(taskRangeStory => {
                                                                                    if (taskRangeStory.IsFaulted) {
                                                                                        // Handle the error...
                                                                                        Debug.Log("Error: "+taskRangeStory.Exception);
                                                                                    } else if (taskRangeStory.IsCompleted) {
                                                                                        DataSnapshot snapshotRangeStory = taskRangeStory.Result;
                                                                                        List<Dictionary<string,string>> userRangeStory;
                                                                                        //Debug.Log($"snapshotRangeStory = {snapshotRangeStory.GetRawJsonValue()}");
                                                                                        if(snapshotRangeStory.GetRawJsonValue() == null){
                                                                                            userRangeStory = null;
                                                                                        }else{
                                                                                            userRangeStory = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshotRangeStory.GetRawJsonValue());
                                                                                        }

                                                                                        currentUser_ = new UserData(auth.CurrentUser, visitedPlacesListVersion, baseCordsData, 
                                                                                                                    friendsList, friendsInvitationsList, acceptedFriendsInvitationsList, 
                                                                                                                    deletedFriendsList, challengesList, userScore, userEarnedScore, userRangeStory);

                                                                                        //por cualquiera de los caminos tiene que estar la user data lista
                                                                                        userDataReady_ = true;
                                                                                        if(haveToUploadData){
                                                                                            //writeUserData();
                                                                                            writeAllUserProperties();
                                                                                        }
                                                                            
                                                                                    }
                                                                                },TaskScheduler.FromCurrentSynchronizationContext());
                                                                            }
                                                                        },TaskScheduler.FromCurrentSynchronizationContext());
                                                                    }
                                                                },TaskScheduler.FromCurrentSynchronizationContext());
                                                            }
                                                        },TaskScheduler.FromCurrentSynchronizationContext());
                                                    }
                                                },TaskScheduler.FromCurrentSynchronizationContext());
                                            }
                                        },TaskScheduler.FromCurrentSynchronizationContext());
                                    }
                                },TaskScheduler.FromCurrentSynchronizationContext());
                            }
                        },TaskScheduler.FromCurrentSynchronizationContext());
                    }
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        },TaskScheduler.FromCurrentSynchronizationContext());       
    }

    /**
      * @brief This method start as many coroutines as types of places are defined on mapRulesHandler.
      * Each coroutine will download the information of that type of place. If you want more information
      * check the mapRulesHandler documentation and the downloadOneTypeOfSite method of this class.
      */
    private void downloadAllPlaces(){
        foreach(string typeSite in mapRulesHandler.getTypesOfSites()){
            //StartCoroutine es como olvidate de esto hasta que termine, 
            //cuando termina ejecuta la siguiente linea como si no hubiera pasado nada
            //es para que no se pause la app mientras se descargan los sitios
            StartCoroutine(downloadOneTypeOfSite(typeSite));
        }
        
    }

    /**
      * @param string that contains the type of the sites you want to download the information.
      * @brief This coroutine tries to download all the places of the given type. If there is no
      * internet connection it waits until there is avaible again. When all the places of all types
      * has already downloaded it sets ON the placesReady_ flag.
      */
    private IEnumerator downloadOneTypeOfSite(string typeSite){
        while(!internetConnection()){//debemos esperar a tener conexion
            yield return new WaitForSeconds(0.5f);
        }
        FirebaseDatabase.DefaultInstance.GetReference($"places/{typeSite}/").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
                Debug.Log("Error: "+task.Exception);
            } else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                //           id                 data
                List<Dictionary<string,string>> listVersion = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(snapshot.GetRawJsonValue());
                Dictionary<string,Dictionary<string,string>> dictionaryVersion = new Dictionary<string,Dictionary<string,string>>();
                for(int i = 0; i <listVersion.Count; i++ ){
                    dictionaryVersion[i.ToString()] = listVersion[i];
                }
                allPlaces_[typeSite] = dictionaryVersion;
                if(allPlaces_.Count == mapRulesHandler.getTypesOfSites().Count){
                    placesReady_ = true;
                }
            }
        },TaskScheduler.FromCurrentSynchronizationContext());
        yield return new WaitForSeconds(0);
    }

    /**
      * @brief This method download the lists of the user social permissions.
      * They are usersThatAllowFriendshipInvitations, usersThatAllowBeChallenged and
      * usersThatAllowAppearedOnRanking. The downloadingAnyOfTheUsersPermissionsLists_
      * is set as a true. The social permissions are downloaded in a parallel way for
      * searching more efficiency if some of them failed the downloadingAnyOfTheUsersPermissionsLists_
      * property will be set as false again.
      */
    private void downloadUsersPermissionsLists(){
        int countOfListsDownloaded = 0;
        downloadingAnyOfTheUsersPermissionsLists_ = true;
        if(FriendData.usersThatAllowFriendshipInvitations_ == null){
            FirebaseDatabase.DefaultInstance.GetReference($"users/usersThatAllowFriendshipInvitations").GetValueAsync().ContinueWith(searchTask => {
                if (searchTask.IsFaulted || searchTask.IsCanceled) {
                    Debug.Log("Fallo al descargar la lista de usersThatAllowFriendshipInvitations "+searchTask.Exception);
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }else if(searchTask.IsCompleted){
                    DataSnapshot snapshotSearch = searchTask.Result;
                    if(snapshotSearch.GetRawJsonValue() != null){
                        Debug.Log($"usersThatAllowFriendshipInvitations_ = {snapshotSearch.GetRawJsonValue()}");
                        FriendData.usersThatAllowFriendshipInvitations_ = JsonConvert.DeserializeObject<List<string>>(snapshotSearch.GetRawJsonValue());
                    }
                }
                if(countOfListsDownloaded == 3){
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfListsDownloaded++;
        }

        if(FriendData.usersThatAllowBeChallenged_ == null){
            FirebaseDatabase.DefaultInstance.GetReference($"users/usersThatAllowBeChallenged").GetValueAsync().ContinueWith(searchTask => {
                if (searchTask.IsFaulted || searchTask.IsCanceled) {
                    Debug.Log("Fallo al descargar la lista de usersThatAllowBeChallenged "+searchTask.Exception);
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }else if(searchTask.IsCompleted){
                    DataSnapshot snapshotSearch = searchTask.Result;
                    if(snapshotSearch.GetRawJsonValue() != null){
                        Debug.Log($"usersThatAllowBeChallenged_ = {snapshotSearch.GetRawJsonValue()}");
                        FriendData.usersThatAllowBeChallenged_ = JsonConvert.DeserializeObject<List<string>>(snapshotSearch.GetRawJsonValue());
                    }
                }
                if(countOfListsDownloaded == 3){
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfListsDownloaded++;
        }

        if(FriendData.usersThatAllowAppearedOnRanking_ == null){
            FirebaseDatabase.DefaultInstance.GetReference($"users/usersThatAllowAppearedOnRanking").GetValueAsync().ContinueWith(searchTask => {
                if (searchTask.IsFaulted || searchTask.IsCanceled) {
                    Debug.Log("Fallo al descargar la lista de usersThatAllowAppearedOnRanking "+searchTask.Exception);
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }else if(searchTask.IsCompleted){
                    DataSnapshot snapshotSearch = searchTask.Result;
                    if(snapshotSearch.GetRawJsonValue() != null){
                        Debug.Log($"usersThatAllowAppearedOnRanking_ = {snapshotSearch.GetRawJsonValue()}");
                        FriendData.usersThatAllowAppearedOnRanking_ = JsonConvert.DeserializeObject<List<string>>(snapshotSearch.GetRawJsonValue());
                    }
                }
                if(countOfListsDownloaded == 3){
                    downloadingAnyOfTheUsersPermissionsLists_ = false;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }else{
            countOfListsDownloaded++;
        }
    }

    /**
      * @brief This method downloads all the data what will be shown on the players ranking
      * and store it as a RankingPlayerData object on the rankingData_ list. It changes the
      * downloadingRankingData_ attribute to reflect the current state of the download.
      */
    public void downloadRankingPlayerData(){
        //Solo necesito descargarme el nombre y el score, con el score puedo calcular el top y el rango.
        downloadingRankingData_ = true;
        foreach(string userId in FriendData.usersThatAllowAppearedOnRanking_){
            FirebaseDatabase.DefaultInstance.GetReference($"users/{userId}/displayName_").GetValueAsync().ContinueWith(displayNameTask => {    
                if (displayNameTask.IsFaulted || displayNameTask.IsCanceled) {
                    downloadingRankingData_ = false;
                    rankingData_.Clear();
                    Debug.Log($"Fallo al descargar la informacion de ranking de {userId} :"+displayNameTask.Exception);
                }else if(displayNameTask.IsCompleted){
                    DataSnapshot snapshotDisplayName = displayNameTask.Result;
                    if(snapshotDisplayName.GetRawJsonValue() != null){
                        string displayName = JsonConvert.DeserializeObject<string>(snapshotDisplayName.GetRawJsonValue());
                        
                        FirebaseDatabase.DefaultInstance.GetReference($"users/{userId}/score_").GetValueAsync().ContinueWith(scoreTask => {    
                            if (scoreTask.IsFaulted || scoreTask.IsCanceled) {
                                downloadingRankingData_ = false;
                                rankingData_.Clear();
                                Debug.Log($"Fallo al descargar la informacion de ranking de {userId} :"+scoreTask.Exception);
                            }else if(scoreTask.IsCompleted){
                                DataSnapshot snapshotScore = scoreTask.Result;
                                int score;
                                if(snapshotScore.GetRawJsonValue() != null){
                                    score = Int32.Parse(JsonConvert.DeserializeObject<string>(snapshotScore.GetRawJsonValue()));

                                }else{
                                    score = 0;
                                    Debug.Log($"users/{userId}/score_ es null");
                                }
                                rankingData_.Add(new RankingPlayerData(displayName, score, userId));

                                if(isRankingDataComplete()){
                                    downloadingRankingData_ = false;
                                }
                            }
                        },TaskScheduler.FromCurrentSynchronizationContext());
                    
                    }else{
                        Debug.Log($"users/{userId}/displayName_ es null");
                    }
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}