using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InputGenerateHack : MonoBehaviour
{
    [Serializable]
    public class InputItem
    {
        public string axisName = "Horizontal";
        public string negativeButton;
        public string positiveButton;
        public string altNegativeButton;
        public string altPositiveButton;
        public float gravity = 1f;
        public float dead = 0.001f;
        public float sensitivity = 1f;
        public bool snap;
        public bool invert;
        public InputTypes type;
        public int axis;
        public int joyNum;

        public enum InputTypes
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2,
        }

        public string GetAllText()
        {
            return
                   $"  - serializedVersion: 3 " +
            "\n" + $"    m_Name: {axisName} " +
            "\n" + $"    descriptiveName:  " +
            "\n" + $"    descriptiveNegativeName:  " +
            "\n" + $"    negativeButton: {negativeButton} " +
            "\n" + $"    positiveButton: {positiveButton} " +
            "\n" + $"    altNegativeButton: {altNegativeButton} " +
            "\n" + $"    altPositiveButton: {altPositiveButton} " +
            "\n" + $"    gravity: {gravity} " +
            "\n" + $"    dead: {dead.ToString("F3")} " +
            "\n" + $"    sensitivity: {sensitivity} " +
            "\n" + $"    snap: {(snap ? 1 : 0).ToString()} " +
            "\n" + $"    invert: {(invert ? 1 : 0).ToString()} " +
            "\n" + $"    type: {(int)type} " +
            "\n" + $"    axis: {axis} " +
            "\n" + $"    joyNum: {joyNum} " +
            "\n" + "";
        }
    }

    public List<InputItem> inputItems = new List<InputItem>();

    [Header("Keys settings")]
    public float gravityForKeys = 3;
    public float deadForKeys = 0.001f;
    public float sensiForKeys = 3f;

    [Header("Joy settings")]
    public float gravityForJoy = 0;
    public float deadForJoy = 0.19f;
    public float sensiForJoy = 1f;

    [DebugButton]
    public void AddPlayerWASD(int playerId, string keys)
    {
        inputItems.Add(new InputItem()
        {
            axisName = "Player " + playerId + " Y",
            positiveButton = keys[0].ToString(),
            negativeButton = keys[2].ToString(),
            gravity = gravityForKeys,
            dead = deadForKeys,
            sensitivity = sensiForKeys,
            snap = true,
            type = InputItem.InputTypes.KeyOrMouseButton,
        });
        inputItems.Add(new InputItem()
        {
            axisName = "Player " + playerId + " X",
            negativeButton = keys[1].ToString(),
            positiveButton = keys[3].ToString(),
            gravity = gravityForKeys,
            dead = deadForKeys,
            sensitivity = sensiForKeys,
            snap = true,
            type = InputItem.InputTypes.KeyOrMouseButton,
        });

    }

    [DebugButton]
    public void AddPlayerArrowKeys(int playerId)
    {
        inputItems.Add(new InputItem()
        {
            axisName = "Player " + playerId + " Y",
            positiveButton = "up",
            negativeButton = "down",
            gravity = gravityForKeys,
            dead = deadForKeys,
            sensitivity = sensiForKeys,
            snap = true,
            type = InputItem.InputTypes.KeyOrMouseButton,
        });
        inputItems.Add(new InputItem()
        {
            axisName = "Player " + playerId + " X",
            negativeButton = "left",
            positiveButton = "right",
            gravity = gravityForKeys,
            dead = deadForKeys,
            sensitivity = sensiForKeys,
            snap = true,
            type = InputItem.InputTypes.KeyOrMouseButton,
        });
    }

    [DebugButton]
    public void AddPlayerJoysticks(int firstPlayerId, int playerCountNumarPar, int firstJoystickId)
    {
        for (int i = firstPlayerId; i < firstPlayerId + playerCountNumarPar; i++)
        {
            // nr par = left stick = primu e la stanga, al doilea e la dreapta...
            bool leftStick = i % 2 == 0;
            AddPlayerJoystick(i, firstJoystickId + i / 2, leftStick);
        }
    }

    public void AddPlayerJoystick(int playerId, int joystickId, bool leftStick)
    {
        inputItems.Add(new InputItem()
        {
            axisName = "Player " + playerId + " X",
            gravity = gravityForJoy,
            dead = deadForJoy,
            sensitivity = sensiForJoy,
            type = InputItem.InputTypes.JoystickAxis,
            axis = leftStick ? 0 : 3,
            joyNum = joystickId,
        });

        inputItems.Add(new InputItem()
        {
            axisName = "Player " + playerId + " Y",
            gravity = gravityForJoy,
            dead = deadForJoy,
            sensitivity = sensiForJoy,
            type = InputItem.InputTypes.JoystickAxis,
            axis = leftStick ? 1 : 4,
            joyNum = joystickId,
            invert = true, // invert Y on joystick - do not ask Y
        });
    }

    public string GetAllText()
    {
        var s = "";
        s += ""
+ "%YAML 1.1"
+ "\n" + "%TAG !u! tag:unity3d.com,2011:"
+ "\n" + "--- !u!13 &1"
+ "\n" + "InputManager:"
+ "\n" + "  m_ObjectHideFlags: 0"
+ "\n" + "  serializedVersion: 2"
+ "\n" + "  m_Axes:"
+ "\n";
        for (int i = 0; i < inputItems.Count; i++)
        {
            s += inputItems[i].GetAllText();
        }
        return s;
    }

    [DebugButton]
    public void SaveToFile()
    {
        var s = GetAllText();
        StreamWriter sw = new StreamWriter("InputManagerOverride.txt");
        sw.Write(s);
        sw.Close();
    }

}
