using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject placeOnStoryPrefab_;

    private List<GameObject> placeOnStory_;
    private bool panelFilled_;
    
    void Awake()
    {
        placeOnStory_ = new List<GameObject>();
        panelFilled_ = false;
        if(firebaseHandler.firebaseHandlerInstance_ != null && firebaseHandler.firebaseHandlerInstance_.userDataIsReady() 
            && firebaseHandler.firebaseHandlerInstance_.actualUser_.friendDataIsComplete()){
                fillPanel();
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_ != null && firebaseHandler.firebaseHandlerInstance_.userDataIsReady() 
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
/*
Tambien hay que hacer el cartel de "no has visitado ningun sitio aun!"
*/
    private void adjustPanelSize(){
        if(placeOnStory_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height + 300*(placeOnStory_.Count-4);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
    }

    public int getStoryCount(){
        return placeOnStory_.Count;
    }
}
