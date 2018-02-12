using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour
{
    #region Unity Bindings

    public bool m_open;

    #endregion

    #region Private Fields

    Animator _animator;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        if (m_open)
        {
            _animator.Play("Open");
            _animator.SetBool("IsOpen", true);
        }
    }

    void DoActivateTrigger()
    {
        _animator.SetBool("IsOpen", !_animator.GetBool("IsOpen"));
    }

    #endregion
}
