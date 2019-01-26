using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlRigidbody : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    public Player player
    {
        get
        {
            if (_player == null)
            {
                _player = GetComponent<Player>();
            }
            return _player;
        }
    }

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

    public int playerNumber { get { return player.playerId; } }

    public float forceMulti = 10f;
    public float fwdOffset = 1f;

    public float balanceForce = 10f;
    public float balanceForceOffset = 1f;

    private void FixedUpdate()
    {
        var playerIn = InputManager.instance.GetPlayerInput(playerNumber);

        var force = new Vector3(playerIn.x, 0, playerIn.y);
        force *= forceMulti;

        rb.AddForceAtPosition(force, transform.InverseTransformPoint(Vector3.forward * fwdOffset));

        rb.AddForceAtPosition(Vector3.up * balanceForce, transform.InverseTransformPoint(Vector3.up * balanceForceOffset));
        rb.AddForceAtPosition(Vector3.down * balanceForce, transform.InverseTransformPoint(Vector3.down * balanceForceOffset));


    }

}
