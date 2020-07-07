using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Components.Popups;

namespace Units.Hero
{
    public class HeroView : Unit
    {
        [SerializeField] private int _lives = 5;
        [SerializeField] private float _speed = 3.0f;
        [SerializeField] private float _jumpForce = 15.0f;

        public float CurrPosPlatform
        {
            get { return _currPosLevel; }
            set
            {
                if (value > _currPosLevel)
                {
                    _currPosLevel = value;
                    HeroFinishPlatform();
                }
            }
        }

        public Action HeroCompletedLevel;

        private float _currPosLevel;
        private Rigidbody2D _heroRigidbody;
        private Animator _animator;
        private SpriteRenderer _sprite;
        //private Vector3 _intitpos;
        private bool canMove = true;

        private int _minCollidersLength = 1;
        private float _radiusOverlap = 1f;
        private float _pointFlip = 0.00f;

        private bool _isGrounded = false;

        private Vector2 capsuleCenter;
        private Vector2 capsuleSize = new Vector2(0.5f, 2f);
        private CapsuleDirection2D capsuleDirection2D;
        private float capsuleAngle;
        private HeroState State
        {
            get { return (HeroState)_animator.GetInteger("State"); }
            set { _animator.SetInteger("State", (int)value); }
        }

        private void Awake()
        {
            _heroRigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _sprite = GetComponentInChildren<SpriteRenderer>();
            capsuleCenter = transform.position;
        }

        private void OnEnable()
        {
            PlayerPrefs.DeleteKey("NumberOfCoins");// do this in GameController
        }

        private void FixedUpdate()
        {
            CheckGround();
        }

        private void Update()
        {

            if (_isGrounded) State = HeroState.Idle;

            if (Input.GetButton("Horizontal")) Move();
            if (_isGrounded && Input.GetButtonDown("Jump")) Jump();
            if (CheckFalling()) Die();
        }

        private void Move()
        {
            if (canMove)
            {
                if (_isGrounded)
                {
                    State = HeroState.Move;
                }

                Vector3 direction = transform.right * Input.GetAxis("Horizontal");

                transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, _speed * Time.deltaTime);

                _sprite.flipX = direction.x < _pointFlip;
            }
            
        }

        public void Stop()
        {
            canMove = false;
        }

        private void Jump()
        {

            _heroRigidbody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        }

        private void CheckGround()
        {
            Collider2D[] colliders = Physics2D.OverlapCapsuleAll(transform.position, capsuleSize, CapsuleDirection2D.Vertical, (float)Math.PI);

            _isGrounded = colliders.Length > _minCollidersLength;

            if (!_isGrounded) State = HeroState.Jump;

        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!(collision.gameObject.tag == "StartPlatform"))
            {
                CurrPosPlatform = collision.transform.parent.position.x;

                // gameObject.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }

            
        }

            

        public void HeroFinishPlatform()
        {
            HeroCompletedLevel?.Invoke();
        }

        private bool CheckFalling()
        {
            if (transform.position.y > -2f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool _isDie = false;

        public override void Die()
        {
            if (_isDie == false)
            {
                _isDie = true;
                HeroController.HeroDeath();
            }
        }
    }
}

public enum HeroState
{
    Idle,
    Move,
    Jump,
}
