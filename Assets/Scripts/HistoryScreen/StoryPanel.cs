using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject placeOnStoryPrefab_;

    private List<GameObject> placeOnStory_;
    private bool panelFilled_;
    private bool setDataTime_;
    
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

    // Update is called once per frame
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

    private void adjustPanelSize(){
        if(placeOnStory_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height + 300*(placeOnStory_.Count-4);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
    }

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

    public bool hidePanel(){
        return placeOnStory_.Count == 0 || setDataTime_ || optionsController.lastOptionClicked_ != null;
    }
}
