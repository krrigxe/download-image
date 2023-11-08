using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using System;

public class DownloadIMG : MonoBehaviour
{
    public static DownloadIMG _instance;
    static int DownloadingCount;

    public delegate void ImageDownloaded();
    public event ImageDownloaded eventImageDownloaded;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    void Start()
    {
        DownloadingCount = 0;   
    }

    public IEnumerator DownloadImgFromUrl(string url,GameObject obj)
    {
        if (DownloadingCount <= 3)
        {
            DownloadingCount += 1;
            UnityWebRequest request=UnityWebRequest.Get(url);
            request.timeout = 10;
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                DownloadingCount -= 1;
                obj.GetComponent<Image>().sprite = obj.GetComponent<WebImage>().defaultSprite;
            }
            else
            {
                DownloadingCount -= 1;
                Debug.Log("image arrived");
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                obj.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                byte[] bytes = texture.EncodeToJPG();
                File.WriteAllBytes(Application.persistentDataPath + obj.name + ".jpg", bytes);
                obj.GetComponent<WebImage>().cached = true;
                if (eventImageDownloaded != null)
                    eventImageDownloaded();

                PlayerPrefs.SetString(obj.name,DateTime.Now.ToString());

            }
        }
        else
        {
            yield return null;
        }
    }
}
