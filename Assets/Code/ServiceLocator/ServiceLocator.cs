using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;

    private readonly Dictionary<Type, IGameService> _serviceDictionary = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddService<T>(T service, bool isAbstract = true) where T : IGameService
    {
        Type keyType = isAbstract ? typeof(T).BaseType : typeof(T);

        if (keyType == null)
        {
            return;
        }

        if (_serviceDictionary.ContainsKey(keyType))
        {
           
            return;
        }

        _serviceDictionary.Add(keyType, service);
    }

    public void RemoveService<T>(bool isAbstract = true) where T : IGameService
    {
        if (isAbstract)
        {
            Type baseType = typeof(T).BaseType;
            if (baseType != null) _serviceDictionary.Remove(baseType);
            else Debug.Log("service type is null");
        }
        else
        {
            _serviceDictionary.Remove(typeof(T));
        }
    }

    public T GetService<T>() where T : IGameService
    {
        IGameService service = _serviceDictionary[typeof(T)];
        return (T)service;
    }

    public bool TryGetService<T>(out T service) where T : IGameService
    {
        service = default;
        if (_serviceDictionary.ContainsKey(typeof(T)))
        {
            service = (T)_serviceDictionary[typeof(T)];
            return true;
        }

        return false;
    }
}