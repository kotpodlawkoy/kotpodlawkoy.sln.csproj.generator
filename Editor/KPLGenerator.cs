using UnityEditor;
using UnityEngine;
using System.IO;

namespace VSCodeEditor
{
    public static class KPLGenerator
    {      
        [MenuItem("Tools/kot_pod_lawkoy | Generate/Settings")]
        public static void OpenPreferences()
        {
            SettingsService.OpenUserPreferences("Preferences/kot_pod_lawkoy | Generation");
        }
    
        [MenuItem("Tools/kot_pod_lawkoy | Generate/Generate")]
        public static void GenerateWithSettings()
        {
            // Просто вызываем метод из KPLPreferences
            KPLPreferences.GenerateWithCurrentSettings();
        }

        [MenuItem("Tools/kot_pod_lawkoy | Generate/Open project folder")]
        public static void OpenProjectFolder()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            System.Diagnostics.Process.Start("explorer.exe", projectPath);
        }

        [MenuItem("Tools/kot_pod_lawkoy | Generate/Delete all .csproj and .sln")]
        public static void DeleteAllProjectFiles()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string projectName = Path.GetFileName(projectPath);
            
            // Удаляем .sln
            string slnPath = Path.Combine(projectPath, $"{projectName}.sln");
            if (File.Exists(slnPath))
            {
                File.Delete(slnPath);
                //Debug.Log($"Deleted: {slnPath}");
            }
            
            // Удаляем все .csproj
            var csprojFiles = Directory.GetFiles(projectPath, "*.csproj");
            foreach (var file in csprojFiles)
            {
                File.Delete(file);
                //Debug.Log($"Deleted: {file}");
            }
            
            Debug.Log("kot_pod_lawkoy | Deletion all .csproj and .sln files complete");
        }

        [MenuItem("Tools/kot_pod_lawkoy | Generate/Generate ALL (ignore settings)")]
        public static void GenerateAll()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            var projectGeneration = new ProjectGeneration(projectPath);
            
            // Генерируем ВСЁ игнорируя настройки
            projectGeneration.GenerateAll(true);
            projectGeneration.Sync();
            
            Debug.Log("kot_pod_lawkoy | Generation ALL .csproj files complete (ignoring settings)");
        }
    }
}