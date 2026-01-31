#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

namespace MaskMYDrama.Editor
{
    /// <summary>
    /// Editor utility to replace TextMeshPro fonts with Noto Sans SC for Chinese character support.
    /// 
    /// 批量替换 TextMeshPro 字体为 Noto Sans SC，以支持简体中文显示。
    /// </summary>
    public class FontReplacer : EditorWindow
    {
        private TMP_FontAsset targetFont;
        private int replacedCount = 0;
        private Vector2 scrollPosition;
        
        [MenuItem("MaskMYDrama/Replace TextMeshPro Font to Noto Sans SC")]
        public static void ShowWindow()
        {
            FontReplacer window = GetWindow<FontReplacer>("Font Replacer");
            window.minSize = new Vector2(400, 300);
            
            // Auto-load Noto Sans SC font
            string fontPath = "Assets/Fonts/Noto_Sans_SC/NotoSansSC-VariableFont_wght SDF.asset";
            window.targetFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
            
            if (window.targetFont == null)
            {
                Debug.LogWarning($"Could not find Noto Sans SC font at {fontPath}. Please assign manually.");
            }
        }
        
        private void OnGUI()
        {
            GUILayout.Label("TextMeshPro 字体替换工具", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "此工具将批量替换场景和预制体中的所有 TextMeshPro 组件字体为 Noto Sans SC，以支持简体中文显示。",
                MessageType.Info);
            
            GUILayout.Space(10);
            
            // Font selection
            targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
                "目标字体 (Noto Sans SC):",
                targetFont,
                typeof(TMP_FontAsset),
                false);
            
