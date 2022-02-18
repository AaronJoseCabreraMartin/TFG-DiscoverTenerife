using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Amistad pendiente de ser aceptada
*/
/**
  * @brief Store the information of the users that the current user has friendships invitations. 
  */
public class newFriendData
{
    /**
      * @brief string that contains the user id of the user that has sended the friendship invitation.
      */
    private string uid_;

    /**
      * @brief string that contains the display name of the user that has sended the friendship invitation.
      */
    private string displayName_;

    /**
      * @brief list of string that contains the accepted friendships invitations of the user
      * that sended the friendship invitation. This is necessary because if the friendship invitation
      * is accepted the user id of the current user has to be added to the list. It has to be a list
      * because more than one friend can accept your invitations before you connect to the app again.
      */
    private List<string> acceptedFriendsInvitations_;
    // si se acepta la peticion esta lista tiene que cambiar y subir los cambios
    // debe ser una lista porque puede ocurrir que mas de un amigo que acepte a la vez

    /**
      * @param string that contains the user id
      * @param string that contains the user display name
      * @param list of strings that contains the user id of the users that have accepted 
      * his invitations.
      * @brief constructor
      */
    public newFriendData(string uid, string displayName, List<string> acceptedFriends){
        uid_ = uid;
        displayName_ = displayName;
        acceptedFriendsInvitations_ = acceptedFriends;
    }

    /**
      * @return string that contains the user id of the represented user.
      * @brief getter of the uid_ property.
      */
    public string getUid(){
        return uid_;
    }

    /**
      * @return string that contains the display name of the represented user.
      * @brief getter of the displayName_ property.
      */
    public string getDisplayName(){
        return displayName_;
    }

    /**
      * @param string that contains the user id of the added user.
      * @brief this method add the given user id to the list of the accepted friends 
      * of the represented user.
      */
    public void addAcceptedFriendInvitation(string uid){
        acceptedFriendsInvitations_.Add(uid);
    }

    /**
      * @return string conversion of the accepted friends list on json formart.
      * @brief this method returns a string conversion of the accepted friends list on the json format.
      */
    public string getStringConversionOfAcceptedFriendInvitations(){
        string conversion = "[";
        for(int index = 0; index < acceptedFriendsInvitations_.Count; index++){
            conversion += acceptedFriendsInvitations_[index];
            if(index + 1 != acceptedFriendsInvitations_.Count){
                conversion += ",";
            }
        }
        conversion += "]";
        return conversion;
    }
}
