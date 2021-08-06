using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public int CollectedCoins { get; private set; }
    public Action<bool> OnPlayerStop;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private CapsuleCollider playerCollider;

    private bool _isOver;
    private bool _isTouchingEnd;

    private void Awake()
    {
        CollectedCoins = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            animator.Play("Idle");
        else if (collision.collider.CompareTag("End"))
            _isTouchingEnd = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(!_isOver && playerRb.velocity.magnitude <= 0.1f)
        {
            _isOver = true;
            OnPlayerStop.Invoke(_isTouchingEnd);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("End"))
            _isTouchingEnd = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Coin"))
        {
            CollectedCoins++;
            Destroy(other.gameObject);
            return;
        }

        if(other.CompareTag("Bomb"))
        {
            var forceDir = (transform.position - other.transform.position).normalized;
            playerRb.AddForce(forceDir * 20, ForceMode.Impulse);

            Destroy(other.gameObject);
        }
    }
}
