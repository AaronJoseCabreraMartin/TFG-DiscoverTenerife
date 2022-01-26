using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Amistad pendiente de ser aceptada
*/
public class newFriendData
{
    private string uid_;
    private string displayName_;

    // si se acepta la peticion esta lista tiene que cambiar y subir los cambios
    // debe ser una lista porque puede ocurrir que mas de un amigo que acepte a la vez
    private List<string> acceptedFriendsInvitations_;

    public newFriendData(string uid, string displayName, List<string> acceptedFriends){
        uid_ = uid;
        displayName_ = displayName;
        acceptedFriendsInvitations_ = acceptedFriends;
    }

    public string getUid(){
        return uid_;
    }

    public string getDisplayName(){
        return displayName_;
    }

    public void addAcceptedFriendInvitation(string uid){
        acceptedFriendsInvitations_.Add(uid);
    }

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
