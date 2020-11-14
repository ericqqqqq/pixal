using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Actor))]
public class Bullet : MonoBehaviour {
    [HideInInspector] public Actor myActor;
    public Vector3 damageSize;
    public LayerMask solidLayer;
    public LayerMask damageLayer;
    public int hitNum;
    public float speed;
    // negative left
    [HideInInspector] public float direction;
    public float lifeTime;
    private float lifeCountdown;

    private void Reset () {
        hitNum = 1;
        speed = 100;
        lifeTime = 1f;
    }
    private void Awake () {
        myActor = GetComponent<Actor> ();
        myActor.solidLayer = solidLayer;
    }

    private void Start () {
        myActor.velocity.x = direction * speed;
        lifeCountdown = lifeTime;
    }

    private void Update () {
        if (IfOverlapDamageBox () && hitNum > 0) {
            Collider2D[] cols = OverlapDamageBox ();
            foreach (Collider2D col in cols) {
                hitNum--;
                if (hitNum > 0) {
                    Debug.Log (col.name);
                    Debug.Log ("hit");
                } else {
                    Destroy (gameObject);
                }
            }
        }
        if (lifeCountdown > 0) lifeCountdown -= Time.deltaTime;

        // if already hit the damagable object or lifetime reached, destroy.
        if (hitNum <= 0 || lifeCountdown < 0) Destroy (gameObject);

        // TODO: if hit the solid, destroy.
    }

    public bool IfOverlapDamageBox () {
        return Physics2D.OverlapBox (transform.position, damageSize, 0, damageLayer);
    }

    public Collider2D[] OverlapDamageBox () {
        return Physics2D.OverlapBoxAll (transform.position, damageSize, 0, damageLayer);
    }
}