using System.IO;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Video : MonoBehaviour
{
    [HideInInspector]
    public VideoPlayer videoPlayer;
    [HideInInspector]
    public string Gender;
    [HideInInspector]
    public int VideoIndex;

    private List<string> videoUrls;

    // Applies Gender and VideoIndex if DataCollector is alive
    private void Awake()
    {
        if(DataCollector.Instance != null)
        {
            //find the steamvr eye and assign it to data collector
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;
            Gender = DataCollector.Instance.Gender;
            VideoIndex = DataCollector.Instance.VideoID;
        }
    }

    // Loads video and Plays it
    private void Start ()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoUrls = new List<string>();
        LoadVideos(Gender + "/");
        CheckDimensions(videoUrls[VideoIndex - 1]);
        videoPlayer.loopPointReached += FinishPlaying;
	}

    // Loads all videos in the Folder
    private void LoadVideos(string Folder)
    {
        DirectoryInfo directory = new DirectoryInfo(Application.dataPath + "/Videos/" + Folder);

        foreach(var file in directory.GetFiles("*.mp4", SearchOption.AllDirectories))
        {
            videoUrls.Add(directory.FullName + file.Name);
        }
        videoUrls.TrimExcess();
    }

    // Make a temporary VideoPlayer GameObject to get dimensions of chosen video
    private void CheckDimensions(string url)
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

    // Plays video with passed in url and dimension we got from CheckDimensions()
    private void PlayVideo(string url, int width, int height)
    {
        videoPlayer.url = url;
        RenderTexture texture = new RenderTexture(width, height, 24);
        videoPlayer.targetTexture = texture;
        RenderSettings.skybox.SetTexture("_MainTex", texture);
        videoPlayer.Play();
    }

    // Stops Playing Video and Quit Application
    private void FinishPlaying(VideoPlayer _videoPlayer)
    {
        // Stops playing video
        _videoPlayer.Stop();

        // Quit Application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Loads StartScene
    private void Restart()
    {
        SceneManager.LoadScene("StartScene");
        Destroy(DataCollector.Instance.gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            Restart();
        }
    }
}
