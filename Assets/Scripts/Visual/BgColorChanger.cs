using UnityEngine;

public class BgColorChanger : MonoBehaviour
{
    public float lowestPos;
    public float highestPos;
    public Transform cam;

    public Color colorA;
    public Color colorB;
    public Material mat;
    void Update()
    {

        mat.SetFloat("_Blend", Mathf.InverseLerp(highestPos, lowestPos, cam.position.y));
        SetFogColor(Mathf.InverseLerp(highestPos, lowestPos, cam.position.y));
    }
    
    // Call this method with a value from 0 to 1
    public void SetFogColor(float t)
    {
        RenderSettings.fogColor = Color.Lerp(colorA, colorB, Mathf.Clamp01(t));
    }

}
