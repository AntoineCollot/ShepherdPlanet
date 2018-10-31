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

    [ContextMenu("Build Mesh")]
    public void BuildMesh()
    {
        int meshCount = (int)(2 * Mathf.PI * areaSize * density);
        CombineInstance[] combineInstances = new CombineInstance[meshCount];
        for (int i = 0; i < meshCount; i++)
        {
            combineInstances[i].mesh = originalMesh;

            //Find a random position in the circle.
            //We do this this way instead of getting a random direction and random distance so we get an even density across the area, otherwise they get stacked at the center
            Vector3 randomPosition;
            do
            {
                randomPosition = new Vector3(Random.Range(-areaSize, areaSize), 0, Random.Range(-areaSize, areaSize)) * 0.5f;
            } while (randomPosition.magnitude > areaSize * 0.5f);


            Matrix4x4 matrix = Matrix4x4.TRS(randomPosition, Quaternion.identity, Vector3.one);
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
