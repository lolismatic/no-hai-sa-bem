using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
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

    public Vector2 GetPlayerInput(int player)
    {
        // set up input axis in Preferences/Input 
        return new Vector2(Input.GetAxis("Player " + player + " X"), Input.GetAxis("Player " + player + " Y"));
        return Vector2.zero;
    }
}
