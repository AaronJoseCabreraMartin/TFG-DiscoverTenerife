using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadHandler : MonoBehaviour
{
    public void DownloadImage(string link, Place toAdvice){
        StartCoroutine(DownloadImageAsync(link,toAdvice));
    }

    private IEnumerator DownloadImageAsync(string link, Place toAdvice){
        WebClient webClient = new WebClient();
        byte [] data = webClient.DownloadData(link);
        Texture2D texture = new Texture2D(128, 128);
        texture.LoadImage(data);
        toAdvice.setImage(Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),new Vector2(0f, 0f)));
        yield return new WaitForSeconds(0);
    }
}
