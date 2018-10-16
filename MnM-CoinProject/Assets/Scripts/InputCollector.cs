using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputCollector : MonoBehaviour
{
    public static InputCollector Instance;
    
    [Header("Under Canvas")]
    [SerializeField]
    private InputField GenderInputField;
    [SerializeField]
    private InputField VideoIdInputField;
    
    [HideInInspector]
    public int VideoID;
    [HideInInspector]
    public string Gender;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Instance = this;
        // Allow input
        GenderInputField.Select();
    }

    public void ProceedToSelectVideo()
    {
        // Create Folder and CSV for user based on input
        Gender = GenderInputField.text;

        // Allow input
        VideoIdInputField.gameObject.SetActive(true);
        VideoIdInputField.Select();
    }

    public void Start360MovieTheatre()
    {
        // Try Parse VideoID Input into int and put in VideoID
        if(!int.TryParse(VideoIdInputField.text,out VideoID))
        {
            Debug.LogWarning("VideoID Input cannot be parse into int.");
        }
        
        // Change to MainScene
        SceneManager.LoadScene("MainScene");
    }
}
