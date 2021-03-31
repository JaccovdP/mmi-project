using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text serverStatus;
    public Text experimentIdText;
    public Toggle groundTruthToggle;

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Start()
    {
        CheckServerAvailability();
        experimentIdText.text = "Experiment ID: " + Controller.Identifier;
        groundTruthToggle.isOn = Controller.groundTruthModeActive;
    }

    public void CheckServerAvailability()
    {
        UnityWebRequest webRequest = new UnityWebRequest("https://mmi-api.cyniox.nl/", "GET");
        
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("cache-control", "no-cache");

        UnityWebRequestAsyncOperation requestHandel = webRequest.SendWebRequest();
        requestHandel.completed += delegate (AsyncOperation pOperation) {
            Debug.Log(webRequest.responseCode);
            string status = webRequest.responseCode == 200 ? "Online" : "Failed, code: " + webRequest.responseCode.ToString() + "\n error: " + webRequest.error;
            serverStatus.text = "Server Status: " + status;
        };
    }

    public void ToggleGroundTruth()
    {
        Controller.groundTruthModeActive = !Controller.groundTruthModeActive;
    }

    public void StartTest(string testFeedback)
    {
        SceneManager.LoadScene(testFeedback);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
