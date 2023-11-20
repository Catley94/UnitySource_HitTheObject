using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStats : MonoBehaviour
{
    
    [SerializeField] private int _cost;
    [SerializeField] private float upgradeSizeBy;
    
    public int GetCost() => _cost;
    
    public float GetUpgradeSizeBy() => upgradeSizeBy;
}
