using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    #region Unity Bindings

    public AudioClip[] m_hurtSounds;
    public float m_health;
    public bool m_chase = true;
    public Vector3 m_lookAtOffset;
    public float m_maxMoveSpeed = 1f;
    public float m_moveSpeedScale = 1f;
    public float m_acceleration = 1f;
    public float m_deceleration = 1f;

    #endregion

    #region Private Fields

    GameObject _player;
    AudioSource _audioSource;
    Animator _animator;
    bool _chasing;
    float _currentMoveSpeed;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _player = GameObject.Find("FPSController");
        _audioSource = gameObject.GetComponent<AudioSource>();
        _animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (m_chase)
        {
            if (!_chasing)
            {
                SetChasing();
                _chasing = true;
            }
            DoChasePlayer(true);
        }
        else if (_chasing)
        {
            SetIdling();
            _chasing = false;
        }
    }

    #endregion

    #region Helper Methods

    void ApplyAnimationState(string anim, float transDuration,
                             bool isIdle, bool isRunning, bool isAttacking, bool isDead)
    {
        _animator.CrossFade(anim, transDuration);
        _animator.SetBool("IsIdle", isIdle);
        _animator.SetBool("IsRunning", isRunning);
        _animator.SetBool("IsAttacking", isAttacking);
        _animator.SetBool("IsDead", isDead);
    }

    void SetIdling() => ApplyAnimationState("Idle", 0.1f, true, false, false, false);

    void SetChasing() => ApplyAnimationState("RunningCycle", 0.1f, false, true, false, false);

    void SetAttacking() => ApplyAnimationState("Attacking", 0.1f, false, false, true, false);

    void SetDead() => ApplyAnimationState("Dead", 0.1f, false, false, false, true);

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

    public void Damage(float amount)
    {
        m_health -= amount;
        if (m_health <= 0)
        {
            // die
        }
        else
        {
            _audioSource.clip = m_hurtSounds[Random.Range(0, m_hurtSounds.Length)];
            _audioSource.Play();
        }
    }

    #endregion
}
