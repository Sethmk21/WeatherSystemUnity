using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
using static Unity.VisualScripting.Metadata;
using static UnityEngine.ParticleSystem;

public class ChangeRainAmt : MonoBehaviour
{
    public ParticleSystem[] particleSystems;
    private int counter = 0;
    private int currentIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

 
    void Update()
    {
        if (currentIndex < particleSystems.Length)
        {
            if (counter % 3 == 0) 
            {
                particleSystems[currentIndex].Play();
                currentIndex++;
            }

            
        }
        counter++;
        if (counter % 10 == 0)
        {
            GameObject[] players = QuestManager.Instance.GetPlayerList();
            foreach (ParticleSystem p in particleSystems)
            {
                if (players.Length > 1)
                {
                    if ((p.transform.position - players[0].transform.position).magnitude > 200 && (p.transform.position - players[1].transform.position).magnitude > 200)
                    {
                        p.Stop();
                        //print("stopped");
                    }
                    else
                    {
                       
                        if (p.isStopped)
                        {
                            p.Play();
                            //print("played");
                        }
                    }
                }
                else
                {
                    if (players.Length == 1)
                    {
                        //print((p.transform.position - players[0].transform.position).magnitude > 200);
                        if ((p.transform.position - players[0].transform.position).magnitude > 200)
                        {
                            p.Stop();
                            //print("stopped");
                        }
                        else
                        {
                            //print("played");
                            if (p.isStopped)
                            {
                                p.Play();
                                //print("played");
                            }
                        }
                    }
                }
            }
        }
    }
    //Function to activeate one child object per frame
    public void ResetCurrentIndex()
    {
        currentIndex = 0;
    }
    public void ChangeEmmission(int newRate)
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            StartCoroutine(fadeRain(particleSystem, newRate, 10));
            //;
        }
    }

    IEnumerator fadeRain(ParticleSystem particleSystem, int newrate, int duration)
    {
        var emissionModule = particleSystem.emission;
        float startRate = emissionModule.rateOverTime.constant;
        // Initialize current rate
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float newRate = Mathf.Lerp(startRate, newrate, t);
            emissionModule.rateOverTime = newRate;

            yield return null;
        }
        if(newrate == 0)
        {
            foreach(ParticleSystem ps in particleSystems)
            {
                ps.Stop();
            }
            //gameObject.SetActive(false);
        }
        
    }
}
