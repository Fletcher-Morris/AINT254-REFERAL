using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rotator : MonoBehaviour {

    public Vector3 axis;
    public bool rotate = true;
    public RotateMode mode;

    private Transform m_transform;

    private void Start()
    {
        m_transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (mode != RotateMode.Update) return;
        Rotate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (mode != RotateMode.FixedUpdate) return;
        Rotate(Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (mode != RotateMode.LateUpdate) return;
        Rotate(Time.deltaTime);
    }

    private void Rotate(float t)
    {
        if (!rotate) return;
        m_transform.Rotate(axis * t);
    }

    public enum RotateMode
    {
        Update,
        FixedUpdate,
        LateUpdate
    }
}