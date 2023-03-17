using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    public List<Window> openedWindows { get => _openedWindows; }
    
    private Canvas canvas { get => FindObjectOfType<Canvas>(); }

    private List<Window> _openedWindows = new List<Window>();

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
        while(openedWindows.Count != 0)
            openedWindows[0].Close();
    }
}
