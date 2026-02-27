using UnityEngine;

using UnityEngine.ProBuilder;

public class PouringState : WeatherState
{
    public PouringState()
    {
        lightIntensityMult = 0.6f;
        availableWeatherStates = new WeatherState[3];
        availableWeatherStates[0] = this;
        wbr = 1.5f;
        //availableWeatherStates[1] = GetComponent<SunnyState>();
        //dayManager = GetComponent<DayManager>();
    }

    public override void StartState()
    {
        ps.GetComponent<ChangeRainAmt>().ResetCurrentIndex();
        dayManager.numOfRain++;
        //print("Starting Pouring");
        base.StartState();
        //dayManager.GetComponent<Light>().color = weatherColor;
        ps.GetComponent<ChangeRainAmt>().ChangeEmmission(24);
        fireManager.Instance.isRaining = true;
        //fireManager.Instance.weatherBasedR = 1.35f;

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
