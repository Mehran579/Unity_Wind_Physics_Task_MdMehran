using UnityEngine;

public class Windrigidbody : MonoBehaviour
{
    // This scripts applies the wind force to the rigidbodies;
    WindZone windzone;
    Rigidbody rb;
    float seed;

    public float forceMultiplier = 5f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        windzone = GameObject.FindWithTag("windzone").GetComponent<WindZone>();
        seed = Random.Range(0f, 100f); // so leaves don't all wobble in sync
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float noiseStrenght = Mathf.PerlinNoise(Time.time * 0.5f + seed, 0f) * 2f - 1f;
        float finalStrength = windzone.windMain + noiseStrenght * windzone.windTurbulence;
        rb.AddForce(windzone.transform.forward * finalStrength * forceMultiplier, ForceMode.Force);
    }
}
