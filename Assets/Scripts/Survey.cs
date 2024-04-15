using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Survey : MonoBehaviour
{
    public TMP_Text surveyTitle;
    public List<SurveySection> surveySections = new List<SurveySection>();

    public void GetSurvey()
    {
        var content = surveyTitle.text + "\n";

        foreach (SurveySection section in surveySections)
        {
            content += section.GetSection() + "\n";
        }
        Debug.Log(content);

        string rootPath = Application.persistentDataPath;
        string userID = PlayerPrefs.GetString("userId");
        string path = rootPath + "/ExperimentData/"+userID+"/"; 

        string destinationPath = path + surveyTitle.text + "_"+ userID + ".txt";
        System.IO.File.WriteAllText(destinationPath, content);
    }

    private void OnDestroy()
    {
        GetSurvey();
    }


}
