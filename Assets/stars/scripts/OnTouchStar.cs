using System;
using System.Linq;
using UnityEngine;

public class OnTouchStar : MonoBehaviour
{
    public int burstParticles = 100;

    private ParticleSystem _particle;

    // Start is called before the first frame update
    void Start()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    void OnMouseDown()
    {
        // Burst particles
        _particle.Emit(burstParticles);
        // Add anchor
        OnMouseEnter();
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0) && !GraphController.Instance.IsNonInitialVertex(gameObject))
        {
            _particle.Play();
            GraphController.Instance.AddVertex(gameObject);
        }
    }

    void Update()
    {
        if (_particle.isPlaying && !Input.GetMouseButton(0)
                                && !GraphController.Instance.IsNonInitialVertex(gameObject))
            _particle.Stop();
    }
}