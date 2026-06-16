using System.IO;
using DavonAllen.ContentPipelineDemo;
using UnityEditor;
using UnityEngine;

namespace DavonAllen.ContentPipelineDemo.Editor
{
    public sealed class ContentPipelineWindow : EditorWindow
    {
        private TextAsset csvSource;
        private DefaultAsset outputFolder;
        private bool includeDisabled;
        private bool generateJson = true;
        private bool generateScriptableObjects = true;
        private Vector2 diagnosticsScroll;
        private ItemCatalogPipelineResult lastResult;
        private int lastGeneratedAssetCount;
        private string lastOutputPath = "Assets/Generated/item_catalog.generated.json";

        [MenuItem("Tools/Davon/Content Pipeline Demo")]
        public static void Open()
        {
            ContentPipelineWindow window = GetWindow<ContentPipelineWindow>("Content Pipeline Demo");
            window.minSize = new Vector2(540, 420);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Item Catalog Import", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            csvSource = (TextAsset)EditorGUILayout.ObjectField("CSV Source", csvSource, typeof(TextAsset), false);
            outputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Output Folder", outputFolder, typeof(DefaultAsset), false);
            includeDisabled = EditorGUILayout.Toggle("Include Disabled Rows", includeDisabled);
            generateJson = EditorGUILayout.Toggle("Generate JSON", generateJson);
            generateScriptableObjects = EditorGUILayout.Toggle("Generate ScriptableObjects", generateScriptableObjects);

            EditorGUILayout.Space(8);

            using (new EditorGUI.DisabledScope(csvSource == null))
            {
                if (GUILayout.Button("Validate And Generate", GUILayout.Height(32)))
                {
                    RunPipeline();
                }
            }

            EditorGUILayout.Space(8);
            DrawResultSummary();
            DrawDiagnostics();
        }

        private void RunPipeline()
        {
            ItemCatalogPipeline pipeline = new ItemCatalogPipeline();
            ItemCatalogPipelineOptions options = new ItemCatalogPipelineOptions
            {
                IncludeDisabled = includeDisabled
            };

            lastResult = pipeline.Run(csvSource.text, options);

            if (lastResult.HasErrors)
            {
                lastGeneratedAssetCount = 0;
                return;
            }

            string outputDirectory = ResolveOutputDirectory();

            if (generateJson)
            {
                Directory.CreateDirectory(outputDirectory);
                lastOutputPath = Path.Combine(outputDirectory, "item_catalog.generated.json").Replace("\\", "/");
                File.WriteAllText(lastOutputPath, lastResult.GeneratedJson);
                AssetDatabase.Refresh();
            }

            lastGeneratedAssetCount = 0;

            if (generateScriptableObjects)
            {
                string assetOutputDirectory = Path.Combine(outputDirectory, "ItemDefinitions").Replace("\\", "/");
                ItemScriptableObjectGenerator generator = new ItemScriptableObjectGenerator();
                lastGeneratedAssetCount = generator.Generate(lastResult.Items, assetOutputDirectory);
            }
        }

        private string ResolveOutputDirectory()
        {
            if (outputFolder == null)
            {
                return "Assets/Generated";
            }

            string path = AssetDatabase.GetAssetPath(outputFolder);

            if (AssetDatabase.IsValidFolder(path))
            {
                return path;
            }

            return "Assets/Generated";
        }

        private void DrawResultSummary()
        {
            if (lastResult == null)
            {
                EditorGUILayout.HelpBox("Select a CSV file and run the pipeline.", MessageType.Info);
                return;
            }

            if (lastResult.HasErrors)
            {
                EditorGUILayout.HelpBox("Validation failed. Fix errors before generating config.", MessageType.Error);
                return;
            }

            string message = "Validated " + lastResult.Items.Count + " item(s).";

            if (generateJson)
            {
                message += " JSON: " + lastOutputPath + ".";
            }

            if (generateScriptableObjects)
            {
                message += " ScriptableObjects: " + lastGeneratedAssetCount + ".";
            }

            MessageType messageType = lastResult.HasWarnings ? MessageType.Warning : MessageType.Info;
            EditorGUILayout.HelpBox(message, messageType);
        }

        private void DrawDiagnostics()
        {
            if (lastResult == null)
            {
                return;
            }

            EditorGUILayout.LabelField("Diagnostics", EditorStyles.boldLabel);
            diagnosticsScroll = EditorGUILayout.BeginScrollView(diagnosticsScroll);

            foreach (PipelineDiagnostic diagnostic in lastResult.Diagnostics)
            {
                MessageType messageType = ToMessageType(diagnostic.Severity);
                EditorGUILayout.HelpBox(diagnostic.ToString(), messageType);
            }

            if (lastResult.Diagnostics.Count == 0)
            {
                EditorGUILayout.HelpBox("No diagnostics.", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
        }

        private static MessageType ToMessageType(DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticSeverity.Error:
                    return MessageType.Error;
                case DiagnosticSeverity.Warning:
                    return MessageType.Warning;
                default:
                    return MessageType.Info;
            }
        }
    }
}
