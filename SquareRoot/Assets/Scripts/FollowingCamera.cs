using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    private enum QualityLevel
    {
        Fastest,
        Moderate,
        BestQuality
    }
	
    public float horizontalDampening = 0.4f;
    public float verticalDampening = 0.4f;

    private Transform trackingTransform;
    private Vector3 constantOffset;

    private Vector2 screenShakeOffset;

    void Start()
    {
        switch((QualityLevel)QualitySettings.GetQualityLevel())
        {
            case QualityLevel.Fastest:
                // TODO screen effects for lowest quality
                break;
            case QualityLevel.Moderate:
                // TODO screen effects for moderate quality
                break;
            case QualityLevel.BestQuality:
                // TODO screen effects for best quality
                break;
            default:
                Debug.LogWarning("Encountered an unexpected quality level value");
                break;
        }
    }

    public void SetTrackingTarget(Transform target)
    {
        trackingTransform = target;
        if(target != null)
        {
            constantOffset = this.transform.position - trackingTransform.position;
        }
    }

    public void ApplyShock(float force)
    {
        randomAngle = Random.Range(0, 360);
        radius = force;
    }

    private float randomAngle;
    private float radius;

    void LateUpdate()
    {
        // screen shake

        radius *= 0.9f;
        randomAngle += (180 + Random.Range(-60, 60));
        screenShakeOffset = new Vector2(Mathf.Sin(randomAngle) * radius, Mathf.Cos(randomAngle) * radius);

        // follow

        if (trackingTransform != null)
        {
            Vector3 targetPosition = trackingTransform.position + constantOffset;
            Vector3 currentPosition = transform.position;
			transform.position = new Vector3(Mathf.Lerp(currentPosition.x, targetPosition.x, Time.deltaTime / horizontalDampening),
                                             Mathf.Lerp(currentPosition.y, targetPosition.y, Time.deltaTime / verticalDampening), 
			                                 targetPosition.z) + (Vector3)screenShakeOffset;
        }
    }
}