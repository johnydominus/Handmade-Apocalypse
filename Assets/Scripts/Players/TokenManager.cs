using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenManager : MonoBehaviour
{
    public int tokens = 6;

    void Start()
    {
        Debug.Log("Player starts with " + tokens + " tokens.");
    }

    public bool SpendToken()
    {
        if (tokens > 0)
        {
            tokens--;
            Debug.Log("Tokens remaining: " + tokens);
            return true;
        }
        return false;
    }
}
