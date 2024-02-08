using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ExtrudeTest : MonoBehaviour
{

    // WIP - not currently working

    private void CreateProBuilderMesh(Vector3[] verts, Vector3[][] holes, float depth)
    {
        var pbMesh = gameObject.GetComponentInChildren<ProBuilderMesh>();
        var points = verts.Select(v => new Vector3(v.x,  v.y, 0)).ToList();
        var result = pbMesh.CreateShapeFromPolygon(points, depth, false, holes);
        pbMesh.ToMesh();
    }

    private Mesh ConvertBackToRegularMesh(ProBuilderMesh pbMesh)
    {
        if (pbMesh != null)
        {
            // pbMesh.ToMesh();
            // pbMesh.Refresh();
            Mesh regularMesh = GetComponent<MeshFilter>().sharedMesh;
            // Destroy(pbMesh);
            return regularMesh;
        }
        return null;
    }
}
