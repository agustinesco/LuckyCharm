using UnityEngine;
using UnityEditor;

namespace LuckyCharm.Editor
{
    public class SpriteImportSettings : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.StartsWith("Assets/Sprites/"))
            {
                TextureImporter importer = (TextureImporter)assetImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.maxTextureSize = 2048;
            }
        }
    }
    
    public static class SpriteConfigurer
    {
        [MenuItem("LuckyCharm/Configure Sprites as UI")]
        public static void ConfigureSpritesAsUI()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.mipmapEnabled = false;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.maxTextureSize = 2048;
                    
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                }
            }
            
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log($"Configured {guids.Length} sprites as UI sprites.");
        }
    }
}
