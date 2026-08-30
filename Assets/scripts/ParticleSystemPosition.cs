using UnityEngine;
using UnityEngine.AI;

public class ParticleSystemPosition : MonoBehaviour
{
    public Terrain terrain;
    ParticleSystem _particlesystem;
    private void Awake()
    {
        _particlesystem = GetComponent<ParticleSystem>();
    }
    void LateUpdate()
    {
        Vector3 pos = Camera.main.transform.position;
        var shape = _particlesystem.shape;
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;
        float halfX = shape.scale.x * 0.5f;
        float halfZ = shape.scale.z * 0.5f;
        pos.x = Mathf.Clamp(pos.x, terrainPos.x + halfX, terrainPos.x + terrainSize.x - halfX);
        pos.z = Mathf.Clamp(pos.z, terrainPos.z + halfZ, terrainPos.z + terrainSize.z - halfZ);
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
    }
}
