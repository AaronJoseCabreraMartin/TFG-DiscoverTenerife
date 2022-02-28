using System;
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
    
    /**
      * @brief contains the maximum number of places that you can store in the pocket for 
      * internetless visits. Its default value is 5.
      */
    static private int maxPlacesStored_ = 5;
    // número máximo de sitios almacenados en el pocket para visitas offline

    /**
      * @return the maximum number of places that you can store in the pocket for 
      * internetless visits.
      * @brief getter of the maxPlacesStored_ static property.
      */
    static public int getMaxPlacesStored(){
        return gameRules.maxPlacesStored_;
    }

    /**
      * @brief contains the time that challenges stay active until 
      * they expire. Its default value is 7 days.
      */
    static private long expiryTimeForChallenges_ = 6048000000000;
    //                                             7d * 24h * 60m * 60s * 10000000

    /**
      * @return the time that challenges stay active until they expire.
      * @brief getter of the expiryTimeForChallenges_ static property.
      */
    static public long getExpiryTimeForChallenges(){
        return gameRules.expiryTimeForChallenges_;
    }

    /**
      * @param long with the timestamp when the challenge was started.
      * @return bool true if the challenge that started on the given
      * timestmap has already expired, false in other case.
      * @brief This method returns true if the current timestamp is bigger
      * than the given timestamp plus the expiryTimeForChallenges static
      * property because if its bigger its means that the expiry time is
      * on the past.
      */
    static public bool challengeHasExpired(long startTimestamp){
      // la suma da el momento exacto en el caduca, si el momento actual es 
      // mayor o igual a ese valor, está caducado.
      return DateTime.Now.Ticks >= startTimestamp + gameRules.expiryTimeForChallenges_;
    }

    /**
      * @param long timestamp of the begginging of the challenge.
      * @param long timestamp of the completation of the challenge.
      * @param double distance in kms to the user.
      * @return int score of the completed challenge.
      * @brief This method calculates the score of a completed challenge using
      * the timestamp of the challenge's begginging, the timestamp of the
      * challenge's completation and the distance in kilometers to the user's base.
      */
    static public int calculateChallengeScore(long startTimestamp, long completationTimestamp, double distanceToUserBase){
      double timeScore = 2.0 - ((double)completationTimestamp)/((double)(startTimestamp+gameRules.expiryTimeForChallenges_));
      //timeScore is always between 1 and 2
      return (int)(timeScore*1.5d*distanceToUserBase);
    }

    /**
      * @brief proportion of points that challenger should get when
      * other user completes his challenges.
      */
    static private double scoreToTheChallenger_ = 0.1d;

    /**
      * @return double with the scoreToTheChallenger_ static
      * property value.
      * @brief getter of the scoreToTheChallenger_ property.
      */
    static public double getScoreToTheChallenger(){
      return gameRules.scoreToTheChallenger_;
    }
}
