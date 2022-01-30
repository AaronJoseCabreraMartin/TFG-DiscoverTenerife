using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noStoryMessage : MonoBehaviour
{
    [SerializeField] private GameObject storyPanel_;
    [SerializeField] private GameObject image_;
    [SerializeField] private GameObject text_;
    [SerializeField] private string sortingText_ = "Choose the sorting option to see your story";
    [SerializeField] private string noStoryText_ = "You don't have visited any place. Try to visit one place and then come back.";

    // Update is called once per frame
    void Update()
    {
        text_.GetComponent<Text>().text = optionsController.lastOptionClicked_ == null ? noStoryText_ : sortingText_;
        bool show = storyPanel_.GetComponent<StoryPanel>().hidePanel();
        image_.GetComponent<Image>().enabled = show;
        text_.SetActive(show);
    }
}
