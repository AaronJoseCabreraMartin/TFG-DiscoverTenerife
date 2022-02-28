using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows the result of player search.
  */
public class SearchedFriendsPanel : MonoBehaviour
{
    /**
      * @brief GameObject the prefab that shows the information of the searched user.
      */
    [SerializeField] private GameObject searchedFriendPrefab_;

    /**
      * @brief GameObject the toast message object that will alert the user with the 
      * errors if they happen.
      */
    [SerializeField] private GameObject toastMessage_;

    /**
      * @brief List<GameObject> List of the generated prefabs that represent the
      * users that were found by the search.
      */
    private List<GameObject> searchedFriends_;

    /**
      * @brief the original height of the panel.
      */
    private float initialHeight_;
    
    /**
      * @brief This method is called before the first frame. It initializes both the 
      * searchedFriends_ and the initialHeight_ property.
      */
    void Awake(){
        searchedFriends_ = new List<GameObject>();
        initialHeight_ = GetComponent<RectTransform>().rect.height;
    }

    /**
      * @param Dictionary<string,string> that contains the information of the searched friend.
      * @brief this method generates a new GameObject with the prefab that is on searchedFriendPrefab_
      * and add it as a child of the panel and then calls the adjustPanelSize method.
      */
    public void addSearchedFriendToPanel(Dictionary<string,string> searchedFriendData){
        
        Debug.Log($"addSearchedFriendToPanel -> {searchedFriendData}");
        foreach(var key in searchedFriendData.Keys){
            Debug.Log($"{key} -> "+searchedFriendData[key]);
        }
        GameObject newSearchedFriend = Instantiate(searchedFriendPrefab_, new Vector3(0, 0, 0), Quaternion.identity);
        newSearchedFriend.GetComponent<SearchedPlayer>().setSearchedPlayerData(searchedFriendData);
        newSearchedFriend.GetComponent<SearchedPlayer>().SetPanel(this);
        newSearchedFriend.transform.SetParent(this.transform);
        searchedFriends_.Add(newSearchedFriend);
        adjustPanelSize();
    }

    /**
      * @brief This method adjust the size of the panel if there is more than 4 elements on it.
      * This method should be called each time that its added a new searched friend. 
      * This method allows the panel keep the aparience when more elements are added or deleted.
      */
    private void adjustPanelSize(){
        if(searchedFriends_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += 300*(searchedFriends_.Count-4);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }else{
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialHeight_);
        }
    }

    /**
      * @return int The count of the searched fiends.
      */
    public int getSearchedFriendsCount(){
        return searchedFriends_.Count;
    }


    /**
      * @brief This method destroy each of the elements of the panel and calls the adjustPanelSize method.
      */
    public void clearSearchedFriendsPanel(){
        while(searchedFriends_.Count != 0){
            Destroy(searchedFriends_[0]);
            searchedFriends_.RemoveAt(0);
        }
        searchedFriends_.Clear();
        adjustPanelSize();
    }

    /**
      * @param string menssage to show on the displayed toast.
      * @param Color32 the color of the displayed toast
      * @param int the duration in seconds of the animation of the displayed toast.
      * @brief this method calls the makeAnimation method of the toastMessage class with the same
      * arguments that its called.
      */
    public void makeToast(string message, Color32 color, int duration){
        toastMessage_.GetComponent<toastMessage>().makeAnimation(message,color,duration);
    }
}
