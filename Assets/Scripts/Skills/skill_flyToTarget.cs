using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill_flyToTarget : MonoBehaviour
{
    public GameObject _atker;
    public GameObject _target;
    public float _damage;
    public float flySpeed = 1f;
    private ParticleSystem ps;
    private bool startTraceTarget = false;
    public GameObject hitEffectObj;
    public float delayFly = 0f;
    public float delayDestroy = 0f;
    public AudioClip soundEffect;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (startTraceTarget)
        {
            if (ps != null)
            {
                if (_target != null)
                {
                    delayFly -= Time.deltaTime;
                    if(delayFly <= 0)
                    {
                        transform.position = Vector3.Lerp(transform.position, _target.transform.position, Time.deltaTime * flySpeed);
                        transform.rotation = Quaternion.LookRotation(_target.transform.position - transform.position);
                        CheckDestination();
                    }
                }
            }
            else
            {
                ps = GetComponent<ParticleSystem>();
                ps.Play(true);
            }
        }

    }

    private void OnEnable()
    {
        startTraceTarget = true;
    }

    private void OnDisable()
    {
        startTraceTarget = false;
    }

    public void SetTarget(GameObject target, GameObject attacker, float damage)
    {
        _target = target;
        _damage = damage;
        _atker = attacker;
        startTraceTarget = true;
        ps = GetComponent<ParticleSystem>();
        ps.Play(true);
        if (soundEffect != null)
        {
            Invoke(nameof(PlaySound), delayFly);
        }
    }

    private void PlaySound()
    {
        AudioManager.Instance.audioSource.PlayOneShot(soundEffect);
    }

    public void CheckDestination()
    {
        if (startTraceTarget)
        {
            if (Vector3.Distance(transform.position, _target.transform.position) < 2f)
            {
                transform.position = _target.transform.position;
                if (_target.tag == "Player")
                {
                    // handle player
                    Debug.Log("skill_flyToTarget  OnParticleCollision   Player");
                    ThirdPersonController pc = _target.GetComponent<ThirdPersonController>();
                    if (pc != null)
                    {
                        pc.ReceieveDamage(_atker, _damage);
                        if (hitEffectObj != null)
                        {
                            Instantiate(hitEffectObj, _target.transform.position, Quaternion.identity);
                        }
                        startTraceTarget = false;
                        //this.gameObject.SetActive(false);
                        Destroy(gameObject, delayDestroy);
                    }

                }

                if (_target.tag == "Enemy")
                {
                    // handle enemy
                    Debug.Log("skill_flyToTarget  OnParticleCollision   Enemy");
                    EnemyAI ai = _target.GetComponent<EnemyAI>();
                    if (ai != null)
                    {
                        ai.ReceieveAttack(_damage);
                        if (hitEffectObj != null)
                        {
                            Instantiate(hitEffectObj, _target.transform.position, Quaternion.identity);
                        }
                        startTraceTarget = false;
                        //this.gameObject.SetActive(false);
                        Destroy(gameObject, delayDestroy);

                    }
                }
            }
        }
    }
}
