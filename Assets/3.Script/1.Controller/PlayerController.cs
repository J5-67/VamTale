using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


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
        TryGetComponent<Scanner>(out sc);
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

        if (IsMoving)
        {
            Vector2 anInput = input;

            if (anInput.y < -0.01f)
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        GameManager.instance.health -= Time.deltaTime * 10;

        if (GameManager.instance.health < 0)
        {
            for (int i = 2; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            SceneManager.LoadScene("GameOver");
        }
    }
}
