using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneWarp : MonoBehaviour
{
    public static SceneWarp instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public IEnumerator Warp(int sceneIndex, Vector2 targetPos)
    {
        var player = PlayerController.Instance;
        var sceneDetails = FindObjectOfType<SceneDetails>();

        GameController.instance.PauseGame(true);
        yield return Fader.instance.FadeIn(0.5f);
        DontDestroyOnLoad(player.gameObject);

        if (sceneDetails != null)
        {
            sceneDetails.UnloadScene();
        }
        else Debug.LogError("No SceneDetails to unload!");
        
        yield return SceneManager.LoadSceneAsync(sceneIndex);

        player.SetPositionAndSnapToTile(targetPos);

        yield return Fader.instance.FadeOut(0.5f);

        GameController.instance.PauseGame(false);

        GameController.instance.StartFreeRoamState();
    }
}
