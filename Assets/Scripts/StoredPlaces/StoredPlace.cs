using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredPlace 
{
    private static int maxStoredPlaces_ = 5;

    private bool stored_;

    private string name_;
    private string address_;
    private float latitude_;
    private float longitude_;
    //private string imageLink_;
    //private Image image_;
    private int newVisitsForThisPlace_;
    private string zone_;
    public long lastVisitTimestamp_;

    StoredPlace(Dictionary<string,string> data){
        name_ = data["name"];
        address_ = data["address"];
        latitude_ = Convert.ToSingle(data["latitude"]);//ToFloat
        longitude_ = Convert.ToSingle(data["longitude"]);
        zone_ = data["zone"];
        //lastVisitTimestamp_ = Int64.Parse(data["lastVisitTimestamp"]);
        newVisitsForThisPlace_ = Int32.Parse(data["newVisitsForThisPlace_"]);
    }

    /*
    Para almacenar por primera vez el sitio
    */
    public static bool storePlace(Place place){
        int index = 0;
        /*
        En principio se guardaran 5 sitios como maximo, estos pueden estar o no guardados
        debo guardar en el primero que este libre, si los sitios están llenos mensaje de error
        y que el usuario elimine un sitio guardado
        */
        while(index < StoredPlace.maxStoredPlaces_ && PlayerPrefs.HasKey(index+"place")){
            index++;
        }
        /*
        ya tiene el numero maximo de sitios guardados, error
        */
        if( index == StoredPlace.maxStoredPlaces_){
            return false;
        }
        PlayerPrefs.SetString(index+"place", index+"place");
        PlayerPrefs.SetString(index+"name_",place.getName());
        PlayerPrefs.SetString(index+"address_",place.getAddress());
        PlayerPrefs.SetFloat(index+"latitude_",(float)place.getLatitude());
        PlayerPrefs.SetFloat(index+"longitude_",(float)place.getLongitude());
        PlayerPrefs.SetString(index+"zone_",place.getZone());
        /*
        Me hace falta el timestamp para controlar, con la hora que no se visite un sitio mas de la cuenta
        el timestamp lo tiene el visitedPlace -.-"
        */
        //PlayerPrefs.SetString(index+"lastVisitTimestamp_",);
        
        PlayerPrefs.SetInt(index+"newVisitsForThisPlace_",0);
        return true;
    }

    
    /*
    La idea es llamar a este metodo para cargar los sitios guardados
    */
    public static StoredPlace loadStoredPlace(int index){
        if(!PlayerPrefs.HasKey(index+"place")){
            Debug.Log($"Error en loadStoredPlace no existe index {index}");
            return null;
        }
        Dictionary<string,string> dictionaryVersion = new Dictionary<string,string>();
        dictionaryVersion["name_"] =  PlayerPrefs.GetString(index+"name_");
        dictionaryVersion["address_"] =  PlayerPrefs.GetString(index+"address_");
        dictionaryVersion["latitude_"] =  PlayerPrefs.GetFloat(index+"latitude_").ToString();
        dictionaryVersion["longitude_"] =  PlayerPrefs.GetFloat(index+"longitude_").ToString();
        dictionaryVersion["zone_"] =  PlayerPrefs.GetString(index+"zone_");
        //dictionaryVersion["lastVisitTimestamp_"] = PlayerPrefs.GetString(index+"lastVisitTimestamp_");
        dictionaryVersion["newVisitsForThisPlace_"] =  PlayerPrefs.GetInt(index+"newVisitsForThisPlace_").ToString();
        return new StoredPlace(dictionaryVersion);
    }

    // te dice si existe o no un lugar guardado en la posicion dada
    public static bool thereIsAPlaceStoredIn(int index){
        return PlayerPrefs.HasKey(index+"place");
    }

    // te dice si existe o no un lugar guardado
    public static bool thereIsAnyPlaceStored(){
        for(int index = 0; index < StoredPlace.maxStoredPlaces_; index++){
            if(PlayerPrefs.HasKey(index+"place")){
                return true;
            }
        }
        return false;
    }

    //borra TODA la info
    public static void eraseStoredData(){
        PlayerPrefs.DeleteAll();
    }

    //borra la info de un lugar concreto
    public static void eraseStoredDataOf(int index){
        if(PlayerPrefs.HasKey(index+"place")){
            PlayerPrefs.DeleteKey(index+"place");
            PlayerPrefs.DeleteKey(index+"name_");
            PlayerPrefs.DeleteKey(index+"address_");
            PlayerPrefs.DeleteKey(index+"latitude_");
            PlayerPrefs.DeleteKey(index+"longitude_");
            PlayerPrefs.DeleteKey(index+"zone_");
            //PlayerPrefs.DeleteKey(index+"lastVisitTimestamp_");
            PlayerPrefs.DeleteKey(index+"newVisitsForThisPlace_");
        }
    }
}
