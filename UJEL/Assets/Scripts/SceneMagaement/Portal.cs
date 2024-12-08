using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destinationPortal;
    PlayerController player;
    SceneDetails sceneDetails;
    public void OnPlayerTriggered(PlayerController player){
        this.player = player;
        player.animator.SetBool("isMoving", false);
        StartCoroutine(SwitchScene());
    }

    private void Start(){
        sceneDetails = FindObjectOfType<SceneDetails>();
    }

    IEnumerator SwitchScene(){
        GameController.instance.PauseGame(true);
        yield return Fader.instance.FadeIn(0.5f);
        DontDestroyOnLoad(gameObject);

        sceneDetails.UnloadScene();

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return Fader.instance.FadeOut(0.5f);
        GameController.instance.PauseGame(false);
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier { A, B, C, D, E, F, G, H, I, J, K, L, M }
