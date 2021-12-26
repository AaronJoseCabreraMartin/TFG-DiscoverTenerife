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

    private double longitude_;
    private double latitude_;
    private double altitude_;

    void Awake(){
        GameObject[] objs = GameObject.FindGameObjectsWithTag("gpsController");
        if (objs.Length > 1 || //si ya existe una gpsController o
            (SceneManager.GetActiveScene().name != "PantallaPrincipal" && //si la escena no es la principal ni
            SceneManager.GetActiveScene().name != "PantallaLugar")) { //la pantalla de un lugar
            Destroy(this.gameObject); //no crees otro
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start(){
        isItPermissionTime = false;
        permissions = new Stack<string>();
        longitude_ = 0;//-16.65696;
        latitude_ = 0;//28.07133;
        altitude_ = 0;//255;
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
      if(Permission.HasUserAuthorizedPermission(Permission.CoarseLocation) == true && Input.location.status == LocationServiceStatus.Running){

        latitude_ = Input.location.lastData.latitude;
        var latitud = latitude_.ToString();
        
        longitude_ = Input.location.lastData.longitude;
        var longitud = longitude_.ToString();
        
        altitude_ = Input.location.lastData.altitude;
        var altitud = altitude_.ToString();

        GameObject.Find("/Canvas/errorImage").gameObject.SetActive(false);//oculta la imagen de error
        //GameObject.Find("/Canvas/errorImage").gameObject.transform.Find("errorMessage").gameObject.GetComponent<Text>().text = "Posicion: latitud:" + latitud + " longitud:" + longitud + " altitud:" + altitud;
        Debug.Log("Posicion: latitud:" + latitud + " longitud:" + longitud + " altitud:" + altitud);
      }else{
        GameObject.Find("/Canvas/errorImage").gameObject.SetActive(true);//muestra la imagen de error
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
}