using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Unity Bindings

    public GameObject player;
    public AudioSource m_audioSource;
    public AudioClip[] m_hurtSounds;

    #endregion

    #region MonoBehaviour

    void Start()
    {

    }

    void Update()
    {
        transform.LookAt(player.transform);
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0;
        transform.eulerAngles = newRot;

        float oldPosY = transform.localPosition.y;
        transform.Translate(new Vector3(0, 0, 1f * Time.deltaTime));
        Vector3 newPos = transform.localPosition;
        newPos.y = oldPosY;
        transform.localPosition = newPos;
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
