using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;

    private readonly Dictionary<Type, IGameService> _serviceDictionary = new();

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddService<T>(T service, bool isAbstract = true) where T : IGameService
    {
        Type keyType;

        if (isAbstract)
        {
            keyType = typeof(T).BaseType;
            if (keyType == null)
            {
                Debug.LogWarning($"[ServiceLocator] {typeof(T).Name} has no base type to register as abstract.");
                return;
            }
        }
        else
        {
            keyType = typeof(T);
        }

        if (_serviceDictionary.ContainsKey(keyType))
        {
            Debug.LogWarning($"[ServiceLocator] Service of type {keyType.Name} is already registered.");
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