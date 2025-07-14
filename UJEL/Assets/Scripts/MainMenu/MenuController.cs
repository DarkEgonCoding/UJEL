using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
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
    [SerializeField] public GameObject NameSelection;
    [SerializeField] public GameObject GenderSelection;
    [SerializeField] public GameObject BackgroundImage;
    [SerializeField] private TextMeshProUGUI Ztxt;
    [SerializeField] private TextMeshProUGUI Xtxt;
    [SerializeField] private float highlightTime = 0.3f;
    private TextMeshProUGUI[] PresentTexts;
    public PlayerControls menuControls;
    public static MenuController instance;
    private Selector currentSelector;
    [SerializeField] private ScreenFader screenFader;
    private string PlayerNameInput;


    private Coroutine highlightRoutine;

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
        IntroCutscene();

        // Turn these on to add the highlighting to the text, only half working right now.
        //menuControls.Main.Interact.performed += ctx => HighlightZ();
        //menuControls.Main.Run.performed += ctx => HighlightX();
    }

    private void HighlightZ()
    {
        highlightRoutine = StartCoroutine(HighlightObject(highlightTime, Ztxt));
    }

    private void HighlightX()
    {
        highlightRoutine = StartCoroutine(HighlightObject(highlightTime, Xtxt));
    }

    private IEnumerator HighlightObject(float seconds, TextMeshProUGUI text)
    {
        if (highlightRoutine != null) yield break;

        Color originalColor = text.color; // Save original color

        // Change color for time
        text.color = Color.blue;
        yield return new WaitForSeconds(seconds);

        // Return to original color
        text.color = originalColor;
    }

    public void UpdateSelector()
    {
        ClearCurrentSelector();
        SetCurrentSelector(FindObjectOfType<Selector>());
    }

    public void SetCurrentSelector(Selector selector)
    {
        currentSelector = selector;
        currentSelector.UpdateMenuVisual();

        menuControls.Main.Up.performed += ctx => currentSelector.MenuUp();
        menuControls.Main.Down.performed += ctx => currentSelector.MenuDown();
        menuControls.Main.Interact.performed += ctx => currentSelector.SelectItem();
        menuControls.Main.Run.performed += ctx => currentSelector.Return();
    }

    public void ClearCurrentSelector()
    {
        if (currentSelector != null)
        {
            menuControls.Main.Up.performed -= ctx => currentSelector.MenuUp();
            menuControls.Main.Down.performed -= ctx => currentSelector.MenuDown();
            menuControls.Main.Interact.performed -= ctx => currentSelector.SelectItem();
            menuControls.Main.Run.performed -= ctx => currentSelector.Return();
        }

        currentSelector = null;
    }

    public void OpenSettingsMenu()
    {
        BackgroundImage.SetActive(true);
        SettingsMenu.SetActive(true);
        MenuObject.SetActive(false);
        UpdateSelector();
    }

    private void IntroCutscene()
    {
        NameSelection.SetActive(false);
        GenderSelection.SetActive(false);
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

        // Display all starting text
        foreach (var text in PresentTexts)
        {
            yield return StartCoroutine(FadeTextToFullAlpha(3f, text));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(FadeTextToZeroAlpha(1f, text));
            yield return new WaitForSeconds(1f);
        }

        // Fade Transition
        screenFader.FadeIn();
        yield return new WaitForSeconds(screenFader.fadeDuration);
        MainMenu.SetActive(true);
        OpenMainMenu();
        StartCoroutine(OpenMainMenuCoroutine());
        screenFader.FadeOut();
    }

    public void OpenMainMenu()
    {
        MenuObject.SetActive(true);
        BackgroundImage.SetActive(true);
        SettingsMenu.SetActive(false);
        Screen.SetActive(true);
        Video.SetActive(true);
        UpdateSelector();
    }

    public IEnumerator OpenMainMenuCoroutine()
    {
        title.alpha = 0;
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

    public void NewGame()
    {
        MenuObject.SetActive(false);
        NameSelection.SetActive(true);
        BackgroundImage.SetActive(false);


        //SceneManager.LoadScene(1);
    }

    public void ReadStringInput(string s)
    {
        PlayerNameInput = s;
    }

    public void SelectedPlayerName()
    {
        if (RuleChecker(PlayerNameInput) == false)
        {
            return;
        }

        NameSelection.SetActive(false);
        BackgroundImage.SetActive(false);
        GenderSelection.SetActive(true);
    }

    private bool RuleChecker(string input)
    {
        if (input.Length > 15 || input.Length < 1) return false; // Must be between 1 and 15 characters
        return true;
    }
}
