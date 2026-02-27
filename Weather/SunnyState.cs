using UnityEngine;

using UnityEngine.ProBuilder;

public class SunnyState : WeatherState
{
    public SunnyState()
    {
        lightIntensityMult = 1.2f;
        availableWeatherStates = new WeatherState[2];
        availableWeatherStates[0] = this;
        wbr = 1;
        //dayManager = GetComponent<DayManager>();
    }

    public override void StartState()
    {
       
        print("Starting sunny");
        base.StartState();
        //dayManager.GetComponent<Light>().color = weatherColor;
        dayManager.numOfRain = 0;
        fireManager.Instance.isRaining = false;
        
    }
    public override WeatherState GetNextState()
    {
        int randomInt = Random.Range(0, availableWeatherStates.Length);
        return availableWeatherStates[randomInt];
    }



}
