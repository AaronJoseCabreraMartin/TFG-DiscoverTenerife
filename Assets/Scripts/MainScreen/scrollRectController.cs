using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This class controls when the user wants to load more places. It also
  * controls the helping text that show the user intructions.
  */
public class scrollRectController : MonoBehaviour
{
    /**
      * @brief GameObject that contains the text that will show the instructions for 
      * load more places to the user.
      */
    [SerializeField] private GameObject loadMorePlacesText_;

    /**
      * @brief GameObject that contains the text that will show the instructions for
      * resort places to the user.
      */
    [SerializeField] private GameObject resortPlacesText_;

    /**
      * @brief string that contains the default text for saying to user that he has to continue
      * sliding up to confirm the action of loading more places.
      */
    [SerializeField] private string continueSlidingForLoadText_ = "Continue sliding up to confirm that you want to load more places!";

    /**
      * @brief string that contains the default text for saying to user that he has to release
      * to load more places.
      */
    [SerializeField] private string stopSlidingForLoadText_ = "Release to load more places!";

    /**
      * @brief string that contains the default text for saying to user that they have to continue
      * sliding down to confirm the action of sorting all places.
      */
    [SerializeField] private string continueSlidingForResortText_ = "Continue sliding down to confirm that you want to resort all places!";

    /**
      * @brief string that contains the default text for saying to user that they have to release
      * to sort again all places.
      */
    [SerializeField] private string stopSlidingForResortText_ = "Release to sort again all places!";


    /**
      * @brief double that contains the point where if the user slide down more, will
      * activate the loading more places process.
      */
    [SerializeField] private double maxLimit_ = 1.03;

    /**
      * @brief double that contains the point where the user, after sliding down enough,
      * will fire the loading more places process.
      */
    [SerializeField] private double minLimit_ = 1.00001;

    /**
      * @brief double that contains the point where if the user slide up more, will
      * activate the loading more places process.
      */
    [SerializeField] private double maxNegativeLimit_ = -0.045;
    
    /**
      * @brief double that contains the point where the user, after sliding up enough,
      * will fire the loading more places process.
      */
    [SerializeField] private double minNegativeLimit_ = -0.012;

    /**
      * @brief true if the user has passed the point where the resorting places
      * process start.
      */
    private bool alreadyAskedForResortPlaces = false;

    /**
      * @brief true if the user has passed the point where the loading more places
      * process start.
      */
    private bool alreadyAskedForNewPlacesDown_ = false;

    /**
      * @param Vector2 with both directions of the scrolling, both vertical and horizontal.
      * @brief it checks if the user has passed the maximum limit, if that is the case it
      * change the text to tell the user that if he release, it will load new places.
      * If the user has passed the maximum limit and then release it calls both
      * the askForNewPlaces method of firebaseHandler class and the
      * changeSceneWithAnimation method of ChangeScene class to reload the scene
      */
    public void OnUserScroll(Vector2 value){
        //sliding down
        if(value[1] > maxLimit_ && !alreadyAskedForResortPlaces){
            alreadyAskedForResortPlaces = true;
        }else if(value[1] < minLimit_ && alreadyAskedForResortPlaces){
            alreadyAskedForResortPlaces = false;
            firebaseHandler.firebaseHandlerInstance_.requestHandler_.sortPlaces();
            firebaseHandler.firebaseHandlerInstance_.requestHandler_.useStartIndex();
            firebaseHandler.firebaseHandlerInstance_.askForNewPlaces();
            GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal");
        }

        //sliding up value[1] = -0,0495233, maxNegativeLimit_ = 0,012, minNegativeLimit_ = -0,045, alreadyAskedForNewPlacesDown_ = True
        if(value[1] < maxNegativeLimit_ && !alreadyAskedForNewPlacesDown_){
            alreadyAskedForNewPlacesDown_ = true;
        }else if(value[1] > minNegativeLimit_ && alreadyAskedForNewPlacesDown_){
            firebaseHandler.firebaseHandlerInstance_.askForNewPlaces();
            alreadyAskedForNewPlacesDown_ = false;
            GameObject.FindGameObjectsWithTag("sceneManager")[0].GetComponent<ChangeScene>().changeSceneWithAnimation("PantallaPrincipal");
        }

        //si ya pase del maximo, pon stopSlidingForLoadText_, si no, pon el continueSlidingForLoadText_
        loadMorePlacesText_.GetComponent<Text>().text = (alreadyAskedForNewPlacesDown_ ? stopSlidingForLoadText_ : continueSlidingForLoadText_);
        resortPlacesText_.GetComponent<Text>().text = (alreadyAskedForResortPlaces ? stopSlidingForResortText_ : continueSlidingForResortText_);
    }
}
