using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] public GameObject MainMenu;
    [SerializeField] public GameObject Screen;
    [SerializeField] public GameObject Video;
    [SerializeField] public GameObject PresentsScreen;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] public GameObject MenuObject;
    [SerializeField] public GameObject SettingsMenu;
    private TextMeshProUGUI[] PresentTexts;
    public PlayerControls menuControls;
    public static MenuController instance;

    private void Awake()
    {
        PresentTexts = PresentsScreen.GetComponentsInChildren<TextMeshProUGUI>(true); // include inactive ones
        menuControls = new PlayerControls();

        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        menuControls.Enable();
    }

    private void OnDisable()
    {
        menuControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setup menus
        var selectorInstance = MenuObject.GetComponent<Selector>();
        selectorInstance.UpdateMenuVisual();
        menuControls.Main.Up.performed += ctx => selectorInstance.MenuUp();
        menuControls.Main.Down.performed += ctx => selectorInstance.MenuDown();
        
        // TODO FIX THIS
        menuControls.Main.Interact.performed += ctx => selectorInstance.SelectItem();

        IntroCutscene();
    }

    public void OpenSettingsMenu()
    {
        SettingsMenu.SetActive(true);
        MenuObject.SetActive(false);
    }

    /* OLD MENU SELECTOR SWITCH CODE
    public void menuSelection(int menu, int selection)
    {
        Debug.Log("MenuSelector item thing run");
        switch (menu)
        {
            case 0:
                switch (selection) // Main Menu
                {
                    case 0: // New Game
                        Debug.Log("New Game");
                        break;
                    case 1: // Load Game
                        Debug.Log("Item 1");
                        break;
                    case 2: // Settings
                        Debug.Log("Item 2");
                        break;
                    case 3: // Quit Game
                        Debug.Log("Item 3");
                        break;
                    default:
                        Debug.LogError("This shouldn't happen.");
                        break;
                }
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                Debug.LogError("This shouldn't happen.");
                break;
        }
    }
    */

    private void IntroCutscene()
    {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        PresentsScreen.SetActive(false);
        Screen.SetActive(false);
        Video.SetActive(false);
        StartCoroutine(StartMenuAnimation());
    }

    public IEnumerator StartMenuAnimation()
    {
        yield return new WaitForSeconds(1f);
        foreach (var text in PresentTexts)
        {
            text.alpha = 0;
        }

        PresentsScreen.SetActive(true);

        foreach (var text in PresentTexts)
        {
            yield return StartCoroutine(FadeTextToFullAlpha(3f, text));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(FadeTextToZeroAlpha(1f, text));
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(0.75f);

        title.alpha = 0;
        MainMenu.SetActive(true);
        Screen.SetActive(true);
        Video.SetActive(true);
        yield return StartCoroutine(FadeTextToFullAlpha(1f, title));
    }

    public IEnumerator FadeTextToFullAlpha(float duration, TextMeshProUGUI text)
    {
        Color color = text.color;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float easedAlpha = t * t; // Quadratic easing-in
            color.a = Mathf.Clamp01(easedAlpha);
            text.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        text.color = color;
    }

    public IEnumerator FadeTextToZeroAlpha(float duration, TextMeshProUGUI text)
    {
        Color color = text.color;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float easedAlpha = 1 - (t * t); // Quadratic easing-in (in reverse)
            color.a = Mathf.Clamp01(easedAlpha);
            text.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        text.color = color;
    }
}
