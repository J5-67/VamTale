using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] public Vector2 input;
    [SerializeField] public float speed;
    private bool IsMoving;
    public Scanner sc;
    private Rigidbody2D rg;
    private Animator an;

    private void Start()
    {
        TryGetComponent(out rg);
        TryGetComponent(out an);
        sc = GetComponent<Scanner>();
        //TryGetComponent(out sc);
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        Vector2 next = input.normalized * speed * Time.fixedDeltaTime;
        rg.MovePosition(rg.position + next);
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        if (an == null)
        {
            return;
        }

        IsMoving = input.sqrMagnitude > 0;

        an.SetBool("IsMoving", IsMoving);

        if(IsMoving)
        {
            Vector2 anInput = input;

            if(anInput.y < -0.01f)
            {
                anInput.x = 0;
                anInput.y = -1;
            }

            an.SetFloat("InputX", anInput.x);
            an.SetFloat("InputY", anInput.y);
        }
    }

    private void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }
}
