using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    public ObjectPooler ObjectPooler;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnParticle(Vector3 position, Quaternion rotation, Color startColor)
    {
        GameObject particle = ObjectPooler.SpawnFromPool(ObjectPooler.PoolType.DestroyParticle, position, rotation);
        var main = particle.GetComponent<ParticleSystem>().main;
        main.startColor = startColor;
        particle.SetActive(true);
    }
}
