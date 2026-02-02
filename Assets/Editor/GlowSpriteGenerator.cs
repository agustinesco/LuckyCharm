using UnityEngine;
using UnityEditor;
using System.IO;

public class GlowSpriteGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Glow Sprite")]
    public static void GenerateGlowSprite()
    {
        string outputFolder = "Assets/Sprites";
        int size = 512;
        
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        
        int center = size / 2;
        float maxRadius = size / 2f;
        
        // Dark gradient to dim background behind cookie
        Color glowColor = new Color(0f, 0f, 0f, 0.85f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                
                // Smooth falloff from center
                float t = dist / maxRadius;
                
                // Use a smooth curve for the glow falloff
                float alpha = 0f;
                if (t < 1f)
                {
                    // Gaussian-like falloff
                    alpha = Mathf.Exp(-t * t * 2f) * glowColor.a;
                }
                
                pixels[y * size + x] = new Color(glowColor.r, glowColor.g, glowColor.b, alpha);
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        byte[] pngData = tex.EncodeToPNG();
        string path = Path.Combine(outputFolder, "cookie_glow.png");
        File.WriteAllBytes(path, pngData);
        
        Object.DestroyImmediate(tex);
        
        AssetDatabase.Refresh();
        
        // Set import settings
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
        
        Debug.Log("Glow sprite generated: " + path);
    }
}
