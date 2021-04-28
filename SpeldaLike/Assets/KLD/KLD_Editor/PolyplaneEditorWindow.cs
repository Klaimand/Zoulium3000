using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class PolyplaneEditorWindow : EditorWindow
{
    //____________________________FIELDS
    Vector2Field planeSquares;
    Vector2Field squareSize;
    FloatField seaLevel;
    Toggle collider;

    Button flatPlaneButton;
    Button noisePlaneButton;

    FloatField noiseStrenght;
    FloatField noiseScale;
    ObjectField materialField;
    //__________________________________

    [MenuItem("Tools/Polyplane Creator")]
    public static void ShowWindow()
    {
        var window = GetWindow<PolyplaneEditorWindow>();
        window.titleContent = new GUIContent("Plane Creator");
        window.minSize = new Vector2(280, 225);
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
        planeSquares.value = KLD_StaticPlaneGenerator.planeSquares;
        planeSquares.RegisterValueChangedCallback<Vector2>(OnPlaneSquaresValueChanged);

        squareSize = rootVisualElement.Q<Vector2Field>("square-size");
        squareSize.value = KLD_StaticPlaneGenerator.squareSize;
        squareSize.RegisterValueChangedCallback<Vector2>(OnSquareSizeValueChanged);

        //FLOAT (SEA LEVEL)
        seaLevel = rootVisualElement.Q<FloatField>("sea-level");
        seaLevel.value = KLD_StaticPlaneGenerator.seaLevel;
        seaLevel.RegisterValueChangedCallback<float>(OnSeaLevelValueChanged);

        //TOGGLE(collider)
        collider = rootVisualElement.Q<Toggle>("collider");
        collider.value = KLD_StaticPlaneGenerator.instCollider;
        collider.RegisterValueChangedCallback<bool>(OnColliderValueChanged);

        //BUTTONS
        flatPlaneButton = rootVisualElement.Q<Button>("flat-plane-button");
        flatPlaneButton.RegisterCallback<ClickEvent>(ev => OnFlatPlaneButtonClick());

        noisePlaneButton = rootVisualElement.Q<Button>("noise-plane-button");
        noisePlaneButton.RegisterCallback<ClickEvent>(ev => OnNoisePlaneButtonClick());

        //FLOATS (NOISE PARAMETERS)
        noiseStrenght = rootVisualElement.Q<FloatField>("noise-strenght");
        noiseStrenght.value = KLD_StaticPlaneGenerator.noiseStrengh;
        noiseStrenght.RegisterValueChangedCallback<float>(OnNoiseStrenghtValueChanged);

        noiseScale = rootVisualElement.Q<FloatField>("noise-scale");
        noiseScale.value = KLD_StaticPlaneGenerator.noiseScale;
        noiseScale.RegisterValueChangedCallback<float>(OnNoiseScaleValueChanged);

        materialField = rootVisualElement.Q<ObjectField>("material");
        //materialField.allowSceneObjects = false;
        materialField.objectType = typeof(Material);
        materialField.value = KLD_StaticPlaneGenerator.material == null ?
            new Material(Shader.Find("Standard")) { name = "newMaterial" } :
            KLD_StaticPlaneGenerator.material;

        materialField.RegisterValueChangedCallback<Object>(OnMaterialValueChange);

    }

    #region Value change callbacks

    void OnPlaneSquaresValueChanged(ChangeEvent<Vector2> evt)
    {
        KLD_StaticPlaneGenerator.planeSquares = new Vector2Int(Mathf.RoundToInt(evt.newValue.x), Mathf.RoundToInt(evt.newValue.y));
    }

    void OnSquareSizeValueChanged(ChangeEvent<Vector2> evt)
    {
        KLD_StaticPlaneGenerator.squareSize = new Vector2Int(Mathf.RoundToInt(evt.newValue.x), Mathf.RoundToInt(evt.newValue.y));
    }

    void OnSeaLevelValueChanged(ChangeEvent<float> evt)
    {
        KLD_StaticPlaneGenerator.seaLevel = evt.newValue;
    }

    void OnColliderValueChanged(ChangeEvent<bool> evt)
    {
        KLD_StaticPlaneGenerator.instCollider = evt.newValue;
    }

    void OnNoiseStrenghtValueChanged(ChangeEvent<float> evt)
    {
        KLD_StaticPlaneGenerator.noiseStrengh = evt.newValue;
    }

    void OnNoiseScaleValueChanged(ChangeEvent<float> evt)
    {
        KLD_StaticPlaneGenerator.noiseScale = evt.newValue;
    }

    void OnMaterialValueChange(ChangeEvent<Object> evt)
    {
        KLD_StaticPlaneGenerator.material = (Material)evt.newValue;
    }



    #endregion


    #region Buttons callbacks

    void OnFlatPlaneButtonClick()
    {
        KLD_StaticPlaneGenerator.CreatePlane(true);
        //Debug.Log("Flat plane click");
    }

    void OnNoisePlaneButtonClick()
    {
        KLD_StaticPlaneGenerator.CreatePlane(false);
        //Debug.Log("Noise plane click");
    }

    #endregion

}
