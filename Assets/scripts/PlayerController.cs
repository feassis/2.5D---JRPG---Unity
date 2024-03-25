using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 100;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerControls playerControls = null;
    private Rigidbody rb;
    private Vector3 movement;

    private const string IS_WALK_PARAM = "IsWalking";

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            return;
        }
        playerControls.Enable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        if (playerControls == null)
        {
            return;
        }
        playerControls.Disable();
    }

    private void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        movement = new Vector3 (x, 0, z).normalized;

        animator.SetBool(IS_WALK_PARAM, movement != Vector3.zero);

        spriteRenderer.flipX = x < 0;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}
