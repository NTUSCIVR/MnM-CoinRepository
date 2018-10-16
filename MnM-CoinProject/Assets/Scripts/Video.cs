using System.IO;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class Video : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private int textureIndex = -1;
    private List<string> videoUrls;
    
    // Use this for initialization
    void Start ()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoUrls = new List<string>();
        LoadVideos();
        CheckDimensions(videoUrls[0]);
	}
	
    void LoadVideos()
    {
        DirectoryInfo directory = new DirectoryInfo(@"C:\MnM Videos\");

        foreach(var file in directory.GetFiles("*.mp4", SearchOption.AllDirectories))
        {
            videoUrls.Add(directory.FullName + file.Name);
        }
        videoUrls.TrimExcess();
    }

    void CheckDimensions(string url)
    {
        GameObject tempVideo = new GameObject();
        VideoPlayer tempvideoPlayer = tempVideo.AddComponent<VideoPlayer>();
        tempvideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        tempvideoPlayer.targetTexture = new RenderTexture(1, 1, 0);
        tempvideoPlayer.source = VideoSource.Url;
        tempvideoPlayer.url = url;
        tempvideoPlayer.prepareCompleted += (VideoPlayer source) =>
        {
            PlayVideo(url, source.texture.width, source.texture.height);
            Destroy(tempVideo);
        };
        tempvideoPlayer.Prepare();
    }

    void PlayVideo(string url, int width, int height)
    {
        videoPlayer.url = url;
        RenderTexture texture = new RenderTexture(width, height, 24);
        videoPlayer.targetTexture = texture;
        RenderSettings.skybox.SetTexture("_MainTex", texture);
        videoPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Keypad1))
        {
            textureIndex = 0;
            CheckDimensions(videoUrls[textureIndex]);
        }
        else if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            textureIndex = 1;
            CheckDimensions(videoUrls[textureIndex]);
        }
    }
}
