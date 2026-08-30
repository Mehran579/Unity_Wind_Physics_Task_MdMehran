using UnityEngine;

public class WindParticleSystem : MonoBehaviour
{
    ParticleSystem _particleSystem;
    WindZone windzone;
    [Header("Noise")]
    public float noiseMultiplier = 1f;

    [Header("Emission")]
    public float minEmission = 300f;
    public float maxEmission = 1000f;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        windzone = GameObject.FindWithTag("windzone").GetComponent<WindZone>();
    }

    private void Update()
    {
        var noise = _particleSystem.noise;
        noise.strength = windzone.windTurbulence * noiseMultiplier;
        float speed01 = Mathf.InverseLerp(0f, 15f, windzone.windMain);
        float turbulence01 = Mathf.InverseLerp(0f, 1f, windzone.windTurbulence);
        float windActivity = (speed01 + turbulence01) / 2f;
        float emissionRate = Mathf.Lerp(minEmission, maxEmission, windActivity);
        var emission = _particleSystem.emission;
        emission.rateOverTime = emissionRate;
    }

    public void OnClickDisableParticles()
    {
        if (_particleSystem.isPaused)
            _particleSystem.Play();
        else
            _particleSystem.Pause();
    }
}
