using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCenterOfGravityManager : MonoBehaviour
{
    private static PlayerCenterOfGravityManager _instance;
    public static PlayerCenterOfGravityManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerCenterOfGravityManager>();
            }
            if (_instance == null)
            {
                _instance = new GameObject("[PlayerCenterOfGravityManager]", typeof(PlayerCenterOfGravityManager)).GetComponent<PlayerCenterOfGravityManager>();
            }
            return _instance;
        }
    }

    public Vector3 centerOfGravity { get; private set; }

    private void Update()
    {
        centerOfGravity = Player.allPlayers.Sum(p => p.transform.position) / Mathf.Max(1, Player.allPlayers.Count);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(centerOfGravity, Vector3.up * 10f);
        Gizmos.DrawLine(centerOfGravity + new Vector3(-1f, 0, -1), centerOfGravity + new Vector3(1, 0, 1));
        Gizmos.DrawLine(centerOfGravity + new Vector3(1f, 0, -1), centerOfGravity + new Vector3(-1, 0, 1));
    }

}


public static class ExtSumPos
{
    public static Vector3 Sum<T>(this IEnumerable<T> list, Func<T, Vector3> funk)
    {
        Vector3 sum = Vector3.zero;
        foreach (var l in list)
        {
            sum += funk(l);
        }
        return sum;
    }
}