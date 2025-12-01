using System.IO;
using UnityEditor;
using UnityEngine;

namespace VSCodeEditor
{
    [InitializeOnLoad]
    public static class KPLInitializer
    {
        static KPLInitializer()
        {
            EditorApplication.delayCall += Initialize;
        }
        
        static void Initialize()
        {
            Debug.Log("kot_pod_lawkoy | Generator is ready to work");
            
            // Можно проверить есть ли .csproj файлы, если нет - предложить сгенерировать
            CheckForMissingProjectFiles();
        }
        
        static void CheckForMissingProjectFiles()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string projectName = Path.GetFileName(projectPath);
            string slnPath = Path.Combine(projectPath, $"{projectName}.sln");
            
            if (!File.Exists(slnPath))
            {
                Debug.Log("kot_pod_lawkoy | .sln file was not found, you can generate this by Tools/kot_pod_lawkoy | Generate .sln and .csproj");
            }
        }
    }
}