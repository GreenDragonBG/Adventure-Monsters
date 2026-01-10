using System;
using BezierSolution;
using UnityEngine;

public class VineTouchSensor : MonoBehaviour
{
   private BezierSpline spline;
   private Collider2D collider2D;
   [SerializeField]public GameObject lightPrefab;

   private void Start()
   {
      spline = GetComponent<BezierSpline>();
      collider2D = GetComponent<Collider2D>();
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

   public void WhenDestroyed()
   {
       GameObject lightInstance =Instantiate(lightPrefab,Vector2.zero, Quaternion.identity);
       BezierWalkerWithSpeed walker = lightInstance.GetComponent<BezierWalkerWithSpeed>();
       walker.MovingForward = false;
       walker.spline = spline;
       walker.NormalizedT =  1f;
       
       collider2D.enabled = false;
   }
}
