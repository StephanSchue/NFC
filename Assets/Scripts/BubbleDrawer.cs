using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A Script where you can draw cyrles a culling mask.
/// @Autor: Stephan Schüritz
/// </summary>
public class BubbleDrawer : MonoBehaviour 
{
	public int brushSize = 10;

    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
	private Texture2D stencialMap;

	private Camera sceneCamera;

	private int maskSize = 0;
	private int maskedPixels = 0;
	private int initMaskedPixels = 0;

    public LayerMask mask;

	public Texture2D[] splashs;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start() 
	{
		sceneCamera = Camera.main;
		meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

		meshRenderer.sortingLayerName = "Bubble";
		meshRenderer.sortingOrder = 0;
	}

    public void SetSplashTexture(int index)
    { 
        meshRenderer.material.mainTexture = splashs[index];

        SetAlpha(1.0f);

        ResetMask();
    }

	/// <summary>
	/// Resets the mask.
	/// </summary>
	public void ResetMask()
	{
		stencialMap = new Texture2D(meshRenderer.material.mainTexture.width, meshRenderer.material.mainTexture.height);

		// Set Color
		PredefineTheMask();
	}

	/// <summary>
	/// Predefine the mask.
	/// </summary>
	private void PredefineTheMask()
	{
		Color32[] cols = stencialMap.GetPixels32();
		Color32[] mainCols = (meshRenderer.material.mainTexture as Texture2D).GetPixels32();

		for(int i = 0; i < cols.Length; ++i) 
		{
			if (mainCols[i].a == 0.0f) 
			{
				cols[i] = Color.black;
			}
			else
			{
				cols[i] = Color.white;
			}
		}

		stencialMap.SetPixels32(cols);
		stencialMap.Apply();

		maskSize = cols.Length;
		CountMaskedPixels(cols);

		initMaskedPixels = maskedPixels;

		cols = null;
		mainCols = null;

		// actually apply all SetPixels, don't recalculate mip levels
		meshRenderer.material.SetTexture("_CutTex", stencialMap);
	}

	/// <summary>
	/// Draws the stencial.
	/// </summary>
	public void DrawStencial()
	{
		Vector3 uvWorldPosition = Vector3.zero;

		if (HitTestUVPosition(ref uvWorldPosition)) 
		{
			Circle32(ref stencialMap, (int)(uvWorldPosition.x * stencialMap.width), (int)(uvWorldPosition.y * stencialMap.height), brushSize, new Color (0f, 0f, 0f, 0f));
		}
	}

	/// <summary>
	/// Calculate the UV Position in position of the mouse
	/// </summary>
	/// <returns><c>true</c>, if test UV position was hit, <c>false</c> otherwise.</returns>
	/// <param name="uvWorldPosition">Uv world position.</param>
	private bool HitTestUVPosition(ref Vector3 uvWorldPosition)
	{
		RaycastHit hit;
		Vector3 mousePos=Input.mousePosition;
		Vector3 cursorPos = new Vector3 (mousePos.x, mousePos.y, 0.0f);
		Ray cursorRay = sceneCamera.ScreenPointToRay (cursorPos);

		if (Physics.Raycast(cursorRay, out hit, 200, mask))
		{
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider == null || meshCollider.sharedMesh == null)
				return false; 
			
			Vector2 pixelUV = new Vector2(hit.textureCoord.x,hit.textureCoord.y);
			uvWorldPosition.x=pixelUV.x;
			uvWorldPosition.y=pixelUV.y;
			uvWorldPosition.z=0.0f;
			return true;
		}
		else
		{ 
			return false;
		}
	}

	/// <summary>
	/// Draw a cicle in the position of the Mask
	/// </summary>
	/// <param name="tex">Tex.</param>
	/// <param name="cx">Cx.</param>
	/// <param name="cy">Cy.</param>
	/// <param name="r">The brush radius</param>
	/// <param name="col">Color.</param>
	private void Circle(ref Texture2D tex, int cx, int cy, int r, Color col)
	{
		int x, y, px, nx, py, ny, d;
		//Color[] colorMain = tex.GetPixels(); // TODO: Remove SetPixel and use SetPixels

		List<Vector4> list = new List<Vector4>();

		for (x = 0; x <= r; x++)
		{
			d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
			for (y = 0; y <= d; y++)
			{
				px = cx + x; // w
				nx = cx - x; // x
				py = cy + y; // y
				ny = cy - y; // z

				list.Add(new Vector4(px, nx, py, ny));
			}
		} 

		for (int i = 0; i < list.Count; i++) 
		{
			/*
			tex.SetPixel(px, py, col);
			tex.SetPixel(nx, py, col);

			tex.SetPixel(px, ny, col);
			tex.SetPixel(nx, ny, col);
			*/

			tex.SetPixel((int)list[i].w, (int)list[i].y, col);
			tex.SetPixel((int)list[i].x, (int)list[i].y, col);

			tex.SetPixel((int)list[i].w, (int)list[i].z, col);
			tex.SetPixel((int)list[i].x, (int)list[i].z, col);
		}

		list.Clear();

		//tex.SetPixels(colorMain);
		tex.Apply();

		// -- Calculate masked the percentage --
		CountMaskedPixels(tex.GetPixels32());
	}

	public void Circle32(ref Texture2D tex, int cx, int cy, int r, Color col)
	{
		int x, y, px, nx, py, ny, d;
		Color32[] tempArray = tex.GetPixels32();
		int stencialMapHeight = stencialMap.height;

		for (x = 0; x <= r; x++)
		{
			d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
			for (y = 0; y <= d; y++)
			{
				px = cx + x;
				nx = cx - x;
				py = cy + y;
				ny = cy - y;

                try
                {
                    if ((py * stencialMapHeight + px) < tempArray.Length) {
                        tempArray [py * stencialMapHeight + px] = col;
                    }

                    if ((py * stencialMapHeight + nx) < tempArray.Length) {
                        tempArray [py * stencialMapHeight + nx] = col;
                    }

                    if ((ny * stencialMapHeight + px) < tempArray.Length) {
                        tempArray [ny * stencialMapHeight + px] = col;
                    }

                    if ((ny * stencialMapHeight + nx) < tempArray.Length) {
                        tempArray [ny * stencialMapHeight + nx] = col;
                    }
                }
                catch (System.Exception ex)
                {
                        
                }
			}
		}

		tex.SetPixels32(tempArray);
		tex.Apply();

		// -- Calculate masked the percentage --
		CountMaskedPixels(tex.GetPixels32());
	}

	/// <summary>
	/// Counts the masked pixels.
	/// </summary>
	public void CountMaskedPixels(Color32[] tempArray)
	{
		maskedPixels = 0;

		for (int i = 0; i < tempArray.Length; i++) 
		{
			if (tempArray[i].r == 0.0f) 
			{
				++maskedPixels;
			}
		}
	}

	/// <summary>
	/// Get the masked percentage.
	/// </summary>
	/// <value>The masked percentage.</value>
	public float MaskedPercentage()
	{
		return (initMaskedPixels-maskedPixels) / (float)(initMaskedPixels-maskSize);
	}

	public void SetAlpha(float alpha)
	{
		meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, alpha);
	}
}
