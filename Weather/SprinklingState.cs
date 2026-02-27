using UnityEngine;

using UnityEngine.ProBuilder;

public class SprinklingState : WeatherState
{
    public SprinklingState()
    {
        lightIntensityMult = 0.9f;
        availableWeatherStates = new WeatherState[3];
        availableWeatherStates[0] = this;
        wbr = 1.15f;
        //availableWeatherStates[1] = GetComponent<SunnyState>();
        //dayManager = GetComponent<DayManager>();
    }

    public override void StartState()
    {
        ps.GetComponent<ChangeRainAmt>().ResetCurrentIndex();
        dayManager.numOfRain++;
        //print("Starting Sprinkling");
        base.StartState();
        //dayManager.GetComponent<Light>().color = weatherColor;
        ps.GetComponent<ChangeRainAmt>().ChangeEmmission(8);
        fireManager.Instance.isRaining = true;
        
        //fireManager.Instance.weatherBasedR = 1.15f;

    }

    public override WeatherState GetNextState()
    {
        fireManager.Instance.weatherBasedR = 1f;
        int randomInt = Random.Range(0, availableWeatherStates.Length);
        if (availableWeatherStates[randomInt].ps != ps)
        {
            ps.GetComponent<ChangeRainAmt>().ChangeEmmission(0);
        }
        return availableWeatherStates[randomInt];
    }

}
