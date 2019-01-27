using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DirectionalFeedback : MonoBehaviour
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

    public Transform hat;
    public float tiltHeight = 1f;

    void Update()
    {
        var input = InputManager.instance.GetPlayerInput(player.playerId);
        var topPoint = hat.position + hat.parent.up * tiltHeight + Vector3.right * input.x + Vector3.forward * input.y;
        //hat.forward = hat.parent.forward;
        hat.up = topPoint - hat.position;
    }
}