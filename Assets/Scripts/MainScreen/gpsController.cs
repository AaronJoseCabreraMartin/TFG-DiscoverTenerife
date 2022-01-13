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
    private bool isItPermissionTime;
    private string nextPermission;
    private Stack<string> permissions;
    private string lastScene_;
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

    // la laguna
    //private double defaultLatitude_ = 28.4697925;
    //private double defaultLongitude_ = -16.3433747;
    
    // a 20 metros de la playa de los cristianos
    ///private double defaultLatitude_ = 28.05015;
    //private double defaultLongitude_ = -16.7177;
    
    // a metros de la playa de las teresitas
    private double defaultLatitude_ = 28.5097;
    private double defaultLongitude_ = -16.18439;
    
    private double defaultAltitude_ = 255;

    static private GameObject errorImage_;

    void Awake(){
        GameObject[] objs = GameObject.FindGameObjectsWithTag("gpsController");
        if (objs.Length > 1){//si ya existe una gpsController
            Destroy(this.gameObject); //no crees otro
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        errorImage_ = GameObject.Find("/Canvas/errorImage").gameObject;
        isItPermissionTime = false;
        gpsIsRunning_ = false;
        permissions = new Stack<string>();
        lastScene_ = SceneManager.GetActiveScene().name;
        Debug.Log("Las coordenadas por defecto deben empezar en 0, 0, 0 por defecto");
       
        CreatePermissionList();
        Input.location.Start();
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
        if (focus == true && isItPermissionTime == true) {
            AskForPermissions();
        }
    }

    void Update(){
        if(lastScene_ != SceneManager.GetActiveScene().name){
            lastScene_ = SceneManager.GetActiveScene().name;
            if(SceneManager.GetActiveScene().name != "PantallaPrincipal" && SceneManager.GetActiveScene().name != "PantallaLugar"){
                Destroy(this.gameObject);
                return;
            }
            errorImage_ = GameObject.Find("/Canvas/errorImage").gameObject;
        }
        
        if(errorImage_ == null){
            errorImage_ = GameObject.Find("/Canvas/errorImage").gameObject;
        }
        
        if(Permission.HasUserAuthorizedPermission(Permission.CoarseLocation) == true && Input.location.status == LocationServiceStatus.Running){
            latitude_ = Input.location.lastData.latitude;
            longitude_ = Input.location.lastData.longitude;
            altitude_ = Input.location.lastData.altitude;

            //Debug.Log($"Posicion: latitud:{latitude_} longitud:{longitude_} altitud:{altitude_}");
            //GameObject.Find("/Canvas/errorImage").gameObject.transform.Find("errorMessage").gameObject.GetComponent<Text>().text = $"Posicion: latitud:{latitude_} longitud:{longitude_} altitud:{altitude_}";

            errorImage_.SetActive(false);
            gpsIsRunning_ = true;

            //GameObject.Find("/Canvas/errorImage").gameObject.SetActive(false);//oculta la imagen de error
        }else{
            errorImage_.SetActive(true);
            latitude_ = defaultLatitude_;
            longitude_ = defaultLongitude_;
            altitude_ = defaultAltitude_;
            gpsIsRunning_ = false;
            //GameObject.Find("/Canvas/errorImage").gameObject.SetActive(true);//muestra la imagen de error
            //GameObject.Find("/Canvas/errorImage").gameObject.transform.Find("errorMessage").gameObject.GetComponent<Text>().text = "Cant access to GPS data";
        }
        

        
        //For debugging propources
        float velocity = 0.001f;
        if (Input.GetKey("up")){
            latitude_ += velocity;
            Debug.Log($"latitude_ = {latitude_}, longitude_ = {longitude_}");
        }
        if (Input.GetKey("down")){
            latitude_ -= velocity;
            Debug.Log($"latitude_ = {latitude_}, longitude_ = {longitude_}");
        }
        if (Input.GetKey("right")){
            longitude_ += velocity;
            Debug.Log($"latitude_ = {latitude_}, longitude_ = {longitude_}");
        }
        if (Input.GetKey("left")){
            longitude_ -= velocity;
            Debug.Log($"latitude_ = {latitude_}, longitude_ = {longitude_}");
        }
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
        optionsController options = GameObject.FindGameObjectsWithTag("optionsController")[0].GetComponent<optionsController>();
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
        
}