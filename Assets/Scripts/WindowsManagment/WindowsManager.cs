using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    public List<Window> openedWindows { get => _openedWindows; }
    
    private Canvas canvas { get => FindObjectOfType<Canvas>(); }

    private List<Window> _openedWindows = new List<Window>();

    private void Start() => DontDestroyOnLoad(canvas);

    public Window Open(WindowType windowType, object data = null)
    {
        GameObject windowObject = Instantiate(Resources.Load("Windows/" + windowType.ToString()), canvas.transform) as GameObject;
        Window window = windowObject.GetComponent<Window>();
        window.Show(this, data);
        _openedWindows.Add(window);
        return window;
    }

    public void CloseAll()
    {
        while(_openedWindows.Count != 0)
            _openedWindows[0].Close();
    }

    public void CloseAllExcept(Window window)
    {
        _openedWindows.Remove(window);
        CloseAll();
        _openedWindows.Add(window);
    }

    public void Close(Window window)
    {
        Window target = _openedWindows.Find(w => window);
        if (target != null)
            target.Close();
    }
}
