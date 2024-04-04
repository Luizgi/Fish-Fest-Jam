using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera Shake")]
    Transform transform;
    float shakeDuration = 0f;
    float shakeMagnitude = 1f;
    float dampingSpeed = 1f;
    Vector3 initialPos;

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }
    private void OnEnable()
    {
        initialPos = transform.localPosition;
    }

    private void Update()
    {
        if(shakeDuration > 0f)
        {
            transform.localPosition = initialPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPos;
        }
    }

    public void TriggerShake(float timeShake)
    {
        shakeDuration = timeShake;
    }
}
