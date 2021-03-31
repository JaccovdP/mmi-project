using Assets.Scripts;
using Feedback;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using VisualFeedback;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public bool TestRunning { get => currentTest != null; }
    public bool TestCompleted { get => currentTest.Time != 0; }
    public bool ExperimentCompleted { get => targetsCompleted == targetsTotal; }

    static int? identifier;
    public static int Identifier
    {
        get
        {
            if (!identifier.HasValue)
            {
                identifier = UnityEngine.Random.Range(1, int.MaxValue);
            }
            return identifier.Value;
        }
    }

    public static bool groundTruthModeActive = false;

    public FeedbackType feedbackType;
    public const int targetsTotal = 10;
    

    public GameObject canvas;
    public GameObject targetPrefab;
    

    public AudioClip tickSoundClip;
    public AudioClip successSoundClip;

    Feedback.Feedback feedback;
    

    FeedbackTest currentTest;
    GameObject currentTargetObject;
    List<FeedbackTest> completedTests = new List<FeedbackTest>(targetsTotal);
    float startTime;

    int longestMobileSide;
    int targetRadius;

    /// <summary>
    /// Distance from the starting point measured in percentage of the smallest screen size
    /// </summary>
    List<float> targetRadii = new List<float>();

    int targetsCompleted = 0;
    // Start is called before the first frame update
    void Start()
    {
        longestMobileSide = Mathf.Max(Screen.width, Screen.height);
        targetRadius = longestMobileSide / 25;
        if (feedbackType == FeedbackType.Audio)
        {
            feedback = new AudioFeedback(gameObject);
        }
        else
        {
            feedback = new HapticFeedback(gameObject);
        }
        float inc = 0.5f / (targetsTotal / 2);
        int half = targetsTotal / 2;
        for (int i = 0; i < targetsTotal; i++)
        {
            targetRadii.Add((i % half + 1) * inc);
        }
        AudioFeedback.successSound = successSoundClip;
        AudioFeedback.tickSound = tickSoundClip;
    }

    // Update is called once per frame
    void Update()
    {
        if (TestRunning)
        {
            if (Input.touchCount > 0)
            {
                if (!TestCompleted)
                {
                    DoTest(Input.GetTouch(0));
                }
            }
            else
            {
                ResetTest();
            }
        }
        else
        {
            
            if (Input.touchCount > 0)
            {
                if (!ExperimentCompleted)
                {
                    StartTest(Input.GetTouch(0));
                }
            }
            else if(ExperimentCompleted)
            {
                SceneManager.LoadScene("Main");
            }

        }


        // Register input

        // Update test object
        // Time and path

        // Save test in list



    }

    void DoTest(Touch touch)
    {
        Point position = new Point(touch.position);
        float dist = currentTest.Path.Count > 0 ? currentTest.Path[currentTest.Path.Count - 1].Distance(position) : 0;
        currentTest.Distance += dist;
        currentTest.Path.Add(position);
        
        if (currentTest.Target.GoalReached(position))
        {
            CompleteTest();
        }
        else
        {
            int interval = (int)Mathf.Clamp((currentTest.Target.center - touch.position).magnitude * 2, 80, 1000);
            feedback.GiveFeedback(interval);
        }
    }

    void ResetTest()
    {
        currentTest = null;
    }

    void CompleteTest()
    {
        Debug.Log("dist measured: " + currentTest.Distance);
        Debug.Log("dist optimal: " + (currentTest.target.Distance(currentTest.Path[0]) - targetRadius));
        currentTest.Time = Time.time - startTime;
        completedTests.Add(currentTest);
        targetsCompleted++;

        // give feedback that they are there
        feedback.Success();
        if (targetsCompleted >= targetsTotal)
        {
            // End
            CompleteExperiment();
        }
    }

    void StartTest(Touch touch)
    {
        currentTest = new FeedbackTest
        {
            ID = targetsCompleted + 1,
            Target = CreateNewTarget(touch),
            Path = new List<Point>()
        };
        currentTest.target = currentTest.Target.center;
        
        Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector2(currentTest.target.x, currentTest.target.y));
        if (currentTargetObject == null)
        {
            currentTargetObject = GameObject.Instantiate(targetPrefab, canvas.transform);
        }
        RectTransform t = (RectTransform)currentTargetObject.transform;
        t.position = pos;
        t.sizeDelta = new Vector2(currentTest.Target.radius, currentTest.Target.radius); ;

        startTime = Time.time;
    }

    void CompleteExperiment()
    {
        Payload payload = new Payload()
        {
            feedbackType = (groundTruthModeActive ? "Baseline_" : "") + (feedbackType == FeedbackType.Audio ? "Audio" : "Haptic"),
            participant = Identifier,
            results = completedTests,
            radius = targetRadius
        };
        string json = JsonUtility.ToJson(payload);
        UnityWebRequest webRequest = new UnityWebRequest("https://mmi-api.cyniox.nl/results/", "POST");
        byte[] encodedPayload = new System.Text.UTF8Encoding().GetBytes(json);
        webRequest.uploadHandler = new UploadHandlerRaw(encodedPayload);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("cache-control", "no-cache");

        UnityWebRequestAsyncOperation requestHandel = webRequest.SendWebRequest();
        requestHandel.completed += delegate (AsyncOperation pOperation) {
            Debug.Log(webRequest.responseCode);
            Debug.Log(webRequest.downloadHandler.text);
        };
        
    }


    Target CreateNewTarget(Touch touch)
    {
        float radius = targetRadii[targetsCompleted] * longestMobileSide;

        Vector2 pos;
        do
        {
            pos = UnityEngine.Random.insideUnitCircle.normalized * radius + touch.position;
        }
        while (!(pos.x > 0 && pos.x < Screen.width && pos.y > 0 && pos.y < Screen.height));
        //TODO Give pos etc

        Target target = new Target(new Point(pos), targetRadius);
        return target;
    }
}

[Serializable]
public class Payload
{
    public int participant;
    public int radius;
    public string feedbackType;
    public List<FeedbackTest> results;
}


[Serializable]
public enum FeedbackType
{
    Haptic,
    Audio
}



[Serializable]
public class FeedbackTest
{
    public int ID;
    public float Time;
    public List<Point> Path = new List<Point>();
    [NonSerialized]
    public Target Target;
    public Point target; 
    public float Distance;
}