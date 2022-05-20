using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This class controls the story of the ranges of the current user.
  * This panel shows all the times the current user has reached a new range and the time when
  * that happen.
  */
public class RangeStoryPanel : adaptableSizePanel
{
    /**
      * @brief GameObject that contains the text where the range name will be displayed
      */
    [SerializeField] private GameObject range_;

    /**
      * @brief GameObject that contains the icon of the range
      */
    [SerializeField] private GameObject rangeImage_;

    /**
      * @brief GameObject that contains the text where the score will be displayed
      */
    [SerializeField] private GameObject score_;

    /**
      * @brief This method is called to instanciate the panel. It creates a prefab for each time 
      * the current user has leveled up his range. It changes the panelFilled_ property to true.
      */
    protected override void fillPanel(){
      for(int i = 0; i < firebaseHandler.firebaseHandlerInstance_.currentUser_.getRangeStoryCount(); i++){
        GameObject newRangeStory = Instantiate(prefab_, new Vector3(0, 0, 0), Quaternion.identity);
        newRangeStory.GetComponent<RankStory>().setData(firebaseHandler.firebaseHandlerInstance_.currentUser_.getRangeStory(i));
        newRangeStory.transform.SetParent(this.transform);
        items_.Add(newRangeStory);
      }
      score_.GetComponent<Text>().text = "Score: " + firebaseHandler.firebaseHandlerInstance_.currentUser_.getScore().ToString();
      string rangeName = gameRules.calculateRange(firebaseHandler.firebaseHandlerInstance_.currentUser_.getScore());
      range_.GetComponent<Text>().text = rangeName;
      if(IconHandler.iconHandlerInstance_ != null){
        rangeImage_.GetComponent<Image>().sprite = IconHandler.iconHandlerInstance_.getSpriteOfRange(rangeName);
      }
      panelFilled_ = true;
    }
}
