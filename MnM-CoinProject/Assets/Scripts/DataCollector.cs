//--------------------------------------------------------------------------------
/*
 * This Script is used for collecing Data generated/User Input
 * 
 * Used in Start Scene, attached to Empty GameObject "DataCollector"
 * Require 3 InputField variables : User ID, Gender(M / m / F / f), Video ID(1 / 2 / 3 / 4)
 * which can be found under Canvas GameObject in Hierarchy.
 * Among the 3 InputFields, only User ID's GameObject need to be active.
 */
//--------------------------------------------------------------------------------

using System.IO;    // For Directory, File, StreamWriter
using UnityEngine;  // Default Unity Script (MonoBehaviour, Application, Tooltip, SerializeField, HideInInspector, DontDestroyOnLoad, Debug, Time)
using UnityEngine.UI; // For InputField
using UnityEngine.SceneManagement; // For SceneManager

public class DataCollector : MonoBehaviour
{
    // For other scripts to access DataCollector
    public static DataCollector Instance;

    [Tooltip("Time interval to collect Headset Position & Rotation. Default: 1.0f")]
    public float dataRecordInterval = 1f;

    [Header("Under Canvas")]
    
    // User ID InputField
    [SerializeField]
    private InputField IdInputField;

    // Gender InputField - (M / m / F / f)
    [SerializeField]
    private InputField GenderInputField;

    // Video ID InputField - (1 / 2 / 3 / 4)
    [SerializeField]
    private InputField VideoIdInputField;
    
    // For holding SteamVR Camera Component
    [HideInInspector]
    public GameObject user;

    // For converting User Input into respective data
    private string dataID;
    [HideInInspector]
    public string Gender;
    [HideInInspector]
    public int VideoID;
    
    // For Recording Head Movement Data
    private bool startRecording = false;
    private float time = 0f;

    // Runs before Start()
    private void Awake()
    {
        // Allow DataCollector Instance to be alive after change scene
        DontDestroyOnLoad(this);
        
        // Creates a folder to hold data output if the folder does not exist.
        if (!Directory.Exists(Application.dataPath + "/Data"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Data");
        }
    }

    // Runs at the start of first frame
    private void Start()
    {
        // Set this instance of DataCollector to allow other scripts to access its variables and data
        Instance = this;
        
        // Allow input of User ID
        IdInputField.Select();
    }

    // Runs at every frame
    private void Update()
    {
        // Start Recording when bool is set to true
        if (startRecording)
        {
            time += Time.deltaTime;

            // Record Head Movement Data every data Record Interval(Default: 1.0f)
            if (time > dataRecordInterval)
            {
                // Reset timer
                time = 0;
                
                // Write generated data into csv file
                PushData(GenerateData());
            }
        }
    }
    
    // Returns generated Head Movement Data string
    private string GenerateData()
    {
        string data = "";
        
        // Get Time Information into data string
        data += System.DateTime.Now.ToString("HH");
        data += ":";
        data += System.DateTime.Now.ToString("mm");
        data += ":";
        data += System.DateTime.Now.ToString("ss");
        data += ":";
        data += System.DateTime.Now.ToString("FFF");

        // Seperator
        data += ",";

        // Get Headset Position in vector 3 format
        string posstr = user.GetComponent<SteamVR_Camera>().head.transform.position.ToString("F3");

        // Change , to . to prevent Position data to be seperated
        data += ChangeLetters(posstr, ',', '.');

        // Seperator
        data += ",";

        // Get Headset Rotation in vector 3 format
        string rotstr = user.GetComponent<SteamVR_Camera>().head.transform.rotation.ToString("F3");

        // Change , to . to prevent Position data to be seperated
        data += ChangeLetters(rotstr, ',', '.');
        
        return data;
    }

    // Edit the current file by adding the new text
    private void PushData(string text)
    {
        // Open csv file at the path return from GetPath()
        StreamWriter sw = File.AppendText(GetPath());
        
        // Write onto the file
        sw.WriteLine(text);

        // Close the file
        sw.Close();
    }

    // Returns the file path being used to store the data
    private string GetPath()
    {
        // If the filepath already exists, create a new file with a duplicate number
        string filePath = Application.dataPath + "/Data/" + dataID + ".csv";
        int duplicateCounts = 0;
        while (true)
        {
            if (File.Exists(filePath))
            {
                ++duplicateCounts;
                filePath = Application.dataPath + "/Data/" + dataID + "(" + duplicateCounts.ToString() + ")" + ".csv";
            }
            else
                break;
        }
        return filePath;
    }

    // Create the csv file
    private void CreateCSV()
    {
        // Create csv file with the path return from GetPath()
        StreamWriter output = File.CreateText(GetPath());

        // Write Title of Data going to be recorded
        output.WriteLine("Time, Position, Rotation");

        // Close file
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
    
    //--------------------------------------------------------------------------------
    //                                  PUBLIC FUNCTIONS
    //--------------------------------------------------------------------------------

    // Link to User ID InputField OnEndEdit()
    // This Registers User ID and use it to create csv file.
    // Then Set Gender InputField to active and allow its input.
    public void ProceedToStateGender()
    {
        // If no text, dont let them proceed
        if (IdInputField.text == null)
            return;

        // Register User ID
        dataID = IdInputField.text;

        // Create csv file with User ID as file name
        CreateCSV();

        // Allow input of Gender
        GenderInputField.gameObject.SetActive(true);
        GenderInputField.Select();
    }

    // Link to Gender InputField OnEndEdit()
    // This Registers Gender based on input, which is used to determine the directory of video files.
    // Then Set Video ID InputField to active and allow its input.
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

        // Allow input of Video ID
        VideoIdInputField.gameObject.SetActive(true);
        VideoIdInputField.Select();
    }

    // Link to Video ID InputField OnEndEdit()
    // This Registers Video ID, which is used to determine the video file to play.
    // Then Set bool to true so can start Recording Head Movement Data.
    // And Change Scene to Main Scene (Watch video)
    public void Start360MovieTheatre()
    {
        // Try Parse VideoID Input into int and put in VideoID
        if (VideoIdInputField.text == null || !int.TryParse(VideoIdInputField.text, out VideoID))
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
}