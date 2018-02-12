using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    #region Unity Bindings

    public AudioClip[] m_hurtSounds;
    public AudioClip m_deathSound;
    public float m_health;
    public bool m_active = true;
    public bool m_chase = true;
    public Vector3 m_lookAtOffset;
    public float m_maxMoveSpeed = 1f;
    public float m_moveSpeedScale = 1f;
    public float m_acceleration = 1f;
    public float m_deceleration = 1f;
    public float m_damage = 1f;
    public float m_range = 1f;
    public float m_damageRate = 1f; // per second

    #endregion

    #region Private Fields

    GameObject _player;
    PlayerController _playerController;
    AudioSource _audioSource;
    Animator _animator;
    bool _dead;
    float _currentMoveSpeed;
    float _lastHit;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _player = GameObject.Find("FPSController");
        _playerController = _player.GetComponentInChildren<PlayerController>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (_dead)
            return;

        if (m_active)
        {
            if (Vector3.Distance(gameObject.transform.position, _player.transform.position) <= m_range)
            {
                SetAttacking();
                if (Time.realtimeSinceStartup - _lastHit >= m_damageRate)
                {
                    _playerController.Damage(m_damage);
                    _lastHit = Time.realtimeSinceStartup;
                }
            }
            else
            {
                if (m_chase)
                {
                    SetChasing();
                    DoChasePlayer(true);
                }
                else
                    SetIdling();
            }
        }
        else
            SetIdling();
    }

    #endregion

    #region Helper Methods

    void ApplyAnimationState(int state) => _animator.SetInteger("State", state);

    void SetIdling() => ApplyAnimationState(0);

    void SetChasing() => ApplyAnimationState(1);

    void SetAttacking() => ApplyAnimationState(2);

    void SetDead() => _animator.SetBool("IsDead", true);

    void DoChasePlayer(bool accelerate)
    {
        // rotate towards player
        transform.LookAt(_player.transform);
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0;
        transform.eulerAngles = newRot + m_lookAtOffset;

        // apply acceleration
        if (accelerate)
            _currentMoveSpeed = Mathf.Min(_currentMoveSpeed + m_acceleration, m_maxMoveSpeed);
        else
            _currentMoveSpeed = Mathf.Max(_currentMoveSpeed - m_deceleration, 0);
        float oldPosY = transform.localPosition.y;
        transform.Translate(new Vector3(0, 0, _currentMoveSpeed * Time.deltaTime * m_moveSpeedScale));
        Vector3 newPos = transform.localPosition;
        newPos.y = oldPosY;
        transform.localPosition = newPos;
    }

    public void Die()
    {
        _dead = true;
        SetDead();
        _audioSource.clip = m_deathSound;
        _audioSource.Play();
    }

    public void Damage(float amount)
    {
        m_health -= amount;
        if (m_health <= 0)
            Die();
        else
        {
            _audioSource.clip = m_hurtSounds[Random.Range(0, m_hurtSounds.Length)];
            _audioSource.Play();
        }
    }

    #endregion
}
