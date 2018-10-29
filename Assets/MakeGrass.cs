using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MakeGrass : MonoBehaviour {

    [SerializeField]
    Mesh originalMesh;


    [SerializeField]
    Mesh otherMesh;

    public float areaSize;

    public float density;

    public Mesh result;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ContextMenu("Build Mesh")]
    public void BuildMesh()
    {
        int meshCount = (int)(2 * Mathf.PI * areaSize * density);
        CombineInstance[] combineInstances = new CombineInstance[meshCount];
        for (int i = 0; i < meshCount; i++)
        {
            combineInstances[i].mesh = originalMesh;
            float randomAngle = 2 * Mathf.PI * Random.Range(0, 1.0f);
            float randomDist = Curves.QuadEaseOut(0,areaSize,Random.Range(0, 1.0f));

            Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(Mathf.Cos(randomAngle) * randomDist, 0, Mathf.Sin(randomAngle) * randomDist), Quaternion.identity, Vector3.one);
            combineInstances[i].transform = matrix;
        }

        result = new Mesh();
        result.CombineMeshes(combineInstances, true, true);

        GetComponent<MeshFilter>().mesh = result;
        SaveMesh(result);
    }

    public void SaveMesh(Mesh mesh)
    {
        AssetDatabase.CreateAsset(mesh, "Assets/generatedMesh.asset");
        AssetDatabase.SaveAssets();
    }


    Mesh CopyMesh(Mesh mesh)
    {
        Mesh newMesh = new Mesh();
        newMesh.vertices = mesh.vertices;
        newMesh.normals = mesh.normals;
        newMesh.uv = mesh.uv;
        newMesh.triangles = mesh.triangles;
        newMesh.tangents = mesh.tangents;
        newMesh.colors = mesh.colors;
        return newMesh;
    }
}
