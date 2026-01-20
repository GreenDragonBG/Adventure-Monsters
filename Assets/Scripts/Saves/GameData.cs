using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool isNewGame = true;
    public string lastScene;
    public Vector3 playerPos;
    public Vector3 cameraPos;
    public List<string> activatedCampfires = new List<string>();
    public List<string> destroyedHearts = new List<string>();
    
    // JSON friendly storage for Parallax
    public List<string> parallaxKeys = new List<string>();
    public List<Vector3> parallaxValues = new List<Vector3>();

    public List<string> finishedGates = new List<string>();
    public bool chomperDead;
    public Vector3 chomperPos;
}