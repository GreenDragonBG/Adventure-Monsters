using System;
using BezierSolution;
using UnityEngine;

public class VineTouchSensor : MonoBehaviour
{
   private BezierSpline spline;
   
   [SerializeField]public GameObject lightPrefab;

   private void Start()
   {
      spline = GetComponent<BezierSpline>();
   }

   private void OnCollisionEnter2D(Collision2D other)
   {
       if (other.gameObject.CompareTag("Player"))
       {
           GameObject lightInstance =Instantiate(lightPrefab,Vector2.zero, Quaternion.identity);
           BezierWalkerWithSpeed walker = lightInstance.GetComponent<BezierWalkerWithSpeed>();
         
           spline.FindNearestPointTo(other.transform.position,out float startingT);
         
           walker.spline = spline;
           walker.NormalizedT =  startingT;
       }
   }
}
