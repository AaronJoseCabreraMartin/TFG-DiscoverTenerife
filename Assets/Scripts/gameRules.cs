using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Esta clase almacenará la información relativa las reglas del juego como por ejemplo:
    distancia máxima a un punto de interés como para contarse como visitado
    cooldown de visitas
    número máximo de sitios guardados en el pocket para visitas offline
*/
public class gameRules{

    // distancia máxima para considerar cerca de un punto de interes, 50metros
    static private double maxDistance_ = 0.05;

    static public double getMaxDistance(){
        return gameRules.maxDistance_;
    }

    // minutos de tiempo de cooldown entre una visita y otra
    static private double minutesOfCooldown_ = 60;
    
    static public double getMinutesOfCooldown(){
        return gameRules.minutesOfCooldown_;
    }
    
    // número máximo de sitios almacenados en el pocket para visitas offline
    static private int maxPlacesStored_ = 5;

    static public int getMaxPlacesStored(){
        return gameRules.maxPlacesStored_;
    }
}
