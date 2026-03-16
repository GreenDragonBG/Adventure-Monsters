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
        foreach (Collider2D c in transform.GetComponentsInChildren<Collider2D>())
        {
            c.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
