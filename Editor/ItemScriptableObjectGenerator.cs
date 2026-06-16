using System.Collections.Generic;
using System.IO;
using DavonAllen.ContentPipelineDemo;
using UnityEditor;
using UnityEngine;

namespace DavonAllen.ContentPipelineDemo.Editor
{
    public sealed class ItemScriptableObjectGenerator
    {
        public int Generate(IEnumerable<ItemDefinition> items, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);

            int generatedCount = 0;

            foreach (ItemDefinition item in items)
            {
                string assetPath = Path.Combine(outputDirectory, item.Id + ".asset").Replace("\\", "/");
                ItemDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<ItemDefinitionAsset>(assetPath);

                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<ItemDefinitionAsset>();
                    AssetDatabase.CreateAsset(asset, assetPath);
                }

                asset.Apply(item);
                EditorUtility.SetDirty(asset);
                generatedCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return generatedCount;
        }
    }
}
