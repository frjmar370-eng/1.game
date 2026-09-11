using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5.5f;
    public float turnSpeed = 12f;
    public float gravity = -18f;
    public Transform cameraTarget;
    CharacterController controller;
    float vertical;
    Vector2 moveInput;
    Vector2 lookInput;
    bool firing;
    float yaw;

    void Awake(){ controller = GetComponent<CharacterController>(); if(!controller) controller=gameObject.AddComponent<CharacterController>(); controller.height=3.8f; controller.radius=.55f; controller.center=new Vector3(0,1.9f,0); yaw=transform.eulerAngles.y; }
    void Update(){
        moveInput = MobileControls.Move;
        lookInput = MobileControls.Look;
        firing = MobileControls.Fire;
        if(moveInput.sqrMagnitude>.01f){
            Vector3 forward=Camera.main ? Camera.main.transform.forward : Vector3.forward; forward.y=0; forward.Normalize();
            Vector3 right=Camera.main ? Camera.main.transform.right : Vector3.right; right.y=0; right.Normalize();
            Vector3 dir=(forward*moveInput.y+right*moveInput.x); if(dir.sqrMagnitude>1)dir.Normalize();
            controller.Move(dir*moveSpeed*Time.deltaTime);
            Quaternion target=Quaternion.LookRotation(dir,Vector3.up); transform.rotation=Quaternion.Slerp(transform.rotation,target,turnSpeed*Time.deltaTime);
        }
        if(lookInput.sqrMagnitude>.001f){ yaw += lookInput.x*140f*Time.deltaTime; }
        if(controller.isGrounded && vertical<0) vertical=-2f; vertical+=gravity*Time.deltaTime; controller.Move(Vector3.up*vertical*Time.deltaTime);
        if(cameraTarget) cameraTarget.rotation=Quaternion.Euler(0,yaw,0);
    }
}

public static class MobileControls
{
    public static Vector2 Move;
    public static Vector2 Look;
    public static bool Fire;
}
