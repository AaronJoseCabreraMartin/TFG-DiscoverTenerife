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
    
}
