using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    protected WindowsManager windowsManager;

    public virtual void Show(WindowsManager windowsManager, object data) 
    {
        this.windowsManager = windowsManager;
    }

    public virtual void Close() 
    {
        windowsManager.openedWindows.Remove(this);
        Destroy(gameObject);
    }
}
