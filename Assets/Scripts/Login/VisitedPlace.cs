using UnityEngine;
using System;
using System.Globalization;

public class VisitedPlace{
    public string type_ ;
    public int id_ ;
    public int timesVisited_ ;
    public long lastVisitTimestamp_;

    public VisitedPlace(string type, int id, int timesVisited, long lastVisitTimestamp ){
        type_ = type;
        id_ = id;
        timesVisited_ = timesVisited;
        lastVisitTimestamp_ = lastVisitTimestamp;
    }

    public string ToJson(){
        string conversion = "{";
        conversion += $"\"type_\" : \"{type_}\",";
        conversion += $"\"id_\" : \"{id_}\",";
        conversion += $"\"timesVisited_\" : \"{timesVisited_}\",";
        conversion += $"\"lastVisitTimestamp_\" : \"{lastVisitTimestamp_}\"}}";//}} porque tienes que escapar uno por ser $""
        return conversion;
    }

    public void newVisitAt(long timeOfTheVisit){
        timesVisited_++;
        lastVisitTimestamp_ = timeOfTheVisit;
    }
}