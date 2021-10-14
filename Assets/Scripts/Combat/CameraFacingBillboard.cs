using UnityEngine;
using System.Collections;

//used in PUO for status image object
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

