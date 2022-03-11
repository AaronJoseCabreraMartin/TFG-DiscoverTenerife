using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the panel that shows all the players that
  * allow appeard on the ranking, it also controls the image that will be
  * shown when the user doesnt have internet connection and the image that
  * will be shown when the ranking data is downloading right now. It inherites
  * from adaptableSizePanel abstract class.
  */
public class rankingPanel : adaptableSizePanel {
  /**
    * @brief GameObject with the image that will be shown when the downloading
    * ranking data process is working.
    */
  [SerializeField] GameObject downloadingDataImage_;
  
  /**
    * This method is called once per frame, it checks if the panel has al ready filled and if
    * the userDataIsReady method of firebaseHandler class returns true it calls the fillPanel method.
    * If the lastCount_ property isnt equal to the current items count it calls the adjustPanelSize
    * method. It also shows the noPlacesImage_ gameObject if the items list is empty, and it shows
    * the downloadingDataImage_ if the isDownloadingRankingDataNow method of firebaseHandler class
    * returns true, it will hide it in other case.
    */
  void Update(){
    if(!panelFilled_ && firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
      fillPanel();
    }
    if(lastCount_ != items_.Count){
      adjustPanelSize();
    }
    noItemsImage_.SetActive(items_.Count == 0);
    downloadingDataImage_.SetActive(firebaseHandler.firebaseHandlerInstance_.isDownloadingRankingDataNow());
  }

  /**
    * @brief This method is called to instanciate the panel. It creates a prefab for each player on the ranking
    * that allowed appear on it. It changes the panelFilled_ property to true.
    */
  protected override void fillPanel(){
    if(firebaseHandler.firebaseHandlerInstance_.isRankingDataComplete()){
      firebaseHandler.firebaseHandlerInstance_.sortRankingDataList();
      foreach(RankingPlayerData userData in firebaseHandler.firebaseHandlerInstance_.getRankingPlayerData()){
        GameObject newRankingPlayer = Instantiate(prefab_, new Vector3(0, 0, 0), Quaternion.identity);
        newRankingPlayer.GetComponent<rankingPlayer>().setData(userData);
        newRankingPlayer.transform.SetParent(this.transform);
        items_.Add(newRankingPlayer);
      }
      //si la lista no se habia descargado el panel sigue sin haberse llenado
      panelFilled_ = true;
    }
  }
}
