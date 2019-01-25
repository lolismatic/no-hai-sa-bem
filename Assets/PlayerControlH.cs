using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlH : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rb;
    public Rigidbody rb
    {
        get
        {
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }
            return _rb;
        }
    }

    public int playerNumber = 0;

    public float forceMulti = 10f;

    public float balanceForce = 10f;
    public float balanceForceOffset = 1f;

    private void FixedUpdate()
    {
        var playerIn = InputManager.instance.GetPlayerInput(playerNumber);

        var force = new Vector3(playerIn.x, 0, playerIn.y);
        force *= forceMulti;

        rb.AddForce(force);

        rb.AddForceAtPosition(Vector3.up * balanceForce, transform.InverseTransformPoint(Vector3.up * balanceForceOffset));
        rb.AddForceAtPosition(Vector3.down * balanceForce, transform.InverseTransformPoint(Vector3.down * balanceForceOffset));


    }

}
