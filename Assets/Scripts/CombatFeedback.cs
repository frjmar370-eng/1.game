using UnityEngine;

public class CombatFeedback : MonoBehaviour
{
    public static CombatFeedback Instance { get; private set; }
    float shake;
    Camera cam;
    Vector3 baseLocal;
    AudioSource audioSource;

    void Awake(){Instance=this;cam=Camera.main;if(cam)baseLocal=cam.transform.localPosition;audioSource=gameObject.AddComponent<AudioSource>();audioSource.playOnAwake=false;audioSource.spatialBlend=0f;}
    void LateUpdate(){if(!cam)return;if(shake>0f){shake=Mathf.MoveTowards(shake,0f,Time.deltaTime*3.8f);cam.transform.localPosition=baseLocal+Random.insideUnitSphere*shake;}else cam.transform.localPosition=baseLocal;}
    public void Shake(float amount){shake=Mathf.Max(shake,amount);}
    public void Shot(){if(audioSource)audioSource.PlayOneShot(MakeClip(105f,.055f));}
    public void Hit(){if(audioSource)audioSource.PlayOneShot(MakeClip(760f,.035f));Shake(.045f);}
    AudioClip MakeClip(float frequency,float duration){int rate=22050;int samples=Mathf.Max(1,Mathf.RoundToInt(rate*duration));AudioClip clip=AudioClip.Create("fx",samples,1,rate,false);float[] data=new float[samples];for(int i=0;i<samples;i++){float t=i/(float)rate;float env=1f-i/(float)samples;data[i]=Mathf.Sin(2f*Mathf.PI*frequency*t)*env*.16f;}clip.SetData(data,0);return clip;}
}
