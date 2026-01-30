using UnityEngine;
using UnityEditor;
using System.IO;

namespace LuckyCharm.Editor
{
    public static class PlaceholderSpriteGenerator
    {
        private static readonly string SpritePath = "Assets/Sprites";
        
        [MenuItem("LuckyCharm/Generate Placeholder Sprites")]
        public static void GeneratePlaceholders()
        {
            if (!Directory.Exists(SpritePath))
            {
                Directory.CreateDirectory(SpritePath);
            }
            
            // Cookie intact - brown circle
            CreateCircleSprite("cookie_intact", 256, new Color(0.76f, 0.53f, 0.26f));
            
            // Cookie crack states - with crack lines
            CreateCrackedCookieSprite("cookie_crack_1", 256, 1);
            CreateCrackedCookieSprite("cookie_crack_2", 256, 2);
            CreateCrackedCookieSprite("cookie_crack_3", 256, 3);
            CreateCrackedCookieSprite("cookie_crack_4", 256, 4);
            
            // Cookie halves
            CreateCookieHalfSprite("cookie_left_half", 256, true);
            CreateCookieHalfSprite("cookie_right_half", 256, false);
            
            // Paper rolled - small cream cylinder
            CreateRolledPaperSprite("paper_rolled", 128, 64);
            
            // Paper unrolled - horizontal rectangle
            CreateUnrolledPaperSprite("paper_unrolled", 512, 128);
            
            // Background - warm wood gradient
            CreateBackgroundSprite("background", 1080, 1920);
            
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Placeholder sprites generated successfully!");
        }
        
        private static void CreateCircleSprite(string name, int size, Color color)
        {
            Texture2D tex = new Texture2D(size, size);
            float center = size / 2f;
            float radius = size / 2f - 4;
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist < radius)
                    {
                        float shade = 1f - (dist / radius) * 0.2f;
                        tex.SetPixel(x, y, new Color(color.r * shade, color.g * shade, color.b * shade, 1f));
                    }
                    else if (dist < radius + 2)
                    {
                        tex.SetPixel(x, y, new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f, 1f));
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            
            SaveTexture(tex, name);
        }
        
        private static void CreateCrackedCookieSprite(string name, int size, int crackCount)
        {
            Texture2D tex = new Texture2D(size, size);
            Color cookieColor = new Color(0.76f, 0.53f, 0.26f);
            float center = size / 2f;
            float radius = size / 2f - 4;
            
            // Draw base cookie
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist < radius)
                    {
                        float shade = 1f - (dist / radius) * 0.2f;
                        tex.SetPixel(x, y, new Color(cookieColor.r * shade, cookieColor.g * shade, cookieColor.b * shade, 1f));
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            
            // Draw crack lines
            Color crackColor = new Color(0.3f, 0.2f, 0.1f);
            for (int i = 0; i < crackCount; i++)
            {
                float angle = (i * 360f / crackCount + 45f) * Mathf.Deg2Rad;
                Vector2 start = new Vector2(center, center);
                Vector2 end = start + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (radius * 0.8f);
                DrawLine(tex, start, end, crackColor, 2);
            }
            
            SaveTexture(tex, name);
        }
        
        private static void CreateCookieHalfSprite(string name, int size, bool isLeft)
        {
            Texture2D tex = new Texture2D(size / 2 + 16, size);
            Color cookieColor = new Color(0.76f, 0.53f, 0.26f);
            float center = size / 2f;
            float radius = size / 2f - 4;
            
            int offsetX = isLeft ? 0 : -(size / 2 - 16);
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    int worldX = x + (isLeft ? 0 : size / 2 - 16);
                    float dist = Vector2.Distance(new Vector2(worldX, y), new Vector2(center, center));
                    
                    bool inHalf = isLeft ? (worldX < center + 8) : (worldX > center - 8);
                    
                    if (dist < radius && inHalf)
                    {
                        float shade = 1f - (dist / radius) * 0.2f;
                        tex.SetPixel(x, y, new Color(cookieColor.r * shade, cookieColor.g * shade, cookieColor.b * shade, 1f));
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            
            SaveTexture(tex, name);
        }
        
        private static void CreateRolledPaperSprite(string name, int width, int height)
        {
            Texture2D tex = new Texture2D(width, height);
            Color paperColor = new Color(0.98f, 0.95f, 0.87f);
            Color shadowColor = new Color(0.9f, 0.87f, 0.79f);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float edgeDist = Mathf.Min(x, width - x - 1);
                    float roundness = Mathf.Sin((float)y / height * Mathf.PI);
                    
                    if (edgeDist < 8 * roundness)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float shade = 0.9f + roundness * 0.1f;
                        tex.SetPixel(x, y, Color.Lerp(shadowColor, paperColor, shade));
                    }
                }
            }
            
            SaveTexture(tex, name);
        }
        
        private static void CreateUnrolledPaperSprite(string name, int width, int height)
        {
            Texture2D tex = new Texture2D(width, height);
            Color paperColor = new Color(0.98f, 0.95f, 0.87f);
            Color edgeColor = new Color(0.92f, 0.89f, 0.81f);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float edgeDistX = Mathf.Min(x, width - x - 1) / (float)width;
                    float edgeDistY = Mathf.Min(y, height - y - 1) / (float)height;
                    float edgeFactor = Mathf.Min(edgeDistX, edgeDistY) * 10f;
                    edgeFactor = Mathf.Clamp01(edgeFactor);
                    
                    tex.SetPixel(x, y, Color.Lerp(edgeColor, paperColor, edgeFactor));
                }
            }
            
            SaveTexture(tex, name);
        }
        
        private static void CreateBackgroundSprite(string name, int width, int height)
        {
            Texture2D tex = new Texture2D(width, height);
            Color topColor = new Color(0.55f, 0.35f, 0.2f);
            Color bottomColor = new Color(0.4f, 0.25f, 0.15f);
            
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / height;
                Color rowColor = Color.Lerp(bottomColor, topColor, t);
                
                for (int x = 0; x < width; x++)
                {
                    // Add subtle noise for wood texture
                    float noise = (Mathf.PerlinNoise(x * 0.02f, y * 0.005f) - 0.5f) * 0.1f;
                    Color pixelColor = new Color(
                        Mathf.Clamp01(rowColor.r + noise),
                        Mathf.Clamp01(rowColor.g + noise),
                        Mathf.Clamp01(rowColor.b + noise),
                        1f
                    );
                    tex.SetPixel(x, y, pixelColor);
                }
            }
            
            SaveTexture(tex, name);
        }
        
        private static void DrawLine(Texture2D tex, Vector2 start, Vector2 end, Color color, int thickness)
        {
            float dist = Vector2.Distance(start, end);
            for (float t = 0; t <= 1f; t += 1f / dist)
            {
                Vector2 point = Vector2.Lerp(start, end, t);
                for (int dx = -thickness; dx <= thickness; dx++)
                {
                    for (int dy = -thickness; dy <= thickness; dy++)
                    {
                        int px = Mathf.RoundToInt(point.x) + dx;
                        int py = Mathf.RoundToInt(point.y) + dy;
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        {
                            tex.SetPixel(px, py, color);
                        }
                    }
                }
            }
        }
        
        private static void SaveTexture(Texture2D tex, string name)
        {
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            string path = Path.Combine(SpritePath, name + ".png");
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(tex);
        }
    }
}
