using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerContactPoints : MonoBehaviour
{
    public static Dictionary<Player, PlayerContactPoints> playerContactPoints = new Dictionary<Player, PlayerContactPoints>();

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

    public static PlayerContactPoints Get(Player fromPlayer)
    {
        PlayerContactPoints gg = null;
        playerContactPoints.TryGetValue(fromPlayer, out gg);
        return gg;
    }

    public Transform leftHand;
    public Transform rightHand;
    public Transform leftShoulderGrab;
    public Transform rightShoulderGrab;
    public Transform middleShoulderBackGrab;
    public Transform assGrab;

    private void Awake()
    {
        playerContactPoints[player] = this;
    }
    
}

public static class PlayerContactPointsExtensions
{
    public static PlayerContactPoints GetContactPoints(this Player player)
    {
        return PlayerContactPoints.Get(player);
    }
}