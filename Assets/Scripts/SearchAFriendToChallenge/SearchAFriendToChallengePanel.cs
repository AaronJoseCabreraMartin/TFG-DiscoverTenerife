using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
  * @brief This class handles the panel that shows the current user's
  * friends that allow be challenged.
  */
public class SearchAFriendToChallengePanel : MonoBehaviour
{
    /**
      * @brief GameObject that contains the prefab that represent 
      * a friend that can be challenged.
      */
    [SerializeField] GameObject challengeableFriendPrefab_;

    /**
      * @brief GameObject that contains the image that will be shown
      * if the current user doesnt have any challengeable friend.
      */
    [SerializeField] GameObject noChallengeableFriendsImage_;

    /**
      * @brief List of the challengeable friend prefab that are instanciated.
      */
    private List<GameObject> challengeableFriend_;

    /**
      * @brief if its true means that the panel was already filled, false in otherwhise.
      */
    private bool panelFilled_;

    /**
      * @brief it stores the last quantity of elements which the panel has adapted its height.
      */
    private int lastCount_;

    /**
      * @brief initial height of the panel.
      */
    private float initialHeight_;
    
    /**
      * @brief This method is called before the first frame, it instanciate the challengeableFriend_, panelFilled_
      * and lastCount_ properties. And if the userDataIsReady method of firebaseHandler class returns true
      * it calls the fillPanel method.
      */
    void Awake()
    {
        challengeableFriend_ = new List<GameObject>();
        panelFilled_ = false;
        initialHeight_ = (float)gameObject.transform.GetComponent<RectTransform>().rect.height;
        lastCount_ = 0;
        noChallengeableFriendsImage_.SetActive(false);
        if(firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        } 
    }

    /**
      * @brief This method is called once per frame, it checks if the panel has al ready filled and if
      * the userDataIsReady method of firebaseHandler class returns true it calls the fillPanel method.
      * It also check if the lastCount_ property isnt equal to the current challengeableFriend_ count.
      */
    void Update()
    {
        if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        }
        if(lastCount_ != challengeableFriend_.Count){
            adjustPanelSize();
        }
        
        noChallengeableFriendsImage_.SetActive(challengeableFriend_.Count == 0);
    }

    /**
      * @brief This method is called to instanciate the panel. It creates a prefab for each challengeable friend
      * that the current user has. It changes the panelFilled_ property to true.
      */
    private void fillPanel(){
        if(FriendData.usersThatAllowBeChallenged_ != null){
          for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.currentUser_.countOfFriendData(); i++){
            FriendData friendData = firebaseHandler.firebaseHandlerInstance_.currentUser_.getFriendData(i);

            // si el usuario esta en la lista de usuarios que admiten ser retados y no tiene un reto del usuario actual
            if(FriendData.usersThatAllowBeChallenged_.Exists(uid => uid == friendData.getUid()) &&
                !friendData.hasAChallengeOfThisUser(firebaseHandler.firebaseHandlerInstance_.currentUser_.getUid()) ){
                  GameObject newChanllegeAFriendObject = Instantiate(challengeableFriendPrefab_, new Vector3(0, 0, 0), Quaternion.identity);
                  newChanllegeAFriendObject.GetComponent<Friend>().setData(friendData);
                  newChanllegeAFriendObject.transform.SetParent(this.transform);
                  newChanllegeAFriendObject.GetComponent<Friend>().setPanel(this.gameObject);
                  challengeableFriend_.Add(newChanllegeAFriendObject);
            }
          }
          //si la lista no se habia descargado el panel sigue sin haberse llenado
          panelFilled_ = true;
        }
    }

    /**
      * @brief This method should be called each time that the panel adds or quits an element. This method
      * adjust the height of the panel to keep the apparence when the number of elements changes.
      */
    private void adjustPanelSize(){
        float baseHeight = (float)(gameObject.transform.GetComponent<RectTransform>().rect.height * 1.25);
        if(challengeableFriend_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += lastCount_ < challengeableFriend_.Count ? baseHeight*(challengeableFriend_.Count-4) : -baseHeight*(lastCount_-challengeableFriend_.Count);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }else{
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialHeight_);
        }
        lastCount_ = challengeableFriend_.Count;
    }

    /**
      * @param GameObject that is attatched to this panel.
      * @brief This method removes the given GameObject of the
      * challengeableFriend_ property list. If the given GameObject
      * isnt on the list this method doesnt do nothing. 
      */
    public void deleteAChallengeableFriend(GameObject toDelete){
      Debug.Log($"deleteAChallengeableFriend with {toDelete}");
      challengeableFriend_.Remove(toDelete);
    }
}
