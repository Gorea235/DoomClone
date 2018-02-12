using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    #region Unity Bindings

    public AudioClip[] m_hurtSounds;
    public float m_health;

    #endregion

    #region Private Fields

    DataStore _dataStorage;
    AudioSource _audioSource;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
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
            _dataStorage.SetSucceeded(false);
        }
        else
        {
            _audioSource.clip = m_hurtSounds[Random.Range(0, m_hurtSounds.Length)];
            _audioSource.Play();
        }
    }

    #endregion
}
