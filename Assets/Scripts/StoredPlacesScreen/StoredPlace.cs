using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredPlace 
{
    public static bool changesToUpdate_ = true;

    private string name_;
    private string address_;
    private float latitude_;
    private float longitude_;
    //private string imageLink_;
    //private Image image_;
    private int newVisitsForThisPlace_;
    private string zone_;
    private long lastVisitTimestamp_;
    private bool visited_;

    StoredPlace(Dictionary<string,string> data){
        name_ = data["name_"];
        address_ = data["address_"];
        latitude_ = Convert.ToSingle(data["latitude_"]);//ToFloat
        longitude_ = Convert.ToSingle(data["longitude_"]);
        zone_ = data["zone_"];
        lastVisitTimestamp_ = data["lastVisitTimestamp_"] == "" ? (long)0 : Int64.Parse(data["lastVisitTimestamp_"]);
        
        newVisitsForThisPlace_ = Int32.Parse(data["newVisitsForThisPlace_"]);
        visited_ = data["visited_"] == "1";
    }

    public string getName(){
        return name_;
    }

    public string getAddress(){
        return address_;
    }

    public double getLatitude(){
        return (double) latitude_;
    }

    public double getLongitude(){
        return (double) longitude_;
    }

    public bool visited(){
        return visited_;
    }

    public long lastVisitTimestamp(){
        return lastVisitTimestamp_;
    }

    public string getZone(){
        return zone_;
    }

    public void oneMoreVisit(){
        visited_ = true;
        newVisitsForThisPlace_++;
        lastVisitTimestamp_ = DateTime.Now.Ticks;
        StoredPlace.changesToUpdate_ = true;
    }

    public int newVisitsForThisPlace(){
        return newVisitsForThisPlace_;
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
        while(index < gameRules.getMaxPlacesStored() && PlayerPrefs.HasKey(index+"place")){
            index++;
        }
        /*
        ya tiene el numero maximo de sitios guardados, error
        */
        if( index == gameRules.getMaxPlacesStored()){
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
        PlayerPrefs.SetString(index+"lastVisitTimestamp_",firebaseHandler.firebaseHandlerInstance_.actualUser_.getTimestampByName(place.getName()).ToString());
        
        PlayerPrefs.SetInt(index+"newVisitsForThisPlace_",0);
        PlayerPrefs.SetInt(index+"visited_", 
            firebaseHandler.firebaseHandlerInstance_.hasUserVisitPlaceByName(place.getName()) ? 1 : 0);
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
        dictionaryVersion["lastVisitTimestamp_"] = PlayerPrefs.GetString(index+"lastVisitTimestamp_");
        dictionaryVersion["newVisitsForThisPlace_"] =  PlayerPrefs.GetInt(index+"newVisitsForThisPlace_").ToString();
        dictionaryVersion["visited_"] = PlayerPrefs.GetInt(index+"visited_").ToString();
        /*string toShow = "";
        foreach(var key in dictionaryVersion.Keys){
            toShow += $"{key} -> {dictionaryVersion[key]} ";
        }
        Debug.Log(toShow);*/
        return new StoredPlace(dictionaryVersion);
    }

    // te dice si existe o no un lugar guardado en la posicion dada
    public static bool thereIsAPlaceStoredIn(int index){
        return PlayerPrefs.HasKey(index+"place");
    }

    // devuelve si un lugar está guardado ya
    public static bool isPlaceStoredByName(string name){
        int index = 0;
        while(index < gameRules.getMaxPlacesStored() && PlayerPrefs.HasKey(index+"place")){
            if(PlayerPrefs.GetString(index+"name_") == name){
                return true;
            }
            index++;
        }
        return false;
    }

    // te dice si existe o no un lugar guardado
    public static bool thereIsAnyPlaceStored(){
        for(int index = 0; index < gameRules.getMaxPlacesStored(); index++){
            if(PlayerPrefs.HasKey(index+"place")){
                return true;
            }
        }
        return false;
    }

    // te dice si queda hueco para guardar otro
    public static bool thereIsSpaceForOtherPlaceStored(){
        for(int index = 0; index < gameRules.getMaxPlacesStored(); index++){
            if(!PlayerPrefs.HasKey(index+"place")){
                return true;
            }
        }
        return false;
    }

    //borra TODA la info
    public static void eraseStoredData(){
        PlayerPrefs.DeleteAll();
        StoredPlace.changesToUpdate_ = false;
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
            PlayerPrefs.DeleteKey(index+"lastVisitTimestamp_");
            PlayerPrefs.DeleteKey(index+"newVisitsForThisPlace_");
        }
    }

    public static void saveStoredPlace(StoredPlace storedPlace){
        for(int index = 0; index < gameRules.getMaxPlacesStored(); index++){
            if(PlayerPrefs.HasKey(index+"place") && PlayerPrefs.GetString(index+"name_") == storedPlace.getName()){
                PlayerPrefs.SetString(index+"name_",storedPlace.getName());
                PlayerPrefs.SetString(index+"address_",storedPlace.getAddress());
                PlayerPrefs.SetFloat(index+"latitude_",(float)storedPlace.getLatitude());
                PlayerPrefs.SetFloat(index+"longitude_",(float)storedPlace.getLongitude());
                PlayerPrefs.SetString(index+"zone_",storedPlace.getZone());
                PlayerPrefs.SetInt(index+"newVisitsForThisPlace_",storedPlace.newVisitsForThisPlace());
                PlayerPrefs.SetInt(index+"visited_", storedPlace.visited() ? 1 : 0);
                PlayerPrefs.SetString(index+"lastVisitTimestamp_",storedPlace.lastVisitTimestamp().ToString());
                break;
            }
        }
        StoredPlace.saveAll();
    }

    public static void saveAll(){
        PlayerPrefs.Save();
    }

    public static void UpdateChanges(){
        StoredPlace.changesToUpdate_ = false;
        for(int index = 0; index < gameRules.getMaxPlacesStored(); index++){
            if(PlayerPrefs.HasKey(index+"place") && PlayerPrefs.GetInt(index+"newVisitsForThisPlace_") != 0){
                //string toShow = "";
                for(int i = 0; i < PlayerPrefs.GetInt(index+"newVisitsForThisPlace_"); i++ ){
                //    toShow += "nueva visita para " + PlayerPrefs.GetString(index+"name_") + " ";
                    firebaseHandler.firebaseHandlerInstance_.userVisitedPlaceByName(
                                                            PlayerPrefs.GetString(index+"name_"),
                                                            Int64.Parse(PlayerPrefs.GetString(index+"lastVisitTimestamp_")));
                }
                //Debug.Log(toShow);
                PlayerPrefs.SetInt(index+"newVisitsForThisPlace_",0);
            }
        }
        PlayerPrefs.Save();
    }
}
