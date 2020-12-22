﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Projectile : MonoBehaviour
{
    [SerializeField] private AnimationCurve PowerCurve;
    [SerializeField] private float Power;
    [Space]
    [SerializeField] private Rigidbody2D Rigidbody;
    [SerializeField] private Animator Animator;
    [SerializeField] private SpriteRenderer Renderer;

    [Header("Ground and ceiling detection")]
    [SerializeField] private Transform Groundcheck;
    [SerializeField] private LayerMask GroundMask;

    [Space]
    [SerializeField] private Transform Ceilingcheck;
    [SerializeField] private LayerMask CeilingMask;

    [Header("wall detection")]
    [SerializeField] private Transform LeftWallcheck;
    [SerializeField] private Transform RightWallcheck;
    [SerializeField] private LayerMask WallMask;

    private int Direction = 1;
    private bool Grounded = false;
    private bool WallCheck = true;

    private void FixedUpdate()
    {
        if (!Grounded)
        {
            //if the projectile has landed, turn it into the sword sprite
            if (Physics2D.OverlapBox(Groundcheck.position, new Vector2(Groundcheck.lossyScale.x, Groundcheck.lossyScale.y) / 10.0f, 0, GroundMask))
            {
                Grounded = true;
                Rigidbody.simulated = false;
                Animator.Play("Land");
            }

            if (WallCheck)
            {
                //if the projectile has hit a wall, make it go into the opposite direction
                if (Physics2D.OverlapBox(LeftWallcheck.position, new Vector2(LeftWallcheck.lossyScale.x, LeftWallcheck.lossyScale.y) / 10.0f, 0, WallMask)
                    ||
                    Physics2D.OverlapBox(RightWallcheck.position, new Vector2(RightWallcheck.lossyScale.x, RightWallcheck.lossyScale.y) / 10.0f, 0, WallMask))
                {
                    StartCoroutine(_DisableWallCheck());
                    FlipDirection();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it's player
        //pick up sword

        //if it's enemy
        //Damage it
    }

    /// <summary>
    /// Set the variables of the sword. Gets called when the object gets instantiated.
    /// </summary>
    /// <param name="vector2">The direction the player is looking.</param>
    public void SetVariables(int direction)
    {
        Direction = direction;
        //if the player is aiming left, make the sword move in the opposite direction
        if (direction < 0)
        {
            Renderer.flipX = true;
        }

        StartCoroutine(_Arc());
    }

    private IEnumerator _Arc()
    {
        float progress = 0;
        while (progress < 1)
        {
            Rigidbody.position += new Vector2(Direction, PowerCurve.Evaluate(progress)) * Power * Time.fixedDeltaTime;
            progress += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

            if (Physics2D.OverlapBox(Ceilingcheck.position, new Vector2(Ceilingcheck.lossyScale.x, Ceilingcheck.lossyScale.y) / 10.0f, 0, CeilingMask))
            {
                progress = PowerCurve.keys[PowerCurve.keys.Length - 2].time;
            }
        }

        //Rigidbody.AddRelativeForce(Vector3.right * 1000);

        //Rigidbody.AddForce((Vector3.right) * 125);
    }

    /// <summary>
    /// Disables the wall check for a couple of frames so it doesn't get triggered on the same wall again.
    /// </summary>
    private IEnumerator _DisableWallCheck()
    {
        WallCheck = false;
        yield return new WaitForSeconds(2);
        WallCheck = true;
    }

    private void FlipDirection()
    {
        Direction = -Direction;
        Renderer.flipX = !Renderer.flipX;
    }
}