using System;
using System.IO;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using WaterTrans.GlyphLoader;

public class SvgTest : MonoBehaviour
{
    public int element = 0;
    public int childElement = 0;
    public Vector3 translation = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
    public Vector3 scale = new (1, -1, 1);

    public Vector3 translateEach = Vector3.zero;
    public Vector3 rotateEach = Vector3.zero;
    public Vector3 scaleEach = new (1, -1, 1);


    [Multiline(16)]
    public string rawSvg;

    private float unit = 1f;

    void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    void Generate()
    {
        var importer = new RuntimeSVGImporter();
        SVGParser.SceneInfo sceneInfo = importer.ParseToSceneInfo(rawSvg);
        var scene = sceneInfo.Scene;

        foreach (var id in sceneInfo.NodeIDs)
        {
            Debug.Log($"{id.Key}: {id.Value}");
        }

        SceneNode node = scene.Root;
        node = node.Children[element];
        node = node.Children[childElement];
        CombineInstance[] combine = new CombineInstance[node.Children.Count];
        var tr = Matrix4x4.TRS(
            translation,
            Quaternion.Euler(rotation),
            scale
    );
    for (var i = 0; i < node.Children.Count; i++)
        {
            var child = node.Children[i];
            scene.Root = child;
            var c = new CombineInstance();
            tr *= Matrix4x4.TRS(
                translateEach,
                Quaternion.Euler(rotateEach),
                scaleEach
            );
            c.mesh = importer.SceneInfoToMesh(sceneInfo, tr);
            combine[i] = c;
        }
        var mf = gameObject.GetComponent<MeshFilter>();
        mf.mesh.CombineMeshes(combine, mergeSubMeshes: true, useMatrices: false);
    }
}
