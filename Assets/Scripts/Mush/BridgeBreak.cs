using System;
using UnityEngine;

public class BridgeBreak : MonoBehaviour
{
    [SerializeField] private HingeJoint2D partToBreak;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BreakBridge();
        }
    }

    private void BreakBridge()
    {
        partToBreak.enabled = false;
    }
}
