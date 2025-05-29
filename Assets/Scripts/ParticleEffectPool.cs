using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.ParticleSystem;

public class ParticleEffectPool : MonoBehaviour
{
    [Header("Particle Effect Prefab")]
    public GameObject newParticlePrefab;
    public GameObject[] particlePrefab;
    List<GameObject> particles = new List<GameObject>();

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // Initialize the pool with inactive particles
        for (int i = 0; i < particlePrefab.Length; i++)
        {

            // Add a helper script to handle OnParticleSystemStopped
            ParticleReturner returner = particlePrefab[i].GetComponent<ParticleReturner>();
            returner.SetPool(this);
            particles.Add(particlePrefab[i]);

            pool.Enqueue(particlePrefab[i]);
        }
        GameManager.OnGameOver += OnGameOver;
    }
    void OnGameOver()
    {
        for(int i = 0; i < particles.Count; i++)
        {
            particles[i].SetActive(false);
        }
       /* foreach (Transform obj in transform)
        {
            obj.gameObject.SetActive(false);
        }*/
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
            particles.Add(particle);
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
