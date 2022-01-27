using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if PLATFORM_ANDROID
using UnityEngine.Android;
//#endif
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gpsController : MonoBehaviour
{
    public static gpsController gpsControllerInstance_;

    private bool isItPermissionTime;
    private string nextPermission;
    private Stack<string> permissions;
    private bool gpsIsRunning_;

    private double longitude_;
    private double latitude_;
    private double altitude_;

    /*
    Para testear estoy estableciendo las coordenadas en ciertos lugares, la idea es que en la version final, si gpsIsRunning está a false
    NO se haga el askForAPlace, porque puede pasar que en lugar de las coordenadas reales se usen las coordenadas por defecto y se muestren
    cosas que están más lejos de lo que deberían.
    Al igual que la característica de poder mover las coordenadas con las flechas del teclado.
    */
    // casa
    //private double defaultLatitude_ = 28.07133;
    //private double defaultLongitude_ = -16.65696;

    // a 40 metros de Mirador El Tabonal Negro (centro)
    //private double defaultLatitude_ = 28.258;
    //private double defaultLongitude_ = -16.611;
    // Parque natural Corona Forestal (centro)
    //private double defaultLatitude_ = 28.2486807;
    //private double defaultLongitude_ = -16.5382264;

    // a 20 metros de la playa de los cristianos (sur)
    //private double defaultLatitude_ = 28.05015;
    //private double defaultLongitude_ = -16.7177;
    
    // a metros de la playa de las teresitas (norte)
    //private double defaultLatitude_ = 28.5097;
    //private double defaultLongitude_ = -16.18439;
    
    // a metros de sendero Monte del agua (Oeste)
    //private double defaultLatitude_ = 28.328925;
    //private double defaultLongitude_ = -16.808909;
    
    // a metros de Piscina natural de guimar (Este)
    private double defaultLatitude_ = 28.2600654;
    private double defaultLongitude_ = -16.3924713;

    private double defaultAltitude_ = 255;

    void Awake(){
        if(gpsController.gpsControllerInstance_ != null){
            Destroy(this.gameObject); //no crees otro
            return;
        }
        gpsController.gpsControllerInstance_ = this;
        DontDestroyOnLoad(this.gameObject);
        isItPermissionTime = false;
        permissions = new Stack<string>();
    
        Debug.Log("Las coordenadas por defecto deben empezar en 0, 0, 0 por defecto");
       
        CreatePermissionList();
        Input.location.Start();
        gpsIsRunning_ = false;
    }

    

    private void CreatePermissionList(){
        isItPermissionTime = true;
        permissions.Push(Permission.CoarseLocation);
        permissions.Push(Permission.FineLocation);
        AskForPermissions();
    }
    
    private void AskForPermissions (){
        while(!(permissions == null || permissions.Count <= 0)){
            nextPermission = permissions.Pop();
            if (Permission.HasUserAuthorizedPermission(nextPermission) == false) {
                Permission.RequestUserPermission(nextPermission);
            }
        }
        isItPermissionTime = false;
    }

    private void OnApplicationFocus(bool focus){
        if (focus && isItPermissionTime) {
            AskForPermissions();
        }
        if(focus){
            Input.location.Start();
        }else{
            Input.location.Stop();
        }
    }

    void Update(){
        if(Permission.HasUserAuthorizedPermission(Permission.CoarseLocation) && 
            Permission.HasUserAuthorizedPermission(Permission.FineLocation) && 
            Input.location.isEnabledByUser &&
            Input.location.status == LocationServiceStatus.Running){
                gpsIsRunning_ = true;

                latitude_ = Input.location.lastData.latitude;
                longitude_ = Input.location.lastData.longitude;
                altitude_ = Input.location.lastData.altitude;
                //Debug.Log($"latitude_={latitude_},longitude_={longitude_}");
                //si la base no ha sido establecida desde que tengas permisos, establecela
            if(firebaseHandler.firebaseHandlerInstance_.actualUser_ != null && !firebaseHandler.firebaseHandlerInstance_.actualUser_.baseEstablished()){
                firebaseHandler.firebaseHandlerInstance_.actualUser_.setBase(latitude_,longitude_);
                firebaseHandler.firebaseHandlerInstance_.writeUserData();
            }
        }else{
            gpsIsRunning_ = false;
            //Debug.Log("GPS NO ACTIVADO!!!");
            latitude_ = defaultLatitude_;
            longitude_ = defaultLongitude_;
            altitude_ = defaultAltitude_;
        }
        
        
        //For debugging propources
        /*float velocity = 0.0001f;
        if (Input.GetKey("up")){
            defaultLatitude_ += velocity;
            Debug.Log($"latitude_ = {latitude_}, longitude_ = {longitude_}");
        }
        if (Input.GetKey("down")){
            defaultLatitude_ -= velocity;
            Debug.Log($"latitude_ = {latitude_}, longitude_ = {longitude_}");
        }
        if (Input.GetKey("right")){
            defaultLongitude_ += velocity;
            Debug.Log($"latitude_ = {latitude_}, longitude_ = {longitude_}");
        }
        if (Input.GetKey("left")){
            defaultLongitude_ -= velocity;
            Debug.Log($"latitude_ = {latitude_}, longitude_ = {longitude_}");
        }*/
    }

    public double getLatitude(){
        return latitude_;
    }

    public double getLongitude(){
        return longitude_;
    }

    public double getAltitude(){
        return altitude_;
    }

    public double CalculateDistance(double latitudeA, double longitudeA, double latitudeB, double longitudeB){
        latitudeA = sexagecimalToRadian(latitudeA);
        longitudeA = sexagecimalToRadian(longitudeA);
        latitudeB = sexagecimalToRadian(latitudeB);
        longitudeB = sexagecimalToRadian(longitudeB);
        //Debug.Log($"latitudeA = {latitudeA} longitudeA = {longitudeA}");
        //Debug.Log($"latitudeB = {latitudeB} longitudeB = {longitudeB}");
        
        double earthRadious = 6377.830272;
        double distanceCalculatedOnKm = earthRadious*Math.Acos((Math.Sin(latitudeA) * Math.Sin(latitudeB)) + Math.Cos(latitudeA) * Math.Cos(latitudeB) * Math.Cos(longitudeB - longitudeA));
        optionsController options = optionsController.optionsControllerInstance_;
        //Debug.Log($"{distanceCalculatedOnKm}kms {distanceCalculatedOnKm * 0.621371}milles");
        return options.distanceInKM() ? distanceCalculatedOnKm : distanceCalculatedOnKm * 0.621371;
    }

    public double CalculateDistanceToUser(double latitudeA, double longitudeA){
        return CalculateDistance(latitudeA, longitudeA, latitude_, longitude_);
    }
    
    private double sexagecimalToRadian(double sexagecimal) {
      return sexagecimal * (Math.PI/180);
    }

    public string getZoneOf(double latitude, double longitude){
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

    public string getActualZoneOfUser(){
        return getZoneOf(latitude_, longitude_);
    }

    public bool gpsIsRunning(){
        return gpsIsRunning_;
    }
}