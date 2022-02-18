using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows the story of the user.
  */
public class StoryPanel : MonoBehaviour
{
    /**
      * @brief GameObject that contains the prefab that represent an element on the current user story.
      */
    [SerializeField] private GameObject placeOnStoryPrefab_;

    /**
      * @brief List<GameObject> list that contains references for all the elements that the panel has.
      */
    private List<GameObject> placeOnStory_;

    /**
      * @brief if true, the panel was already filled, false in other case.
      */
    private bool panelFilled_;

    /**
      * @brief if true, its time to set the data of all the elements of the panel.
      */
    private bool setDataTime_;
    
    /**
      * This method is called before first frame, it initializes the atributtes placeOnStory_, it sets the 
      * panelFilled_ to false and the setDataTime_ to true. If both userDataIsReady and friendDataIsComplete
      * of firebaseHandler class return true, it calls the fillPanel method
      */
    void Awake()
    {
        placeOnStory_ = new List<GameObject>();
        panelFilled_ = false;
        setDataTime_ = true;
        if(firebaseHandler.firebaseHandlerInstance_ != null && firebaseHandler.firebaseHandlerInstance_.userDataIsReady() 
            && firebaseHandler.firebaseHandlerInstance_.actualUser_.friendDataIsComplete()){
                fillPanel();
        } 
    }

    /**
      * This method is called each frame, it check: 
      * - if the user is changing the sorting method clicking on the options, it sets the setDataTime_
      * property to true.
      * - if the user stopped changing the options, setDataTime_ and panelFilled_ are true it calls
      * the setData method and it changes the setDataTime_ property to false
      * - if the panelFilled_ property is false and both the userDataIsReady and the friendDataIsComplete 
      * methods of firebaseHandler class returns true, it calls the fillPanel method. 
      */
    void Update()
    {
        if(optionsController.lastOptionClicked_ != null){
            setDataTime_ = true;
        }else if(optionsController.lastOptionClicked_ == null && setDataTime_ && panelFilled_){
            setData();
            setDataTime_ = false;
        }else if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_ != null && firebaseHandler.firebaseHandlerInstance_.userDataIsReady() 
            && firebaseHandler.firebaseHandlerInstance_.actualUser_.friendDataIsComplete()){
                fillPanel();
        }
    }

    /**
      * This method instantiate a placeOnStoryPrefab_ prefab for each element on the
      * visitedPlaces_ atribute of UserData class. It also change the panelFilled_ property to true
      * and it also calls the adjustPanelSize method.
      */
    private void fillPanel(){
        for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.actualUser_.countOfVisitedPlaces(); i++){
            GameObject storyPlace = Instantiate(placeOnStoryPrefab_, new Vector3(0, 0, 0), Quaternion.identity);
            storyPlace.GetComponent<StoryPlace>().setData(firebaseHandler.firebaseHandlerInstance_.actualUser_.getStoryPlaceData(i));
            storyPlace.transform.SetParent(this.transform);
            placeOnStory_.Add(storyPlace);
        }
        panelFilled_ = true;
        adjustPanelSize();
    }

    /**
      * This method should be called when the elements of the panel changes.
      * It adapts its height to make it keep the appearance.
      */
    private void adjustPanelSize(){
        if(placeOnStory_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height + 300*(placeOnStory_.Count-4);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
    }

    /**
      * This method set the data of each prefab will shown, it sorts the data before
      * giving it to the prefabs. 
      */
    private void setData(){
        if(optionsController.optionsControllerInstance_.sortStoryByFirstVisit()){
            for(int i = 0; i < placeOnStory_.Count; i++){
                placeOnStory_[i].GetComponent<StoryPlace>().setData(firebaseHandler.firebaseHandlerInstance_.actualUser_.getStoryPlaceData(i));
            }
        }else{
            List<int> sortedIndex = new List<int>();
            for(int i = 0; i < placeOnStory_.Count; i++){
                sortedIndex.Add(i);
            }
            sortedIndex.Sort(delegate(int a, int b){
                VisitedPlace visitedPlaceA = firebaseHandler.firebaseHandlerInstance_.actualUser_.getStoryPlaceData(a);
                VisitedPlace visitedPlaceB = firebaseHandler.firebaseHandlerInstance_.actualUser_.getStoryPlaceData(b);
                if(visitedPlaceA.lastVisitTimestamp_ == visitedPlaceB.lastVisitTimestamp_){
                    return 0;
                }else{
                    //      queremos que ordene el mayor timestamp primero
                    return (visitedPlaceA.lastVisitTimestamp_ > visitedPlaceB.lastVisitTimestamp_) ? -1 : 1;
                }
            });
            for(int i = 0; i < placeOnStory_.Count; i++){
                placeOnStory_[i].GetComponent<StoryPlace>().setData(firebaseHandler.firebaseHandlerInstance_.actualUser_.getStoryPlaceData(sortedIndex[i]));
            }
        }
    }

    /**
      * @return bool true if the panel should be hidden, false in other case.
      */
    public bool hidePanel(){
        return placeOnStory_.Count == 0 || setDataTime_ || optionsController.lastOptionClicked_ != null;
    }
}
