using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ServiceLocator
{
    private readonly static Dictionary<string, string> services = new Dictionary<string, string>();

    public static void Get<TInterface>()
    {
        Debug.Log("Get called");
        string key = typeof(TInterface).Name;
        if (!services.ContainsKey(key))
        {
            throw new ArgumentException($"There is no service registered for the {key} interface!");
        }

        var serviceName = services[key];
        var serviceType = GetType(serviceName);
        var constructors = serviceType.GetConstructors().First();
        var parameters = constructors.GetParameters();
        var instantiatedObjects = new List<object>();
        foreach (var param in parameters )
        {
            var paramType = param.ParameterType;
        }
    }

    public static void Register<TInterface, TService>() where TService : class
    {
        Debug.Log("Register called");
        string key = typeof(TInterface).Name;
        string value = typeof(TService).Name;

        var constructors = typeof(TService).GetConstructors();
        Debug.Log(constructors.Length);
        /*
        if (constructors.Length != 1)
        {
            Debug.Log("More than one constructor");
            throw new ArgumentException($"The service {value} has more than one constructor!");
        }
        */

        if (services.ContainsKey(key))
        {
            Debug.Log("Already registered");
            throw new ArgumentException($"There is already a service registered for ${key} interface!");
        }

        services.Add(key, value);
        Debug.Log("Registered!");
    }

    private static Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null) return type;
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = a.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }
}
