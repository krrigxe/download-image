using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class WebImage : MonoBehaviour
{
    [SerializeField]
    private string url;

    public Sprite defaultSprite;
    public bool cached;
    TimeSpan timeCover =new TimeSpan(168,0,0);

    private void OnEnable()
    {
        DownloadIMG._instance.eventImageDownloaded += GetImage;
    }
    private void OnDisable()
    {
        DownloadIMG._instance.eventImageDownloaded -= GetImage;
    }
    private void Start()
    {
        GetImage();
    }
    public void GetImage()
    {
        if (File.Exists(Application.persistentDataPath+gameObject.name+".jpg"))
        {
            cached = true;
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + gameObject.name + ".jpg");
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(byteArray);
            gameObject.GetComponent<Image>().sprite= Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            StartCoroutine(DownloadIMG._instance.DownloadImgFromUrl(url, gameObject));
        }
    }
    private void Update()
    {
        if (cached)
        {
            CalculateTime();
        }
    }
    void CalculateTime()
    {
        DateTime currentTime = DateTime.Now;
        DateTime ImageDownloadedTime = DateTime.Parse(PlayerPrefs.GetString(gameObject.name));
        TimeSpan coverTime = currentTime.Subtract(ImageDownloadedTime);
        if (coverTime >= timeCover)
        {
            if (File.Exists(Application.persistentDataPath + gameObject.name + ".jpg"))
            {
                File.Delete(Application.persistentDataPath + gameObject.name + ".jpg");
            }
        }
    }
}
