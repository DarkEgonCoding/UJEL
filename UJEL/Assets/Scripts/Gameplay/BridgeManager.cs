using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BridgeManager : MonoBehaviour
{
    bool inBridgeEnter;
    [SerializeField] PlayerController player;
    bool inBridgeExit;
    
    void EnterBridge(Collider2D other){
        other.transform.parent.GetComponent<TilemapRenderer>().sortingLayerID = SortingLayer.NameToID("Bridge-1");
        other.transform.parent.Find("OnBridgeCol").GetComponent<TilemapCollider2D>().enabled = true;
        other.transform.parent.Find("OffBridgeCol").GetComponent<TilemapCollider2D>().enabled = false;
        player.onBridge = true;
    }
    
    void ExitBridge(Collider2D other){
        other.transform.parent.GetComponent<TilemapRenderer>().sortingLayerID = SortingLayer.NameToID("Bridge+1");
        other.transform.parent.Find("OnBridgeCol").GetComponent<TilemapCollider2D>().enabled = false;
        other.transform.parent.Find("OffBridgeCol").GetComponent<TilemapCollider2D>().enabled = true;
        player.onBridge = false;
    }

    void OnTriggerEnter2D(Collider2D other){
        switch(other.tag){
            case "BridgeEnter":
                inBridgeEnter = true;
                if(!inBridgeExit) EnterBridge(other);
            break;
            case "BridgeExit":
                inBridgeExit = true;
                if(!inBridgeEnter) ExitBridge(other);
            break;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        switch(other.tag){
            case "BridgeEnter":
                inBridgeEnter = false;
                if(inBridgeExit)
                {
                    ExitBridge(other);
                } 
            break;
            case "BridgeExit":
                inBridgeExit = false;
                if(inBridgeEnter)
                {
                    EnterBridge(other);
                }
            break;
        }
    }
}
