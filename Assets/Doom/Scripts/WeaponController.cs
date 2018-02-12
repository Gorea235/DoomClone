using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Rng = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    #region Unity Bindings

    public Camera m_playerCamera;
    public int m_numberBulletsPerShot;
    public float m_spreadRadius;
    /// <summary>
    /// The number of sectors to attempt to spread the shots
    /// over. This means for a number of bullets > than this, there will be at
    /// least 1 bullet in each sector, making the spread guaranteed to be more
    /// even.
    /// </summary>
    public int m_sectorSplit = 4;
    public GameObject m_bulletTrail;

    #endregion

    #region Private Fields

    Vector2 _centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);
    readonly float _sectorSize;

    #endregion

    #region Constructor

    public WeaponController()
    {
        _sectorSize = 360f / m_sectorSplit;
    }

    #endregion

    #region MonoBehaviour

    void Update()
    {
        bool buttonFire1 = CrossPlatformInputManager.GetButtonDown("Fire1");

        if (buttonFire1)
        {
            Debug.Log("fire1 clicked");
            GameObject hit = HitScanCenterPoint();
            Debug.Log(hit);
            EnemyController enemy = hit?.GetComponent<EnemyController>();
            enemy?.Damage();
            foreach (var point in GetSpreadPoints())
                Debug.Log(point);
        }
    }

    #endregion

    #region Helper Methods

    bool GetHitFromScreenPoint(Vector3 screenPoint, out RaycastHit hit)
    {
        Ray hitScanRay = m_playerCamera.ScreenPointToRay(screenPoint);
        return Physics.Raycast(hitScanRay, out hit);
    }

    RaycastHit[] GetHitsFromScreenPoints(Vector3[] screenPoints)
    {
        RaycastHit hit;
        List<RaycastHit> hits = new List<RaycastHit>();
        foreach (Vector3 point in screenPoints)
            if (GetHitFromScreenPoint(point, out hit))
                hits.Add(hit);
        return hits.ToArray();
    }

    /// <summary>
    /// Simple hit-scan for the centre of the screen.
    /// </summary>
    /// <returns>The hit GameObject.</returns>
    GameObject HitScanCenterPoint()
    {
        RaycastHit hitScanHit;
        if (GetHitFromScreenPoint(_centerScreen, out hitScanHit))
        {
            Instantiate(m_bulletTrail).GetComponent<BulletTrailController>().Init(gameObject.transform.position, hitScanHit.point);
            return hitScanHit.collider.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Attempts to spread shots evenly over spread area.
    /// To do this, we split the area into <see cref="m_sectorSplit"/> sectors,
    /// and randomly pick a position in each for each shot.
    /// 
    /// If the bullet count isn't a multiple of <see cref="m_sectorSplit"/> exactly,
    /// then the remainder is split evenly over the area (i.e. split into 3rds
    /// for a remainder of 3, or over the whole area for a remainder of 1).
    /// </summary>
    /// <returns>The spread points.</returns>
    internal Vector3[] GetSpreadPoints(float z = 0)
    {
        // the rotation and distance from centre for each shot
        Tuple<float, float>[] points = new Tuple<float, float>[m_numberBulletsPerShot];
        int nextIndex = 0;

        // insert standard sector shots
        int shotsPerQuarter = (int)Mathf.Floor(m_numberBulletsPerShot / (float)m_sectorSplit);
        if (shotsPerQuarter > 0)
            for (int r = 0; r < m_sectorSplit; r++)
                for (int i = 0; i < shotsPerQuarter; i++)
                    InsertSpreadPoint(_sectorSize, r, ref nextIndex, ref points);

        // insert remaining shots
        int remaining = m_numberBulletsPerShot % m_sectorSplit;
        float remainingSectorSize = 360f / remaining;
        for (int r = 0; r < remaining; r++)
            InsertSpreadPoint(remainingSectorSize, remaining, ref nextIndex, ref points);

        // calculate final shot vectors on screen
        Vector3[] spreadPoints = new Vector3[m_numberBulletsPerShot];
        for (int i = 0; i < m_numberBulletsPerShot; i++)
        {
            // given the rotation and the distance from centre, we calculate the
            // vector that it represents
            spreadPoints[i] = new Vector3(points[i].Item2 * Mathf.Cos(points[i].Item1 * Mathf.Deg2Rad),
                                          points[i].Item2 * Mathf.Sin(points[i].Item1 * Mathf.Deg2Rad),
                                          z);
        }
        return spreadPoints;
    }

    void InsertSpreadPoint(float sectorSize, int rotation, ref int nextIndex, ref Tuple<float, float>[] points)
    {
        points[nextIndex] = Tuple.Create(
            Mathf.LerpAngle(sectorSize * rotation, sectorSize * (rotation + 1), Rng.value), // rotation
            m_spreadRadius * Rng.value); // distance from centre
        nextIndex++;
    }

    void CreateBulletTrails(params Vector3[] endPoints)
    {
        foreach (Vector3 point in endPoints)
            Instantiate(m_bulletTrail).GetComponent<BulletTrailController>().Init(gameObject.transform.position, point);
    }

    #endregion
}
