using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
using static Unity.VisualScripting.Metadata;
using static UnityEngine.ParticleSystem;

public class ChangeWindAmt1 : MonoBehaviour
{
    public ParticleSystem[] particleSystems;
    public WindTracker[] triggerZones;
    private int counter = 0;
    private int currentIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

 
    void Update()
    {
        //if (currentIndex < particleSystems.Length)
        //{
        //    if (counter % 3 == 0) 
        //    {
        //        particleSystems[currentIndex].Play();
        //        currentIndex++;
        //    }

            
        //}
        //counter++;
        //if (counter % 10 == 0)
        //{
        //    GameObject[] players = QuestManager.Instance.GetPlayerList();
        //    foreach (ParticleSystem p in particleSystems)
        //    {
        //        if (players.Length > 1)
        //        {
        //            if ((p.transform.position - players[0].transform.position).magnitude > 200 && (p.transform.position - players[1].transform.position).magnitude > 200)
        //            {
        //                p.Stop();
        //                //print("stopped");
        //            }
        //            else
        //            {
                       
        //                if (p.isStopped)
        //                {
        //                    p.Play();
        //                    //print("played");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (players.Length == 1)
        //            {
        //                //print((p.transform.position - players[0].transform.position).magnitude > 200);
        //                if ((p.transform.position - players[0].transform.position).magnitude > 200)
        //                {
        //                    p.Stop();
        //                    //print("stopped");
        //                }
        //                else
        //                {
        //                    //print("played");
        //                    if (p.isStopped)
        //                    {
        //                        p.Play();
        //                        //print("played");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
    //Function to activeate one child object per frame
    public void ResetCurrentIndex()
    {
        currentIndex = 0;
    }
    public void ChangeEmmission(float newRate, int index)
    {
        if (index < particleSystems.Length)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                if (particleSystems[i].isStopped == true)
                {
                    particleSystems[i].Play();
                }
                if (index == i)
                {
                    StartCoroutine(FadeWind(particleSystems[i], newRate, 0.5f));
                    triggerZones[i].TurnOn();

                }
                else
                {
                    StartCoroutine(FadeWind(particleSystems[i], 5f, 0.5f));
                    triggerZones[i].TurnOff();
                }
                //;
            }
        }
        if (newRate <= 0)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                StartCoroutine(FadeWind(particleSystems[i], 0f, 0.5f));
                triggerZones[i].TurnOff();
            }
        }
    }

    IEnumerator FadeWind(ParticleSystem particleSystem, float newrate, float duration)
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
        if(newrate.Equals(0))
        {
            foreach(ParticleSystem ps in particleSystems)
            {
                ps.Stop();
            }
            //gameObject.SetActive(false);
        }
        
    }

    
}
