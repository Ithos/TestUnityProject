using UnityEngine;
using System.Collections;

public class EnergyCharger : MonoBehaviour {

    public string[] acceptedTags;

    public float waitTime = 30.0f;
    public float chargeTime = 30.0f;
    public float maxEnergy = 100.0f;

    public GameObject particleEffect;

    private float _acumEnergy = 0.0f;
    private float _acumTime = 0.0f;
    private ParticleSystem _emitter;
    private float _emissionRate;

	// Use this for initialization
	void Start () {
        if (particleEffect != null)
        {
            _emitter = particleEffect.GetComponent<ParticleSystem>();
            _emissionRate = _emitter.emissionRate;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_acumTime < chargeTime)
        {
            _acumTime += Time.deltaTime;
        }
        else if(_acumEnergy < maxEnergy)
        {
            _acumEnergy += maxEnergy * (Time.deltaTime / chargeTime);

            if (particleEffect != null)
            {
                particleEffect.SetActive(true);
                if (_emitter != null)
                {
                    _emitter.emissionRate = _emissionRate * (_acumEnergy / maxEnergy);
                }
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (acceptedTags != null)
        {
            bool flag = false;
            for (int i = 0; i < acceptedTags.Length; ++i)
            {
                flag |= other.CompareTag(acceptedTags[i]);
            }

            if (flag && _acumTime >= waitTime)
            {
                PlayerMovement mov = other.GetComponent<PlayerMovement>();

                if (mov != null)
                {
                    mov.Energy += _acumEnergy;
                }
                else
                {
                    AIEnemyMovement enem = other.GetComponent<AIEnemyMovement>();

                    if (enem != null)
                    {
                        enem.Energy += _acumEnergy;
                    }
                }

                particleEffect.SetActive(false);
                _acumEnergy = 0.0f;
                _acumTime = 0.0f;
            }
        }
    }
}
