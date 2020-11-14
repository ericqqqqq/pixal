using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor (typeof (Bullet))]
public class BulletEditor : Editor {

    public BoxBoundsHandle boxBoundHandle = new BoxBoundsHandle ();

    private void OnSceneGUI () {
        Bullet bullet = (Bullet) target;

        boxBoundHandle.size = bullet.damageSize;
        boxBoundHandle.center = bullet.transform.position;
        boxBoundHandle.SetColor (Color.red);

        EditorGUI.BeginChangeCheck ();
        boxBoundHandle.DrawHandle ();
        if (EditorGUI.EndChangeCheck ()) {
            Undo.RecordObject (bullet, "Modify bullet Area");

            bullet.damageSize = boxBoundHandle.size;
        }

    }
}