using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static List<Player> allPlayers = new List<Player>();

    [Header("Core player settings")]
    public int playerId = 0;

    private void OnEnable()
    {
        allPlayers.Add(this);
    }

    private void OnDisable()
    {
        allPlayers.Remove(this);
    }
}