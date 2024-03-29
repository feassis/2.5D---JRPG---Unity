using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float sprintSpeed = 5;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int minSteps;
    [SerializeField] private int maxSteps;
    
    private int stepsInGrass;

    private PlayerControls playerControls = null;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movingInGrass;
    private float stepTimer;
    private int stepsToEncounter;

    private const string IS_WALK_PARAM = "IsWalking";
    private const string BATTLESCENE = "BattleScene";
    private const float TIME_PER_STEP = 0.5f;
    private bool isRunning = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();

        playerControls.Player.Run.performed += ctx => OnRunningPressed();
        playerControls.Player.Run.canceled += ctx => OnRunningCanceled();

        CalculateStpesToNextEncounter();
    }

    private void OnRunningCanceled()
    {
        isRunning = false;
    }

    private void OnRunningPressed()
    {
        isRunning = true;
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
        rb.MovePosition(transform.position + movement * GetSpeed() * Time.fixedDeltaTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, grassLayer);

        movingInGrass = colliders.Length != 0 && movement != Vector3.zero;

        if(movingInGrass)
        {
            stepTimer += Time.fixedDeltaTime;

            if (stepTimer > TIME_PER_STEP)
            {
                stepTimer = 0;
                stepsInGrass++;
            
                if(stepsInGrass >= stepsToEncounter)
                {
                    stepsInGrass = 0;
                    CalculateStpesToNextEncounter();
                    SceneManager.LoadScene(BATTLESCENE);
                }
            }
        }
    }

    private void CalculateStpesToNextEncounter()
    {
        stepsToEncounter = UnityEngine.Random.Range(minSteps, maxSteps);
    }

    private float GetSpeed()
    {
        return isRunning ? sprintSpeed : speed;
    }
}