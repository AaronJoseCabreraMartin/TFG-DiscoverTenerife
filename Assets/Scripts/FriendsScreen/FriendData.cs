using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that stores the information of one of the current user's friend.
  */
public class FriendData
{
    /**
      * @brief string that contains the user id of the represented user.
      */
    private string uid_;

    /**
      * @brief string that contains the display name of the represented user.
      */
    private string displayName_;
    
    /**
      * @brief string that contains the current rank of the represented user.
      */
    private string rank_;
    
    /**
      * @brief int that contains the current score of the represented user.
      */
    private int score_;

    /**
      * @brief List that contains strings with the user id of the users that has removed
      * their frienship with the represented user. It has to be a list because more than
      * one user can erase their friendship before the represented user connects again to 
      * the app.
      */
    private List<string> deletedFriends_;

    /**
      * @brief List that contains challengeData objects that represents all the challenges
      * that the represented friend has.
      */
    private List<challengeData> challenges_;

    /**
      * @brief List that contains the invitations that this user sended and the other
      * player accepted de invitation.
      */
    private List<string> acceptedFriendsInvitations_;

    /**
      * @brief List of strings that contains the user id of the users that
      * allow receive friendships invitations from other players.
      */
    public static List<string> usersThatAllowFriendshipInvitations_;

    /**
      * @brief List of strings that contains the user id of the users that
      * allow receive challenges from other players that are they friends.
      */
    public static List<string> usersThatAllowBeChallenged_;

    /**
      * @brief List of strings that contains the user id of the users that
      * allow be shown on the ranking of the players.
      */
    public static List<string> usersThatAllowAppearedOnRanking_;

    /**
      * @brief static property that stored the information of the chosen friend.
      */
    public static FriendData chosenFriend_;


    /**
      * @param string user id of the represented user.
      * @param string display name of the represented user.
      * @param List<string> list of strings that contains the user ids from the users
      * that have deleted the friendship.
      * @param List<Dictionary<string,string>> list that contains all the information on
      * dictionaries of strings of all the challenges that represented friend has.
      * @brief Constructor that initialize the uid_, the displayName_, the deletedFriends_ and
      * challenges_ properties with the given parameters.
      */
    public FriendData(string uid, string displayName, List<string> deletedFriends, List<Dictionary<string,string>> challengeData, 
                      List<string> acceptedFriends, int score){
        uid_ = uid;
        displayName_ = displayName;
        deletedFriends_ = deletedFriends;
        acceptedFriendsInvitations_ = acceptedFriends;
        challenges_ = new List<challengeData>();
        score_ = score;
        foreach(Dictionary<string,string> challengeInfo in challengeData ){
          challenges_.Add(new challengeData(challengeInfo));
        }

    }

    /**
      * @return string with the user id of the represented user.
      * @brief getter of the user id.
      */
    public string getUid(){
        return uid_;
    }

    /**
      * @return string with the display name of the represented user.
      * @brief getter of the user id.
      */
    public string getDisplayName(){
        return displayName_;
    }

    /**
      * @return int with the friend's score
      * @brief getter of the score_ property
      */
    public int getScore(){
      return score_;
    }

    /**
      * @param string with the user id of the user that has deleted the friendship.
      * @brief this method add the given user id to the deletedFriends_ property list.
      */
    public void addDeletedFriend(string uid){
        deletedFriends_.Add(uid);
    }

    /**
      * @return string with a string conversion of the deletedFriends_ list on JSON format.
      * @brief this method convers the deletedFriends_ property into a string with the JSON format
      * and returns the conversion.
      */
    public string getStringConversionOfDeletedFriends(){
        string conversion = "[";
        for(int index = 0; index < deletedFriends_.Count; index++){
            conversion += "\""+ deletedFriends_[index] +"\"";
            if(index + 1 != deletedFriends_.Count){
                conversion += ",";
            }
        }
        conversion += "]";
        return conversion;
    }

