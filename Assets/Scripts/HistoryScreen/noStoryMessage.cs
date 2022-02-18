using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controls the message that is showed when the story of the current user is empty.
  */
public class noStoryMessage : MonoBehaviour
{
    /**
      * @brief GameObject with the panel that shows the story of the current user.
      */
    [SerializeField] private GameObject storyPanel_;

    /**
      * @brief GameObject with the image that is showed when the story of the current user is empty.
      */
    [SerializeField] private GameObject image_;
    
    /**
      * @brief GameObject with the text that is showed when the story of the current user is empty.
      */
    [SerializeField] private GameObject text_;
    
    /**
      * @brief 
      */
    [SerializeField] private string sortingText_ = "Choose the sorting option to see your story";
    
    /**
      * @brief 
      */
    [SerializeField] private string noStoryText_ = "You don't have visited any place. Try to visit one place and then come back.";

    /**
      * @brief 
      */
    void Update()
    {
        text_.GetComponent<Text>().text = optionsController.lastOptionClicked_ == null ? noStoryText_ : sortingText_;
        bool show = storyPanel_.GetComponent<StoryPanel>().hidePanel();
        image_.GetComponent<Image>().enabled = show;
        text_.SetActive(show);
    }
}
