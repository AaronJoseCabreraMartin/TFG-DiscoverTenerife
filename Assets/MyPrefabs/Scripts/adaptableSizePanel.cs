using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class adaptableSizePanel : MonoBehaviour
{
    /**
      * @brief GameObject that contains the prefab that represent an element of the panel. 
      */
    [SerializeField] protected GameObject prefab_;

    /**
      * @brief GameObject that contains the image that will be shown when the panel is empty. 
      */
    [SerializeField] protected GameObject noItemsImage_;

    /**
      * @brief if its true means that the panel was already filled, false in otherwhise.
      */
    protected bool panelFilled_;

    /**
      * @brief List of the prefabs that are instanciated.
      */
    protected List<GameObject> items_;

    /**
      * @brief it stores the last quantity of items which the panel has adapted its height.
      */
    protected int lastCount_;

    /**
      * @brief initial height of the panel.
      */
    protected float initialHeight_;
    
    /**
      * This method is called before the first frame, it instanciate the items, panelFilled_
      * and lastCount_ properties. And if the userDataIsReady method of firebaseHandler class returns true
      * it calls the fillPanel method.
      */
    void Awake(){
        items_ = new List<GameObject>();
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
      * If the lastCount_ property isnt equal to the current items count it calls the adjustPanelSize
      * method. It also shows the noPlacesImage_ gameObject if the items list is empty.
      */
    void Update(){
        if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            fillPanel();
        }
        if(lastCount_ != items_.Count){
            adjustPanelSize();
        }
        noItemsImage_.SetActive(items_.Count == 0);
    }

    /**
      * @brief This method is called to instanciate the panel and add the prefabs as panels elements.
      * It is an abstract method so, every class that inherit from this class must have its own
      * fillPanel method.
      */
    protected abstract void fillPanel();

    /**
      * This method should be called each time that the panel adds or quits an element. This method
      * adjust the height of the panel to keep the apparence when the number of elements changes.
      */
    protected void adjustPanelSize(){
        float baseHeight = (float)(gameObject.transform.GetComponent<RectTransform>().rect.height * 1.25);
        if(items_.Count > 4){
            float newHeight = GetComponent<RectTransform>().rect.height;
            newHeight += lastCount_ < items_.Count ? baseHeight*(items_.Count-4) : -baseHeight*(lastCount_-items_.Count);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }else{
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialHeight_);
        }
        lastCount_ = items_.Count;
    }

    /**
      * @param GameObject that contains the prefab element that has been deleted.
      * @brief tries to remove the given gameobject of the items property. If the 
      * gameobject isnt on the list, it will raise an index out of range exception.
      */
    public void elementDeleted(GameObject elementDeleted){
        items_.RemoveAt(items_.FindIndex(element => element == elementDeleted));
    }
}
