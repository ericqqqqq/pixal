using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
    public Vector3 barrel; // make barrel visibal
    public Bullet bullet;
    private void Reset () {
        transform.localPosition = Vector3.zero;
    }

    public void Fire (Vector3 direction) {
        Bullet b = Instantiate (bullet, transform.TransformPoint (barrel), Quaternion.identity);
        b.direction = Mathf.Sign (direction.x);
    }

}