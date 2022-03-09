using UnityEngine;

namespace graph.edge
{
    public class EdgeLineRenderer : MonoBehaviour, IEdge
    {
        private LineRenderer _lineRenderer;

        public Vector2 startPosition { get; set; }
        public Vector2 endPosition { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
        }

        // Update is called once per frame
        void Update()
        {
            if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.SetPosition(0, startPosition);
            _lineRenderer.SetPosition(1, endPosition);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}