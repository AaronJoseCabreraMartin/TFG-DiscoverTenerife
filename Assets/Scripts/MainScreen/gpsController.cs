using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if PLATFORM_ANDROID
using UnityEngine.Android;
//#endif
using UnityEngine.UI;


public class gpsController : MonoBehaviour
{
    bool isItPermissionTime = false;
    string nextPermission;
    Stack<string> permissions = new Stack<string>();

    void Start()
    {
        OpenAllPermissions();
        Input.location.Start();
    }

    public void OpenAllPermissions()
    {
        isItPermissionTime = true;
        CreatePermissionList();

    }
    void CreatePermissionList()
    {
        permissions = new Stack<string>();
        permissions.Push(Permission.CoarseLocation);
        permissions.Push(Permission.FineLocation);
        AskForPermissions();
    }
    void AskForPermissions ()
    {
        if (permissions == null || permissions.Count <= 0)
        {
            isItPermissionTime = false;
            return;
        }
        nextPermission = permissions.Pop();

        if (nextPermission == null)
        {
            isItPermissionTime = false;
            return;
        }
        if (Permission.HasUserAuthorizedPermission(nextPermission) == false)
        {
            Permission.RequestUserPermission(nextPermission);
        }
        else
        {
            if (isItPermissionTime == true)
                AskForPermissions();
        }
        Debug.Log("Unity>> permission " + nextPermission + "  status ;" + Permission.HasUserAuthorizedPermission(nextPermission));
    }

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("Unity>> focus ....  " + focus + "   isPermissionTime : " + isItPermissionTime);
        if (focus == true && isItPermissionTime == true)
        {
            AskForPermissions();
        }
    }

    void Update()
    {
      if(Permission.HasUserAuthorizedPermission(Permission.CoarseLocation) == true && Input.location.status == LocationServiceStatus.Running){

        var latitud = Input.location.lastData.latitude.ToString();
        var longitud = Input.location.lastData.longitude.ToString();
        var altitud = Input.location.lastData.altitude.ToString();
        //gameObject.transform.Find("errorImage").gameObject.SetActive(false);//oculta la imagen de error
        gameObject.transform.Find("errorImage").gameObject.transform.Find("errorMessage").gameObject.GetComponent<Text>().text = "Posicion: latitud:" + latitud + " longitud:" + longitud + " altitud:" + altitud;
        Debug.Log("Posicion: latitud:" + latitud + " longitud:" + longitud + " altitud:" + altitud);
      }else{
        //gameObject.transform.Find("errorImage").gameObject.SetActive(true);//muestra la imagen de error
        gameObject.transform.Find("errorImage").gameObject.transform.Find("errorMessage").gameObject.GetComponent<Text>().text = "Cant access to GPS data";
      }
    }
}