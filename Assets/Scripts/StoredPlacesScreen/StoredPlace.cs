using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that stores the data of one of the stored places.
  */
public class StoredPlace 
{
    /**
      * @brief true if any of the stored places has changes to update.
      */
    public static bool changesToUpdate_ = true;

    /**
      * @brief string that stores the name of the stored place.
      */
    private string name_;

    /**
      * @brief string that stores the address of the stored place.
      */
    private string address_;

    /**
      * @brief float that stores the latitude of the stored place.
      */
    private float latitude_;

    /**
      * @brief float that stores the longitude of the stored place.
      */
    private float longitude_;

    //private string imageLink_;
    //private Image image_;
    
    /**
      * @brief int that stores the number of new visits to this stored
      * place.
      */
    private int newVisitsForThisPlace_;
    
    /**
      * @brief string that stores the zone that the stored place belongs.
      */
    private string zone_;

    /**
      * @brief long that contains the timestamp of the last visit.
      */
    private long lastVisitTimestamp_;

    /**
      * @brief Boolean value, its true when the stored place was already visited.
      */
    private bool visited_;

    /**
      * @param Dictionary with the string conversion of the data of the 
      * stored place.
      * @brief Constructor, it initialices all the properties of the class.
      * It expects a dictionary with the following entries: name_, address_,
      * latitude_, longitude_, zone_, lastVisitTimestamp_, newVisitsForThisPlace_
      * and visited_.
      */
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

    /**
      * @return string that contains the property name_'s value.
      * @brief getter of the name_ property. 
      */
    public string getName(){
        return name_;
    }

    /**
      * @return string that contains the property address_'s value.
      * @brief getter of the address_ property. 
      */
    public string getAddress(){
        return address_;
    }

    /**
      * @return double that contains the property latitude_'s value.
      * @brief getter of the latitude_ property. 
      */
    public double getLatitude(){
        return (double) latitude_;
    }

    /**
      * @return double that contains the property longitude_'s value.
      * @brief getter of the longitude_ property. 
      */
    public double getLongitude(){
        return (double) longitude_;
    }

    /**
      * @return bool that contains the property visited_'s value.
      * @brief getter of the visited_ property. 
      */
    public bool visited(){
        return visited_;
    }

    /**
      * @return long that contains the property lastVisitTimestamp_'s value.
      * @brief getter of the lastVisitTimestamp_ property. 
      */
    public long lastVisitTimestamp(){
        return lastVisitTimestamp_;
    }

    /**
      * @return string that contains the property zone_'s value.
      * @brief getter of the zone_ property. 
      */
    public string getZone(){
        return zone_;
    }

    /**
      * @brief Register a new visit to the represented stored place.
      * It uses the current timestamp. It changes the visited_ property to true.
      * It also set the changesToUpdtate_ static property to true.
      */
    public void oneMoreVisit(){
        visited_ = true;
        newVisitsForThisPlace_++;
        lastVisitTimestamp_ = DateTime.Now.Ticks;
        StoredPlace.changesToUpdate_ = true;
    }

    /**
      * @return int that contains the property newVisitsForThisPlace_'s value.
      * @brief getter of the newVisitsForThisPlace_ property. 
      */
    public int newVisitsForThisPlace(){
        return newVisitsForThisPlace_;
    }

    /**
      * @param Place place to store.
      * @return true if the given place was stored, false in other case.
      * @brief This method stores the properties of the stored place 
      * on the current device. If the user cant save more places it returns
      * false, it returns true in other case. The maximum quantity of places
      * that the user can store is defined on the gameRules class.
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
        PlayerPrefs.SetString(index+"lastVisitTimestamp_",firebaseHandler.firebaseHandlerInstance_.currentUser_.getTimestampByName(place.getName()).ToString());
        
        PlayerPrefs.SetInt(index+"newVisitsForThisPlace_",0);
        PlayerPrefs.SetInt(index+"visited_", 
            firebaseHandler.firebaseHandlerInstance_.hasUserVisitPlaceByName(place.getName()) ? 1 : 0);
        return true;
    }

    
    /**
      * @param int index of the stored place you want to obtain. 
      * @return StoredPlace that is on the index-th position, It can be null.
      * @brief It checks if the current device has stored the asked place, if that
      * is not the case, it returns null, if that is the case it returns
      * a StoredPlace object with the stored information on the given index.
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

    /**
      * @param int position that you want to check if there is a stored place.
      * @return bool true if exists a place stored on that position, false in other case.
      * @brief This method return true if exists a place stored on the given position, 
      * it returns false in other case.
      */
    public static bool thereIsAPlaceStoredIn(int index){
      // te dice si existe o no un lugar guardado en la posicion dada
      return PlayerPrefs.HasKey(index+"place");
    }

    /**
      * @param string name of the place that you want to check if its stored.
      * @return bool true if there is a place stored with the given name.
      * @brief This method returns true if there is a place stored that has
      * the given name, false in other case.
      */
    public static bool isPlaceStoredByName(string name){
      // devuelve si un lugar está guardado ya
      int index = 0;
      while(index < gameRules.getMaxPlacesStored() && PlayerPrefs.HasKey(index+"place")){
          if(PlayerPrefs.GetString(index+"name_") == name){
              return true;
          }
          index++;
      }
      return false;
    }

    /**
      * @return true if there is one or more places stored, false in other case.
      * @brief This method returns false if you dont have any place stored, it
      * returns true in other case.
      */
    public static bool thereIsAnyPlaceStored(){
      // te dice si existe o no al menos un lugar guardado
      for(int index = 0; index < gameRules.getMaxPlacesStored(); index++){
          if(PlayerPrefs.HasKey(index+"place")){
              return true;
          }
      }
      return false;
    }

    /**
      * @return true if the current user can store another place.
      * @brief This method returns true if the current user can store one or more places,
      * it returns false if the current user cant store more places.
      */
    public static bool thereIsSpaceForOtherPlaceStored(){
        // te dice si queda hueco para guardar otro
        for(int index = 0; index < gameRules.getMaxPlacesStored(); index++){
            if(!PlayerPrefs.HasKey(index+"place")){
                return true;
            }
        }
        return false;
    }

    /**
      * @brief This method delete all the stored information on the current device.
      * It also sets the static property changesToUpdate_ to false.
      */
    public static void eraseStoredData(){
      //borra TODA la info
      PlayerPrefs.DeleteAll();
      StoredPlace.changesToUpdate_ = false;
    }

    /**
      * @param int position of the place that you want to erase.
      * @brief This method deletes all the information of the place
      * that is on the given index. If there isnt any information on that position
      * it wont do nothing.
      */
    public static void eraseStoredDataOf(int index){
    //borra la info de un lugar concreto
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

    /**
      * @param StoredPlace that you want to save the changes.
      * @brief This method saves on the current device the changes of
      * the given stored place. If the given place isnt stored, it will
      * show a message on the console.
      */
    public static void saveStoredPlace(StoredPlace storedPlace){
        if(StoredPlace.isPlaceStoredByName(storedPlace.getName())){
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
        }else{
          Debug.Log("Llamando a saveStoredPlace cuando no "+ storedPlace.getName() + " no esta guardado");
        }
    }

    /**
      * @brief Calls the PlayerPrefs's Save method in order to
      * write on the current device all the stored information. 
      */
    public static void saveAll(){
        PlayerPrefs.Save();
    }

    /**
      * @brief This method calls the userVisitedPlaceByName with each of the 
      * stored places that have been visited and they didnt uploaded the changes.
      */
    public static void UpdateChanges(){//WTF el nombre deberia ser UploadChanges.
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
