﻿using System.Collections;
using UnityEngine;

#pragma warning disable 0109

namespace Scripts.World
{
    public class GroundInteraction : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";

        [Header("Keyboard")]
        [SerializeField] private KeyCode groundInteraction = KeyCode.DownArrow;

        private bool isInInteraction;
        private new Collider2D collider;

        private void Awake()
        {
            collider = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (!isInInteraction)
            {
                return;
            }

            if (Input.GetKeyDown(groundInteraction))
            {
                DoInteraction();
            }
        }

        private void DoInteraction()
        {
            collider.enabled = false;
            isInInteraction = false;

            StartCoroutine(EnableGround());
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag(PLAYER_TAG))
            {
                return;
            }

            isInInteraction = true;
        }

        private IEnumerator EnableGround()
        {
            yield return new WaitForSeconds(0.5f);
            collider.enabled = true;
        }
    }
}