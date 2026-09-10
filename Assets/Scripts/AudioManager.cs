using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get;private set;}
    AudioSource source;
    void Awake(){if(Instance&&Instance!=this){Destroy(gameObject);return;}Instance=this;DontDestroyOnLoad(gameObject);source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;}
    public void Click(){Tone(720,.045f);}
    public void Shoot(){Tone(110,.08f);}
    public void Hit(){Tone(420,.035f);}
    public void Pickup(){Tone(900,.12f);}
    public void Damage(){Tone(70,.14f);}
    void Tone(float hz,float seconds){if(!source)return;AudioClip clip=AudioClip.Create("tone",Mathf.CeilToInt(44100*seconds),1,44100,false);float[] data=new float[clip.samples];for(int i=0;i<data.Length;i++){float t=(float)i/44100f;data[i]=Mathf.Sin(2*Mathf.PI*hz*t)*Mathf.Exp(-8f*t)*.16f;}clip.SetData(data,0);source.PlayOneShot(clip);Destroy(clip,seconds+.1f);}
}
