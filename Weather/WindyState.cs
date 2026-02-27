using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindyState : WeatherState
{
    private bool once = false;
    private Rigidbody rb;
    private Vector3 windDirection = -Vector3.right;
    private int index;
    public float windStrength = 0.8f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public WindTracker[] zones;
    public WindyState()
    {
        lightIntensityMult = 1f;
        availableWeatherStates = new WeatherState[3];
        availableWeatherStates[0] = this;
        wbr = 1;
        //dayManager = GetComponent<DayManager>();
    }
    public override void StartState()
    {
        base.StartState();
        index = 2;
        ps.GetComponent<ChangeWindAmt1>().ChangeEmmission(50, index);
        fireManager.Instance.isWindy = true;

    }
    public override void DoWeather()
    {
        base.DoWeather();
        if (dayManager.timeOfDay % 10 > 0 && dayManager.timeOfDay % 10 < 1 && once == false)
        {
            once = true;
            Invoke("changeOnce", 2);
            index = Random.Range(0, zones.Length);
            //ps.GetComponent<ChangeWindAmt1>().ChangeEmmission(50, index);
            //if (dayManager.timeOfDay % 4 > 0 && dayManager.timeOfDay % 4 < 1 && zones[index].things.Count > 0)
            //{
            //    for (int i = 0; i < zones[index].things.Count; i++)
            //    {

            //        if (zones[index].things[i] != null)
            //        {
            //            rb = zones[index].things[i].GetComponent<Rigidbody>();
            //            if (rb != null && rb.useGravity == true)
            //            {
            //                //print("adding wind force to " +  gameObject.name);
            //                rb.linearVelocity = Vector3.zero;
            //                float gust = Mathf.PerlinNoise(Time.time * 0.5f, 0f);
            //                Vector3 finalWind = ((windDirection * windStrength) / Mathf.Max(rb.mass, 2)) * gust;
            //                rb.AddForce(finalWind, ForceMode.Force);
            //            }
            //        }

            //    }
            //}

        }
    }
    public override WeatherState GetNextState()
    {

        ps.GetComponent<ChangeWindAmt1>().ChangeEmmission(0, 1);
        int randomInt = Random.Range(0, availableWeatherStates.Length);
        fireManager.Instance.isWindy = false;
        return availableWeatherStates[randomInt];
    }
    private void changeOnce()
    {
        once = false;
    }
}
