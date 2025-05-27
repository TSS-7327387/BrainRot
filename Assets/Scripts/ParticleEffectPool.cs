using UnityEngine;
using System.Collections.Generic;

public class ParticleEffectPool : MonoBehaviour
{
    [Header("Particle Effect Prefab")]
    public GameObject newParticlePrefab;
    public GameObject[] particlePrefab;


    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // Initialize the pool with inactive particles
        for (int i = 0; i < particlePrefab.Length; i++)
        {

            // Add a helper script to handle OnParticleSystemStopped
            ParticleReturner returner = particlePrefab[i].GetComponent<ParticleReturner>();
            returner.SetPool(this);

            pool.Enqueue(particlePrefab[i]);
        }
        GameManager.OnGameOver += OnGameOver;
    }
    void OnGameOver()
    {
        foreach (Transform obj in transform)
        {
            obj.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Show particle effect at a specified position.
    /// </summary>
    public void ShowEffect(Vector3 position)
    {
        GameObject particle;

        if (pool.Count > 0)
        {
            particle = pool.Dequeue();
        }
        else
        {
            // Optional: expand pool if needed
            particle = Instantiate(newParticlePrefab,transform);
            ParticleReturner returner = particle.AddComponent<ParticleReturner>();
            returner.SetPool(this);
        }

        particle.transform.position = position;
        particle.SetActive(true);

       /* ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }*/
    }

    /// <summary>
    /// Called by ParticleReturner when particle stops
    /// </summary>
    public void ReturnToPool(GameObject particle)
    {
        particle.SetActive(false);
        pool.Enqueue(particle);
    }
}
