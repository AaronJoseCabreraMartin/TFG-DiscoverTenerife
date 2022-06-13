using System;
using System.Linq;
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
      // tiene que ser 2 - [0,1] por que así cuanto menos tiempo haya usado, más cerca de 0 da la división y entonces, más cerca de 2 está
      //double timeScore = 2.0 - ((double)(completationTimestamp-startTimestamp))/((double)(gameRules.expiryTimeForChallenges_-startTimestamp));
      double timeScore = 1.0 + ((double)(gameRules.expiryTimeForChallenges_-completationTimestamp))/((double)(gameRules.expiryTimeForChallenges_-startTimestamp));
      //Debug.Log($"startTimestamp = {startTimestamp} completationTimestamp = {completationTimestamp} distanceToUserBase = {distanceToUserBase} timeScore = {timeScore}");
      //Debug.Log($"total de puntuación por completar el reto: {timeScore*distanceToUserBase}");
      //timeScore is always between 1 and 2
      //el maximo de puntuacion será estrictamente dependiente de la distancia a la base...
      //como minimo la score es igual a la distancia de la base y como maximo sera el doble de esa distancia.
      return (int)(timeScore*distanceToUserBase);
    }

    /**
      * @brief proportion of points that challenger should get when
      * other user completes his challenges.
      */
    static private double scoreToTheChallenger_ = 0.1d;

    /**
      * @param double that is the score that earned the challenged person.
      * @return double with scored that the challenger should earn.
      * @brief this method returns the score that the challenger should
      * obtain knowing how much score the challenged person earned.
      */
    static public double getScoreToTheChallenger(double scoreOfChallenged){
      return gameRules.scoreToTheChallenger_*scoreOfChallenged;
    }

    /**
      * @brief points that the user will receive as minimum for visiting a new place.
      */
    static private double baseBonusNewVisit_ = 15.0d;
    
    /**
      * @brief points that the user will receive as minimum for visiting a place that
      * he has already visited.
      */
    static private double baseBonusVisit_ = 10.0d;

    /**
      * @param int number of visits of the visited place.
      * @param int number of visits of the most visited place.
      * @param bool true if the user never has visited that place. Its default value
      * is false.
      * @return double score that should earn the current user.
      * @brief This method calculates the score that the current user has to earn when
      * he visits a place. For calculating the score it needs the visits of that place,
      * the number of visits of the most visited place and it also needs if the user
      * has already visit that place or not. The score will be higher if the user visits
      * a new place than if the user visits a place that he has already visited it. The
      * score will be proportional to the number of visits that place has compared to
      * the most visited place.
      */
    static public double getScoreForVisitingAPlace(int placeVisits, int maxVisits, bool newVisit = false){
      double scoreBonus = 2.0d - ((double)placeVisits+1.0d)/((double)(maxVisits)+1.0d);
      //scoreBonus esta entre (1, 2] y será más alto cuantas menos visitas haya recibido el lugar comparándolo con el que más visitas ha recibido.
      //la score recibida esta entre 10 y 20 si es un sitio ya visitado y entre 15 y 30 si es un sitio nuevo
      //Debug.Log($"placeVisits = {placeVisits} maxVisits = {maxVisits} newVisit = {newVisit}");
      //Debug.Log($"scoreBonus = {scoreBonus} formula = "+scoreBonus * (newVisit ? gameRules.baseBonusNewVisit_ : gameRules.baseBonusVisit_));
      return scoreBonus * (newVisit ? gameRules.baseBonusNewVisit_ : gameRules.baseBonusVisit_);
    }

    /**
      * @brief Dictionary that contains the names of the ranges and the minimum
      * score that a user have to has to be considered of that range. They have
      * to be sorted as higher ranger first.
      */
    static private Dictionary<string,int> ranges_ = new Dictionary<string,int>(){
      //Cuanto mas alto el rango mayor diferencia de puntos.
      {"Mencey",       50000},//50k
      {"Achimencey",   10000},//10k
      {"Guañameñe",     3000},//3k
      {"Tagorero",      1500},//1.5k
      {"Sigoñes",        750},//0.75k
      {"Cichiciquitzos", 300},//0.3k
      {"Achicaxna",      100},//0.1k
      {"Achicaxnais",      0},//0
    };

    /**
      * @param int with the user's score.
      * @return string with the current user range name.
      * @brief This method returns the name of the range that the user should
      * be. To calculate the user range it uses the user's score. If the score
      * is a negative number it will return the empty string.
      */
    static public string calculateRange(int score){
      //Como estan ordenados de mayor a menor desde que encuentres uno que la puntuacion dada es menor que 
      //la del rango, ese es el rango del usuario.
      foreach(var rangeName in gameRules.ranges_.Keys){
        //Debug.Log("range = "+rangeName+$" -> score to be on the range = {gameRules.ranges_[rangeName]}");
        if(score >= gameRules.ranges_[rangeName]){
          //Debug.Log($"score = {score} -> range = "+rangeName);
          return rangeName;
        }
      }
      Debug.Log($"Invalid score: {score}");
      return "";
    }

    /**
      * @param string searched range name
      * @return int with the index of the correspondent range
      * @brief This method returns the correspondent index of the given
      * range name, if the given string isnt any defined range, it will
      * return -1
      */
    static public int getIndexOfRange(string range){
      return Array.IndexOf(gameRules.ranges_.Keys.ToArray(),range);
    }

    /**
      * @return string with the name defined for anonymous users
      */
    static public string getAnonymousName(){
      return "Anonymous";
    }
}
