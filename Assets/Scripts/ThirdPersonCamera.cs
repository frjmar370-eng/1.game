using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 4.2f, -7.5f);
    public float followSharpness = 14f;
    public float pitch = 12f;
    public float minDistance = 2f;
    public float collisionRadius = .25f;
    public LayerMask collisionMask = ~0;

    void LateUpdate()
    {
        if(!target) return;
        Quaternion orbit = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        Vector3 pivot = target.position + Vector3.up * 1.65f;
        Vector3 desired = pivot + orbit * offset;
        Vector3 ray = desired - pivot;
        float distance = ray.magnitude;
        if(distance > .01f && Physics.SphereCast(pivot, collisionRadius, ray.normalized, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
            desired = pivot + ray.normalized * Mathf.Max(minDistance, hit.distance - collisionRadius);
        transform.position = Vector3.Lerp(transform.position, desired, 1f-Mathf.Exp(-followSharpness*Time.deltaTime));
        transform.LookAt(pivot + Vector3.up*.15f);
    }
}
