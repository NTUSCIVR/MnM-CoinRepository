using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataCollector : MonoBehaviour
{
    public static DataCollector Instance;

    [Tooltip("Time interval to collect Headset Position & Rotation. Default: 1.0f")]
    public float dataRecordInterval = 1f;

    [Header("Under Canvas")]
    [SerializeField]
    private InputField IdInputField;
    [SerializeField]
    private InputField GenderInputField;
    [SerializeField]
    private InputField VideoIdInputField;

    [HideInInspector]
    public bool startRecording = false;
    [HideInInspector]
    public GameObject user;
    [HideInInspector]
    public int VideoID;
    [HideInInspector]
    public string Gender;
    
    private string dataID;
    private float time = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (!Directory.Exists(Application.dataPath + "/Data"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Data");
        }
    }

    private void Start()
    {
        Instance = this;
        // Allow input
        IdInputField.Select();
    }
    
    public void ProceedToStateGender()
    {
        // If no text, dont let them proceed
        if (IdInputField.text == null)
            return;

        dataID = IdInputField.text;

        CreateCSV();

        // Allow input
        GenderInputField.gameObject.SetActive(true);
        GenderInputField.Select();
    }

    public void ProceedToSelectVideo()
    {
        // If no text, dont let them proceed
        if (GenderInputField.text == null)
            return;

        // Set Gender for user based on input
        switch (GenderInputField.text)
        {
            case "F":
                Gender = "Female";
                break;
            case "f":
                Gender = "Female";
                break;
            case "M":
                Gender = "Male";
                break;
            case "m":
                Gender = "Male";
                break;
        }

        // Allow input
        VideoIdInputField.gameObject.SetActive(true);
        VideoIdInputField.Select();
    }

    public void Start360MovieTheatre()
    {
        // Try Parse VideoID Input into int and put in VideoID
        if(VideoIdInputField.text == null || !int.TryParse(VideoIdInputField.text,out VideoID))
        {
            // If no text, dont let them proceed
            Debug.LogWarning("VideoID Input cannot be parse into int.");
            return;
        }

        // Start recording Head Movement
        startRecording = true;

        // Change to MainScene
        SceneManager.LoadScene("MainScene");
    }

    // Generate Head Movement Data
    private string GenerateData()
    {
        string data = "";
        data += System.DateTime.Now.ToString("HH");
        data += ":";
        data += System.DateTime.Now.ToString("mm");
        data += ":";
        data += System.DateTime.Now.ToString("ss");
        data += ":";
        data += System.DateTime.Now.ToString("FFF");
        data += ",";
        string posstr = user.GetComponent<SteamVR_Camera>().head.transform.position.ToString("F3");
        data += ChangeLetters(posstr, ',', '.');
        data += ",";
        string rotstr = user.GetComponent<SteamVR_Camera>().head.transform.rotation.ToString("F3");
        data += ChangeLetters(rotstr, ',', '.');
        return data;
    }

    private void Update()
    {
        if (startRecording)
        {
            // Record Head Movement Data every dataRecordInterval
            time += Time.deltaTime;
            if (time > dataRecordInterval)
            {
                time = 0;
                PushData(GenerateData());
            }
        }
    }

    // Edit the current file by adding the new text
    private void PushData(string text)
    {
        StreamWriter sw = File.AppendText(GetPath() + ".csv");
        sw.WriteLine(text);
        sw.Close();
    }

    // Returns the file path being used to store the data
    private string GetPath()
    {
        // If the filepath already exists, create a new file with a duplicate number
        string filePath = Application.dataPath + "/Data/" + dataID;
        int duplicateCounts = 0;
        while (true)
        {
            if (Directory.Exists(filePath))
            {
                ++duplicateCounts;
                filePath = Application.dataPath + "/Data/" + dataID + "(" + duplicateCounts.ToString() + ")";
            }
            else
                break;
        }
        return filePath;
    }

    private void CreateCSV()
    {
        // Create the csv file
        StreamWriter output = File.CreateText(GetPath() + ".csv");
        output.WriteLine("Time, Position, Rotation");
        output.Close();
    }

    // Change "letter" in "str" to "toBeLetter"
    private string ChangeLetters(string str, char letter, char toBeLetter)
    {
        char[] ret = str.ToCharArray();
        for (int i = 0; i < ret.Length; ++i)
        {
            if (ret[i] == letter)
            {
                ret[i] = toBeLetter;
            }
        }
        return new string(ret);
    }
}
