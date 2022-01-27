using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noStoryMessage : MonoBehaviour
{
    [SerializeField] private GameObject storyPanel_;
    [SerializeField] private GameObject image_;
    [SerializeField] private GameObject text_;

    // Update is called once per frame
    void Update()
    {
        bool show = storyPanel_.GetComponent<StoryPanel>().getStoryCount() == 0;
        image_.GetComponent<Image>().enabled = show;
        text_.SetActive(show);
    }
}
