using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatoSun : MonoBehaviour
{
    [SerializeField]
    private Vector3 Flamerotation;
    [SerializeField]
    private float time = 0.1f;
    [SerializeField]
    public float rote; //保存すべき変数 && transform.rotation.eulerAngeles
    [SerializeField]
    private float timescale =1;
    [SerializeField]
    private Material[] skyBox;
    private Light light; 



    //

    // Start is called before the first frame update
    void Awake()
    {
        rote = PlayerPrefs.GetFloat("TimeRote");
        var r = Vector3.zero;
        r.x = PlayerPrefs.GetFloat("Sun_x");
        r.y = PlayerPrefs.GetFloat("Sun_y");
        r.z = PlayerPrefs.GetFloat("Sun_z");
        light = GetComponent<Light>();
        transform.rotation = new Quaternion(0,0,0,0);
        transform.Rotate(r);
        light.intensity = 0.85f;
    }

    // Update is called once per frame
    void Update()
    {
        var sunpos = Time.deltaTime * time;
        transform.Rotate(Flamerotation , sunpos);
        rote += sunpos;
        if(rote > 360)
        {
            rote = 0;
        }
        
        if(rote >= 0 && rote < 160 )
        {
            RenderSettings.skybox = skyBox[1];
            RenderSettings.fogColor = new Color(0.4705883f ,0.5960785f , 0.8039216f);
            light.intensity = 0.85f;
        }
        else if(rote >= 160 && rote < 180)
        {
            RenderSettings.skybox = skyBox[2];
            RenderSettings.fogColor = new Color(0.6886792f ,0.4474613f , 0.3079565f);
            light.intensity = 0.3f;
        }
        else if(rote >=180 && rote < 340)
        {
            RenderSettings.skybox = skyBox[3];
            RenderSettings.fogColor = Color.black;
            light.intensity = 0.04f;
        }
        else if(rote >= 340 && rote < 360)
        {
            RenderSettings.skybox = skyBox[0];
            light.intensity = 0.4f;
        }
        
        if(Input.GetKeyDown(KeyCode.K))
        {
            GoSleep();
        }
    }

    public void GoSleep()
    {
        transform.rotation = new Quaternion(0,0,0,0);
        if(rote < 180)//hiru
        {
            transform.Rotate(349.85f , 68.64f , 49.23f);
            rote = 180;
        }
        else 
        {
            transform.Rotate(9.691f , -111.762f , -49.3f);
            rote = 0;
        }
    }

    public void FirstRotate()
    {
        transform.rotation = new Quaternion(0,0,0,0);
        transform.Rotate(9.691f , -111.762f , -49.3f);
        rote = 0;
    }
}
