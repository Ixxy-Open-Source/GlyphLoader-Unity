using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;
using WaterTrans.GlyphLoader;

public class GlyphLoaderExtrusionTest : MonoBehaviour
{
    public string fontPath;
    public string text = "Hello";
    public Color color = Color.white;

    public float ExtrusionDepth = 0.25f;
    public Vector3 translation = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
    public Vector3 scale = new (1, -1, 1);

    private float unit = 1f;

    void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    void Generate()
    {
        var svg = GenerateSvg();

        var baseTransform = Matrix4x4.TRS(
            translation,
            Quaternion.Euler(rotation),
            scale
        );
        var importer = new RuntimeSVGImporter();
        var mf = gameObject.GetComponent<MeshFilter>();
        mf.mesh = importer.ParseToMesh(svg, baseTransform, ExtrusionDepth);
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
