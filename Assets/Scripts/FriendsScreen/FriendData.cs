using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that stores the information of a friend of the current user.
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
      * @brief string that contains the actual rank of the represented user.
      */
    private string rank_;
    
    /**
      * @brief int that contains the actual score of the represented user.
      */
    private int score_;

    /**
      * List that contains strings with the user id of the users that has removed
      * their frienship with the represented user. It has to be a list because more than
      * one user can erase their friendship before the represented user connects again to 
      * the app.
      */
    private List<string> deletedFriends_;

    /**
      * @param string user id of the represented user.
      * @param string display name of the represented user.
      * @param List<string> list of strings that contains the user ids from the users
      * that have deleted the friendship.
      * @brief Constructor that initialize the uid_, the displayName_ and the deletedFriends_ properties
      * with the given parameters.
      */
    public FriendData(string uid, string displayName, List<string> deletedFriends){
        uid_ = uid;
        displayName_ = displayName;
        deletedFriends_ = deletedFriends;
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
}
