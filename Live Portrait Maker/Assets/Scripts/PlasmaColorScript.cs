
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class PlasmaColorScript : MonoBehaviour
{
	[HideInInspector] public Material ForceMaterial;
	[HideInInspector] public bool ActiveChange=true;
	private string shader = "PlasmaColor";
	[HideInInspector] [Range(0, 1)] public float _Alpha = 1f;
	[HideInInspector] [Range(0.1f, 8f)]public float _Colors = 6f;
	[HideInInspector] [Range(0.1f, 4f)]public float _Offset = 2.5f;
	[HideInInspector] private float _TimeX = 0;
	[HideInInspector] [Range(0, 3)]public float Speed = 1;


	[HideInInspector] public int ShaderChange=0;
	Material tempMaterial;

	Material defaultMaterial;
	Image CanvasImage;

	
	void Awake()
	{
		if (this.gameObject.GetComponent<Image> () != null) 
		{
			CanvasImage = this.gameObject.GetComponent<Image> ();
		}
	}
	void Start ()
	{  
		ShaderChange = 0;
	}


	void Update()
	{
      
       			
			if ((ShaderChange == 0) && (ForceMaterial != null)) 
		{
			ShaderChange=1;
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
		
				CanvasImage.material = ForceMaterial;

			ForceMaterial.hideFlags = HideFlags.None;
			ForceMaterial.shader=Shader.Find(shader);
			ActiveChange=false;

		}
		if ((ForceMaterial == null) && (ShaderChange==1))
		{
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
		
				CanvasImage.material = tempMaterial;

			ShaderChange=0;
		}
		
	
		if (ActiveChange)
		{
		
				CanvasImage.material.SetFloat("_Alpha", 1-_Alpha);
				CanvasImage.material.SetFloat("_Colors", _Colors);
				CanvasImage.material.SetFloat("_Offset", _Offset);
				_TimeX+=Time.deltaTime*Speed;
				if (_TimeX>100) _TimeX=0;
				CanvasImage.material.SetFloat("_TimeX", _TimeX);	
	
		}
	
	}
	
	
	
	void OnEnable()
	{
		if (this.gameObject.GetComponent<Image> () != null) 
		{
			if (CanvasImage==null) CanvasImage = this.gameObject.GetComponent<Image> ();
		} 
		if (defaultMaterial == null) {
			defaultMaterial = new Material(Shader.Find("Sprites/Default"));
			 
			
		}
		if (ForceMaterial==null)
		{
			ActiveChange=true;
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = tempMaterial;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material = tempMaterial;
			}
		}
		else
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = ForceMaterial;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material = ForceMaterial;
			}
		}
		
	}
}

