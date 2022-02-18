using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if PLATFORM_ANDROID
using UnityEngine.Android;
//#endif
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
  * @brief class that handles the access to the GPS sensor of the current dispositive.
  * This class follows the singleton pattern, so its only one instance during each excecution.
  */
public class gpsController : MonoBehaviour
{
    /**
      * @brief reference to the gpsController instance
      */
    public static gpsController gpsControllerInstance_;

    /**
      * @brief true if its time to ask permission, false in other case
      */
    private bool isItPermissionTime;

    /**
      * @brief Stack<string> that contains all the permissions we have to ask.
      */
    private Stack<string> permissions;

    /**
      * @brief true if the gps is avaible, false in other case.
      */
    private bool gpsIsRunning_;

    /**
      * @brief double that stores the longitude number of the current user current gps location.
      */
    private double longitude_;

    /**
      * @brief double that stores the latitude number of the current user current gps location.
      */
    private double latitude_;

    /**
      * @brief double that stores the altitude number of the current user current gps location.
      */
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


    /**
      * This method is called before the first frame. If there is another gpsController, it
      * destroy this instance and return nothing. Other case, it set the gpsControllerInstance_ 
      * static property as a reference of this gameObject. It also instantiate the rest of 
      * the properties, calls the CreatePermissionList method and start the gps location process.
      */
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

    /**
      * This method set the isItPermissionTime attribute as true, add both the CoarseLocation
      * and the FineLocation permissions to the permissions property, and then, call the
      * AskForPermissions method of gpsController.
      */
    private void CreatePermissionList(){
        isItPermissionTime = true;
        permissions.Push(Permission.CoarseLocation);
        permissions.Push(Permission.FineLocation);
        AskForPermissions();
    }
    
    /**
      * This method if the permissions stack property is empty, it calls the 
      * CreatePermissionList first. In other case, ask for each permission that 
      * is on the permissions property. When it finish, it sets the isItPermissionTime
      * to false.
      */
    private void AskForPermissions (){
        //Añadido
        if(permissions.Count == 0){
            CreatePermissionList();
        }else{
            while(!(permissions == null || permissions.Count <= 0)){
                string nextPermission = permissions.Pop();
                if (Permission.HasUserAuthorizedPermission(nextPermission) == false) {
                    Permission.RequestUserPermission(nextPermission);
                }
            }
            isItPermissionTime = false;
        }
    }

    /**
      * @param bool true if the application is focusing in, false if the application
      * is focusing out.
      * @brief this method is called each time the application either focus in or focus out.
      * If the application is focusing in and the isItPermissionTime property is true it calls
      * the AskForPermissions method.
      * If the application is focusing in it starts the Input.Location process.
      * If the application is focusing out it stops the Input.Location process.
      */
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

    /**
      * This method is called on each frame. 
      * - If the user has given both CoarseLocation and
      * FineLocation permissions, the gps service is active on the user's device and
      * LocationService is running, it sets the gpsIsRunning_ property to true, gets the
      * latitude, the longitude and the altitude of the user then it checks if the
      * current user hasnt stablished its base, if that is the case it set the user base
      * as the current latitude and longitude and call the writeUserData method of firebaseHandler.
      * - If that is not the case, it sets the gpsIsRunning_ as false and it sets the 
      * latitude_, longitude_ and altitude_ as the default ones.
      */
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

    /**
      * @return the latitude_ property value.
      * @brief getter of the latitude_ property.
      */
    public double getLatitude(){
        return latitude_;
    }

    /**
      * @return the longitude_ property value.
      * @brief getter of the longitude_ property.
      */
    public double getLongitude(){
        return longitude_;
    }

    /**
      * @return the altitude_ property value.
      * @brief getter of the altitude_ property.
      */
    public double getAltitude(){
        return altitude_;
    }

    /**
      * @param double that contains the latitude of the first point.
      * @param double that contains the longitude of the first point.
      * @param double that contains the latitude of the second point.
      * @param double that contains the longitude of the second point.
      * @return double the distance on the choosen unit.
      * @brief this method returns the distance between the two geographical points given on the
      * choosen distance. It expects that the given parameters are on Hexadecimal notation, it converts
      * them on sexagecimal using the sexagecimalToRadian method. It checks the distanceInKM
      * method of optionsController class to choose on which unit it has to return the distance.
      */
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

    /**
      * @param double that contains the latitude of the point.
      * @param double that contains the longitude of the point.
      * @return double that contains the distance between the user and the given point on 
      * the choosen unit.
      * @brief it calls the CalculateDistance method with the given coordenades and 
      * the user current coordenades.
      */
    public double CalculateDistanceToUser(double latitudeA, double longitudeA){
        return CalculateDistance(latitudeA, longitudeA, latitude_, longitude_);
    }
    
    /**
      * @param double sexagecimal number to convert.
      * @return double radian conversion of the given number.
      * @brief this method converts the given sexagecimal number to his radian number equivalent.
      */
    private double sexagecimalToRadian(double sexagecimal) {
      return sexagecimal * (Math.PI/180);
    }

    /**
      * @param double latitude of the point
      * @param double longitude of the point
      * @return string it returns the zone of the given point. If it doesnt
      * fit on any of the defined zones, it returns "Can't Find Zone of given point"
      */
    public string getZoneOf(double latitude, double longitude){
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

    /**
      * @return string current zone of the current user.
      * @brief it calls the getZoneOf method with the user coordenades and returns what
      * that method returns.
      */
    public string getActualZoneOfUser(){
        return getZoneOf(latitude_, longitude_);
    }

    /**
      * @return true if the gps is running, false in other case.
      * @brief getter of the gpsIsRunning_ property.
      */
    public bool gpsIsRunning(){
        return gpsIsRunning_;
    }
}