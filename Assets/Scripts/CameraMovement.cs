﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float magnitudeThreshold = 0.1F;
    private float move = 10;
    private static int maxSamples = 1;

    private float loLim = 0.005F;
    private float hiLim = 0.08F;
    private int steps = 0;
    private bool stateH = false;
    private float fHigh = 10.0F;
    private float curAcc = 0F;
    private float fLow = 0.1F;
    private float avgAcc;
    private int lastStep = 0;

    // Variables for vector averaging
    private float avgMagnitude = 0;
    private float[] pastMagnitudes = new float[maxSamples];
    private float avgDirection = 0;
    private float[] pastDirections = new float[maxSamples];
    private int head = 0;
    private float PI = Mathf.PI;


    // Use this for initialization
    void Start()
    {
        for(int i = 0; i<maxSamples; i++) {
            pastMagnitudes[i] = 0;
            pastDirections[i] = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (findVector() && stepDetector()) {
            Vector3 temp = new Vector3(transform.position.x + move*avgMagnitude * Mathf.Cos(avgDirection), -3, transform.position.z + move*avgMagnitude * Mathf.Sin(avgDirection));
            transform.position = temp;
        }

    }


    public bool stepDetector()
    {
        curAcc = Mathf.Lerp(curAcc, Input.acceleration.magnitude, Time.deltaTime * fHigh);
        avgAcc = Mathf.Lerp(avgAcc, Input.acceleration.magnitude, Time.deltaTime * fLow);
        float delta = curAcc - avgAcc;
        if (!stateH)
        {
            if (delta > hiLim)
            {
                stateH = true;
                steps++;
            }
        }
        else
        {
            if (delta < loLim)
            {
                stateH = false;
            }
        }

        avgAcc = curAcc;
        //calDistance(steps);

        if (lastStep == steps)
        {
            return false;
        }
        else
        {
            lastStep = steps;
            return true;
        }
    }

    public bool findVector()
    {
        float ax = Input.acceleration.x;
        float az = Input.acceleration.z;

        float magnitude = Mathf.Sqrt(ax * ax + az * az);
        avgMagnitude = avgMagnitude + ((magnitude - pastMagnitudes[head]) / maxSamples);
        pastMagnitudes[head] = magnitude;

        float direction;

        if (ax == 0)
        {
            if (az > 0)
            {
                direction = PI / 2;
            }
            else
            {
                direction = 3 * PI / 2;
            }
        }
        else if (az == 0)
        {
            if (ax > 0)
            {
                direction = 0;
            }
            else
            {
                direction = PI;
            }
        }
        else
        {
            direction = Mathf.Atan(az / ax);

            if (az > 0 && ax < 0)
            {
                direction = PI - direction;
            }
            else if (az < 0 && ax < 0)
            {
                direction += PI;
            }
            else if (az < 0 && ax > 0)
            {
                direction = 2 * PI - direction;
            }
        }
        avgDirection = avgDirection + (direction - pastDirections[head]) / maxSamples;
        pastDirections[head] = direction;
        head++;
        if (head == maxSamples)
        {
            head = 0;
        }

        return avgMagnitude > magnitudeThreshold;
    }
}
