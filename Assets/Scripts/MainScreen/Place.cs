using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Place
{
    //SerializeField significa que se puede ver en el inspector (para poder convertirlo en JSON)
    [SerializeField] private string name_;
    [SerializeField] private string address_;
    [SerializeField] private double latitude_;
    [SerializeField] private double longitude_;
    //[SerializeField] private byte[] rawImage_;
    [SerializeField] private string imageLink_;
    [SerializeField] private int timesItHasBeenVisited_;
    public static DownloadHandler downloadHandler_;

    private Sprite image_;
    private bool ready_ = false;

    //public Place(string lineToSplit, Sprite image, byte[] rawImage)
    public Place(string lineToSplit)
    {
        string[] lineSplited = lineToSplit.Split(';');
        name_ = lineSplited[0];
        address_ = lineSplited[1];
        latitude_ = Convert.ToDouble(lineSplited[2]);
        longitude_ = Convert.ToDouble(lineSplited[3]);
        imageLink_ = lineSplited[4];

        //demasiada carga en el servidor, mejor guardar solo la url de la imagen y despues descargarla con corrutinas

        Place.downloadHandler_.DownloadImage(imageLink_,this);
    }

    public string getName()
    {
        return name_;
    }

    public string getAddress()
    {
        return address_;
    }

    public double getLatitude()
    {
        return latitude_;
    }

    public double getLongitude()
    {
        return longitude_;
    }

    public Sprite getImage()
    {
        return image_;
    }

    public void setImage(Sprite image){
        image_ = image;
        ready_ = true;
    }
    public bool isReady()
    {
        return ready_;
    }

    
}
