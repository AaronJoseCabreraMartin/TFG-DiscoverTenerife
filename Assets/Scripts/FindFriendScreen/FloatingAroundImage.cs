
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingAroundImage : MonoBehaviour
{
    
    [SerializeField] private Image image_;
    [SerializeField] private float velocityX_ = 100.0f;
    [SerializeField] private float velocityY_ = 100.0f;

    // Update is called once per frame
    void Update(){
    
        float newXPosition = image_.transform.localPosition[0];
        if(image_.transform.localPosition[0] + velocityX_*Time.deltaTime > 250 || image_.transform.localPosition[0] + velocityX_*Time.deltaTime < -300){
            velocityX_*=-1;
        }
        newXPosition += velocityX_*Time.deltaTime;

        float newYPosition = image_.transform.localPosition[1];
        if(image_.transform.localPosition[1] + velocityY_*Time.deltaTime > 475 || image_.transform.localPosition[1] + velocityY_*Time.deltaTime < -610){
            velocityY_*=-1;
        }
        newYPosition += velocityY_*Time.deltaTime;
        
        image_.transform.localPosition = new Vector3(newXPosition,newYPosition,0);
        
    }
}
