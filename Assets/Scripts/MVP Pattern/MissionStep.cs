using System;
using UnityEngine;

[Serializable]
public class MissionStep
{
    public string missionID;
    [TextArea(2, 4)]
    public string hintText; // El texto que se mostrará como objetivo
}