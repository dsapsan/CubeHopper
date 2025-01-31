﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMotor : MonoBehaviour
{
    [SerializeField]
    private Camera MainCamera;

    [SerializeField]
    private Vector3 Direction;

    [SerializeField]
    private float MinSpeed;

    [SerializeField]
    private float MinAverageThreshold;

    [SerializeField]
    private float AverageSpeed;

    [SerializeField]
    private float AverageMaxThreshold;

    [SerializeField]
    private float MaxSpeed;

    [SerializeField]
    private float SpeedMultiplierPerPlatform;

    private Transform MotorTransorm;
    private Vector3 StartPosition;
    private float Speed;
    private float SpeedMultiplier = 1f;

    private void Start()
    {
        MotorTransorm = transform;
        StartPosition = MotorTransorm.localPosition;

        Overlord.Progress.Changed += OnProgressChanged;
        Overlord.PlayerPosition.Changed += OnPlayerPositionChanged;

        EventDispatcher<NextPlatformEvent>.OnEvent += OnNextPlatform;
    }

    private void OnNextPlatform(NextPlatformEvent obj)
    {
        SpeedMultiplier *= SpeedMultiplierPerPlatform;
    }

    private void OnPlayerPositionChanged(Vector3 obj)
    {
        var HeightPosition = MainCamera.WorldToScreenPoint(obj).y / Screen.height;

        if (HeightPosition < AverageMaxThreshold)
            Speed = Mathf.Lerp(MaxSpeed, AverageSpeed, HeightPosition / AverageMaxThreshold);
        else if (HeightPosition < MinAverageThreshold)
            Speed = AverageSpeed;
        else
            Speed = Mathf.Lerp(AverageSpeed, MinSpeed, (HeightPosition - MinAverageThreshold) / (1f - MinAverageThreshold));
    }

    private void OnProgressChanged(GameProgress obj)
    {
        if (obj == GameProgress.Beginning)
        {
            MotorTransorm.localPosition = StartPosition;
            SpeedMultiplier = 1f;
        }
    }

    void Update ()
    {
        if (Overlord.Progress.Value == GameProgress.Processing)
            MotorTransorm.localPosition += Direction * Speed * SpeedMultiplier * Time.deltaTime;
    }

    private void OnDestroy()
    {
        Overlord.Progress.Changed -= OnProgressChanged;
    }
}
