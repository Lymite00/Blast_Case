using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Shake Settings")]
    public float shakeDuration;
    public float shakeMagnitude;

    private Vector3 _originalPosition;
    private float _shakeTimeRemaining;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _originalPosition = transform.position;
    }

    private void Update()
    {
        if (_shakeTimeRemaining > 0)
        {
            transform.position = _originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            _shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            transform.position = _originalPosition;
        }
    }

    public void ShakeCamera()
    {
        _shakeTimeRemaining = shakeDuration;
    }
}