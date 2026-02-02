using UnityEngine;
using UnityEditor;
using System.IO;

public class FortuneCookieSpriteGenerator : EditorWindow
{
    private Texture2D sourceTexture;
    private string outputFolder = "Assets/Sprites/FortuneCookie";
    
    [MenuItem("Tools/Generate Fortune Cookie Sprites")]
    public static void ShowWindow()
    {
        GetWindow<FortuneCookieSpriteGenerator>("Fortune Cookie Generator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Fortune Cookie Sprite Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Generate Sprites"))
        {
            if (sourceTexture == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a source texture", "OK");
                return;
            }
            GenerateSprites();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Auto-Generate from FortuneCokie.png"))
        {
            AutoGenerate();
        }
    }
    
    private void AutoGenerate()
    {
        sourceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/FortuneCokie.png");
        if (sourceTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find Assets/Sprites/FortuneCokie.png", "OK");
            return;
        }
        GenerateSprites();
    }
    
    private void GenerateSprites()
    {
        // Ensure source texture is readable
        string sourcePath = AssetDatabase.GetAssetPath(sourceTexture);
        TextureImporter importer = AssetImporter.GetAtPath(sourcePath) as TextureImporter;
        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }
        
        // Create output folder
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            AssetDatabase.Refresh();
        }
        
        int width = sourceTexture.width;
        int height = sourceTexture.height;
        Color[] pixels = sourceTexture.GetPixels();
        
        // Generate intact sprite (copy of original)
        SaveTexture(pixels, width, height, "fortune_cookie_intact.png");
        
        // Generate crack stages (1-4)
        for (int stage = 1; stage <= 4; stage++)
        {
            Color[] crackedPixels = GenerateCrackStage(pixels, width, height, stage);
            SaveTexture(crackedPixels, width, height, $"fortune_cookie_crack_{stage}.png");
        }
        
        // Generate left and right halves
        Color[] leftHalf = GenerateLeftHalf(pixels, width, height);
        SaveTexture(leftHalf, width, height, "fortune_cookie_left_half.png");
        
        Color[] rightHalf = GenerateRightHalf(pixels, width, height);
        SaveTexture(rightHalf, width, height, "fortune_cookie_right_half.png");
        
        AssetDatabase.Refresh();
        
        // Set import settings for all generated textures
        SetSpriteImportSettings();
        
        EditorUtility.DisplayDialog("Success", "Fortune cookie sprites generated!", "OK");
    }
    
    private Color[] GenerateCrackStage(Color[] original, int width, int height, int stage)
    {
        Color[] result = (Color[])original.Clone();
        
        // Find the center of the cookie (approximate)
        int centerX = width / 2;
        int centerY = height / 2;
        
        // Crack parameters based on stage
        float crackIntensity = stage / 4f;
        int numCracks = stage + 1;
        
        // Draw crack lines
        Color crackColor = new Color(0.3f, 0.2f, 0.1f, 1f); // Dark brown
        Color shadowColor = new Color(0.2f, 0.15f, 0.08f, 0.8f);
        
        System.Random rand = new System.Random(42 + stage); // Consistent seed per stage
        
        for (int crack = 0; crack < numCracks; crack++)
        {
            // Start from center area
            float angle = (crack * 360f / numCracks + rand.Next(-20, 20)) * Mathf.Deg2Rad;
            float startOffset = 10 + rand.Next(0, 20);
            
            int startX = centerX + (int)(Mathf.Cos(angle) * startOffset);
            int startY = centerY + (int)(Mathf.Sin(angle) * startOffset);
            
            // Draw jagged crack line
            float currentAngle = angle;
            int x = startX;
            int y = startY;
            int crackLength = (int)(30 + crackIntensity * 60 + rand.Next(0, 30));
            
            for (int i = 0; i < crackLength; i++)
            {
                // Add some randomness to the direction
                currentAngle += (rand.Next(-30, 30) * Mathf.Deg2Rad * 0.1f);
                
                x += (int)(Mathf.Cos(currentAngle) * 2);
                y += (int)(Mathf.Sin(currentAngle) * 2);
                
                // Draw crack pixel with some width
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int px = Mathf.Clamp(x + dx, 0, width - 1);
                        int py = Mathf.Clamp(y + dy, 0, height - 1);
                        int idx = py * width + px;
                        
                        if (original[idx].a > 0.1f) // Only draw on cookie pixels
                        {
                            if (dx == 0 && dy == 0)
                            {
                                result[idx] = Color.Lerp(result[idx], crackColor, 0.8f);
                            }
                            else
                            {
                                result[idx] = Color.Lerp(result[idx], shadowColor, 0.4f);
                            }
                        }
                    }
                }
                
                // Branch occasionally for later stages
                if (stage >= 3 && rand.Next(0, 100) < 10)
                {
                    DrawBranchCrack(result, original, x, y, currentAngle + (rand.Next(0, 2) == 0 ? 0.5f : -0.5f), 
                                   width, height, rand, crackColor, shadowColor, 10);
                }
            }
        }
        
        return result;
    }
    
    private void DrawBranchCrack(Color[] result, Color[] original, int startX, int startY, 
                                  float angle, int width, int height, System.Random rand,
                                  Color crackColor, Color shadowColor, int length)
    {
        int x = startX;
        int y = startY;
        float currentAngle = angle;
        
        for (int i = 0; i < length; i++)
        {
            currentAngle += (rand.Next(-20, 20) * Mathf.Deg2Rad * 0.1f);
            x += (int)(Mathf.Cos(currentAngle) * 2);
            y += (int)(Mathf.Sin(currentAngle) * 2);
            
            int px = Mathf.Clamp(x, 0, width - 1);
            int py = Mathf.Clamp(y, 0, height - 1);
            int idx = py * width + px;
            
            if (original[idx].a > 0.1f)
            {
                result[idx] = Color.Lerp(result[idx], crackColor, 0.6f);
            }
        }
    }
    
    private Color[] GenerateLeftHalf(Color[] original, int width, int height)
    {
        Color[] result = new Color[original.Length];
        
        // Find the cookie bounds
        int minX = width, maxX = 0, minY = height, maxY = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (original[y * width + x].a > 0.1f)
                {
                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }
        
        int cookieCenterX = (minX + maxX) / 2;
        
        // Copy left portion with jagged edge
        System.Random rand = new System.Random(123);
        
        for (int y = 0; y < height; y++)
        {
            int jaggedOffset = rand.Next(-5, 8);
            int cutX = cookieCenterX + jaggedOffset;
            
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                if (x < cutX && original[idx].a > 0.1f)
                {
                    result[idx] = original[idx];
                    
                    // Add darker edge near the cut
                    if (x > cutX - 8)
                    {
                        float edgeFactor = 1f - ((cutX - x) / 8f);
                        result[idx] = Color.Lerp(result[idx], new Color(0.4f, 0.3f, 0.2f, result[idx].a), edgeFactor * 0.5f);
                    }
                }
                else
                {
                    result[idx] = Color.clear;
                }
            }
        }
        
        return result;
    }
    
    private Color[] GenerateRightHalf(Color[] original, int width, int height)
    {
        Color[] result = new Color[original.Length];
        
        // Find the cookie bounds
        int minX = width, maxX = 0, minY = height, maxY = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (original[y * width + x].a > 0.1f)
                {
                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }
        
        int cookieCenterX = (minX + maxX) / 2;
        
        // Copy right portion with jagged edge (use same seed for matching edge)
        System.Random rand = new System.Random(123);
        
        for (int y = 0; y < height; y++)
        {
            int jaggedOffset = rand.Next(-5, 8);
            int cutX = cookieCenterX + jaggedOffset;
            
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                if (x >= cutX && original[idx].a > 0.1f)
                {
                    result[idx] = original[idx];
                    
                    // Add darker edge near the cut
                    if (x < cutX + 8)
                    {
                        float edgeFactor = 1f - ((x - cutX) / 8f);
                        result[idx] = Color.Lerp(result[idx], new Color(0.4f, 0.3f, 0.2f, result[idx].a), edgeFactor * 0.5f);
                    }
                }
                else
                {
                    result[idx] = Color.clear;
                }
            }
        }
        
        return result;
    }
    
    private void SaveTexture(Color[] pixels, int width, int height, string filename)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.SetPixels(pixels);
        tex.Apply();
        
        byte[] pngData = tex.EncodeToPNG();
        string path = Path.Combine(outputFolder, filename);
        File.WriteAllBytes(path, pngData);
        
        DestroyImmediate(tex);
        Debug.Log($"Saved: {path}");
    }
    
    private void SetSpriteImportSettings()
    {
        string[] files = Directory.GetFiles(outputFolder, "*.png");
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
    
    [MenuItem("Tools/Auto-Generate Fortune Cookie Sprites")]
    public static void AutoGenerateMenuItem()
    {
        var window = CreateInstance<FortuneCookieSpriteGenerator>();
        window.AutoGenerate();
        DestroyImmediate(window);
    }
}
