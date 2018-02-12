using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    #region Unity Bindings

    public AudioClip[] m_hurtSounds;
    public float m_health;

    #endregion

    #region Private Fields

    AudioSource _audioSource;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    #endregion

    #region Helper Methods

    public void Damage(float amount)
    {
        m_health -= amount;
        Debug.Log(m_health);
        if (m_health <= 0)
        {
            Debug.Log("failed game");
        }
        else
        {
            _audioSource.clip = m_hurtSounds[Random.Range(0, m_hurtSounds.Length)];
            _audioSource.Play();
        }
    }

    #endregion
}
