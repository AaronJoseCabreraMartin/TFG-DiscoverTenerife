using UnityEngine;
using System;
using System.Globalization;

/**
  * @brief Class that stores the information of a visited place.
  */
public class VisitedPlace{
    // WTF las cosas no deberian ser publicas hay que hacerle getters y setters a todo.

    /**
      * @brief string that contains the type of the represented place.
      */
    public string type_;
    
    /**
      * @brief int that contains the id of the represented place.
      */
    public int id_;

    /**
      * @brief int that contains the times that the current user has visited the represented place.
      */
    public int timesVisited_ ;
    
    /**
      * @brief long that contains the timestamp that the current user last visit to the represented place.
      */
    public long lastVisitTimestamp_;

    /**
      * @param string type of the represented place.
      * @param int id of the represented place.
      * @param int times that the current user has visited the represented place.
      * @param long timestamp of the last visit of the current user to the represented place.
      * @brief Constructor that simply set the given params as attributes of this object.
      */
    public VisitedPlace(string type, int id, int timesVisited, long lastVisitTimestamp ){
        type_ = type;
        id_ = id;
        timesVisited_ = timesVisited;
        lastVisitTimestamp_ = lastVisitTimestamp;
    }

    /**
      * @return string that contains a JSON conversion of the current object.
      * @brief Converts this object in a string JSON formatted.  
      */
    public string ToJson(){
        string conversion = "{";
        conversion += $"\"type_\" : \"{type_}\",";
        conversion += $"\"id_\" : \"{id_}\",";
        conversion += $"\"timesVisited_\" : \"{timesVisited_}\",";
        conversion += $"\"lastVisitTimestamp_\" : \"{lastVisitTimestamp_}\"}}";//}} porque tienes que escapar uno por ser $""
        return conversion;
    }

    /**
      * @param long timestamp of the visit
      * @brief this method register a new visit for the represented place.
      * It adds one new visit and change the lastVisitTimestamp_ to the given timestamp.
      */
    public void newVisitAt(long timeOfTheVisit){
        timesVisited_++;
        lastVisitTimestamp_ = timeOfTheVisit;
    }
}