    /**
      * @param string that contains the place id of the place that the user has
      * to visit for completing the challenge.
      * @param string that contains the type of the place that the user has to
      * visit for completing the challenge.
      * @param string that contains the user id of the user that has challenged
      * the represented user.
      * @brief this method instanciate a new challengeData object with the given information
      * and the current ticks and it adds the new challengeData object to the challenges_
      * property. 
      */
    public void createNewChallenge(string placeId, string placeType, string challengerId){
      Dictionary<string,string> challengeInfo = new Dictionary<string,string>();
      challengeInfo["placeId_"] = placeId;
      challengeInfo["placeType_"] = placeType;
      challengeInfo["challengerId_"] = challengerId;
      challengeInfo["startTimestamp_"] = DateTime.Now.Ticks.ToString();
      challenges_.Add(new challengeData(challengeInfo));
    }

    /**
      * @param string that contains the user id of the user that we are going
      * to check.
      * @return bool, true if the represented user has a challenge of the
      * given user id.
      * @brief This method returns true if the represented user has a challenge
      * of the user that has the given user id, in other case, it returns false.
      */
    public bool hasAChallengeOfThisUser(string uid){
      return challenges_.Exists(challenge => challenge.getChallengerId() == uid);
    }

    /**
      * @return string that contains the JSON conversion in a string.
      * @brief This method returns a string conversion of the whole list
      * of challenges of the represented user. It uses the ToJson method
      * of the challengeData class.
      */
    public string getStringConversionOfChallenges(){
      string conversion = "[";
        for(int index = 0; index < challenges_.Count; index++){
          conversion += challenges_[index].ToJson();
          if(index + 1 != challenges_.Count){
            conversion += ",";
          }
        }
        conversion += "]";
        return conversion;
    }
  
    /**
      * @param string with the user id of the user that has accepted the friendship invitation
      * that this user has sended.
      * @brief This method adds the given user id to the list of friendships invitations accepted if
      * the given user id isnt on the acceptedFriendsInvitations_ property list.
      */
    public void addANewAcceptedFriend(string uid){
      if(acceptedFriendsInvitations_.Find(element => element == uid) == null){
        acceptedFriendsInvitations_.Add(uid);
      }
    }

    /**
      * @return string with the conversion of acceptedFriendsInvitations_ list property in JSON format.
      * @brief This method returns a string that contains the conversion of the
      * acceptedFriendsInvitations_ list property in JSON format.
      */
    public string getStringConversionOfNewAcceptedFriends(){
      string conversion = "[";
      for(int index = 0; index < acceptedFriendsInvitations_.Count; index++){
        conversion +="\"" + acceptedFriendsInvitations_[index] + "\"";
        if(index + 1 != acceptedFriendsInvitations_.Count){
          conversion += ",";
        }
      }
      conversion += "]";
      return conversion;
    }
    
    /**
      * @param string with the name of the property that you want to obtain the json conversion
      * @return string witht the json conversion of the value of the given propertie
      * @brief This method calls the correspondent method of getStringConversionOf*
      * Si se le da un nombre erroneo retornara un string vacio
      */
    public string getJSONof(string property){
      if(property == "challenges_"){
        return getStringConversionOfChallenges();
      }if(property == "deletedFriends_"){
        return getStringConversionOfDeletedFriends();
      }if(property == "acceptedFriends_"){
        return getStringConversionOfNewAcceptedFriends();
      }else{
        Debug.Log("Propiedad desconocida en getJSONof de friendData: " + property);
        return "";
      }
    }

    /**
      * @param string with the user id
      * @return challengeData with the information of the searched challenge
      * @brief This method returns the challenge information of the challenge that
      * has as a challenger the user with the given uid. If there isnt any challenge
      * with that user id it returns null.
      */
    public challengeData getChallengeOfUser(string uid){
      return challenges_.Find(challenge => challenge.getChallengerId() == uid);
    }
}
