using UnityEngine;

public class ParticleCulling : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.CollisionModule collisionModule;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        collisionModule = ps.collision;
    }

    void OnBecameInvisible()
    {
        ps.Pause();
        collisionModule.enabled = false;
    }

    void OnBecameVisible()
    {
        ps.Play();
        collisionModule.enabled = true;
    }

}
