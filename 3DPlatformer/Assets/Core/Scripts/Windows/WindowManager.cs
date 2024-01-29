using System.Collections.Generic;
using Core.Scripts.Windows;
using UnityEngine;
using System;

public class WindowManager : MonoBehaviour
{
    [SerializeField] private UIWindow[] windows;

    private readonly Dictionary<Type, UIWindow> _instanceWindows = new();

    private void Awake()
    {
        foreach (var uiWindow in windows)
        {
            var type = uiWindow.GetType();
            if (!_instanceWindows.ContainsKey(type))
            {
                var instanceWindow = uiWindow;
                instanceWindow.Initialize(this);
                instanceWindow.Hide();
                _instanceWindows.Add(type, uiWindow);
            }
        }
    }

    public T OpenWindow<T>() where T : UIWindow
    {
        var window = GetWindow<T>();
        
        window.Show();
        return window;
    }

    public T CloseWindow<T>() where T : UIWindow
    {
        var window = GetWindow<T>();
        
        window.Hide();
        return window;
    }

    public T GetWindow<T>() where T : UIWindow
    {
        if (_instanceWindows.ContainsKey(typeof(T)))
        {
            return _instanceWindows[typeof(T)] as T;
        }

        throw new Exception("Was not present in collection");
    }
}
