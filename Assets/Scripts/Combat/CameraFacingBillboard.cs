using UnityEngine;
using System.Collections;

/// <summary>
/// used in PlayerUnitObject.cs for showing a small square on the PU with any status effects
/// </summary>
public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera;

    void Start()
    {

        m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}

