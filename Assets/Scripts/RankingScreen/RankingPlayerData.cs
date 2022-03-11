using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that stores the information of each player that is 
  * shown on the ranking.
  */
public class RankingPlayerData {
    /**
      * @brief string that contains the display name of the represented user.
      */
    private string name_;

    /**
      * @brief string that contains the user id of the represented user.
      */
    private string uid_;

    /**
      * @brief string that contains the score of the represented user.
      */
    private int score_;

    /**
      * @brief string that contains the position of the represented user on
      * the ranking.
      */
    private int top_;

    /**
      * @brief string that contains the range name of the represented user.
      */
    private string range_;

    /**
      * @param string with the represented user's display name.
      * @param int with the represented user's score.
      * @param string with the represented user's id.
      * @param string (optional) with the represented user's range.
      * @param int (optional) with the represented user's position on the player ranking.
      * @brief The constructor of the class it sets each property as the correspondent given value.
      * If the range param is the empty string this method calls the static method calculateRange 
      * of the gameRules class to set the correspondant range to the range_ property.
      */
    public RankingPlayerData(string name, int score, string uid, string range = "", int top = 0){
        name_ = name;
        uid_ = uid;
        score_ = score;
        range_ = range;
        top_ = top;

        if(range_ == ""){
            range_ = gameRules.calculateRange(score_);
        }
    }

    /**
      * @return string with the represented player name.
      * @brief Getter of the name_ property.
      */
    public string getName(){
        return name_;
    }

    /**
      * @return string with the represented player id.
      * @brief Getter of the uid_ property.
      */
    public string getUid(){
        return uid_;
    }

    /**
      * @return int with the represented player score.
      * @brief Getter of the score_ property.
      */
    public int getScore(){
        return score_;
    }

    /**
      * @return int with the represented player ranking position.
      * @brief Getter of the top_ property.
      */
    public int getTop(){
        return top_;
    }

    /**
      * @param int with the new ranking position of the represented player.
      * @brief Setter of the top_ property.
      */
    public void setTop(int top){
        top_ = top;
    }

    /**
      * @return string with the represented player range.
      * @brief Getter of the range_ property.
      */
    public string getRange(){
        return range_;
    }
}
