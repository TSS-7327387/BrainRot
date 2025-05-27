using UnityEngine;

public class ParticleReturner : MonoBehaviour
{
    private ParticleEffectPool pool;
    private ParticleSystem ps;
    public bool destroy;
    public void SetPool(ParticleEffectPool pool)
    {
        this.pool = pool;
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();

        // Make sure particle system doesn't auto-stop
      /*  if (ps != null)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);*/
    }

    void OnParticleSystemStopped()
    {
        if(destroy)
            Destroy(gameObject);
        else
            pool.ReturnToPool(gameObject);
    }
}

