using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Unity Bindings

    public AudioSource m_audioSource;
    public AudioClip[] m_hurtSounds;
    public float m_health;
    public bool m_chase = true;
    public float m_moveSpeed = 1f;

    #endregion

    #region Private Fields

    GameObject _player;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _player = GameObject.Find("FPSController");
    }

    void Update()
    {
        if (m_chase)
            ChasePlayer();
    }

    #endregion

    #region Helper Methods

    void ChasePlayer()
    {
        transform.LookAt(_player.transform);
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0;
        transform.eulerAngles = newRot;

        float oldPosY = transform.localPosition.y;
        transform.Translate(new Vector3(0, 0, m_moveSpeed * Time.deltaTime));
        Vector3 newPos = transform.localPosition;
        newPos.y = oldPosY;
        transform.localPosition = newPos;
    }

    public void Damage(float amount)
    {
        m_health -= amount;
        if (m_health <= 0)
        {
            // die
        }
        else
        {
            m_audioSource.clip = m_hurtSounds[Random.Range(0, m_hurtSounds.Length)];
            m_audioSource.Play();
        }
    }

    #endregion
}
