using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentPipeline : MonoBehaviour
{
    public static ExperimentPipeline Instance { get; private set; }

    [SerializeField] private Experiment experiment;
    private bool isDebugMode =false;
    private int currentSceneIndex = 0;
    private float currentTime = 0;
    private bool hasStarted = false;
    private List<SceneDuration> sceneDurations;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            sceneDurations = experiment.sceneDurations;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
    }


    public void StartExperiment()
    {
        currentSceneIndex = 0;
        hasStarted = true;
        ResetTimer();
        SceneManager.LoadScene(sceneDurations[currentSceneIndex].sceneName);
    }

    public void ResetTimer()
    {
        currentTime = 0;
    }

    public void StopTimer()
    {
        hasStarted = false;
    }

    public void NextScene()
    {
        currentSceneIndex++;
        if (currentSceneIndex < sceneDurations.Count)
        {
          
            ResetTimer();
            isDebugMode = sceneDurations[currentSceneIndex].debugEnabled;
            SceneManager.LoadScene(sceneDurations[currentSceneIndex].sceneName);
        }
        else
        {
            StopTimer();
            DataCollector.Instance.StopAcquisition();
            Debug.Log("Experiment Finished");
            SceneManager.LoadScene("Welcome");
        }
    }

    public void Update()
    {
        if(hasStarted)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= sceneDurations[currentSceneIndex].durationInSeconds)
            {
                NextScene();
            }
        }
        
    }

    public void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            normal = { textColor = Color.white },
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        // Optionally, you can also set alignment, word wrap, etc.
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.wordWrap = true;

        if (hasStarted && isDebugMode)
        {
            /*        GUILayout.Label("Current Scene: " + sceneDurations[currentSceneIndex].sceneName);*/

            float timeLeft = (sceneDurations[currentSceneIndex].durationInSeconds - currentTime);
            int minutes = (int)timeLeft / 60;
            int seconds = (int)timeLeft % 60;

            string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            GUILayout.Label("Time Left: " + formattedTime, labelStyle);
        }
    }
}
