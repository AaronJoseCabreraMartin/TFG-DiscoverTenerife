
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that makes the given image bouncing randomly between the limits of his father.
  */
public class FloatingAroundImage : MonoBehaviour
{
    /**
      * @brief the image that will be animated.
      */
    [SerializeField] private Image image_;

    /**
      * @brief the velocity of the image on the X axis.
      */
    [SerializeField] private float velocityX_ = 100.0f;

    /**
      * @brief the velocity of the image on the Y axis.
      */
    [SerializeField] private float velocityY_ = 100.0f;

    /**
      * This method is called each frame, it calculates if the image will move out of bounds.
      * If that is the case, it change the X axis velocity or the Y axis velocity to make it bounce
      * on the limit. Other wise it will move the image on the X axis with the X axis velocity 
      * and the same for the Y axis. It also use the Time.deltaTime to make the movement smoothly.
      */
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
