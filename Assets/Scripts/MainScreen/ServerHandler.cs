using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.IO;

public class ServerHandler : MonoBehaviour
{
    private int actualPlace_ = 0;

    private List<Place> placeList_;

    private bool ready_;
    // Start is called before the first frame update
    void Start()
    {
        ready_ = false;
        placeList_ = new List<Place>();
        //preguntarle al servidor por los places
        // Read the file and display it line by line.  
        foreach (string line in System.IO.File.ReadLines(@"Assets\Data\miradores\miradores.txt")){  
            byte[] fileData = File.ReadAllBytes(@"Assets\Data\miradores\"+line.Split(';')[0]+".png");
            Texture2D texture = new Texture2D(128, 128);
            texture.LoadImage(fileData);
            placeList_.Add(new Place(line, Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),new Vector2(0f, 0f))));
        }
        ready_ = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // quizas se pueda hacer que reciba la ubicacion actual para elegir que sitio poner
    public Place askForAPlace()
    {
        Place toReturn = placeList_[actualPlace_];
        actualPlace_++;
        if(actualPlace_ >= placeList_.Count){
            actualPlace_ = 0;
        }
        return toReturn;
    }

    public bool isReady(){
        return ready_;
    }
}
