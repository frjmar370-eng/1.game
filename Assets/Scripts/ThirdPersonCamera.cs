using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0,4.2f,-7.5f);
    public float followSharpness=12f;
    public float pitch=15f;
    void LateUpdate(){
        if(!target)return;
        Quaternion orbit=Quaternion.Euler(pitch,target.eulerAngles.y,0);
        Vector3 desired=target.position+orbit*offset;
        transform.position=Vector3.Lerp(transform.position,desired,1-Mathf.Exp(-followSharpness*Time.deltaTime));
        transform.LookAt(target.position+Vector3.up*1.8f);
    }
}
