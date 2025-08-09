using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;
    [SerializeField] Color bgHighlightedColor;

    private Dictionary<string, int> sceneNameToBuildIndex = new Dictionary<string, int>
    {
        {"MainMenu", 0 },
        {"Bedroom", 1 },
        {"Home", 2 },
        {"Starting_Town", 3 },
        {"TemplateScene", 4 },
        {"RouteTest", 5 }
    };


    public Color HighlightedColor => highlightedColor;
    public Color BgHighlightedColor => bgHighlightedColor;

    public Dictionary<string, int> SceneNameToBuildIndex => sceneNameToBuildIndex;

    public static GlobalSettings i { get; private set; }

    private void Awake(){
        if (i == null){
            i = this;
        }
    }
}
