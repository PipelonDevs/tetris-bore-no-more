using System.Collections;
using System.Collections.Generic;
using Unity.Sentis;
using UnityEngine;
using System;


public class Classifier : MonoBehaviour
{
    [SerializeField] private ModelAsset modelAsset;
    private Model runtimeModel;
    IWorker worker;

    public Action<TensorFloat> onDataAcquired;
    private GameManipulator gameManipulator;

    void Start()
    {
        gameManipulator = FindObjectOfType<GameManipulator>();
        onDataAcquired += OnDataAcquired;

        runtimeModel = ModelLoader.Load(modelAsset);





        worker = WorkerFactory.CreateWorker(BackendType.CPU, runtimeModel, verbose: true);


    }





    private void OnDataAcquired(TensorFloat inputTensor)
    {
        worker.Execute(inputTensor);

        // Iterate through the output layer names of the model and print the output from each
        foreach (var outputName in runtimeModel.outputs)
        {
            TensorFloat outputTensor = worker.PeekOutput(outputName) as TensorFloat;
 
            outputTensor.MakeReadable();
            float[] values = outputTensor.ToReadOnlyArray();
           /* Debug.Log(values[0]+" "+ values[1] + " " + values[2] + " " + values[3]);*/
            int i = 0;
            foreach (var value in outputTensor.ToReadOnlyArray())
            {
        /*        Debug.Log(i +" "+value);*/
                i++;
            }

            gameManipulator.updateSpeedChange(values[0], values[1], values[2], values[3]);
        }

     

    }
}