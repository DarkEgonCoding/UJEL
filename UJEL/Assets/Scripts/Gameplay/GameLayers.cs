using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] public LayerMask solidObjectsLayer;
    [SerializeField] public LayerMask grassLayer;
    [SerializeField] public LayerMask interactableLayer;
    [SerializeField] public LayerMask ledgeLayer;
    [SerializeField] public LayerMask portalLayer;
    [SerializeField] public LayerMask waterLayer;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask fovLayer;

    public static GameLayers i { get; set; }

    private void Awake(){
        i = this;
    }

    public LayerMask SolidLayer{
        get => solidObjectsLayer;
    }
    public LayerMask GrassLayer{
        get => grassLayer;
    }
    public LayerMask InteractableLayer{
        get => interactableLayer;
    }
    public LayerMask LedgeLayer{
        get => ledgeLayer;
    }
    public LayerMask PortalLayer{
        get => portalLayer;
    }
    public LayerMask WaterLayer{
        get => waterLayer;
    }
    public LayerMask TriggerableLayers {
        get => grassLayer | portalLayer;
    }
    public LayerMask PlayerLayer {
        get => playerLayer;
    }
}
