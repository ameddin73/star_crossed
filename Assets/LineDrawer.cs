using System;
using System.Collections;
using System.Collections.Generic;
using graph;
using graph.edge;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public GameObject edgePrefab;
    private readonly List<IEdge> _lines;
    private bool _searching;

    public LineDrawer()
    {
        _lines = new List<IEdge>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (!_searching)
            {
                _searching = true;
                var gObject = Instantiate(edgePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                var lr = gObject.AddComponent<EdgeLineRenderer>();
                lr.startPosition = mousePos;
                lr.endPosition = mousePos;
                _lines.Add(lr);
            }
            else
            {
                _searching = false;
            }
        }
        else if (_searching)
        {
            _lines[_lines.Count - 1].endPosition = mousePos;
            
            var intersections = GraphTools.PolygonIntersections(_lines);
            foreach (var line in _lines)
            {
                ((EdgeLineRenderer)line).color = Color.gray;
            }

            foreach (var (item1, item2) in intersections)
            {
                // Invalid cast
                if (item1 is EdgeLineRenderer lineRenderer1) lineRenderer1.color = Color.red;
                if (item2 is EdgeLineRenderer lineRenderer2) lineRenderer2.color = Color.red;
            }
        }
    }
}