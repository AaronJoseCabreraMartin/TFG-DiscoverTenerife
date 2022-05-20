using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Esta clase almacenará la información relativa a la zona geográfica de la tierra en la que se encuentre
ubicada la app, en el caso del prototipo Tenerife. Almacena datos como:
    los nombres de estas zonas.
    qué tipos de puntos de interés existen.
*/
/**
  * @brief This class stores the geographic zone releated values, like which are the zones of the map.
  *
  * This class shouldn't be initialized, all its properties and methods are static.
  */
public class mapRulesHandler
{
    /**
      * @brief List<string> that contains the name of each of the zones that is divided the map of the app.
      */
    private static List<string> zoneNames_ = new List<string> { "North", "West", "Center", "East", "South"};

    /**
      * @return List<string> that contains the name of each of the zones of the map.
      * @brief getter of the zoneNames_ static property.
      */
    public static List<string> getZoneNames(){
        return mapRulesHandler.zoneNames_;
    }
    
    /*
    Cuidado, en optionsController hay una dependencia con los tipos de los sitios
    porque se muestran en pantalla, esto no se actualizaría automáticamente habría
    que cambiarlo manualmente.
    */
    /**
      * @brief List<string> that contains the valid types of the interesting points.
      *
      * You have to be aware that changing this attribute wont change the options menu, you will
      * have to change the option menu manually.
      */
    private static List<string> typesOfSites_ = new List<string> { "beaches", "hikingRoutes", "naturalParks", "naturalPools", "viewpoints"};

    /**
      * @return List<string> that contains the valid type of the interesting points.
      * @brief getter of the typeOfSites_ static property.
      */
    public static List<string> getTypesOfSites(){
        return mapRulesHandler.typesOfSites_;
    }

    /**
      * @brief double stores the earth radious
      */
    public static double earthRadious = 6377.830272;
    
    /**
      * @brief double stores the conversion of kms to milles
      */
    public static double fromKMtoMilles = 0.621371;

    /**
      * @param double latitude of the point
      * @param double longitude of the point
      * @return string it returns the zone of the given point. If it doesnt
      * fit on any of the defined zones, it returns "Can't Find Zone of given point"
      */
    public static string getZoneOf(double latitude, double longitude){
        //WTF este metodo deberia estar en maprulesHandler!!!!
        if(latitude <= 28.60634 && latitude >= 28.40631 &&
            longitude <= -16.11673 && longitude >= -16.93788){
            return "North";
        }
            
        if(latitude < 28.40631 && latitude >= 28.147504 &&
            longitude <= -16.67719 && longitude > -16.93788 ){
            return "West";
        }

        if(latitude < 28.40631 && latitude > 28.147504 &&
            longitude <= -16.53193 && longitude > -16.67719 ){
            return "Center";
        }

        if(latitude < 28.40631 && latitude > 28.147504 &&
            longitude < -16.11673 && longitude > -16.53193 ){
            return "East";
        }

        if(latitude < 28.147504 && latitude >= 27.99321 &&
            longitude < -16.11673 && longitude > -16.93788 ){
            return "South";
        }

        Debug.Log($"{latitude}, {longitude} no esta en ninguno");
        return $"Can't Find Zone of: {latitude}, {longitude}";
    }
}