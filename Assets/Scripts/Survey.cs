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
        int surveyNumber = PlayerPrefs.GetInt("surveyNumber", 1);
        string path = rootPath + "/ExperimentData/"+userID+"/"; 

        if(surveyNumber <=3)
        {
            string destinationPath = path + surveyTitle.text +"_" + surveyNumber + "_" + userID + ".txt";
            System.IO.File.WriteAllText(destinationPath, content);
            PlayerPrefs.SetInt("surveyNumber", surveyNumber + 1);
        }
        else
        {
            surveyNumber = 1;
            string destinationPath = path + surveyTitle.text + "_" + surveyNumber + "_" + userID + ".txt";
            System.IO.File.WriteAllText(destinationPath, content);
            PlayerPrefs.SetInt("surveyNumber", surveyNumber+1);
        }
        
    }

    private void OnDestroy()
    {
        GetSurvey();
    }


}
