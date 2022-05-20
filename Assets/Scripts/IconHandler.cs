using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
  * @brief Stores all the icons that could change during a game session. For example 
  * the range icons. This class follows the singleton pattern so you could only
  * have one instance of this class at the same time.
  */
public class IconHandler : MonoBehaviour
{
    /**
      * @brief Reference to the unique instance of this class
      */
    public static IconHandler iconHandlerInstance_ = null;
    
    /**
      * @brief List that stores all the images of the icons asociated with each range.
      */
    [SerializeField] private List<Sprite> allRangesImages_;

    /**
      * @brief Sprite with the icon that represent that a range icon wasnt found.
      */
    [SerializeField] private Sprite noRangeIconFound_;

    /**
      * @brief this method is called before the first frame, it checks if exits
      * another IconHandler instance, on that case destroy this gameobject.
      */
    void Awake()
    {
        if(IconHandler.iconHandlerInstance_ != null){
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        IconHandler.iconHandlerInstance_ = this;
    }

    /**
      * @param string with the name of the range that you want to obtain the icon of.
      * @brief This method returns the sprite associated with the given range, if the
      * range name is invalid or there isnt any sprite for that range, it will return
      * the noRangeIconFound sprite.
      */
    public Sprite getSpriteOfRange(string rangeName){
        int index = gameRules.getIndexOfRange(rangeName);
        return index >= 0 ? allRangesImages_[index] : noRangeIconFound_;
    } 
}
