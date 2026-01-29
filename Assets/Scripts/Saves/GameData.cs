using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
    
    //PlayerAbilities
    public bool hasUnlockedDash;
    
    // Parallax
    public List<string> parallaxKeys = new List<string>();
    public List<Vector3> parallaxValues = new List<Vector3>();
    
    //Gates
    public List<string> finishedGates = new List<string>();
    
    //Bosses
        //Chomper
    public bool chomperDead;
    public Vector3 chomperPos;
        //Mush
        public bool mushIsDead;
}