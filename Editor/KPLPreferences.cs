using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace VSCodeEditor
{
    public static class KPLPreferences
    {
        // Ключи для сохранения настроек
        private const string PREFS_EMBEDDED = "kotpodlawkoy.Generate.Embedded";
        private const string PREFS_LOCAL = "kotpodlawkoy.Generate.Local";
        private const string PREFS_REGISTRY = "kotpodlawkoy.Generate.Registry";
        private const string PREFS_GIT = "kotpodlawkoy.Generate.Git";
        private const string PREFS_BUILTIN = "kotpodlawkoy.Generate.BuiltIn";
        private const string PREFS_TARBALL = "kotpodlawkoy.Generate.Tarball";
        private const string PREFS_UNKNOWN = "kotpodlawkoy.Generate.Unknown";
        private const string PREFS_PLAYER = "kotpodlawkoy.Generate.Player";
        
        [SettingsProvider]
        public static SettingsProvider CreateKPLPreferences()
        {
            return new SettingsProvider("Preferences/kot_pod_lawkoy | Generation", SettingsScope.User)
            {
                label = "kot_pod_lawkoy | .sln Generation",
                guiHandler = DrawPreferencesGUI,
                keywords = new HashSet<string>(new[] { 
                    "KPL", "Generation", "csproj", "project", "sln", "solution", "kot_pod_lawkoy", "kotpodlawkoy", 
                    "embedded", "local", "registry", "git", "builtin", "tarball", "player"
                })
            };
        }
        
        static void DrawPreferencesGUI(string searchContext)
        {
            // Загружаем текущие значения
            bool embedded = EditorPrefs.GetBool(PREFS_EMBEDDED, true);
            bool local = EditorPrefs.GetBool(PREFS_LOCAL, true);
            bool registry = EditorPrefs.GetBool(PREFS_REGISTRY, false);
            bool git = EditorPrefs.GetBool(PREFS_GIT, false);
            bool builtin = EditorPrefs.GetBool(PREFS_BUILTIN, false);
            bool tarball = EditorPrefs.GetBool(PREFS_TARBALL, false);
            bool unknown = EditorPrefs.GetBool(PREFS_UNKNOWN, false);
            bool player = EditorPrefs.GetBool(PREFS_PLAYER, false);
            
            EditorGUILayout.LabelField("Generate .csproj files for:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            embedded = EditorGUILayout.Toggle("Embedded packages", embedded);
            local = EditorGUILayout.Toggle("Local packages", local);
            registry = EditorGUILayout.Toggle("Registry packages", registry);
            git = EditorGUILayout.Toggle("Git packages", git);
            builtin = EditorGUILayout.Toggle("Built-in packages", builtin);
            tarball = EditorGUILayout.Toggle("Local tarball", tarball);
            unknown = EditorGUILayout.Toggle("Packages from unknown sources", unknown);
            player = EditorGUILayout.Toggle("Player projects", player);
            
            EditorGUI.indentLevel--;
            
            // Сохраняем если были изменения
            if (GUI.changed)
            {
                EditorPrefs.SetBool(PREFS_EMBEDDED, embedded);
                EditorPrefs.SetBool(PREFS_LOCAL, local);
                EditorPrefs.SetBool(PREFS_REGISTRY, registry);
                EditorPrefs.SetBool(PREFS_GIT, git);
                EditorPrefs.SetBool(PREFS_BUILTIN, builtin);
                EditorPrefs.SetBool(PREFS_TARBALL, tarball);
                EditorPrefs.SetBool(PREFS_UNKNOWN, unknown);
                EditorPrefs.SetBool(PREFS_PLAYER, player);
                
                Debug.Log("kot_pod_lawkoy | Settings saved");
            }
            
            EditorGUILayout.Space(20);
            
            // КНОПКА ДЛЯ ГЕНЕРАЦИИ
            if (GUILayout.Button("Generate .csproj", GUILayout.Height(30)))
            {
                GenerateWithCurrentSettings();
            }
        }
        
       // static void GenerateWithCurrentSettings()
       // {
       //     string projectPath = Directory.GetParent(Application.dataPath).FullName;
       //     var projectGeneration = new ProjectGeneration(projectPath);
       //     
       //     // ПРАВИЛЬНЫЙ ДОСТУП через интерфейс
       //     var assemblyNameProvider = ((IGenerator)projectGeneration).AssemblyNameProvider;
       //     var flags = GetCurrentFlags();
       //     assemblyNameProvider.ToggleProjectGeneration(flags);
       //     
       //     projectGeneration.Sync();
       //     
       //     Debug.Log("kot_pod_lawkoy | Generation with recent setting complete");
       // }
       //static void GenerateWithCurrentSettings()
       // {
       //     string projectPath = Directory.GetParent(Application.dataPath).FullName;
       //     
       //     // СОЗДАЁМ НОВЫЙ AssemblyNameProvider с нуля
       //     var customProvider = new AssemblyNameProvider();
       //     
       //     // ВЫКЛЮЧАЕМ ВСЁ сначала
       //     var allFlags = 
       //         ProjectGenerationFlag.Embedded |
       //         ProjectGenerationFlag.Local |
       //         ProjectGenerationFlag.Registry |
       //         ProjectGenerationFlag.Git |
       //         ProjectGenerationFlag.BuiltIn |
       //         ProjectGenerationFlag.LocalTarBall |
       //         ProjectGenerationFlag.Unknown |
       //         ProjectGenerationFlag.PlayerAssemblies;
       //     
       //     customProvider.ToggleProjectGeneration(allFlags); // Это выключит ВСЁ
       //     
       //     // ВКЛЮЧАЕМ ТОЛЬКО то что выбрано
       //     var selectedFlags = GetCurrentFlags();
       //     customProvider.ToggleProjectGeneration(selectedFlags); // Это включит выбранное
       //     
       //     // Создаём новый ProjectGeneration с нашим провайдером
       //     var customProjectGeneration = new ProjectGeneration(
       //         projectPath, 
       //         customProvider, 
       //         new FileIOProvider(), 
       //         new GUIDProvider()
       //     );
       //     
       //     // Показываем что будем генерировать
       //     Debug.Log("kot_pod_lawkoy | Generating with flags: " + selectedFlags);
       //     PrintSettingsToConsole();
       //     
       //     customProjectGeneration.Sync();
       //     
       //     Debug.Log("kot_pod_lawkoy | Generation complete");
       // }

        public static void GenerateWithCurrentSettings()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            
            // СОЗДАЁМ КАСТОМНЫЙ AssemblyNameProvider
            var customProvider = new KPLAssemblyNameProvider();
            
            // УСТАНАВЛИВАЕМ НАШИ ФЛАГИ ПРЯМО В СВОЙСТВО
            customProvider.SetProjectGenerationFlagDirectly(GetCurrentFlags());
            
            // Создаём новый ProjectGeneration с нашим провайдером
            var customProjectGeneration = new ProjectGeneration(
                projectPath, 
                customProvider, 
                new FileIOProvider(), 
                new GUIDProvider()
            );
            
            // Показываем что будем генерировать
            //PrintSettingsToConsole();
            
            // ВАЖНО: НЕ вызываем GenerateAll() - он всегда включает все флаги!
            // Вызываем только Sync()
            customProjectGeneration.Sync();
            
            Debug.Log("kot_pod_lawkoy | Generation complete");
        }

        static void PrintSettingsToConsole()
        {
            var sb = new StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════╗");
            sb.AppendLine("║    kot_pod_lawkoy | GENERATION SETTINGS  ║");
            sb.AppendLine("╠══════════════════════════════════════════╣");
            
            sb.AppendLine(EditorPrefs.GetBool(PREFS_EMBEDDED, true) ? "║ ✅ Embedded packages" : "║ ❌ Embedded packages");
            sb.AppendLine(EditorPrefs.GetBool(PREFS_LOCAL, true) ? "║ ✅ Local packages" : "║ ❌ Local packages");
            sb.AppendLine(EditorPrefs.GetBool(PREFS_REGISTRY, false) ? "║ ✅ Registry packages" : "║ ❌ Registry packages");
            sb.AppendLine(EditorPrefs.GetBool(PREFS_GIT, false) ? "║ ✅ Git packages" : "║ ❌ Git packages");
            sb.AppendLine(EditorPrefs.GetBool(PREFS_BUILTIN, false) ? "║ ✅ Built-in packages" : "║ ❌ Built-in packages");
            sb.AppendLine(EditorPrefs.GetBool(PREFS_TARBALL, false) ? "║ ✅ Local tarball" : "║ ❌ Local tarball");
            sb.AppendLine(EditorPrefs.GetBool(PREFS_UNKNOWN, false) ? "║ ✅ Packages from unknown sources" : "║ ❌ Packages from unknown sources");
            sb.AppendLine(EditorPrefs.GetBool(PREFS_PLAYER, false) ? "║ ✅ Player projects" : "║ ❌ Player projects");
            
            sb.AppendLine("╚══════════════════════════════════════════╝");
            
            Debug.Log(sb.ToString());
        }
        
        private class KPLAssemblyNameProvider : AssemblyNameProvider
        {
            // Публичный метод для прямой установки флагов
            public void SetProjectGenerationFlagDirectly(ProjectGenerationFlag flags)
            {
                // Используем рефлексию чтобы установить приватное поле
                var field = typeof(AssemblyNameProvider).GetField("m_ProjectGenerationFlag", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (field != null)
                {
                    field.SetValue(this, flags);
                }
                else
                {
                    // Если рефлексия не работает, используем ToggleProjectGeneration
                    // Сначала выключаем всё
                    var allFlags = ProjectGenerationFlag.Embedded | ProjectGenerationFlag.Local | 
                                  ProjectGenerationFlag.Registry | ProjectGenerationFlag.Git | 
                                  ProjectGenerationFlag.BuiltIn | ProjectGenerationFlag.LocalTarBall | 
                                  ProjectGenerationFlag.Unknown | ProjectGenerationFlag.PlayerAssemblies;
                    
                    // Включаем только нужные флаги
                    base.ToggleProjectGeneration(allFlags); // выключит всё
                    base.ToggleProjectGeneration(flags); // включит нужные
                }
            }
            
            // Переопределяем IsInternalizedPackagePath чтобы использовал текущие флаги
            public new bool IsInternalizedPackagePath(string path)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return false;
                }
                
                var packageInfo = FindForAssetPath(path);
                if (packageInfo == null)
                {
                    return false;
                }
                
                var packageSource = packageInfo.source;
                var currentFlags = this.ProjectGenerationFlag;
                
                switch (packageSource)
                {
                    case UnityEditor.PackageManager.PackageSource.Embedded:
                        return !currentFlags.HasFlag(ProjectGenerationFlag.Embedded);
                    case UnityEditor.PackageManager.PackageSource.Registry:
                        return !currentFlags.HasFlag(ProjectGenerationFlag.Registry);
                    case UnityEditor.PackageManager.PackageSource.BuiltIn:
                        return !currentFlags.HasFlag(ProjectGenerationFlag.BuiltIn);
                    case UnityEditor.PackageManager.PackageSource.Unknown:
                        return !currentFlags.HasFlag(ProjectGenerationFlag.Unknown);
                    case UnityEditor.PackageManager.PackageSource.Local:
                        return !currentFlags.HasFlag(ProjectGenerationFlag.Local);
                    case UnityEditor.PackageManager.PackageSource.Git:
                        return !currentFlags.HasFlag(ProjectGenerationFlag.Git);
                    #if UNITY_2019_3_OR_NEWER
                    case UnityEditor.PackageManager.PackageSource.LocalTarball:
                        return !currentFlags.HasFlag(ProjectGenerationFlag.LocalTarBall);
                    #endif
                }
                
                return false;
            }
        }

        public static ProjectGenerationFlag GetCurrentFlags()
        {
            ProjectGenerationFlag flags = 0;
            
            if (EditorPrefs.GetBool(PREFS_EMBEDDED, true)) flags |= ProjectGenerationFlag.Embedded;
            if (EditorPrefs.GetBool(PREFS_LOCAL, true)) flags |= ProjectGenerationFlag.Local;
            if (EditorPrefs.GetBool(PREFS_REGISTRY, true)) flags |= ProjectGenerationFlag.Registry;
            if (EditorPrefs.GetBool(PREFS_GIT, true)) flags |= ProjectGenerationFlag.Git;
            if (EditorPrefs.GetBool(PREFS_BUILTIN, true)) flags |= ProjectGenerationFlag.BuiltIn;
            if (EditorPrefs.GetBool(PREFS_TARBALL, true)) flags |= ProjectGenerationFlag.LocalTarBall;
            if (EditorPrefs.GetBool(PREFS_UNKNOWN, true)) flags |= ProjectGenerationFlag.Unknown;
            if (EditorPrefs.GetBool(PREFS_PLAYER, true)) flags |= ProjectGenerationFlag.PlayerAssemblies;
            
            return flags;
        }
    }
}