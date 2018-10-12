using UnityEngine;
using System.Collections;

public class RippleEffect : MonoBehaviour
{
    public Camera c;
    public float seaLevel = -1.0f;
    public AnimationCurve waveform = new AnimationCurve(
        new Keyframe(0.00f, 0.50f, 0, 0),new Keyframe(0.05f, 1.00f, 0, 0),new Keyframe(0.15f, 0.10f, 0, 0),new Keyframe(0.25f, 0.80f, 0, 0),
        new Keyframe(0.35f, 0.30f, 0, 0),new Keyframe(0.45f, 0.60f, 0, 0),new Keyframe(0.55f, 0.40f, 0, 0),new Keyframe(0.65f, 0.55f, 0, 0),
        new Keyframe(0.75f, 0.46f, 0, 0),new Keyframe(0.85f, 0.52f, 0, 0),new Keyframe(0.99f, 0.50f, 0, 0)
    );//预设的涟漪振幅曲线
    [Range(0.01f, 1.0f)]
    public float refractionStrength = 0.5f;//折射强度，也就是涟漪效果强度
    public Color reflectionColor = Color.gray;//反射默认色 灰色
    [Range(0.01f, 1.0f)]
    public float reflectionStrength = 0.7f;//反射效果强度，可以理解为涟漪的阴影
    [Range(0.0f, 3.0f)]
    public float waveSpeed = 1.25f;//传播速度
    [SerializeField]
    Shader shader;
    class Droplet
    {
        Vector2 position;
        float time = 1000.0f;
        public Droplet() { }
        public void Reset(Vector2 pos){
            position = new Vector2(0.5f, 0.5f);
            time = 0;
        }
        public void Update(){
            time += Time.deltaTime;
        }
        public Vector4 MakeShaderParameter(float aspect){
            return new Vector4(position.x * aspect, position.y, time, 0);
        }
    }
    Droplet[] droplets;
    Texture2D gradTexture;
    Material material;
    float timer;
    int dropCount;
    void UpdateShaderParameters()//更新shader参数
    {
        material.SetVector("_Drop1", droplets[0].MakeShaderParameter(c.aspect));
        material.SetVector("_Drop2", droplets[1].MakeShaderParameter(c.aspect));
        material.SetVector("_Drop3", droplets[2].MakeShaderParameter(c.aspect));
        material.SetFloat("_SeaLevel", seaLevel);
        material.SetColor("_Reflection", reflectionColor);
        material.SetVector("_Params1", new Vector4(c.aspect, 1, 1 / waveSpeed, 0));
        material.SetVector("_Params2", new Vector4(1, 1 / c.aspect, refractionStrength, reflectionStrength));
    }
    void Start()
    {
        //c = GetComponent<Camera>();//获取相机组件
        if (c == null)
            Debug.Log("error  it is null");
        droplets = new Droplet[3];
        for(int i = 0;i < droplets.Length;i++)
        {
            droplets[i] = new Droplet();
        }//初始化涟漪数据
        gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false);
        gradTexture.wrapMode = TextureWrapMode.Clamp;
        gradTexture.filterMode = FilterMode.Bilinear;
        for (var i = 0; i < gradTexture.width; i++)
        {
            var x = 1.0f / gradTexture.width * i;
            var a = waveform.Evaluate(x);
            gradTexture.SetPixel(i, 0, new Color(a, a, a, a));
        }//初始化振幅贴图（也就是把waveform曲线初始化到gradTexture上面）
        gradTexture.Apply();
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        material.SetTexture("_GradTex", gradTexture);
        UpdateShaderParameters();
    }
    void Update()
    {
        foreach (var d in droplets) d.Update();//更新每个涟漪
        UpdateShaderParameters();
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material); //效果作用于画面
    }
    public void SetSeaLevel(float seaLevel_)
    {
        seaLevel = seaLevel_;
    }
    public void Emit(Vector2 pos)// call this to emit a ripple
    {
        droplets[dropCount++ % droplets.Length].Reset(pos);
    }
}
