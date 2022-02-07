﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class BalloonEnemy : FloatingEnemy
    {
        public bool diving = false;
        public bool rising = false;

        public AudioSource diveSound;

        protected override void Start()
        {
            base.Start();
            diveSound = GetComponent<AudioSource>();
        }

        public override void Move()
        {
            if (!diving && PlayerBelow() && PlayerInRange())
            {
                Dive();
            }
            Rigidbody2D rbE = gameObject.GetComponent<Rigidbody2D>();

            // reverse at walls
            if (CTD.IsLefted && rbE.velocity.x <= 0 || CTD.IsRighted && rbE.velocity.x >= 0)
            {
                UpdateDirection(!isLookingLeft);
            }
            if (!diving && !rising)
            {
                rbE.velocity = LookingDirection() * speed + new Vector2(0, rbE.velocity.y);
            }
            else
            {
                rbE.velocity = new Vector2(0, rbE.velocity.y);
            }
            if (!CTD.IsCeiled)
            {
                rbE.AddForce(new Vector2(0, 200 * Time.deltaTime * speed));
            }
            if (diving && !rising)
            {
                Rigidbody2D r = gameObject.GetComponent<Rigidbody2D>();
                r.AddForce(Vector2.down * 1000f*Time.deltaTime);
            }
            else if (rising && !diving)
            {
                Rigidbody2D r = gameObject.GetComponent<Rigidbody2D>();
                r.AddForce(Vector2.up * 1000f*Time.deltaTime);
            }
            if (CTD.IsGrounded)
            {
                diving = false;
                rising = true;
            }
            if (CTD.IsCeiled)
            {
                rising = false;
            }
        }

        private void Dive()
        {
            diving = true;
            diveSound.Play();
        }
    }
}