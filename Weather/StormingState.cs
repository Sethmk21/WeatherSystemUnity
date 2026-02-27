using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class StormingState : WeatherState
{
    private fragManager strikeTarget;
    public GameObject fireVfx;
    public GameObject lightning;
    private GameObject p;
    public List<AudioSource> thunderSounds = new List<AudioSource>();
    private GameObject currentL;
    private bool once = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public StormingState()
    {
        lightIntensityMult = 0.6f;
        availableWeatherStates = new WeatherState[3];
        availableWeatherStates[0] = this;
        wbr = 1.5f;
        //availableWeatherStates[1] = GetComponent<SunnyState>();
        //dayManager = GetComponent<DayManager>();
    }
    void Awake()
    {
        thunderSounds = GetComponents<AudioSource>().ToList();
    }
    public override void StartState()
    {
        ps.GetComponent<ChangeRainAmt>().ResetCurrentIndex();
        dayManager.numOfRain++;
        //print("Starting Pouring");
        base.StartState();
        //dayManager.GetComponent<Light>().color = weatherColor;
        ps.GetComponent<ChangeRainAmt>().ChangeEmmission(16);
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
    public override void DoWeather()
    {
        base.DoWeather();
        if (dayManager.timeOfDay % 10 > 0 && dayManager.timeOfDay % 10 < 1 && once == false)
        {
            
            once = true;
            Invoke("changeOnce", 2);
            int rand = Random.Range(0, 10);
            if (rand < 8)
            {
                
                strikeTarget = fireManager.Instance.GetRandomBurnable();
                
                currentL = Instantiate(lightning, new Vector3(strikeTarget.transform.position.x, 60, strikeTarget.transform.position.z), Quaternion.identity);
                currentL.GetComponent<ParticleSystem>().Play();
                
                
                Invoke("HitRandom", 1f);
            }
            else
            {
                
                
                GameObject[] players = QuestManager.Instance.GetPlayerList();
                if (players.Length > 0)
                {
                    p = players[Random.Range(0, players.Length)];
                    if (p != null)
                    {
                        currentL = Instantiate(lightning, new Vector3(p.transform.position.x, 60, p.transform.position.z), p.transform.rotation);
                        currentL.GetComponent<ParticleSystem>().Play();
                        currentL.GetComponent<LightFollow>().player = p;

                        Invoke("HitPlayer", 1f);
                    }
                }
               
                //p.GetComponent<PlayerController>().isRiding = false;
                //p.transform.parent.parent
                //p.transform.parent = null;
                //p.GetComponent<PlayerController>().currentRideable = null;
                
               

            }
        }
    }
    public void HitRandom()
    {
       
        //print("Striking " + strikeTarget.gameObject);
        thunderSounds[Random.Range(0, 4)].Play();
        //print("Object Destroyed by fire: " + manager.gameObject);
        strikeTarget.rb.useGravity = false;
        strikeTarget.Collider.enabled = false;
        strikeTarget.mr.enabled = false;
        strikeTarget.appearScript.turnOffLOD();
        strikeTarget.canBeSpread = false;
        var bd1 = strikeTarget.GetBurnableData();
        GameObject fragParent = strikeTarget.GetFragParent();
        var flameTrigger = strikeTarget.GetFlameTrigger();
        flameTrigger.enabled = true;
        var originalRad = strikeTarget.GetFlameTriggerRadius();
        //print(fragParent+ " fire");
        if (fragParent != null && fragParent.transform.childCount > 0)
        {
            foreach (destroyResultingObjects child in strikeTarget.droList)
            {
                child.mr.enabled = true;
                child.mc.enabled = true;
                child.gameObject.layer = strikeTarget.gameObject.layer;
                child.rb.includeLayers = strikeTarget.rb.includeLayers;
                child.mr.material = strikeTarget.mr.material;


                float massScale = 1;
                if (strikeTarget.rb.mass >= 500)
                {
                    massScale = 1.5f;
                }
                else if (strikeTarget.rb.mass >= 50)
                {
                    massScale = 0.4f;
                }
                if (child.rb.mass > (strikeTarget.rb.mass * 0.3) + (100 * massScale))//any fragments that are larger than 1/4 the mass of the original object will be disabled to prevent large chunks remaining
                {
                    child.mc.enabled = false;
                    child.mr.enabled = false;
                }

            }
            strikeTarget.appearScript.someDeactivated = false;
            strikeTarget.GetFragParent().SetActive(true);
            fragParent.SetActive(true);
            flameTrigger.radius = originalRad;
            flameTrigger.enabled = false;
            bd1.Item2[3] = 0;

        }
        foreach (var cor in strikeTarget.activeCorutines)
        {
            StopCoroutine(cor);
        }
        strikeTarget.activeCorutines.Clear();
        // Perform the sphere cast and get all hits
        Collider[] hits = Physics.OverlapSphere(strikeTarget.transform.position, 5);
        print(hits);
        foreach (Collider hit in hits)
        {
            //print("hit");
            fragManager fm = hit.gameObject.GetComponent<fragManager>();
            if (fm != null)
            {
                var bd = fm.GetBurnableData();
                bd.Item1[0] = true;
                if (bd.Item2[3] <= 0)
                {
                    bd.Item2[3] = bd.Item2[2] * 25;
                }
                else
                {
                    bd.Item2[3] += 10f;
                    if (fm.firevfx.transform.localScale.x < 2)
                    {
                        fm.firevfx.transform.localScale += Vector3.one * 0.001f;
                    }
                }
                fm.SetBurnableData(bd.Item1, bd.Item2);
                GameObject x = Instantiate(fireVfx, fm.GetComponent<MeshRenderer>().bounds.center, Quaternion.Euler(-90, 0, 0), fm.transform);
                x.GetComponent<FlameVfxLife>().fm = fm;
                fm.firevfx = x;
                //print("Igniting " + fm.gameObject);
            }
        }
        Invoke("blowUp", 0.2f);
    }

    public void HitPlayer()
    {
        currentL.transform.position = new Vector3(p.transform.position.x, 60, p.transform.position.z);
        if (p.transform.parent != null)
        {
            strikeTarget = p.transform.parent.parent.gameObject.GetComponent<fragManager>();
        }
        else { strikeTarget = null; }
        
        if (strikeTarget != null)
        {
            strikeTarget.rb.useGravity = false;
            strikeTarget.Collider.enabled = false;
            strikeTarget.mr.enabled = false;
            strikeTarget.appearScript.turnOffLOD();
            strikeTarget.canBeSpread = false;
            var bd1 = strikeTarget.GetBurnableData();
            GameObject fragParent = strikeTarget.GetFragParent();
            var flameTrigger = strikeTarget.GetFlameTrigger();
            flameTrigger.enabled = true;
            var originalRad = strikeTarget.GetFlameTriggerRadius();
            if (fragParent != null && fragParent.transform.childCount > 0)
            {
                foreach (destroyResultingObjects child in strikeTarget.droList)
                {
                    child.mr.enabled = true;
                    child.mc.enabled = true;
                    child.gameObject.layer = strikeTarget.gameObject.layer;
                    child.rb.includeLayers = strikeTarget.rb.includeLayers;
                    child.mr.material = strikeTarget.mr.material;


                    float massScale = 1;
                    if (strikeTarget.rb.mass >= 500)
                    {
                        massScale = 1.5f;
                    }
                    else if (strikeTarget.rb.mass >= 50)
                    {
                        massScale = 0.4f;
                    }
                    if (child.rb.mass > (strikeTarget.rb.mass * 0.3) + (100 * massScale))//any fragments that are larger than 1/4 the mass of the original object will be disabled to prevent large chunks remaining
                    {
                        child.mc.enabled = false;
                        child.mr.enabled = false;
                    }

                }
                strikeTarget.appearScript.someDeactivated = false;
                strikeTarget.GetFragParent().SetActive(true);
                fragParent.SetActive(true);
                flameTrigger.radius = originalRad;
                flameTrigger.enabled = false;
                bd1.Item2[3] = 0;

            }
            foreach (var cor in strikeTarget.activeCorutines)
            {
                StopCoroutine(cor);
            }
            strikeTarget.activeCorutines.Clear();
        }
        if (p.GetComponent<PlayerController>().currentRideable == null)
        {

            
            p.GetComponent<RagdollController>().ActivateRagdoll(true);
            Collider[] hits = Physics.OverlapSphere(p.transform.position, 5);
            //print(hits);
            foreach (Collider hit in hits)
            {
                //print("hit");
                fragManager fm = hit.gameObject.GetComponent<fragManager>();
                if (fm != null)
                {
                    var bd = fm.GetBurnableData();
                    bd.Item1[0] = true;
                    if (bd.Item2[3] <= 0)
                    {
                        bd.Item2[3] = bd.Item2[2] * 25;
                    }
                    else
                    {
                        bd.Item2[3] += 10f;
                        if (fm.firevfx.transform.localScale.x < 2)
                        {
                            fm.firevfx.transform.localScale += Vector3.one * 0.001f;
                        }
                    }
                    fm.SetBurnableData(bd.Item1, bd.Item2);
                    GameObject x = Instantiate(fireVfx, fm.GetComponent<MeshRenderer>().bounds.center, Quaternion.Euler(-90, 0, 0), fm.transform);
                    x.GetComponent<FlameVfxLife>().fm = fm;
                    fm.firevfx = x;
                    //print("Igniting " + fm.gameObject);
                }
            }
            Invoke("blowUpPlayer", 0.2f);
        }
        else
        {
            if (p.GetComponent<PlayerController>().currentRideable.GetRideableName() == "Horse")
            {
                p.GetComponent<PlayerController>().isRiding = false;
                p.transform.parent = null;
                p.GetComponent<PlayerController>().currentRideable = null;
                p.GetComponent<RagdollController>().ActivateRagdoll(true);
                Collider[] hits = Physics.OverlapSphere(p.transform.position, 5);
                //print(hits);
                foreach (Collider hit in hits)
                {
                    //print("hit");
                    fragManager fm = hit.gameObject.GetComponent<fragManager>();
                    if (fm != null)
                    {
                        var bd = fm.GetBurnableData();
                        bd.Item1[0] = true;
                        if (bd.Item2[3] <= 0)
                        {
                            bd.Item2[3] = bd.Item2[2] * 25;
                        }
                        else
                        {
                            bd.Item2[3] += 10f;
                            if (fm.firevfx.transform.localScale.x < 2)
                            {
                                fm.firevfx.transform.localScale += Vector3.one * 0.001f;
                            }
                        }
                        fm.SetBurnableData(bd.Item1, bd.Item2);
                        GameObject x = Instantiate(fireVfx, fm.GetComponent<MeshRenderer>().bounds.center, Quaternion.Euler(-90, 0, 0), fm.transform);
                        x.GetComponent<FlameVfxLife>().fm = fm;
                        fm.firevfx = x;
                        //print("Igniting " + fm.gameObject);
                    }
                }
                Invoke("blowUpPlayer", 0.2f);
            }
            
        }
       
    }
    public void blowUp()
    {
        Collider[] hits = Physics.OverlapSphere(strikeTarget.transform.position, 2);
        for (int i = 0; i < hits.Count(); i++)
        {
            if (hits[i].TryGetComponent(out destroyResultingObjects dro))
            {
                //print("taco bell " + dro.gameObject);
                dro.gameObject.GetComponent<Rigidbody>().AddExplosionForce(500, transform.position, 500, 20, ForceMode.Impulse);
            }
        }
    }

    public void blowUpPlayer()
    {
        //currentL.transform.position = new Vector3(p.transform.position.x, 60, p.transform.position.z);
        Collider[] hits = Physics.OverlapSphere(p.transform.position, 1);
        thunderSounds[Random.Range(0, 4)].Play();
        for (int i = 0; i < hits.Count(); i++)
        {
            if (hits[i].TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(75, transform.position - new Vector3(0, 2, 0), 500, 20, ForceMode.Impulse);
            }
        }
    }
    private void changeOnce()
    {
        once = false;
    }
}
