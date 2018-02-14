using System.Collections;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    #region Unity Bindings

    public float m_startSequenceLength;
    public GameObject m_startSequenceSphere;
    public Vector3 m_startSequenceSphereMaxScale;
    public MonoBehaviour[] m_startSequenceDisabledControllers;
    public GameObject m_startSequenceEndTrigger;

    public float m_endSequenceLength;
    public GameObject m_endSequenceRamp;
    public Vector3 m_endSequenceRampFinalPosition;

    public float m_winBonusScore;

    #endregion

    #region Private Fields

    Vector3 _startSequenceSphereStartScale;
    float _startSequenceScalePercent;
    Vector3 _endSequenceRampStartPosition;
    float _endSequencePositionPercent;
    int _triggerCount;

    #endregion

    #region Private Properties

    Vector3 StartSequenceSphereScale
    {
        get { return m_startSequenceSphere.transform.localScale; }
        set { m_startSequenceSphere.transform.localScale = value; }
    }
    float StartSequenceSphereScalePercent
    {
        get { return _startSequenceScalePercent; }
        set
        {
            _startSequenceScalePercent = Mathf.Clamp01(value);
            StartSequenceSphereScale = Vector3.Lerp(_startSequenceSphereStartScale,
                                                    m_startSequenceSphereMaxScale,
                                                    _startSequenceScalePercent);
        }
    }

    Vector3 EndSequenceRampPosition
    {
        get { return m_endSequenceRamp.transform.position; }
        set { m_endSequenceRamp.transform.position = value; }
    }
    float EndSequenceRampPositionPercent
    {
        get { return _endSequencePositionPercent; }
        set
        {
            _endSequencePositionPercent = Mathf.Clamp01(value);
            EndSequenceRampPosition = Vector3.Lerp(_endSequenceRampStartPosition,
                                                   m_endSequenceRampFinalPosition,
                                                   _endSequencePositionPercent);
        }
    }

    #endregion

    #region MonoBehaviour

    void Start()
    {
        StartCoroutine("StartSequence");
    }

    void DoActivateTrigger()
    {
        switch (_triggerCount)
        {
            case 0:
                StartCoroutine("EndSequence");
                break;
            case 1:
                Debug.Log("won game");
                DataStore dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
                dataStorage.AddScore(m_winBonusScore);
                dataStorage.SetSucceeded(true);
                // change scene
                break;
        }
        _triggerCount++;
    }

    #endregion

    #region Helper Methods

    float PercentDelta(float animTime) => Time.deltaTime / animTime;

    void ApplyEnabled(MonoBehaviour[] controllers, bool state)
    {
        foreach (MonoBehaviour controller in controllers)
            controller.enabled = state;
    }

    IEnumerator StartSequence()
    {
        ApplyEnabled(m_startSequenceDisabledControllers, false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _startSequenceSphereStartScale = StartSequenceSphereScale;
        _startSequenceScalePercent = 0;
        while (StartSequenceSphereScalePercent < 1)
        {
            yield return new WaitForEndOfFrame();
            StartSequenceSphereScalePercent += PercentDelta(m_startSequenceLength);
        }
        ApplyEnabled(m_startSequenceDisabledControllers, true);
        Destroy(m_startSequenceSphere);
        m_startSequenceEndTrigger?.BroadcastMessage("DoActivateTrigger");
    }

    IEnumerator EndSequence()
    {
        m_endSequenceRamp.SetActive(true);
        _endSequenceRampStartPosition = EndSequenceRampPosition;
        _endSequencePositionPercent = 0;
        while (EndSequenceRampPositionPercent < 1)
        {
            yield return new WaitForEndOfFrame();
            EndSequenceRampPositionPercent += PercentDelta(m_endSequenceLength);
        }
    }

    #endregion
}
