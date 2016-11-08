﻿using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// This Class is use for creating the horse layer for the horses and set ignore collision with the water
/// </summary>
[InitializeOnLoad]
public class HorseLayers : Editor
{
    static HorseLayers()
    {
        CreateLayer();
    }

    static void CreateLayer()
    {


        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");
        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
            Debug.LogWarning("Layers is null: " + (layers == null));
            return;
        }

        SerializedProperty layerSP = layers.GetArrayElementAtIndex(20);
        if (layerSP.stringValue != "Horse")
        {
            Debug.Log("Setting up layers.  Layer " + "[20]" + " is now called " + "[Horse]");
            layerSP.stringValue = "Horse";
        }

        Physics.IgnoreLayerCollision(4, 20);

        tagManager.ApplyModifiedProperties();
    }
}

