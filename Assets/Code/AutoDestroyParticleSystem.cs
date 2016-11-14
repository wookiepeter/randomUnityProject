using UnityEngine;

/*
    Wenn ein PartikelSystem nicht mehr auf playing ist zerstöre das Object
*/
 class AutoDestroyParticleSystem : MonoBehaviour
 {
    ParticleSystem _particleSystem;
    public void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    public void Update()
    {
        if (_particleSystem.isPlaying)
            return;
        Destroy(gameObject);
    }
 }

