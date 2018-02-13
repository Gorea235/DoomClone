using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    #region Unity Bindings

    public AudioClip[] m_hurtSounds;
    public float m_maxHealth;
    public Mask m_healthBarMask;
    public Text m_HealthPercentText;

    #endregion

    #region Private Fields

    const string healthTextFormat = "{0:P}";

    DataStore _dataStorage;
    AudioSource _audioSource;
    float _health;
    Vector2 _defaultHealthBarMaskSize;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _defaultHealthBarMaskSize = m_healthBarMask.rectTransform.sizeDelta;
        _health = m_maxHealth;
    }

    void Start()
    {
        UpdateHealthBar();
    }

    #endregion

    #region Helper Methods

    void UpdateHealthBar()
    {
        float percent = (_health / m_maxHealth);
        m_healthBarMask.rectTransform.sizeDelta = new Vector2(_defaultHealthBarMaskSize.x * percent,
                                                              _defaultHealthBarMaskSize.y);
        m_HealthPercentText.text = string.Format(healthTextFormat, percent);
    }

    public void Damage(float amount)
    {
        // update health
        _health -= amount;
        // update UI
        UpdateHealthBar();
        // check if failed, otherwise play hurt sound
        if (_health <= 0)
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
