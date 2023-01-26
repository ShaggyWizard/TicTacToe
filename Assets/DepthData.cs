using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthData : MonoBehaviour
{
    public List<int> depth;
    public static DepthData instance;
    private void Awake()
    {
        instance = this;
    }
}
