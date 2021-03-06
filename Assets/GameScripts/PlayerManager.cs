﻿using NDream.AirConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<int, int> playerMap = new Dictionary<int, int>();
    public Cinemachine.CinemachineTargetGroup Group;
    public GameObject playerGameObject;
    private static PlayerManager _instance;
    public static PlayerManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerManager>();
            }
            if (_instance == null)
            {
                _instance = new GameObject("[PlayerManager]", typeof(PlayerManager)).GetComponent<PlayerManager>();
            }
            return _instance;
        }
    }

    public int CreateNewPlayer(int deviceId)
    {
        if (playerMap.ContainsKey(deviceId)) {
            return playerMap[deviceId];
        }

        var player = Instantiate(playerGameObject);

        Group.AddMember(player.transform, 1, 1);

        var playerId = AirConsole.instance.ConvertDeviceIdToPlayerNumber(deviceId);

        playerMap.Add(deviceId, playerId);

        player.GetComponent<Player>().playerId = playerId;

        return playerId;
    }
}
