using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;

public class Place
{
    //SerializeField significa que se puede ver en el inspector (para poder convertirlo en JSON)
    [SerializeField] private string name_;
    [SerializeField] private string address_;
    [SerializeField] private double latitude_;
    [SerializeField] private double longitude_;
    [SerializeField] private string imageLink_;
    [SerializeField] private int timesItHasBeenVisited_;

    private Sprite image_;
    private bool ready_ = false;

    public Place(Dictionary<string,string> data){
        address_ = data["address_"];
        imageLink_ = data["imageLink_"];
        latitude_ = Convert.ToDouble(data["latitude_"]);
        longitude_ = Convert.ToDouble(data["longitude_"]);
        name_ = data["name_"];
        timesItHasBeenVisited_ = Int32.Parse(data["timesItHasBeenVisited_"]);
        image_ = null;
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

    public bool isReady()
    {
        return ready_;
    }

    public void startDownload()
    {
        if(image_ == null){
            WebClient webClient = new WebClient();
            byte[] data = webClient.DownloadData(imageLink_);
            Texture2D texture = new Texture2D(128, 128);
            texture.LoadImage(data);
            image_ = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),new Vector2(0f, 0f));
            ready_ = true;
        }
    }
    
    public void oneMoreVisit(){
        timesItHasBeenVisited_++;
    }

    public string ToJson(){
        string toReturn = "{";
        toReturn += $"\"address_\" : \"{address_}\",";
        toReturn += $"\"imageLink_\" : \"{imageLink_}\",";
        string correctLatitude = latitude_.ToString().Replace(".",",");
        toReturn += $"\"latitude_\" : \"{correctLatitude}\",";
        string correctLongitude = longitude_.ToString().Replace(".",",");
        toReturn += $"\"longitude_\" : \"{correctLongitude}\",";
        toReturn += $"\"name_\" : \"{name_}\",";
        toReturn += $"\"timesItHasBeenVisited_\" : \"{timesItHasBeenVisited_}\"}}";
        return toReturn;
    }

    public int getTimesItHasBeenVisited(){
        return timesItHasBeenVisited_;
    }
}
