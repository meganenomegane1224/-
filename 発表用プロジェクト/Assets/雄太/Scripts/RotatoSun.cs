using UnityEngine;

public class RotatoSun : MonoBehaviour
{
    [SerializeField]
    private bool canRote = true;
    [SerializeField]
    private Vector3 Flamerotation;   //太陽の回転方向を定義
    [SerializeField]
    private float time = 0.1f;       //太陽の回転量を定義
    [SerializeField]
    public float rote;               //太陽がどれくらい回転しているかを格納。この値は保存され、回転量の相対値として用いられる
    [SerializeField]
    private Material[] skyBox;       //時間帯によってskyboxを変えるために配列化して定義
    private Light light;             // 回転させるlightをアタッチ

    //定数を宣言

    // Start is called before the first frame update
    void Awake()
    {
        if(canRote == false)return;
        rote = PlayerPrefs.GetFloat("TimeRote");//保存されている回転量の絶対値を取得
        var r = new Vector3(PlayerPrefs.GetFloat("Sun_x"),  PlayerPrefs.GetFloat("Sun_y"),PlayerPrefs.GetFloat("Sun_z"));//保存されている太陽のRotaionを取得
        light = GetComponent<Light>();//lighコンポーネントを取得
        transform.rotation = new Quaternion(0,0,0,0);//太陽の回転位置をリセット
        transform.Rotate(r);//取得したRotaion分回転する
        light.intensity = 0.85f;//明るさを設定
    }

    // Update is called once per frame
    void Update()
    {
        if(canRote == false)return;
        var sunpos = Time.deltaTime * time;//1秒当たりの回転量を設定
        transform.Rotate(Flamerotation , sunpos);//回転を実行
        rote += sunpos;//回転の絶対値を更新

        //回転量が360を超えたら、0にリセット
        if(rote >= 360)
        {
            rote = 0;
        }
        
        //以下、回転量の絶対値ごとに、昼、夕方、夜、朝方を切り替え　それに伴い明るさ、skbox、fogカラーを変更
        
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
    }

    public void GoSleep()//寝る等の時間帯が変わるイベントの際、ここを呼び出すことで、夜なら昼に、昼なら夜に変更できる
    {
        if(canRote == false)return;
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

    public void FirstRotate()//最初にゲームが実行されたときに呼び出される。太陽の位置を初期設定する
    {
        if(canRote == false)return;
        transform.rotation = new Quaternion(0,0,0,0);
        transform.Rotate(9.691f , -111.762f , -49.3f);
        rote = 0;
    }
}
