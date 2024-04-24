using AdultLink;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using Random = System.Random;

public class GameManipulator : MonoBehaviour
{
    [SerializeField] private HealBar boredomBar;
    [SerializeField] private HealBar flowBar;
    [SerializeField] private HealBar anxietyBar;
    [SerializeField] private HealBar neutralBar;

    const double GoldenRatio = 1.61803398874989484820458683436;
    [SerializeField] private float multiplier = 10;
    [SerializeField] private float boredomFactor = 0.5f;
    [SerializeField] private float startingStep = 3f;

    private float minBoredom = float.MaxValue;
    private float maxBoredom = float.MinValue;
    private float minFlow = float.MaxValue;
    private float maxFlow = float.MinValue;
    private float minAnxiety = float.MaxValue;
    private float maxAnxiety = float.MinValue;
    private float minNeutral = float.MaxValue;
    private float maxNeutral = float.MinValue;
    private int maxIters = 15;

    private Piece piece;
    public Action<float, float, float, float> onDataClassified;
    public float randomo;


    public void Start()
    {
        onDataClassified += updateSpeedChange;

        piece = FindObjectOfType<Piece>();
    }

    public void Update()
    {
        randomo = UnityEngine.Random.Range(0, 1);
    }

    public void changeFallingSpeed(float speed)
    {
        
            piece.stepDelay = speed;
    }



    public void updateSpeedChange(float neutral, float boredom, float flow, float anxiety)
    {


        if (maxIters >0)
        {

            minBoredom = Mathf.Min(minBoredom, boredom);
            maxBoredom = Mathf.Max(maxBoredom, boredom);
            minFlow = Mathf.Min(minFlow, flow);
            maxFlow = Mathf.Max(maxFlow, flow);
            minAnxiety = Mathf.Min(minAnxiety, anxiety);
            maxAnxiety = Mathf.Max(maxAnxiety, anxiety);
            minNeutral = Mathf.Min(minNeutral, neutral);
            maxNeutral = Mathf.Max(maxNeutral, neutral);
            maxIters--;
        }
   



        updateBars(neutral, boredom, flow, anxiety);

    }


    int counter_n = 0;
    int counter_b = 0;
    int counter_f = 0;
    int counter_a = 0;


    float prev_neutral = 0f;
    float prev_boredom = 0f;
    float prev_flow = 0f;
    float prev_anxiety = 0f;

    float max_speed = float.MinValue;
    float prev_speed = float.MinValue;
    int iterator = 0;

    public void updateBars(float neutral, float boredom, float flow, float anxiety)
    {
        var boredomValue = (boredom - minBoredom) / (maxBoredom - minBoredom);
        var flowValue = (flow - minFlow) / (maxFlow - minFlow);
        var anxietyValue = (anxiety - minAnxiety) / (maxAnxiety - minAnxiety);
        var neutralValue = (neutral - minNeutral) / (maxNeutral - minNeutral);

        if (prev_neutral == neutralValue) counter_n++;
        if (prev_boredom == boredomValue) counter_b++;
        if (prev_flow == flowValue) counter_f++;
        if (prev_anxiety == anxietyValue) counter_a++;

        if (counter_b > 5)
        {
            minBoredom = float.MaxValue;
            maxBoredom = float.MinValue;
        }
        if (counter_f > 5)
        {
            minFlow = float.MaxValue;
            maxFlow = float.MinValue;
        }
        if (counter_a > 5)
        {
            minAnxiety = float.MaxValue;
            maxAnxiety = float.MinValue;
        }
        if (counter_n > 5)
        {
            minNeutral = float.MaxValue;
            maxNeutral = float.MinValue;
        }


        boredomBar.changeFill(boredomValue);
        flowBar.changeFill(flowValue);
        anxietyBar.changeFill(anxietyValue);
        neutralBar.changeFill(neutralValue);

        prev_anxiety = anxietyValue;
        prev_boredom = boredomValue;
        prev_neutral = neutralValue;
        prev_flow = flowValue;

        float calculatedSpeed = (flowValue - boredomValue) * anxietyValue + neutralValue;

        if (calculatedSpeed > max_speed)
        {
            max_speed = calculatedSpeed;
        }

        float speed = Mathf.Clamp(calculatedSpeed/max_speed * 3, 0.5f, 3f);
        if(iterator++ < 10)
        {
            // changeFallingSpeed(speed);
            iterator = 0;
        }
         

        prev_speed = speed;
    }

    public void OnDestroy()
    {
        onDataClassified -= updateSpeedChange;
        Debug.Log($"Min/Max Boredom: {minBoredom}/{maxBoredom}");
        Debug.Log($"Min/Max Flow: {minFlow}/{maxFlow}");
        Debug.Log($"Min/Max Anxiety: {minAnxiety}/{maxAnxiety}");
        Debug.Log($"Min/Max Neutral: {minNeutral}/{maxNeutral}");
    }

}
