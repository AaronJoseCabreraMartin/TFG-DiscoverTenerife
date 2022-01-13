using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Esta clase almacenará la información relativa a la zona geográfica de la tierra en la que se encuentre
ubicada la app, en el caso del prototipo Tenerife. Almacena datos como:
    los nombres de estas zonas.
    qué tipos de puntos de interés existen.
*/
public class mapRulesHandler
{
    private static List<string> zoneNames_ = new List<string> { "North", "West", "Center", "East", "South"};

    public static List<string> getZoneNames(){
        return mapRulesHandler.zoneNames_;
    }
    
    /*
    Cuidado, en optionsController hay una dependencia con los tipos de los sitios
    porque se muestran en pantalla, esto no se actualizaría automáticamente habría
    que cambiarlo manualmente.
    */
    private static List<string> typesOfSites_ = new List<string> { "beaches", "hikingRoutes", "naturalParks", "naturalPools", "viewpoints"};

    public static List<string> getTypesOfSites(){
        return mapRulesHandler.typesOfSites_;
    }
    
}
