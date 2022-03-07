using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows all the challenges
  * that the current user has, it also controls the image that will be
  * shown when the user doesnt have any challenge.
  */
public class challengesPanelController : MonoBehaviour
{
    /**
      * @brief GameObject that contains the prefab that represent a challenge. 
      */
    [SerializeField] private GameObject challengePrefab_;

    /**
      * @brief GameObject that contains the image that will be shown when the current user
      * dont have any challenge. 
      */
    [SerializeField] private GameObject noChallengesImage_;

    /**
      * @brief if its true means that the panel was already filled, false in otherwhise.
      */
    private bool panelFilled_;

    /**
      * @brief List of the challenges prefab that are instanciated.
      */
    private List<GameObject> challenges_;

    /**
      * @brief it stores the last quantity of challenges which the panel has adapted its height.
      */
    private int lastCount_;

    /**
      * @brief initial height of the panel.
      */
    private float initialHeight_;
    
    /**
      * This method is called before the first frame, it instanciate the challenges_, panelFilled_
      * and lastCount_ properties. And if the userDataIsReady method of firebaseHandler class returns true
      * it calls the fillPanel method.
      */
    void Awake()
    {
        challenges_ = new List<GameObject>();
        panelFilled_ = false;
        initialHeight_ = (float)gameObject.transform.GetComponent<RectTransform>().rect.height;
        lastCount_ = 0;
        if(firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        } 
    }

    /**
      * This method is called once per frame, it checks if the panel has al ready filled and if
      * the userDataIsReady method of firebaseHandler class returns true it calls the fillPanel method.
      * If the lastCount_ property isnt equal to the current challenges_ count it calls the adjustPanelSize
      * method. It also shows the noChallengesImage_ gameObject if the challenges_ list is empty.
      */
    void Update(){
        if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        }
        if(lastCount_ != challenges_.Count){
            adjustPanelSize();
        }
        noChallengesImage_.SetActive(challenges_.Count == 0);
    }

    /**
      * This method is called to instanciate the panel. It creates a prefab for each new challenge invitation
      * that the current user has. It changes the panelFilled_ property to true.
      */
    private void fillPanel(){
        for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.currentUser_.getQuantityOfChallenges(); i++){
            GameObject challengeObject = Instantiate(challengePrefab_, new Vector3(0, 0, 0), Quaternion.identity);
            challengeObject.GetComponent<challengePrefabController>().setChallengeData(firebaseHandler.firebaseHandlerInstance_.currentUser_.getChallenge(i));
            challengeObject.transform.SetParent(this.transform);
            challengeObject.GetComponent<challengePrefabController>().setPanel(this.gameObject);
            challenges_.Add(challengeObject);
        }
        panelFilled_ = true;
    }

    /**
      * This method should be called each time that the panel adds or quits an element. This method
      * adjust the height of the panel to keep the apparence when the number of elements changes.
      */
    private void adjustPanelSize(){
        float baseHeight = (float)(gameObject.transform.GetComponent<RectTransform>().rect.height * 1.25);
        if(challenges_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += lastCount_ < challenges_.Count ? baseHeight*(challenges_.Count-4) : -baseHeight*(lastCount_-challenges_.Count);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }else{
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialHeight_);
        }
        lastCount_ = challenges_.Count;
    }

    /**
      * @param GameObject that contains the prefab element that has been deleted.
      * @brief tries to remove the given gameobject of the challenges_ property. If the 
      * gameobject isnt on the list, it will raise an index out of range exception.
      */
    public void challengeDeleted(GameObject invitationDeleted){
        challenges_.RemoveAt(challenges_.FindIndex(element => element == invitationDeleted));
    }
}
