using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * AbstractPolygon is the data representation of the polygon being drawn between
 * nodes. The polygon is a an in-order collection of Vector2s representing the vertices (nodes)
 *
 * A polygon is complete when the vector twos form a non-intersecting polygon.
 */
public class AbstractPolygonController : MonoBehaviour
{
    public static AbstractPolygonController Instance { get; private set; }

    private List<GameObject> _vertices;
    private List<LineRenderer> _edges;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        // Draw edges
        for (var i = 0; i < _vertices.Count(); i++)
        {
            // Check if line exists else create it
            if (i >= _edges.Count())
            {
                GameObject gObject = new GameObject("PolygonLineContainer");
                var lr = gObject.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                _edges.Add(lr);
            }

            if (i >= _edges.Count()) throw new Exception("Too few edges drawing polygon.");
            // Set edges
            _edges[i].SetPosition(0, _vertices[i].transform.position);
            // Only set endpoint if there's another vertex
            if (i + 1 < _vertices.Count()) _edges[i].SetPosition(1, _vertices[i + 1].transform.position);
        }

        // If touch search, or destroy
        if (_vertices.Any() && Input.GetMouseButton(0))
        {
            // Draw searching line
            _edges[_edges.Count() - 1].SetPosition(0, _vertices[_vertices.Count() - 1].transform.position);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _edges[_edges.Count() - 1].SetPosition(1, touchPosition);
        }
        else
        {
            // Clear data in singleton
            Reset();
        }
    }

    public void AddVertex(GameObject vertex)
    {
        if (!_vertices.Contains(vertex))
        {
            _vertices.Add(vertex);
        }
    }

    public bool IsVertex(GameObject vertex)
    {
        return _vertices.Contains(vertex);
    }

    bool IsNonIntersecting()
    {
        // TODO detect intersection
        return true;
    }

    private void Reset()
    {
        _edges?.ForEach(lr => Destroy(lr.gameObject));
        _vertices = new List<GameObject>();
        _edges = new List<LineRenderer>();
    }
}