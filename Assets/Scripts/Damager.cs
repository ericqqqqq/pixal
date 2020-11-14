using System.Collections;
using System.Collections.Generic;
using PixalHelper;
using UnityEngine;

public class Damager : MonoBehaviour {
    public Vector3 size;
    public LayerMask enemyLayer;
    public bool IfOverlapDamageBox () {
        return Physics2D.OverlapBox (transform.position, size, 0, enemyLayer);
    }
}