using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Android.LowLevel;
using UnityEngine.TextCore.Text;

public class Ledge : MonoBehaviour
{
    [SerializeField] public int xDir;
    [SerializeField] public int yDir;

    private void Awake(){
        GetComponent<SpriteRenderer>().enabled = false;
    }

}
