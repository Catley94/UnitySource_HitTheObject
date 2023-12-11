using System;
using System.Collections;
using System.Collections.Generic;
using Destructible2D.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{

    [SerializeField] private float tapSize;
    [SerializeField] private TMP_Text tapSizeText;
    [SerializeField] private int increaseCostBy = 1;
    [SerializeField] private TMP_Text costText;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            PurchaseUpgrade(transform.GetChild(0).GetComponent<UpgradeStats>().GetCost());
        });
        
        tapSizeText.text = "Tap Size: " + tapSize;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            UpgradeStats upgradeStats = child.GetComponent<UpgradeStats>();
            if (FindObjectOfType<CurrencyManager>().CanPurchase(upgradeStats.GetCost()))
            {
                child.GetComponent<Button>().interactable = true;
            }
            else
            {
                child.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void PurchaseUpgrade(int cost)
    {
        D2dTapToStamp tapToStamp = FindObjectOfType<D2dTapToStamp>();
        UpgradeStats firstChildUpgradeStats = transform.GetChild(0).GetComponent<UpgradeStats>();
        FindObjectOfType<CurrencyManager>().Purchase(cost);
        tapToStamp.Size = new Vector2(
            tapToStamp.Size.x + firstChildUpgradeStats.GetComponent<UpgradeStats>().GetUpgradeSizeBy(), 
            tapToStamp.Size.y + firstChildUpgradeStats.GetComponent<UpgradeStats>().GetUpgradeSizeBy());
        IncreaseTapSize(firstChildUpgradeStats.GetComponent<UpgradeStats>().GetUpgradeSizeBy());

        int newCost = firstChildUpgradeStats.GetComponent<UpgradeStats>().GetCost() +
                      increaseCostBy *
                      FindObjectOfType<MonsterManager>().GetRound();
        firstChildUpgradeStats.GetComponent<UpgradeStats>().SetCost(newCost);
        costText.text = "Cost: " + newCost;
    }

    private void IncreaseTapSize(float _upgradeSizeBy)
    {
        tapSize += _upgradeSizeBy;
        tapSizeText.text = "Tap Size: " + tapSize;
    }

}
