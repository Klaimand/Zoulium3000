using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class PolyplaneEditorWindow : EditorWindow
{
    //FIELDS
    Vector2Field planeSquares;
    Vector2Field squareSize;
    FloatField seaLevel;

    Button flatPlaneButton;
    Button noisePlaneButton;

    FloatField noiseStrenght;
    FloatField noiseScale;
    ObjectField materialField;

    [MenuItem("Tools/Polyplane Creator")]
    public static void ShowWindow()
    {
        var window = GetWindow<PolyplaneEditorWindow>();
        window.titleContent = new GUIContent("Plane Creator");
        window.minSize = new Vector2(280, 300);
    }

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/KLD/KLD_Editor/PolyplaneEditorWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/KLD/KLD_Editor/PolyplaneEditorStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        InitializeFields();
    }

    void InitializeFields()
    {
        //VECTOR2
        planeSquares = rootVisualElement.Q<Vector2Field>("plane-squares");
        planeSquares.RegisterValueChangedCallback<Vector2>(OnPlaneSquaresValueChanged);

        //BUTTONS
        flatPlaneButton = rootVisualElement.Q<Button>("flat-plane-button");
        noisePlaneButton = rootVisualElement.Q<Button>("noise-plane-button");

        flatPlaneButton.RegisterCallback<ClickEvent>(ev => OnFlatPlaneButtonClick());
        noisePlaneButton.RegisterCallback<ClickEvent>(ev => OnNoisePlaneButtonClick());


    }

    //void OnSearchTextChanged(ChangeEvent<string> evt)
    void OnPlaneSquaresValueChanged(ChangeEvent<Vector2> evt)
    {
        //change plane squares in base class
    }

    void OnFlatPlaneButtonClick()
    {
        Debug.Log("Flat plane click");
    }

    void OnNoisePlaneButtonClick()
    {
        Debug.Log("Noise plane click");
    }


}
