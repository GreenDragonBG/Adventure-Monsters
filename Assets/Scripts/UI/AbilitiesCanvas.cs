using System;
using UnityEngine;

public class AbilitiesCanvas : MonoBehaviour
{
   [SerializeField] public GameObject target;

   private void Update()
   {
      transform.position = target.transform.position;
   }
}
