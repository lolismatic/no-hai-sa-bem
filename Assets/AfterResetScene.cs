using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AfterResetScene : MonoBehaviour
{
    public static int music = 0;

    void Awake()
    {
        if (music >= 2)
        {
            Destroy(gameObject);
        }
        music += 1;

        DontDestroyOnLoad(gameObject);
    }

}