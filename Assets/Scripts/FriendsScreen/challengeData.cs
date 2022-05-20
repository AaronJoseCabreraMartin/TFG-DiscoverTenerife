using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that stores the information of one
  * challenge.
  */
public class challengeData 
{
    /**
      * @brief Id of the site that the user has to visit to
      * complete the challenge.
      */
    private int placeId_;
    
    /**
      * @brief type of the site that the user has to visit to
      * complete the challenge.
      */
    private string placeType_;

    /**
      * @brief ticks where the challenge was started.
      */
    private long startTimestamp_;

    /**
      * @brief User ID of the user that has sended 
      * this challenge.
      */
    private string challengerId_;

    /**
      * @brief This property stores the value of the rest the other properties as 
      * strings
      */
    private Dictionary<string,string> dictionaryVersion_;

    /**
      * @param Dictionary<string,string> that contains the string version
      * of the callenge's data.
      * @brief The constructor of the class, it initialices all the properties.
      * It expects a dictionary with at least the following entries: placeId_,
      * placeType_, startTimestamp_ and challengerId_.
      */
    public challengeData(Dictionary<string,string> stringInformation){
        placeId_ = Int32.Parse(stringInformation["placeId_"]);
        placeType_ = stringInformation["placeType_"];
        startTimestamp_ = Int64.Parse(stringInformation["startTimestamp_"]);
        challengerId_ = stringInformation["challengerId_"];
        dictionaryVersion_ = stringInformation;
    }

    /**
      * @return int with the id of the place that the user has to visit
      * to complete the challenge.
      * @brief getter of the place's id.
      */
    public int getPlaceId(){
        return placeId_;
    }

    /**
      * @return string with the type of the place that the user has to visit
      * to complete the challenge.
      * @brief getter of the place's type.
      */
    public string getPlaceType(){
        return placeType_;
    }

    /**
      * @return long with the ticks of when the challenge was sended.
      * @brief getter of the timestamp of the challenge start.
      */
    public long getStartTimestamp(){
        return startTimestamp_;
    }

    /**
      * @return string with the id of the user that has sended the
      * challenge.
      * @brief getter of the id the user that has sended the
      * challenge.
      */
    public string getChallengerId(){
        return challengerId_;
    }

    /**
      * @return string with the conversion of this object.
      * @brief This method returns a string conversion, that follows the
      * JSON format, of this object.
      */
    public string ToJson(){
        string conversion = "{";
        conversion += $"\"placeId_\":\"{placeId_}\",";
        conversion += $"\"placeType_\":\"{placeType_}\",";
        conversion += $"\"startTimestamp_\":\"{startTimestamp_}\",";
        conversion += $"\"challengerId_\":\"{challengerId_}\"";
        conversion += "}";
        return conversion;
    }

    /**
      * @brief Getter of the dictionaryVersion property.
      */
    public Dictionary<string,string> toDictionaryVersion(){
      return dictionaryVersion_;
    }
}
