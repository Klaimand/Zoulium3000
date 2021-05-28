using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDialog", menuName = "KLD/KLD_Dialog", order = 0)]
public class KLD_Dialog : ScriptableObject
{
    public string characterName = "character";
    public Sprite characterSprite = null;

    public List<TextPanel> panels = new List<TextPanel>();
}

[System.Serializable]
public class TextPanel
{
    [TextArea(3, 10)] string text;
}
