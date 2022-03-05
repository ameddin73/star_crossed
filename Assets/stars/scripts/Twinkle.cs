using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Twinkle : MonoBehaviour
{

    public float minIntensity = 5f;
    public float maxIntensity = 6f;
    public float blinkLevel = 5.1f;
    public float blinkIntensity = 1f;
    public float flickerSpeed  = 5f;

    private float _random;

    private Light2D _light;

    // Start is called before the first frame update
    void Start()
    {
        _random = Random.Range(0.0f, 65535.0f);
        _light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float noise = Mathf.PerlinNoise(_random, Time.time * flickerSpeed);
        _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        if (_light.intensity < blinkLevel) _light.intensity = blinkIntensity;
    }
}
