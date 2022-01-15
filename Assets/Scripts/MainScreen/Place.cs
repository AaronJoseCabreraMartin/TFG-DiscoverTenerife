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
    [SerializeField] private string zone_;

    private Sprite image_;
    private bool ready_ = false;
    //private WebClient webClient_ = null;
    static public WebClient webClient_ = null;

    public Place(Dictionary<string,string> data){
        address_ = data["address_"];
        imageLink_ = data["imageLink_"];
        latitude_ = Convert.ToDouble(data["latitude_"]);
        longitude_ = Convert.ToDouble(data["longitude_"]);
        name_ = data["name_"];
        timesItHasBeenVisited_ = Int32.Parse(data["timesItHasBeenVisited_"]);
        zone_ = data["zone_"];
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

    public string getZone()
    {
        return zone_;
    }

    public Sprite getImage()
    {
        return image_;
    }

    public bool isReady()
    {
        return ready_;
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
        toReturn += $"\"zone_\" : \"{zone_}\",";
        toReturn += $"\"timesItHasBeenVisited_\" : \"{timesItHasBeenVisited_}\"}}";
        return toReturn;
    }

    public int getTimesItHasBeenVisited(){
        return timesItHasBeenVisited_;
    }

    public void startDownload()
    {
        if(image_ == null ){
            webClient_ = new WebClient();
            byte[] data = webClient_.DownloadData(imageLink_);
            Texture2D texture = new Texture2D(128, 128);
            texture.LoadImage(data);
            image_ = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),new Vector2(0f, 0f));
            ready_ = true;
        }
    }

    // Se queda freezeado en negro por algun motivo
    /*public void startDownload()
    {
        if(image_ == null ){
            Place.webClient_ = new WebClient();
            //byte[] data = webClient_.DownloadData(imageLink_);
            Uri urilink = new Uri(imageLink_);
            //Place.webClient_.DownloadDataAsync(urilink);
            System.Threading.AutoResetEvent waiter = new System.Threading.AutoResetEvent(false);
            Place.webClient_.DownloadDataAsync(urilink, waiter);
            Place.webClient_.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCallback);
            waiter.WaitOne();
        }
    }

    private void DownloadDataCallback(object sender, DownloadDataCompletedEventArgs e){
        System.Threading.AutoResetEvent waiter = (System.Threading.AutoResetEvent)e.UserState;

        try
        {
            // If the request was not canceled and did not throw
            // an exception, display the resource.
            if (!e.Cancelled && e.Error == null)
            {
                byte[] data = (byte[])e.Result;
                //string textData = System.Text.Encoding.UTF8.GetString(data);

                //Console.WriteLine(textData);
                Texture2D texture = new Texture2D(128, 128);
                texture.LoadImage(data);
                image_ = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),new Vector2(0f, 0f));
                ready_ = true;
                Place.webClient_ = null;
            }
        }
        finally
        {
            // Let the main application thread resume.
            waiter.Set();
        }
    }

    void OnDestroy(){
        Place.webClient_.DownloadDataCompleted -= new DownloadDataCompletedEventHandler(DownloadDataCallback);
    }*/
}
