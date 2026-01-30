using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill_hit : MonoBehaviour
{
    public float destroyTime = 1f;
    private ParticleSystem ps;
    public AudioClip soundEffect;

    // Start is called before the first frame update
    public void PlayEffect()
    {
        gameObject.SetActive(true);
        ps = GetComponent<ParticleSystem>();
        ps.Play(true);
        if (soundEffect != null)
        {
            PlaySound();
        }

        Invoke(nameof(EndPlayEffect), destroyTime);
    }

    public void EndPlayEffect()
    {
        //ps = GetComponent<ParticleSystem>();
        //ps.Stop(true);
        //gameObject.SetActive(false);

        Destroy(gameObject);
    }

    private void PlaySound()
    {
        AudioManager.Instance.audioSource.PlayOneShot(soundEffect);
    }
}
