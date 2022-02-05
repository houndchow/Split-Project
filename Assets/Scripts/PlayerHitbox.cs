﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class PlayerHitbox : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == 3)
            {
                PlayerMovement pm = GetComponentInParent<PlayerMovement>();
                if (!pm.invulTime.IsRunning())
                {
                    BoxCollider2D col = GetComponent<BoxCollider2D>();
                    Rigidbody2D PlayerBody = col.attachedRigidbody;
                    if (PlayerBody.velocity.x < collider.attachedRigidbody.velocity.x)
                    {
                        PlayerBody.AddForce(new Vector2(1500, 0));
                    }
                    else
                    {
                        PlayerBody.AddForce(new Vector2(-1500, 0));
                    }
                    pm.TakeDamage(20);
                }
            }
        }

    }
}
