using UnityEngine;
using UnityEditor;
using System.IO;

public class ClockSpriteGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Clock Sprites")]
    public static void GenerateClockSprites()
    {
        string outputFolder = "Assets/Sprites/Clock";
        
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        // Generate clock face (circle with border)
        GenerateClockFace(outputFolder, 256);
        
        // Generate clock fill (solid circle for radial fill)
        GenerateClockFill(outputFolder, 256);
        
        // Generate hour hand
        GenerateClockHand(outputFolder, "clock_hour_hand", 20, 80, new Color(0.3f, 0.25f, 0.2f));
        
        // Generate minute hand
        GenerateClockHand(outputFolder, "clock_minute_hand", 12, 100, new Color(0.4f, 0.35f, 0.3f));
        
        // Generate center dot
        GenerateCenterDot(outputFolder, 24);
        
        AssetDatabase.Refresh();
        SetSpriteImportSettings(outputFolder);
        
        Debug.Log("Clock sprites generated successfully!");
        EditorUtility.DisplayDialog("Success", "Clock sprites generated in " + outputFolder, "OK");
    }
    
    private static void GenerateClockFace(string folder, int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        
        int center = size / 2;
        float outerRadius = size / 2 - 4;
        float innerRadius = outerRadius - 8;
        
        // Background color (warm cream)
        Color bgColor = new Color(0.98f, 0.95f, 0.88f, 1f);
        Color borderColor = new Color(0.6f, 0.45f, 0.3f, 1f);
        Color tickColor = new Color(0.5f, 0.4f, 0.3f, 1f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (dist <= outerRadius)
                {
                    if (dist > innerRadius)
                    {
                        // Border
                        float edgeSoftness = Mathf.Clamp01((outerRadius - dist) * 2);
                        pixels[y * size + x] = Color.Lerp(Color.clear, borderColor, edgeSoftness);
                    }
                    else
                    {
                        // Inner fill
                        pixels[y * size + x] = bgColor;
                    }
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        // Draw hour markers
        for (int hour = 0; hour < 12; hour++)
        {
            float angle = hour * 30f * Mathf.Deg2Rad - Mathf.PI / 2;
            float markerInner = innerRadius - 20;
            float markerOuter = innerRadius - 8;
            
            for (float r = markerInner; r <= markerOuter; r += 0.5f)
            {
                int px = center + (int)(Mathf.Cos(angle) * r);
                int py = center + (int)(Mathf.Sin(angle) * r);
                
                if (px >= 0 && px < size && py >= 0 && py < size)
                {
                    // Draw thicker marker
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int idx = Mathf.Clamp(py + dy, 0, size - 1) * size + Mathf.Clamp(px + dx, 0, size - 1);
                            pixels[idx] = tickColor;
                        }
                    }
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(folder, "clock_face.png"), pngData);
        Object.DestroyImmediate(tex);
    }
    
    private static void GenerateClockFill(string folder, int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        
        int center = size / 2;
        float radius = size / 2 - 16;
        
        Color fillColor = new Color(0.85f, 0.65f, 0.4f, 0.6f); // Warm orange, semi-transparent
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (dist <= radius)
                {
                    float edgeSoftness = Mathf.Clamp01((radius - dist) * 0.5f);
                    pixels[y * size + x] = Color.Lerp(Color.clear, fillColor, edgeSoftness);
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(folder, "clock_fill.png"), pngData);
        Object.DestroyImmediate(tex);
    }
    
    private static void GenerateClockHand(string folder, string name, int width, int height, Color color)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        
        int centerX = width / 2;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = Mathf.Abs(x - centerX);
                
                // Tapered hand shape
                float maxWidth = Mathf.Lerp(width / 2f, 1f, (float)y / height);
                
                if (dx <= maxWidth)
                {
                    float edgeSoftness = Mathf.Clamp01((maxWidth - dx) * 2);
                    pixels[y * width + x] = Color.Lerp(Color.clear, color, edgeSoftness);
                }
                else
                {
                    pixels[y * width + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(folder, name + ".png"), pngData);
        Object.DestroyImmediate(tex);
    }
    
    private static void GenerateCenterDot(string folder, int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        
        int center = size / 2;
        float radius = size / 2 - 2;
        
        Color dotColor = new Color(0.35f, 0.28f, 0.2f, 1f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (dist <= radius)
                {
                    float edgeSoftness = Mathf.Clamp01((radius - dist) * 2);
                    pixels[y * size + x] = Color.Lerp(Color.clear, dotColor, edgeSoftness);
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(folder, "clock_center.png"), pngData);
        Object.DestroyImmediate(tex);
    }
    
    private static void SetSpriteImportSettings(string folder)
    {
        string[] files = Directory.GetFiles(folder, "*.png");
        foreach (string file in files)
        {
            string assetPath = file.Replace("\\", "/");
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
        }
    }
}
