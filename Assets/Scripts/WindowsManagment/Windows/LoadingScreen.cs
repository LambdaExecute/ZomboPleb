using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LoadingScreen : Window
{
    public UnityEvent onScreenShowed = new UnityEvent();

    [SerializeField] private GameObject icon;
    [SerializeField] private TMP_Text currentLevel;
    [SerializeField] private TMP_Text title;

    private CanvasGroup canvasGroup;

    public override void Show(WindowsManager windowsManager, object data)
    {
        base.Show(windowsManager, data);
        canvasGroup = GetComponent<CanvasGroup>();
        
        DontDestroyOnLoad(gameObject);
        
        StartCoroutine(IShow());
    }

    private IEnumerator IShow()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            yield return null;
            canvasGroup.alpha = i;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSecondsRealtime(0.5f);

        currentLevel.text = LevelManager.currentLevel.ToString();
        CanvasGroup levelGroup = currentLevel.gameObject.GetComponent<CanvasGroup>();
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            yield return null;
            levelGroup.alpha = i;
        }
        levelGroup.alpha = 1;

        yield return new WaitForSecondsRealtime(1f);

        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            yield return null;
            levelGroup.alpha = i;
        }
        levelGroup.alpha = 0;

        onScreenShowed.Invoke();
        onScreenShowed.RemoveAllListeners();
        
        StartCoroutine(IShowIcon());

        while (true)
        {
            yield return null;
            icon.transform.Rotate(new Vector3(0, 0, -75f * Time.deltaTime));
        }
    }

    private IEnumerator IShowIcon()
    {
        CanvasGroup iconGroup = icon.GetComponent<CanvasGroup>();
        CanvasGroup titleGroup = title.GetComponent<CanvasGroup>();

        for(float i = 0; i <= 1; i += Time.deltaTime)
        {
            yield return null;
            titleGroup.alpha = i;
            iconGroup.alpha = i;
        }
        titleGroup.alpha = 1;
        iconGroup.alpha = 1;
    }

    public override void Close()
    {
        StartCoroutine(IHide());
    }

    private IEnumerator IHide()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            yield return null;
            canvasGroup.alpha = i;
        }
        canvasGroup.alpha = 0;
    }
}
