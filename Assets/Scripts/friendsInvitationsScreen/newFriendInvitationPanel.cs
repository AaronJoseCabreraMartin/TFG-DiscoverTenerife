using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newFriendInvitationPanel : MonoBehaviour
{
    [SerializeField] private GameObject invitationPrefab_;

    private List<GameObject> invitations_;
    private bool panelFilled_;
    private int lastCount_;
    
    void Awake()
    {
        invitations_ = new List<GameObject>();
        panelFilled_ = false;
        lastCount_ = 0;
        if(firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        }
        if(lastCount_ != invitations_.Count){
            adjustPanelSize();
        }
    }

    private void fillPanel(){
        for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.actualUser_.countOfNewFriendData(); i++){
            GameObject newInvitationObject = Instantiate(invitationPrefab_, new Vector3(0, 0, 0), Quaternion.identity);
            newInvitationObject.GetComponent<newFriendInvitation>().setData(firebaseHandler.firebaseHandlerInstance_.actualUser_.getNewFriendData(i));
            newInvitationObject.transform.SetParent(this.transform);
            newInvitationObject.GetComponent<newFriendInvitation>().setPanel(this.gameObject);
            invitations_.Add(newInvitationObject);
        }
        panelFilled_ = true;
    }

    private void adjustPanelSize(){
        if(invitations_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += lastCount_ < invitations_.Count ? 300*(invitations_.Count-4) : -300*(lastCount_-invitations_.Count);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
        lastCount_ = invitations_.Count;
    }

    public int getInvitationsCount(){
        return invitations_.Count;
    }

    public void invitationDeleted(GameObject invitationDeleted){
        invitations_.RemoveAt(invitations_.FindIndex(element => element == invitationDeleted));
    }
}
