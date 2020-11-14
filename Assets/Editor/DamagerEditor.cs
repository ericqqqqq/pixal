using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor (typeof (Damager))]
public class DamagerEditor : Editor {

    public BoxBoundsHandle boxBoundHandle = new BoxBoundsHandle ();

    private void OnSceneGUI () {
        Damager damager = (Damager) target;

        boxBoundHandle.size = damager.size;
        boxBoundHandle.center = damager.transform.position;
        boxBoundHandle.SetColor (Color.red);

        EditorGUI.BeginChangeCheck ();
        boxBoundHandle.DrawHandle ();
        if (EditorGUI.EndChangeCheck ()) {
            Undo.RecordObject (damager, "Modify Damager Area");

            damager.size = boxBoundHandle.size;
        }

    }
}