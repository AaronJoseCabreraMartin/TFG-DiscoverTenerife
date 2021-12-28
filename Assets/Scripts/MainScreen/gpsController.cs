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

    private double longitude_;
    private double latitude_;
    private double altitude_;

    static private GameObject errorImage_;

    void Awake(){
        GameObject[] objs = GameObject.FindGameObjectsWithTag("gpsController");
        if (objs.Length > 1){//si ya existe una gpsController
            Destroy(this.gameObject); //no crees otro
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        errorImage_ = GameObject.Find("/Canvas/errorImage").gameObject;
    }

    void Start(){
        isItPermissionTime = false;
        permissions = new Stack<string>();
        lastScene_ = SceneManager.GetActiveScene().name;
        Debug.Log("estas coordenadas deben empezar en 0, 0, 0 por defecto");
        // casa
        longitude_ = -16.65696;
        latitude_ = 28.07133;
        // la laguna
        /*longitude_ = -16.3433747;
        latitude_ = 28.4697925;*/
        altitude_ = 255;
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
        //Debug.Log("Unity>> focus ....  " + focus + "   isPermissionTime : " + isItPermissionTime);
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
            //GameObject.Find("/Canvas/errorImage").gameObject.SetActive(false);//oculta la imagen de error
        }else{
            errorImage_.SetActive(true);
            //GameObject.Find("/Canvas/errorImage").gameObject.SetActive(true);//muestra la imagen de error
            //GameObject.Find("/Canvas/errorImage").gameObject.transform.Find("errorMessage").gameObject.GetComponent<Text>().text = "Cant access to GPS data";
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
}