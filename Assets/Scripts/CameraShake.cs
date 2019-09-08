using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float _trauma;
    private Camera _camera;

    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    /// <summary>
    /// Amount trauma decays per second; a value of 1 means it all dissipates in a second.
    /// </summary>
    public float TraumaDecayRate = 1;

    /// <summary>
    /// Amount camera is rotated away from its target by shake effect (0 disables)
    /// </summary>
    [SerializeField]
    public float ShakeDeflectionAngle = 10;

    /// <summary>
    /// Amount camera is rotated about its look vector by shake effect
    /// </summary>
    [SerializeField]
    public float ShakeRotation = 10;

    /// <summary>
    /// Amount camera is repositioned from its intented position by shake effect
    /// </summary>
    [SerializeField]
    public float ShakePosition = 0.1f;

    [SerializeField]
    public float ShakePower = 2;

    [SerializeField]
    public float ShakeFrequency = 2;

    public void AddTrauma(float amount)
    {
        _trauma += amount;
    }

    // Update is called once per frame
    void Update()
    {

        float shake = CurrentShake;
        if (shake > 0)
        {
            float now = Time.time * ShakeFrequency;
            float px = NoiseSample(now, 0);
            float py = NoiseSample(now, 0.6f);
            float pz = NoiseSample(now, 0.9f);

            float dx = NoiseSample(now, 0.1f);
            float dy = NoiseSample(now, 0.5f);
            float dz = NoiseSample(now, 0.4f);

            transform.position = new Vector3(px, py, pz) * ShakePosition * shake;
            transform.RotateAround(_camera.transform.position, _camera.transform.forward, dz * ShakeRotation * shake);
            transform.RotateAround(_camera.transform.position, _camera.transform.right, dx * ShakeDeflectionAngle * shake);
            transform.RotateAround(_camera.transform.position, _camera.transform.up, dy * ShakeDeflectionAngle * shake);
        }

        float returnToIdentity = Mathf.Pow(1 - shake, 3);
        transform.position = Vector3.Lerp(transform.position, Vector3.zero, returnToIdentity);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, returnToIdentity);

        _trauma = Mathf.Clamp01(_trauma - Time.deltaTime * TraumaDecayRate);
    }

    public float CurrentTrauma { get { return Mathf.Clamp01(_trauma); } }
    public float CurrentShake { get { return Mathf.Pow(CurrentTrauma, ShakePower); } }

    private float NoiseSample(float offset, float y)
    {
        return Mathf.PerlinNoise(offset, y) * 2 - 1;
    }
}
