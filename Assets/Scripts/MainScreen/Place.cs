using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;

/**
  * @brief class that stores all the information of a place. It also has to download the image of the place.
  */
public class Place
{
    /**
      * @brief name of the represented place.
      */
    [SerializeField] private string name_;

    /**
      * @brief address of the represented place.
      */
    [SerializeField] private string address_;

    /**
      * @brief latitude of the geographical coordenades of the represented place.
      */
    [SerializeField] private double latitude_;

    /**
      * @brief longitude of the geographical coordenades of the represented place.
      */
    [SerializeField] private double longitude_;

    /**
      * @brief imageLink of the image of the represented place.
      */
    [SerializeField] private string imageLink_;

    /**
      * @brief times that this place has been visited for any user.
      */
    [SerializeField] private int timesItHasBeenVisited_;

    /**
      * @brief zone of the map that the represented place belongs.
      */
    [SerializeField] private string zone_;

    /**
      * @brief Image of the reprecented place.
      */
    private Sprite image_;

    /**
      * @brief bool that control if this place is ready to use.
      */
    private bool ready_ = false;

    /**
      * @brief web client that download the image.
      */
    static public WebClient webClient_ = null;
    //private WebClient webClient_ = null;

    /**
      * @param Dictionary<string,string> data on string version of this place.
      * It expects a dictionary that has entries for addres_, imageLink_, latitude_,
      * longitude_, name_, timesItHasBeenVisited_ and zone_. It also initializes the
      * image_ property to null.
      */
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

    /**
      * @return string with the name_'s property value
      * @brief getter of the name_ attribute
      */
    public string getName(){
        return name_;
    }

    /**
      * @return string with the address's property value
      * @brief getter of the address attribute
      */
    public string getAddress(){
        return address_;
    }

    /**
      * @return string with the latitude_'s property value
      * @brief getter of the latitude_ attribute
      */
    public double getLatitude(){
        return latitude_;
    }

    /**
      * @return string with the longitude_'s property value
      * @brief getter of the longitude_ attribute
      */
    public double getLongitude(){
        return longitude_;
    }

    /**
      * @return string with the zone_'s property value
      * @brief getter of the zone_ attribute
      */
    public string getZone(){
        return zone_;
    }

    /**
      * @return string with the image_'s property value
      * @brief getter of the image_ attribute
      */
    public Sprite getImage(){
        return image_;
    }

    /**
      * @return string with the ready_'s property value, true if this place is ready, false
      * in other case.
      * @brief getter of the ready_ attribute
      */
    public bool isReady(){
        return ready_;
    }

    /**
      * @brief add one to the timesItHasBeenVisited_ property.
      */
    public void oneMoreVisit(){
        timesItHasBeenVisited_++;
    }

    /**
      * @return string that contains a string conversion of this object. The conversion
      * follows the JSON format.
      */
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

    /**
      * @return string with the timesItHasBeenVisited_'s property value
      * @brief getter of the timesItHasBeenVisited_ attribute
      */
    public int getTimesItHasBeenVisited(){
        return timesItHasBeenVisited_;
    }

    /**
      * @brief if the image_ property is null, this start the donwload method of the
      * WebClient_ property and initialize the image_ porperty to the real image. 
      */
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
