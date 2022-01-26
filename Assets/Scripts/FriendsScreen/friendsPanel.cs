using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class friendsPanel : MonoBehaviour
{
    [SerializeField] private GameObject friendPrefab_;

    private List<GameObject> friends_;
    private bool panelFilled_;
    private int lastCount_;
    
    void Awake()
    {
        friends_ = new List<GameObject>();
        panelFilled_ = false;
        lastCount_ = 0;
        if(firebaseHandler.firebaseHandlerInstance_.userDataIsReady() && firebaseHandler.firebaseHandlerInstance_.actualUser_.friendDataIsComplete()){
            fillPanel();
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_.userDataIsReady() && firebaseHandler.firebaseHandlerInstance_.actualUser_.friendDataIsComplete()){
            fillPanel();
        }
        if(lastCount_ != friends_.Count){
            adjustPanelSize();
        }
    }

    private void fillPanel(){
        for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.actualUser_.countOfFriendData(); i++){
            GameObject friendObject = Instantiate(friendPrefab_, new Vector3(0, 0, 0), Quaternion.identity);
            friendObject.GetComponent<Friend>().setData(firebaseHandler.firebaseHandlerInstance_.actualUser_.getFriendData(i));
            friendObject.transform.SetParent(this.transform);
            friendObject.GetComponent<Friend>().setPanel(this.gameObject);
            friends_.Add(friendObject);
        }
        panelFilled_ = true;
    }

    private void adjustPanelSize(){
        if(friends_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += lastCount_ < friends_.Count ? 300*(friends_.Count-4) : -300*(lastCount_-friends_.Count);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
        lastCount_ = friends_.Count;
    }

    public int getInvitationsCount(){
        return friends_.Count;
    }

    public void friendDeleted(GameObject friendDeleted){
        friends_.Remove(friendDeleted);
        //firebaseHandler.firebaseHandlerInstance_.actualUser_.deleteFriendByName(friendDeleted.GetComponent<Friend>().getName());
    }
}
