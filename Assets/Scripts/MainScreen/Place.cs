using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Place
{
    private string name_;
    private string address_;
    private double latitude_;
    private double longitude_;
    private Sprite image_;

    private int timesItHasBeenVisited_;

    public Place(string lineToSplit, Sprite image)
    {
        string[] lineSplited = lineToSplit.Split(';');
        name_ = lineSplited[0];
        address_ = lineSplited[1];
        latitude_ = Convert.ToDouble(lineSplited[2]);
        longitude_ = Convert.ToDouble(lineSplited[3]);
        image_ = image;
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
}
