using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Amistad ya aceptada por ambos
*/
public class FriendData
{
    private string uid_;
    private string displayName_;
    //private string rank_;
    //private int score_;

    // para registrar cuando un amigo borra a otro,
    // es una lista porque puede que varias personas te borren antes 
    // de que ese usuario se conecte
    private List<string> deletedFriends_;

    public FriendData(string uid, string displayName, List<string> deletedFriends){
        uid_ = uid;
        displayName_ = displayName;
        deletedFriends_ = deletedFriends;
    }

    public string getUid(){
        return uid_;
    }

    public string getDisplayName(){
        return displayName_;
    }

    public void addDeletedFriend(string uid){
        deletedFriends_.Add(uid);
    }

    public string getStringConversionOfDeletedFriends(){
        string conversion = "[";
        for(int index = 0; index < deletedFriends_.Count; index++){
            conversion += deletedFriends_[index];
            if(index + 1 != deletedFriends_.Count){
                conversion += ",";
            }
        }
        conversion += "]";
        return conversion;
    }
}
