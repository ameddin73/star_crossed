using UnityEngine;

public class TouchAndDrag : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private GameObject _originAnchor, _destAnchor;

    public GameObject OriginAnchor
    {
        get => _originAnchor;
        set => _originAnchor = value;
    }

    public GameObject DestAnchor
    {
        get => _destAnchor;
        set => _destAnchor = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update position
        if (_originAnchor != null)
        {
            _lineRenderer.SetPosition(0, _originAnchor.transform.position);
        }

        if (_destAnchor != null)
        {
            _lineRenderer.SetPosition(1, _destAnchor.transform.position);
        }
        else
        {
            // Update endpoint to touch
            if (Input.GetMouseButton(0))
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _lineRenderer.SetPosition(1, touchPosition);
            }
            else
            {
                // Otherwise destroy the object
                Destroy(gameObject);
            }
        }
    }
}