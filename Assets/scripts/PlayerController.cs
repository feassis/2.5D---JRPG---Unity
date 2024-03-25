using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    private PlayerControls playerControls = null;

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

        Debug.Log($"({x} , {z})");
    }
}
