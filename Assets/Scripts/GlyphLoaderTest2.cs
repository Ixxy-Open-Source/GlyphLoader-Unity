using System.IO;
using Unity.VectorGraphics;
using UnityEngine;
using WaterTrans.GlyphLoader;

public class GlyphLoaderTest2 : MonoBehaviour
{
    public string fontPath;
    public string text = "Hello";
    public Color color = Color.white;

    public string Id;
    public int element = -1;
    public int childElement = -1;

    public Vector3 translation = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
    public Vector3 scale = new (1, -1, 1);

    public Vector3 translateEach = Vector3.zero;
    public Vector3 rotateEach = Vector3.zero;
    public Vector3 scaleEach = new (1, 1, 1);

    private float unit = 1f;

    void Start()
    {
        Generate();
    }

    private string _GetNodeInfo(SceneNode node)
    {
        return $"{node.Children?.Count ?? 0} children. {node.Shapes?.Count ?? 0} shapes.";
    }

    [ContextMenu("Generate")]
    void Generate()
    {
        var svg = GenerateSvg();
        var importer = new RuntimeSVGImporter();

        SVGParser.SceneInfo sceneInfo = importer.ParseToSceneInfo(svg);

        if (!string.IsNullOrEmpty(Id))
        {
            // Extract a node by it's id
            sceneInfo = sceneInfo.FromNodeId(Id);
        }
        else if (element != -1)
        {
            // Extract a node by index
            sceneInfo = sceneInfo.FromChildIndex(element);
            if (childElement != -1)
            {
                sceneInfo = sceneInfo.FromChildIndex(childElement);
            }
        }

        var scene = sceneInfo.Scene;
        int numChildren = scene.Root.Children?.Count ?? 0;

        // Some useful debugging info
        for (var i = 0; i < numChildren; i++)
        {
            var child = scene.Root.Children[i];
            Debug.Log($"Child {i}: {_GetNodeInfo(child)}");
        }
        foreach (var id in sceneInfo.NodeIDs)
        {
            Debug.Log($"NodeIDs: {id.Key}: {_GetNodeInfo(id.Value)}");
        }
        foreach (var opacity in sceneInfo.NodeOpacity)
        {
            Debug.Log($"NodeOpacity: {opacity.Key}: {opacity.Value}");
        }

        int numShapes = scene.Root.Shapes?.Count ?? 0;
        bool singleNode = numChildren == 0 && numShapes > 0;
        int numMeshes = singleNode ? 1 : numChildren;

        CombineInstance[] combine = new CombineInstance[numMeshes];
        var tr = Matrix4x4.TRS(
            translation,
            Quaternion.Euler(rotation),
            scale
        );

        for (var i = 0; i < numMeshes; i++)
        {
            tr *= Matrix4x4.TRS(
                translateEach,
                Quaternion.Euler(rotateEach),
                scaleEach
            );

            // Edge case - a single node with shapes but no children
            SVGParser.SceneInfo newSceneInfo = sceneInfo;

            // Common case - a node with children
            if (!singleNode)
            {
                newSceneInfo = sceneInfo.FromChildIndex(i);
            }

            var c = new CombineInstance();
            c.mesh = importer.SceneInfoToMesh(newSceneInfo, tr);
            combine[i] = c;
        }
        var mf = gameObject.GetComponent<MeshFilter>();
        mf.mesh.CombineMeshes(combine, mergeSubMeshes: true, useMatrices: false);
    }

    private string GenerateSvg()
    {
        float x = 0;
        float y = 0;
        var svg = new System.Text.StringBuilder();
        svg.AppendLine(
            "<svg width='440' height='140' viewBox='0 0 440 140' xmlns='http://www.w3.org/2000/svg' version='1.1'>");
        var stream = new FileStream(fontPath, FileMode.Open, FileAccess.Read);
        var typeface = new Typeface(stream);
        double baseline = typeface.Baseline * unit;

        foreach (char character in text)
        {
            var glyphIndex = typeface.CharacterToGlyphMap[character];
            var geometry = typeface.GetGlyphOutline(glyphIndex, unit);
            double advanceWidth = typeface.AdvanceWidths[glyphIndex] * unit;
            string svgPath = geometry.Figures.ToString(x, y + baseline);
            svg.AppendLine($"<path d='{svgPath}' fill='#{ColorUtility.ToHtmlStringRGB(color)}' stroke-width='0' />");
            x += (float)advanceWidth;
        }

        svg.AppendLine("</svg>");
        return svg.ToString();
    }
}
