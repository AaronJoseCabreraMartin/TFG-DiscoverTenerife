using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that represents each one of the elements of the rankingPanel. 
  */
public class rankingPlayer : MonoBehaviour {
    /**
      * @brief GameObject that contains the text where the represented player
      * display name will be shown.
      */
    [SerializeField] private GameObject name_;

    /**
      * @brief GameObject that contains the text where the represented player
      * score will be shown.
      */
    [SerializeField] private GameObject score_;

    /**
      * @brief GameObject that contains the text where the represented player
      * ranking position will be shown.
      */
    [SerializeField] private GameObject top_;

    /**
      * @brief GameObject that contains the text where the represented player
      * range name will be shown.
      */
    [SerializeField] private GameObject range_;

    /**
      * @brief GameObject that contains the image of the represented player's
      * range.
      */
    [SerializeField] private GameObject rangeImage_;

    /**
      * @param RankingPlayerData with all the information that will be shown
      * @brief This method changes the text of each of the text properties
      * calling the getters of the RankingPlayerData class.
      */
    public void setData(RankingPlayerData dataToShow){
        name_.GetComponent<Text>().text = dataToShow.getName()+ " ";
        score_.GetComponent<Text>().text = "Score: " + dataToShow.getScore()+ " ";
        top_.GetComponent<Text>().text = "Top: " + dataToShow.getTop()+ " ";
        range_.GetComponent<Text>().text = "Range: \n" + dataToShow.getRange() + " ";
        if(IconHandler.iconHandlerInstance_ != null){
          rangeImage_.GetComponent<Image>().sprite = IconHandler.iconHandlerInstance_.getSpriteOfRange(dataToShow.getRange());
        }
    }
}
