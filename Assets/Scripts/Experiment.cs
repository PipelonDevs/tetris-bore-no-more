using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SceneDuration
{
    public string sceneName;
    public float durationInSeconds;
    public bool debugEnabled;
}


[CreateAssetMenu(fileName = "Experiment", menuName = "Experiment")]
public class Experiment : ScriptableObject
{
    public List<SceneDuration> sceneDurations ;
    



}
