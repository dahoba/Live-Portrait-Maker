
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class PlasmaColorScript : MonoBehaviour
{

    private string shader = "PlasmaColor";
    [HideInInspector] [Range(0, 1)] public float _Alpha = 1f;
    [HideInInspector] [Range(0.1f, 8f)] public float _Colors = 6f;
    [HideInInspector] [Range(0.1f, 4f)] public float _Offset = 2.5f;
    [HideInInspector] private float _TimeX = 0;
    [HideInInspector] [Range(0, 3)] public float Speed = 1;

    Image CanvasImage;


    void Awake()
    {
        if (CanvasImage != null)
        {
            CanvasImage = this.gameObject.GetComponent<Image>();
        }
        Material tempMaterial = new Material(Shader.Find(shader));
        tempMaterial.hideFlags = HideFlags.None;
        CanvasImage.material = tempMaterial;
    }


    void Update()
    {
        CanvasImage.material.SetFloat("_Alpha", 1 - _Alpha);
        CanvasImage.material.SetFloat("_Colors", _Colors);
        CanvasImage.material.SetFloat("_Offset", _Offset);
        _TimeX += Time.deltaTime * Speed;
        if (_TimeX > 100) _TimeX = 0;
        CanvasImage.material.SetFloat("_TimeX", _TimeX);
    }




}

