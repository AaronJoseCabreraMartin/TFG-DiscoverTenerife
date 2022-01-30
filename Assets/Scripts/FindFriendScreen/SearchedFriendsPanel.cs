using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchedFriendsPanel : MonoBehaviour
{
    [SerializeField] private GameObject searchedFriendPrefab_;
    [SerializeField] private GameObject toastMessage_;

    private List<GameObject> searchedFriends_;
    
    void Awake()
    {
        searchedFriends_ = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

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

    private void adjustPanelSize(){
        if(searchedFriends_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += 300*(searchedFriends_.Count-4);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
    }

    public int getSearchedFriendsCount(){
        return searchedFriends_.Count;
    }

    public void clearSearchedFriendsPanel(){
        while(searchedFriends_.Count != 0){
            Destroy(searchedFriends_[0]);
        }
        searchedFriends_.Clear();
        adjustPanelSize();
    }

    public void makeToast(string message, Color32 color, int duration){
        toastMessage_.GetComponent<toastMessage>().makeAnimation(message,color,duration);
    }
}
