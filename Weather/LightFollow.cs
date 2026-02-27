using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public GameObject player;
    public float Life = 4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, Life);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3 (player.transform.position.x, 60, player.transform.position.z);
        }
        
    }
}
