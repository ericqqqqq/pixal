using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (Shooter))]
public class ShooterEditor : Editor {
    void OnSceneGUI () {
        Shooter shooter = target as Shooter;
        Transform transform = shooter.transform;

        shooter.barrel = transform.InverseTransformPoint (
            Handles.PositionHandle (
                transform.TransformPoint (
                    shooter.barrel
                ),
                transform.rotation
            )
        );
    }
}