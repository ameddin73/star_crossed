using System;
using System.Linq;
using UnityEngine;

public class OnTouchStar : MonoBehaviour
{
    public int burstParticles = 100;

    public LineRenderer lineRenderer;
    private CircleCollider2D _collider;
    private ParticleSystem _particle;

    private bool _isAnchor;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
        _particle = GetComponent<ParticleSystem>();
        _isAnchor = false;
    }

    void OnMouseDown()
    {
        StartLine();

        // Burst particles
        _particle.Emit(burstParticles);
        _particle.Play();
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0) && !_isAnchor)
        {
            // Check if we're touching a line and if so lock it + start a new one
            // TODO update to prefab
            var lr = FindObjectsOfType<LineRenderer>()
                .Where(lr => lr.GetComponent<TouchAndDrag>().DestAnchor == null).ToArray();
            // If there's a line, finish it off and start a new one
            if (lr.Length > 0)
            {
                lr[0].GetComponent<TouchAndDrag>().DestAnchor = gameObject;
                StartLine();
            }

            _isAnchor = true;
        }
    }

    void Update()
    {
        if (!Input.GetMouseButton(0) && !_isAnchor) _particle.Stop();
    }

    private void StartLine()
    {
        if (!_isAnchor)
        {
            // Create line
            var lr = Instantiate(lineRenderer);
            lr.positionCount = 2;
            // Set origin anchor
            lr.GetComponent<TouchAndDrag>().OriginAnchor = gameObject;

            _isAnchor = true;
        }
    }
}