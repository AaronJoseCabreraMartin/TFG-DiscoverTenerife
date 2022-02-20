using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows the new friendships invitations.
  */
public class newFriendInvitationPanel : MonoBehaviour
{
    /**
      * @brief GameObject that contains the prefab that represent a friendship invitation. 
      */
    [SerializeField] private GameObject invitationPrefab_;

    /**
      * @brief List of the invitations prefab that are instanciated.
      */
    private List<GameObject> invitations_;

    /**
      * @brief if its true means that the panel was already filled, false in otherwhise.
      */
    private bool panelFilled_;

    /**
      * @brief it stores the last quantity of invitations which the panel has adapted its height.
      */
    private int lastCount_;
    
    /**
      * This method is called before the first frame, it instanciate the invitations_, panelFilled_
      * and lastCount_ properties. And if the userDataIsReady method of firebaseHandler class returns true
      * it calls the fillPanel method.
      */
    void Awake()
    {
        invitations_ = new List<GameObject>();
        panelFilled_ = false;
        lastCount_ = 0;
        if(firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        } 
    }

    /**
      * This method is called once per frame, it checks if the panel has al ready filled and if
      * the userDataIsReady method of firebaseHandler class returns true it calls the fillPanel method.
      * It also check if the lastCount_ property isnt equal to the current invitations_ count.
      */
    void Update()
    {
        if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        }
        if(lastCount_ != invitations_.Count){
            adjustPanelSize();
        }
    }

    /**
      * This method is called to instanciate the panel. It creates a prefab for each new friendship invitation
      * that the current user has. It changes the panelFilled_ property to true.
      */
    private void fillPanel(){
        for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.currentUser_.countOfNewFriendData(); i++){
            GameObject newInvitationObject = Instantiate(invitationPrefab_, new Vector3(0, 0, 0), Quaternion.identity);
            newInvitationObject.GetComponent<newFriendInvitation>().setData(firebaseHandler.firebaseHandlerInstance_.currentUser_.getNewFriendData(i));
            newInvitationObject.transform.SetParent(this.transform);
            newInvitationObject.GetComponent<newFriendInvitation>().setPanel(this.gameObject);
            invitations_.Add(newInvitationObject);
        }
        panelFilled_ = true;
    }

    /**
      * This method should be called each time that the panel adds or quits an element. This method
      * adjust the height of the panel to keep the apparence when the number of elements changes.
      */
    private void adjustPanelSize(){
        if(invitations_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += lastCount_ < invitations_.Count ? 300*(invitations_.Count-4) : -300*(lastCount_-invitations_.Count);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
        lastCount_ = invitations_.Count;
    }

    /**
      * @return int the number of new friendships invitations.
      * @brief getter of the invitations_.Count property.
      */
    public int getInvitationsCount(){
        return invitations_.Count;
    }

    /**
      * @param GameObject that contains the prefab element that has been deleted.
      * @brief tries to remove the given gameobject of the invitations_ property. If the 
      * gameobject isnt on the list, it will raise an index out of range exception.
      */
    public void invitationDeleted(GameObject invitationDeleted){
        invitations_.RemoveAt(invitations_.FindIndex(element => element == invitationDeleted));
    }
}
