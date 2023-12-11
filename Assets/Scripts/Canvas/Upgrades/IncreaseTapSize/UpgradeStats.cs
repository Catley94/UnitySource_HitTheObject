using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeStats : MonoBehaviour
{
    
    [FormerlySerializedAs("_cost")] [SerializeField] private int cost;
    [SerializeField] private float upgradeSizeBy;
    
    public int GetCost() => cost;
    
    public float GetUpgradeSizeBy() => upgradeSizeBy;

    public void SetCost(int _cost)
    {
        cost = _cost;
    }
}
