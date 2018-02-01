using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class WeaponController : MonoBehaviour
{
    #region Unity Bindings

    public Camera _playerCamera;

    #endregion

    #region Private Fields

    Vector2 _centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);

    #endregion

    #region MonoBehaviour

    void Update()
    {
        bool buttonFire1 = CrossPlatformInputManager.GetButtonDown("Fire1");

        if (buttonFire1)
        {
            Debug.Log("fire1 clicked");
            Debug.Log(HitScanCenterPoint());
        }
    }

    #endregion

    #region Helper Methods

    GameObject HitScanCenterPoint()
    {
        Ray hitScanRay = _playerCamera.ScreenPointToRay(_centerScreen);
        RaycastHit hitScanHit;
        if (Physics.Raycast(hitScanRay, out hitScanHit))
            return hitScanHit.collider.gameObject;
        return null;
    }

    #endregion
}
