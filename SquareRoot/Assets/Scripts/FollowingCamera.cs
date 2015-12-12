using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    private enum QualityLevel
    {
        Fastest,
        Moderate,
        BestQuality
    }

	//camera boundaries 
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;
	
    public float horizontalDampening = 0.4f;
    public float verticalDampening = 0.4f;

    private Transform trackingTransform;
    private Vector3 constantOffset;

    private Vector2 screenShakeOffset;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            SetTrackingTarget(player.transform);
        }

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
        constantOffset = this.transform.position - trackingTransform.position;
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
			Vector3 v3;
			v3.x = Mathf.Lerp(currentPosition.x, targetPosition.x, Time.deltaTime / horizontalDampening);
			v3.y = Mathf.Lerp(currentPosition.y, targetPosition.y, Time.deltaTime / verticalDampening); 
			transform.position = new Vector3(Mathf.Clamp(v3.x, minX, maxX),
			                                 Mathf.Clamp(v3.y, minY, maxY), 
			                                 targetPosition.z) + (Vector3)screenShakeOffset;
        }
        
       	//camera boundaries 
       	
    }
}