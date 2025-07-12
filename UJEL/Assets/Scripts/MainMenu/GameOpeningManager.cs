using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOpeningManager : MonoBehaviour
{
    [SerializeField] public Dialog dialogOne;

    // Start is called before the first frame update
    void Start()
    {
        DialogManager.Instance.ShowDialog(dialogOne);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
