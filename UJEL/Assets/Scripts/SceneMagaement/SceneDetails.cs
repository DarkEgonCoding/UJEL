using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] AudioClip sceneMusic;
    [SerializeField] private float startSeconds;
    [SerializeField] private bool doFade;
    bool isLoaded;
    List<SavableEntity> savableEntities;
    [SerializeField] public string sceneName;

    private void Start(){
        if (sceneMusic != null){
            AudioManager.instance.PlayMusic(sceneMusic, fade: doFade, startSeconds: startSeconds);
        }
        LoadScene();
    }

    public void LoadScene(){
        if (!isLoaded){
            isLoaded = true;

            savableEntities = GetSavableEntitiesInScene();
            SavingSystem.i.RestoreEntityStates(savableEntities);
        }
    }

    public void UnloadScene(){
        if (isLoaded){
            savableEntities = GetSavableEntitiesInScene();
            SavingSystem.i.CaptureEntityStates(savableEntities);

            isLoaded = false;
        }
    }

    public List<SavableEntity> GetSavableEntitiesInScene(){
        var currScene = SceneManager.GetSceneByName(sceneName);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();
        return savableEntities;
    }
}
