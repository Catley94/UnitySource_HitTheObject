using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapManager : MonoBehaviour
{

    [SerializeField] private int tapSize = 1;

    public void Increase()
    {
        tapSize += 1;
    }

}
