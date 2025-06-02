using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HowlEngine.Helpers;

// =====================================================================================================================
// NOTES: 
//  An assembly is a copmpiled .dll or .exe file that contains compiled code (classes, interfaces, structs, etc.).
//  In C#, compiled code goes into an assembly, including any .csproj that is included in the main applications .csproj.
//  Example: MyGame.exe is an assembly containing Player, Enemy, GameManager, etc.
// =====================================================================================================================

public class TypeFactory{

    /// <summary>
    /// Cache of all types within the assembly (an optimisation as code is already compiled, not dynamically made.)
    /// </summary>
    private Dictionary<string, Type> _typeCache;

    /// <summary>
    /// The current assembly where this TypeFactory instance is defined.
    /// </summary>
    private Assembly _assembly;


    /// <summary>
    /// Creates a new instance of TypeFactory.
    /// </summary>

    public TypeFactory(){
        new TypeFactory(typeof(HowlApp).Assembly);
    }

    /// <summary>
    /// Creates a new instance of TypeFactory.
    /// </summary>
    /// <param name="assembly">The specified assembly where the Types are loacted.</param>

    public TypeFactory(Assembly assembly){

        // Get the current assembly where this TypeFactory instance is defined.

        _assembly = assembly;

        // Load all types (classes, structs, interfaces, etc.), specifically the metadata.
        // Add all types to a dictionary where:
        // Key:     "HowlEngine.ECS.Entity".
        // Value:   Type object that holds info of that class.

        _typeCache = _assembly.GetTypes().ToDictionary(t=>t.FullName, t=>t);
        foreach(var t in _assembly.GetTypes()){
            Console.WriteLine(t.ToString());
        }
    }
    

    /// <summary>
    /// Creates a new instance of a Type within the loaded assembly stored by this TypeFactory.
    /// </summary>
    /// <param name="typeName">The specified name of the Type.</param>
    /// <returns></returns>

    public object CreateInstance(string typeName){    
        
        // Get the type from the loaded type cache.
        
        var type = FindType(typeName);

        // Find the contructor of the given Type and call the contructor dynamically.
        
        return Activator.CreateInstance(type);
    }


    /// <summary>
    /// Creates a new instance of a Type within the loaded assembly stored by this TypeFactory.
    /// </summary>
    /// <param name="typeName">The specified name of the Type.</param>
    /// <param name="args">The arguments used to pass through into the Types constructor.</param>
    /// <returns></returns>

    public object CreateInstance(string typeName, object[] args){
        // Get the type from the loaded type cache.
        
        var type = FindType(typeName);

        // Find the contructor of the given Type and call the contructor dynamically.
        
        return Activator.CreateInstance(type, args);
    }


    /// <summary>
    /// Returns a cached Type.
    /// </summary>
    /// <param name="typeName">The name of the type stored within the chached types dictionary.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    private Type FindType(string typeName){
        return _typeCache.TryGetValue(typeName, out var type)
        ? type
        : throw new Exception($"Type '{typeName}' not found.");
    }
}