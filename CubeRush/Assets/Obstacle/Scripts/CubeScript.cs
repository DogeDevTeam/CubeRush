using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public Color CubeColor;
    public Color HighlightColor;
    public bool IsChangeColor;
    private Material CubeMaterial;

    void Start()
    {
        CubeMaterial = GetComponent<Renderer>().material;
        CubeMaterial.color = CubeColor;
    }

    void FixedUpdate()
    {
        if (IsChangeColor)
        {
            CubeMaterial.color = HighlightColor;
        }
        else
        {
            CubeMaterial.color = CubeColor;
        }

        IsChangeColor = false;
    }
}
