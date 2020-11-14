///<summary>A Pixel Perfect RigidBody Component</summary>
using System.Collections;
using System.Collections.Generic;
using PixalHelper;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Actor : MonoBehaviour {
    [HideInInspector] public LayerMask solidLayer;
    [HideInInspector] public Vector2 movementCounter = Vector2.zero;
    [HideInInspector] public BoxCollider2D actorBoxCollider;
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public float maxFall;
    [HideInInspector] public float gravity;

    private float actorBoxColliderSizeShrinkFactor = 0.9f;

    private void Reset () {
        actorBoxCollider = GetComponent<BoxCollider2D> ();
    }

    private void Update () {
        if (MoveX (velocity.x * Time.deltaTime)) velocity.x = 0;

        if (MoveY (velocity.y * Time.deltaTime)) velocity.y = 0;

        if (!IsGrounded () && gravity != 0) {
            velocity.y = Math.Approach (velocity.y, -maxFall, gravity * Time.deltaTime);
        }
    }

    ///<summary>
    /// Take distance in float, process/round the distance in int.
    /// Try its best to appoarch the input.
    ///</summary>
    ///<params>negative: left, positive: right</params>
    ///<return>Returns true when the path is blocked</return>
    public bool MoveX (float distance) {
        movementCounter.x += distance;
        int moveX = (int) Mathf.Round (movementCounter.x);

        if (moveX != 0) {
            movementCounter.x = movementCounter.x - (float) moveX;
            return PerformXMovement (moveX);
        }
        return false;
    }

    ///<summary>
    /// Take distance in float, process/round the distance in int.
    /// Try its best to appoarch the input.
    ///</summary>
    ///<params>negative: down, positive: up</params>
    ///<return>Returns true when the path is blocked</return>
    public bool MoveY (float distance) {
        movementCounter.y += distance;
        int moveY = (int) Mathf.Round (movementCounter.y);

        if (moveY != 0) {
            movementCounter.y = movementCounter.y - (float) moveY;
            return PerformYMovement (moveY);
        }
        return false;
    }

    ///<summary>Perform movement Horizontally, move whole pixel a time</summary>
    ///<params>negative: left, positive: right</params>
    ///<return>Returns true when the path is blocked</return>
    public bool PerformXMovement (float move) {
        int step = (int) Mathf.Sign (move);
        while (move != 0) {
            if (IsSolid (Vector2.right * step)) {
                movementCounter.x = 0f;
                return true;
            }

            transform.position += new Vector3 (step, 0f, 0f);
            move -= step;
            Physics2D.SyncTransforms ();
        }
        return false;
    }

    ///<summary>Perform movement Vertically, move whole pixel a time</summary>
    ///<params>negative: down, positive: up</params>
    ///<return>Returns true when the path is blocked</return>
    public bool PerformYMovement (float move) {
        int step = (int) Mathf.Sign (move);
        while (move != 0) {
            if (IsSolid (Vector2.up * step)) {
                movementCounter.y = 0f;
                return true;
            }

            transform.position += new Vector3 (0f, step, 0f);
            move -= step;
            Physics2D.SyncTransforms ();
        }
        return false;
    }

    ///<summary>Check if the given direction has a solid layer</summary>
    ///<params>Unit Vector indicates the direction</params>
    ///<return>Returns true is a solid layer exists</return>
    public bool IsSolid (Vector2 direction) {
        Bounds myBoxColliderShape = GetMyBoxColliderShape ();

        // have to shrink the size for a little bit.
        return IfOverlapSolidColliderByBox (
            (Vector2) myBoxColliderShape.center + direction,
            (Vector2) myBoxColliderShape.size * actorBoxColliderSizeShrinkFactor
        );
    }

    ///<summary>Check if step on a solid layer</summary>
    ///<return>Returns true is a solid layer exists</return>
    public bool IsGrounded () {
        return IsSolid (Vector2.down);
    }

    public Bounds GetBoxColliderShape (BoxCollider2D col) {
        return col.bounds;
    }

    public Bounds GetMyBoxColliderShape () {
        return GetBoxColliderShape (actorBoxCollider);
    }

    public bool IfOverlapSolidColliderByBox (Vector2 center, Vector2 size) {
        return Physics2D.OverlapBox (center, size, 0, solidLayer);
    }
}