            if (targetFont == null)
            {
                EditorGUILayout.HelpBox("请先选择或分配 Noto Sans SC 字体资源。", MessageType.Warning);
                
                if (GUILayout.Button("自动查找 Noto Sans SC"))
                {
                    string[] guids = AssetDatabase.FindAssets("NotoSansSC-VariableFont_wght SDF");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        targetFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                        if (targetFont != null)
                        {
                            Debug.Log($"找到字体: {path}");
                        }
                    }
                }
                return;
            }
            
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("替换范围:", EditorStyles.boldLabel);
            bool replaceInScene = EditorGUILayout.Toggle("场景中的对象", true);
            bool replaceInPrefabs = EditorGUILayout.Toggle("预制体", true);
            bool replaceInProject = EditorGUILayout.Toggle("项目中的所有预制体", false);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("开始替换", GUILayout.Height(30)))
            {
                replacedCount = 0;
                
                if (replaceInScene)
                {
                    ReplaceFontsInScene();
                }
                
                if (replaceInPrefabs)
                {
                    ReplaceFontsInSelectedPrefabs();
                }
                
                if (replaceInProject)
                {
                    ReplaceFontsInAllPrefabs();
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                EditorUtility.DisplayDialog(
                    "替换完成",
                    $"已替换 {replacedCount} 个 TextMeshPro 组件的字体为 Noto Sans SC。",
                    "确定");
            }
            
            GUILayout.Space(10);
            
            if (replacedCount > 0)
            {
                EditorGUILayout.HelpBox($"已替换 {replacedCount} 个组件", MessageType.Info);
            }
        }
        
        /// <summary>
        /// 替换场景中所有 TextMeshPro 组件的字体
        /// </summary>
        private void ReplaceFontsInScene()
        {
            TextMeshProUGUI[] tmpComponents = FindObjectsOfType<TextMeshProUGUI>(true);
            TextMeshPro[] tmp3DComponents = FindObjectsOfType<TextMeshPro>(true);
            
            foreach (var tmp in tmpComponents)
            {
                if (tmp.font != targetFont)
                {
                    tmp.font = targetFont;
                    EditorUtility.SetDirty(tmp);
                    replacedCount++;
                }
            }
            
            foreach (var tmp in tmp3DComponents)
            {
                if (tmp.font != targetFont)
                {
                    tmp.font = targetFont;
                    EditorUtility.SetDirty(tmp);
                    replacedCount++;
                }
            }
            
            Debug.Log($"场景中替换了 {tmpComponents.Length + tmp3DComponents.Length} 个 TextMeshPro 组件");
        }
        
        /// <summary>
        /// 替换选中的预制体中的字体
        /// </summary>
        private void ReplaceFontsInSelectedPrefabs()
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            
            foreach (GameObject obj in selectedObjects)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(obj) || PrefabUtility.IsPartOfPrefabInstance(obj))
                {
                    ReplaceFontsInGameObject(obj);
                }
            }
        }
        
        /// <summary>
        /// 替换项目中所有预制体的字体
        /// </summary>
        private void ReplaceFontsInAllPrefabs()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    bool modified = false;
                    TextMeshProUGUI[] tmpComponents = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
                    TextMeshPro[] tmp3DComponents = prefab.GetComponentsInChildren<TextMeshPro>(true);
                    
                    foreach (var tmp in tmpComponents)
                    {
                        if (tmp.font != targetFont)
                        {
                            tmp.font = targetFont;
                            modified = true;
                            replacedCount++;
                        }
                    }
                    
                    foreach (var tmp in tmp3DComponents)
                    {
                        if (tmp.font != targetFont)
                        {
                            tmp.font = targetFont;
                            modified = true;
                            replacedCount++;
                        }
                    }
                    
                    if (modified)
                    {
                        EditorUtility.SetDirty(prefab);
                        Debug.Log($"已更新预制体: {path}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 替换 GameObject 及其子对象中的所有 TextMeshPro 字体
        /// </summary>
        private void ReplaceFontsInGameObject(GameObject obj)
        {
            TextMeshProUGUI[] tmpComponents = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshPro[] tmp3DComponents = obj.GetComponentsInChildren<TextMeshPro>(true);
            
            foreach (var tmp in tmpComponents)
            {
                if (tmp.font != targetFont)
                {
                    tmp.font = targetFont;
                    EditorUtility.SetDirty(tmp);
                    replacedCount++;
                }
            }
            
            foreach (var tmp in tmp3DComponents)
            {
                if (tmp.font != targetFont)
                {
                    tmp.font = targetFont;
                    EditorUtility.SetDirty(tmp);
                    replacedCount++;
                }
            }
        }
        
        /// <summary>
        /// 快速替换当前场景的字体（菜单项）
        /// </summary>
        [MenuItem("MaskMYDrama/Quick Replace Font in Scene")]
        public static void QuickReplaceFontInScene()
        {
            string fontPath = "Assets/Fonts/Noto_Sans_SC/NotoSansSC-VariableFont_wght SDF.asset";
            TMP_FontAsset targetFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
            
            if (targetFont == null)
            {
                EditorUtility.DisplayDialog(
                    "错误",
                    $"找不到 Noto Sans SC 字体资源: {fontPath}\n请使用 Font Replacer 窗口手动分配。",
                    "确定");
                ShowWindow();
                return;
            }
            
            int count = 0;
            TextMeshProUGUI[] tmpComponents = FindObjectsOfType<TextMeshProUGUI>(true);
            TextMeshPro[] tmp3DComponents = FindObjectsOfType<TextMeshPro>(true);
            
            foreach (var tmp in tmpComponents)
            {
                if (tmp.font != targetFont)
                {
                    tmp.font = targetFont;
                    EditorUtility.SetDirty(tmp);
                    count++;
                }
            }
            
            foreach (var tmp in tmp3DComponents)
            {
                if (tmp.font != targetFont)
                {
                    tmp.font = targetFont;
                    EditorUtility.SetDirty(tmp);
                    count++;
                }
            }
            
            AssetDatabase.SaveAssets();
            
            EditorUtility.DisplayDialog(
                "完成",
                $"已替换 {count} 个 TextMeshPro 组件的字体为 Noto Sans SC。",
                "确定");
        }
    }
}
#endif

