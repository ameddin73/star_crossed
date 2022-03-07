using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeController : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetPosition(int index, Vector3 position)
    {
        if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetPosition(index, position);
    }
}