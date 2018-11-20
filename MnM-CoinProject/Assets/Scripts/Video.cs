//--------------------------------------------------------------------------------
/*
 * This Script is used for playing 360 Degree Video once, and the application will close afterwards.
 * Allow Inputs to : Restart(Load StartScene and Input User ID and video choice again),
 *                   Increase Volume,
 *                   Decrese Volume,
 *                   Skip Video
 * 
 * Used in Main Scene, attached to Empty GameObject "Video", with Video Player Component attached.
 * Component Settings : Source(URL), URL(Leave Empty), PlayOnAwake(False),
 *                      WaitForFirstFrame(True), Loop(False), PlayBackSpeed(1),
 *                      RenderMode(Render Texture), TargetTexture(None), AspectRatio(Fit Horizontally),
 *                      AudioOutputMode(Direct), ControlledTrack(1), Track 0(True),
 *                      Mute(False), Volume(Default: 0.5)
 */
//--------------------------------------------------------------------------------

using System.IO;    // For Directory, File, StreamWriter
using UnityEngine;  // Default Unity Script (MonoBehaviour, Application, Tooltip, SerializeField, HideInInspector, DontDestroyOnLoad, Debug, Time)
using UnityEngine.Video; // For VideoPlayer, VideoRenderMode, VideoSource
using System.Collections.Generic; // For List<>
using UnityEngine.SceneManagement; // For SceneManager

public class Video : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    // Holds video files loaded from designated path
    private List<string> videoUrls;

    // Variables to hold data from DataCollector's instance, to determine where to look for video files
    private string Gender;
    private int VideoIndex;

    // Runs before Start()
    private void Awake()
    {
        if(DataCollector.Instance != null)
        {
            // Find the SteamVR eye GameObject and assign it to DataCollector
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;

            // Applies Gender and VideoIndex if DataCollector is alive
            Gender = DataCollector.Instance.Gender;
            VideoIndex = DataCollector.Instance.VideoID;
        }
    }

    // Runs at the start of first frame
    private void Start ()
    {
        // Get Video Player Component
        videoPlayer = GetComponent<VideoPlayer>();
        
        // Intiate List
        videoUrls = new List<string>();

        // Loads video based on Gender which user input in Start Scene
        LoadVideos(Gender);

        // Plays video based on Video Index which user input in Start Scene
        CheckDimensions(videoUrls[VideoIndex - 1]);

        // Listen for video finish playing's event call, calls FinishPlaying() afterwards
        videoPlayer.loopPointReached += FinishPlaying;
	}

    // Loads all videos in the Folder
    private void LoadVideos(string Folder)
    {

#if UNITY_EDITOR
        
        // Load videos from C drive if running through Unity Editor
        DirectoryInfo directory = new DirectoryInfo(@"C:\Videos\" + Folder + "\\");
#else
        // Load videos from Application Data if running through Release exe file
        DirectoryInfo directory = new DirectoryInfo(Application.dataPath + "/Videos/" + Folder + "/");
#endif

        // Loads every mp4 file path into List of string, videoUrls
        foreach (var file in directory.GetFiles("*.mp4", SearchOption.AllDirectories))
        {
            videoUrls.Add(directory.FullName + file.Name);
        }

        // Trim Excess to prevent extra space in List
        videoUrls.TrimExcess();
    }

    // Make a temporary VideoPlayer GameObject to get dimensions of chosen video
    // Proceed to play the video as soon as it is prepared
    private void CheckDimensions(string url)
    {
        // Preparing information for video player component
        GameObject tempVideo = new GameObject();
        VideoPlayer tempvideoPlayer = tempVideo.AddComponent<VideoPlayer>();
        tempvideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        tempvideoPlayer.targetTexture = new RenderTexture(1, 1, 0);
        tempvideoPlayer.source = VideoSource.Url;
        tempvideoPlayer.url = url;

        // Listen for prepare complete event call, and proceed to play the video with prepared information
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

        // Set RenderTexture onto Skybox and Play the video on Skybox
        RenderSettings.skybox.SetTexture("_MainTex", texture);
        videoPlayer.Play();
    }

    // Stops Playing Video and Quit Application
    private void FinishPlaying(VideoPlayer _videoPlayer)
    {
        // Stops playing video
        _videoPlayer.Stop();

#if UNITY_EDITOR
        // Stop running Application
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit Application
        Application.Quit();
#endif
    }

    // Loads StartScene
    private void Restart()
    {
        SceneManager.LoadScene("StartScene");
        // Destroy current DataCollector Instance, as StartScene will have its new instance of DataCollector
        Destroy(DataCollector.Instance.gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        // Proceed to Restart if 'R' is pressed
        if (Input.GetKeyUp(KeyCode.R))
        {
            Restart();
        }

        // Increase volume by 0.1f if 'Up Arrow' is pressed
        if(Input.GetKeyUp(KeyCode.UpArrow))
        {
            if(videoPlayer.GetDirectAudioVolume(0) + 0.1f < 1f)
                videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) + 0.1f);
            else
                videoPlayer.SetDirectAudioVolume(0, 1f);
        }

        // Decrease volume by 0.1f if 'Down Arrow' is pressed
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (videoPlayer.GetDirectAudioVolume(0) - 0.1f > 0f)
                videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) - 0.1f);
            else
                videoPlayer.SetDirectAudioVolume(0, 0f);
        }

        // When video is playing
        if (videoPlayer.isPlaying)
        {
            // Skip to second last second
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FinishPlaying(videoPlayer);
            }
        }
    }
}