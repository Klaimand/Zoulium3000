using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class PolyplaneEditorWindow : EditorWindow
{
    [MenuItem("Tools/Polyplane Creator")]
    public static void ShowWindow()
    {
        var window = GetWindow<PolyplaneEditorWindow>();
        window.titleContent = new GUIContent("Plane Creator");
        window.minSize = new Vector2(800, 600);
    }

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/KLD/KLD_Editor/PolyplaneEditorWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);
    }
}
