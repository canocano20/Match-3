using System.Collections;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(ReturnToPool());
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitUntil(() => !_particleSystem.IsAlive(true));
        gameObject.SetActive(false);
    }
}
