using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static Dictionary<int, Vector2> PendingMovements { get; } = new Dictionary<int, Vector2>();

    private static InputManager _instance;
    public static InputManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputManager>();
            }
            if (_instance == null)
            {
                _instance = new GameObject("[InputManager]", typeof(InputManager)).GetComponent<InputManager>();
            }
            return _instance;
        }
    }
    
    void Start()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int playerId, JToken data)
    {
        var message = data.ToObject<ClientMovementMessage>();

        var actualPlayerID = PlayerManager.instance.CreateNewPlayer(playerId);
        PendingMovements[actualPlayerID] = new Vector2(message.Tilt.X, message.Tilt.Y);
    }

    public Vector2 GetPlayerInput(int playerId)
    {
        if (PendingMovements.ContainsKey(playerId)) {
            var temp = PendingMovements[playerId];

            Debug.Log($"{temp.x}, {temp.y}");
            
            return temp;
        }

        if (playerId > 3)
        {
            return Vector2.zero;
        }

        return new Vector2(Input.GetAxis("Player " + playerId + " X"), Input.GetAxis("Player " + playerId + " Y"));
    }
}
