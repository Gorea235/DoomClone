using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Unity Bindings

    public AudioSource m_audioSource;
    public AudioClip[] m_hurtSounds;

    #endregion

    #region MonoBehaviour

    void Start()
    {

    }

    void Update()
    {

    }

    #endregion

    #region Helper Methods

    public void Damage()
    {
        m_audioSource.clip = m_hurtSounds[Random.Range(0, m_hurtSounds.Length)];
        m_audioSource.Play();
    }

    #endregion
}
