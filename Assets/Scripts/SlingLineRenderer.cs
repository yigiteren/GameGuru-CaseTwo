using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SlingLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform endTransform;

    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endTransform.position);
    }
}
