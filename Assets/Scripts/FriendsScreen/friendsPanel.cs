using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows the friends of the current user.
  */
public class friendsPanel : MonoBehaviour
{
    /**
      * @brief GameObject that contains the prefab that shows the friends information.
      */
    [SerializeField] private GameObject friendPrefab_;

    /**
      * @brief List<GameObject> that contains all the prefabs that the panel is showing.
      */
    private List<GameObject> friends_;

    /**
      * @brief true if the panel is already filled, false in other case.
      */
    private bool panelFilled_;

    /**
      * @brief count of the elements that the panel is showing the last time it adapted its height.
      */
    private int lastCount_;
    
    /**
      * This method is called before the first frame, it initialices the friends_, panelFilled_ and 
      * lastCount_ property. And also, if both methods userDataIsReady and friendDataIsComplete of the 
      * firebaseHandler class returned true, it calls the fillPanel method.
      */
    void Awake()
    {
        friends_ = new List<GameObject>();
        panelFilled_ = false;
        lastCount_ = 0;
        if(firebaseHandler.firebaseHandlerInstance_.userDataIsReady() && firebaseHandler.firebaseHandlerInstance_.actualUser_.friendDataIsComplete()){
            fillPanel();
        } 
    }

    /**
      * This method is called each frame.
      * If the panel isnt filled and both methods userDataIsReady and friendDataIsComplete of the 
      * firebaseHandler class returned true, it calls the fillPanel method.
      * If the lastCount_ property inst equal to friends_.Count property it calls the adjustPanelSize.
      */
    void Update()
    {
        if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_.userDataIsReady() && firebaseHandler.firebaseHandlerInstance_.actualUser_.friendDataIsComplete()){
            fillPanel();
        }
        if(lastCount_ != friends_.Count){
            adjustPanelSize();
        }
    }

    /**
      * This method instantiate a new friendPrefab_ for each friend of the current user and it stores
      * it on the friends_ list. It also change the value of the property panelFilled_ to true. 
      */
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

    /**
      * This method should be called when a new element is added to the panel.
      * It makes the panel grow in height to keep the aspect when the number of elements
      * changes. It uptade the lastCount_ property value.
      */
    private void adjustPanelSize(){
        if(friends_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += lastCount_ < friends_.Count ? 300*(friends_.Count-4) : -300*(lastCount_-friends_.Count);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
        lastCount_ = friends_.Count;
    }

    /**
      * @return int the friends_.Count value.
      * @brief Getter of the friends_.Count property.
      */
    public int getInvitationsCount(){
        return friends_.Count;
    }

    /**
      * @param GameObject that contains the friend prefab that the current user has deleted the
      * friendship.
      * @brief Remove the given GameObject of the friends_ property. 
      */
    public void friendDeleted(GameObject friendDeleted){
        friends_.Remove(friendDeleted);
        //WTF porque esto esta comentado!???!?!? deberia subir los cambios a firebase, no???
        //firebaseHandler.firebaseHandlerInstance_.actualUser_.deleteFriendByName(friendDeleted.GetComponent<Friend>().getName());
    }
}
