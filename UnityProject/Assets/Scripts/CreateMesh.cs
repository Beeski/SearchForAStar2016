using UnityEngine;
using System.Collections;

public class CreateMesh : MonoBehaviour 
{
	[SerializeField] private Material Mat;
	[SerializeField] private float Size = 1.0f;

	private MeshRenderer mMeshRenderer;
	private MeshFilter mMesh;

	public Material Material {
		get { return Mat; }
		set { Mat = value; }
	}
	
	private Vector3 [] GetVerts( float size )
	{
		Vector3 [] verts = new Vector3[7]; 
		float wide = size * 0.5f;
		float narrow = size * 0.15f;
		
		verts[0] = new Vector3( 0.0f, wide, 0.0f );
		verts[1] = new Vector3( -wide, 0.0f, 0.0f );
		verts[2] = new Vector3( wide, 0.0f, 0.0f );
		verts[3] = new Vector3( -narrow, 0.0f, 0.0f );
		verts[4] = new Vector3( narrow, 0.0f, 0.0f );
		verts[5] = new Vector3( -narrow, -wide, 0.0f );
		verts[6] = new Vector3( narrow, -wide, 0.0f );

		return verts;
	}
	
	private int [] GetTriangles()
	{
		int [] starTriangles = new int[9];
		
		starTriangles[0] = 0;
		starTriangles[1] = 2;
		starTriangles[2] = 1;
		starTriangles[3] = 3;
		starTriangles[4] = 4;
		starTriangles[5] = 5;
		starTriangles[6] = 4;
		starTriangles[7] = 6;
		starTriangles[8] = 5;

		return starTriangles;
	}
	
	private Mesh DoCreateMesh()
	{
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = GetVerts( Size ); 
		m.triangles = GetTriangles();
		m.RecalculateNormals();
		
		return m;
	}
	
	void Start() 
	{
		mMeshRenderer = gameObject.AddComponent<MeshRenderer>();
		mMesh = gameObject.AddComponent<MeshFilter>();
		mMesh.mesh = DoCreateMesh();
		mMeshRenderer.material = Mat;
	}
}
