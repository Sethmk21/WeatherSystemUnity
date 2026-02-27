using UnityEngine;

public class SnowingState : WeatherState
{
    public GameObject[] roads = new GameObject[24];
    public PhysicsMaterial ice;
    public SnowingState()
    {
        lightIntensityMult = 1f;
        availableWeatherStates = new WeatherState[3];
        availableWeatherStates[0] = this;
        wbr = 1f;

    }

    public override void StartState()
    {
        ps.GetComponent<ChangeRainAmt>().ResetCurrentIndex();
        base.StartState();
        //dayManager.GetComponent<Light>().color = weatherColor;
        ps.GetComponent<ChangeRainAmt>().ChangeEmmission(10);

        foreach(GameObject road in roads)
        {
            road.GetComponent<BoxCollider>().enabled = true;
            road.GetComponent<MeshCollider>().material = ice;
            road.GetComponent<PlayerOnIce>().enabled = true;
        }


    }

    public override WeatherState GetNextState()
    {
        fireManager.Instance.weatherBasedR = 1f;
        int randomInt = Random.Range(0, availableWeatherStates.Length);
        if (availableWeatherStates[randomInt].ps != ps)
        {
            ps.GetComponent<ChangeRainAmt>().ChangeEmmission(0);
        }

        foreach (GameObject road in roads)
        {
            road.GetComponent<BoxCollider>().enabled = false;
            road.GetComponent<MeshCollider>().material = null;
            road.GetComponent<PlayerOnIce>().enabled = false;
        }
        return availableWeatherStates[randomInt];
    }
}
