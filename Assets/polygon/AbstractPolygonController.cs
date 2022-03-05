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
    private List<GameObject> _vertices;
    private List<LineRenderer> _edges;

    // Start is called before the first frame update
    void Start()
    {
        _vertices = new List<GameObject>();
        _edges = new List<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Draw edges
        for (var i = 0; i < _vertices.Count(); i += 2)
        {
            // Part of complete vertex pair
            if (_vertices.Count() > i)
            {
                // Check if line exists else create it
                var lineIndex = i % 2;
                if (lineIndex > _edges.Count())
                {
                    var lr = new LineRenderer();
                    lr.positionCount = 2;
                    _edges.Add(lr);
                }

                if (lineIndex > _edges.Count()) throw new Exception("Tew few edges drawing polygons.");
                // Set edges
                _edges[lineIndex].SetPosition(0, _vertices[i].transform.position);
                _edges[lineIndex].SetPosition(1, _vertices[i + 1].transform.position);
            }
        }

        // If touch search, or destroy
        if (Input.GetMouseButton(0))
        {
            // Draw searching line
            _edges[_edges.Count() - 1].SetPosition(0, _vertices[_vertices.Count() - 1].transform.position);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _edges[_edges.Count() - 1].SetPosition(1, touchPosition);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void AddVertex(GameObject vertex)
    {
        if (!_vertices.Contains(vertex))
        {
            _vertices.Add(vertex);
        }
    }

    bool IsNonIntersecting()
    {
        // TODO detect intersection
        return true;
    }
}