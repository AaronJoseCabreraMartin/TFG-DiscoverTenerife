using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Esta clase almacenará la información relativa las reglas del juego como por ejemplo:
    distancia máxima a un punto de interés como para contarse como visitado
    cooldown de visitas
    número máximo de sitios guardados en el pocket para visitas offline
*/

/**
  * @brief This class store as static properties the parameters that affect the game rules.
  * For example it stores the maximum distance to a point to be considered as a valid visit.
  */
public class gameRules{

    /**
      * @brief double this static property stores the maximum distance between the player and an
      * interesting point to be considerer as a visit. It is represented on kilometers, its default
      * is 50m which is 0.05km
      */
    static private double maxDistance_ = 0.05;

    /**
      * @return double that contains the maximum distance in kilometers to decide if a player is too
      * far away from an interesting point to visit that point.
      * @brief getter of the maxDistance_ static property.
      */
    static public double getMaxDistance(){
        return gameRules.maxDistance_;
    }

    /**
      * @brief contains the time in minutes that a player has to wait to visit again a place. Its
      * default value is 60 minutes.
      */
    static private double minutesOfCooldown_ = 60;
    
    /**
      * @return double that contains the time in minutes that a player has to wait to visit again a
      * place.
      * @brief getter of the minutesOfCooldown_ static property.
      */
    static public double getMinutesOfCooldown(){
        return gameRules.minutesOfCooldown_;
    }
    
    // número máximo de sitios almacenados en el pocket para visitas offline
    /**
      * @brief contains the maximum number of places that you can store in the pocket for 
      * internetless visits. Its default value is 5.
      */
    static private int maxPlacesStored_ = 5;

    /**
      * @return the maximum number of places that you can store in the pocket for 
      * internetless visits.
      * @brief getter of the maxPlacesStored_ static property.
      */
    static public int getMaxPlacesStored(){
        return gameRules.maxPlacesStored_;
    }
}
