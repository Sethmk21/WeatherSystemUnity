using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WindTracker : MonoBehaviour
{

    public List<GameObject> things = new List<GameObject>();
    public BoxCollider box;
    public Vector3 startpos;
    private void Start()
    {
        box = GetComponent<BoxCollider>();
        startpos = box.center;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.isKinematic == false && other.attachedRigidbody.mass <= 200)
        {
            things.Add(other.gameObject);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        things.Remove(other.gameObject);
    }
    public void TurnOff()
    {
        box.center -= new Vector3(0, 300, 0);
        Invoke("off", 0.2f);
    }
    public void TurnOn()
    {
        on();
        box.center = startpos;
   
    }

    private void off()
    {
        box.enabled = false;
    }
    private void on()
    {
        box.enabled = true;
    }
}
