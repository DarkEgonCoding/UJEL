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
    private Selector currentSelector;
    [SerializeField] private ScreenFader screenFader;

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
        SettingsMenu.SetActive(true);
        MenuObject.SetActive(false);
        UpdateSelector();
    }

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
}
