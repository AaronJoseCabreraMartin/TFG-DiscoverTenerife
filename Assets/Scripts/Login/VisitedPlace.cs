using UnityEngine;
using System;

public class VisitedPlace{
    public string type_;
    public int id_;
    public int timesVisited_;
    public VisitedPlace(string type, int id, int timesVisited){
        type_ = type;
        id_ = id;
        timesVisited_ = timesVisited;
    }

    public string ToJson(){
        string conversion = "{";
        conversion += $"\"type_\" : \"{type_}\",";
        conversion += $"\"id_\" : {id_},";
        conversion += $"\"timesVisited_\" : {timesVisited_} }}";//}} porque tienes que escapar uno por ser $""
        return conversion;
    }
}