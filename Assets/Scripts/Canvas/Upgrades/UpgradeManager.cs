using System;
using System.Collections;
using System.Collections.Generic;
using Destructible2D.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{

    [SerializeField] private float tapSize;
    [SerializeField] private TMP_Text tapSizeText;
    [FormerlySerializedAs("increaseCostBy")] [SerializeField] private int increaseTapUpgradeCostBy = 1;
    [SerializeField] private int increaseScratchUpgradeCostBy = 2000;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private int selectedIndex = 0;
    [SerializeField] private GameObject lessThanButton;
    [SerializeField] private GameObject moreThanButton;
    [SerializeField] private GameObject scratchSizeUI;

    private bool dragToStampEnabled = false;
    private Color originalButtonColour;
    
    
    // Start is called before the first frame update
    void Start()
    {

        foreach (Transform button in transform)
        {
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                PurchaseUpgrade(transform.GetChild(selectedIndex).GetComponent<UpgradeStats>().GetCost());
            });
        }
        
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            PurchaseUpgrade(transform.GetChild(selectedIndex).GetComponent<UpgradeStats>().GetCost());
        });
        
        tapSizeText.text = "Tap Size: " + tapSize;
        
        SetButtonsEnabledState(selectedIndex);
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
                if(i == 1 && dragToStampEnabled)
                {
                    child.GetComponent<Button>().interactable = false;
                }
                else
                {
                    child.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                child.GetComponent<Button>().interactable = false;
            }
        }
        
        if(selectedIndex == 0)
        {
            lessThanButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            lessThanButton.GetComponent<Button>().interactable = true;
        }
        
        if(selectedIndex == transform.childCount - 1)
        {
            moreThanButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            moreThanButton.GetComponent<Button>().interactable = true;
        }
    }
    
    public void IncreaseSelectedIndex()
    {
        if (selectedIndex < transform.childCount - 1)
        {
            selectedIndex++;
        }
        else
        {
            selectedIndex = 0;
        }

        SetButtonsEnabledState(selectedIndex);
    }

    private void SetButtonsEnabledState(int _selectedIndex)
    {
        switch (_selectedIndex)
        {
            case 0:
                foreach (Transform button in transform)
                {
                    if (transform.GetChild(0).GetComponent<Button>() != button.GetComponent<Button>())
                    {
                        button.gameObject.SetActive(false);
                    }
                    else
                    {
                        button.gameObject.SetActive(true);
                        SetCostText(button.GetComponent<UpgradeStats>().GetCost());
                    }
                }

                break;
            case 1:
                foreach (Transform button in transform)
                {
                    if (transform.GetChild(1).GetComponent<Button>() != button.GetComponent<Button>())
                    {
                        button.gameObject.SetActive(false);
                    }
                    else
                    {
                        button.gameObject.SetActive(true);
                        SetCostText(button.GetComponent<UpgradeStats>().GetCost());
                    }
                    
                }

                break;
            case 2:
                if (dragToStampEnabled)
                {
                    foreach (Transform button in transform)
                    {
                        if (transform.GetChild(2).GetComponent<Button>() != button.GetComponent<Button>())
                        {
                            button.gameObject.SetActive(false);
                        }
                        else
                        {
                            // ColorBlock colours = moreThanButton.GetComponent<Button>().colors;
                            // colours.normalColor = originalButtonColour;
                            button.gameObject.SetActive(true);
                            SetCostText(button.GetComponent<UpgradeStats>().GetCost());
                        }
                    }
                }
                else
                {
                    DecreaseSelectedIndex();
                    // originalButtonColour = moreThanButton.GetComponent<Button>().colors.normalColor;
                    // ColorBlock colours = moreThanButton.GetComponent<Button>().colors;
                    // colours.normalColor = colours.disabledColor;
                }
                break;
            default:
                break;
        }
    }

    public void DecreaseSelectedIndex()
    {
        if (selectedIndex > 0)
        {
            selectedIndex--;
        }
        else
        {
            selectedIndex = transform.childCount - 1;
        }
        
        SetButtonsEnabledState(selectedIndex);
    }

    public void PurchaseUpgrade(int cost)
    {
        D2dTapToStamp tapToStamp = FindObjectOfType<D2dTapToStamp>();
        UpgradeStats childUpgradeStats = transform.GetChild(selectedIndex).GetComponent<UpgradeStats>();
        FindObjectOfType<CurrencyManager>().Purchase(cost);

        switch (selectedIndex)
        {
            case 0:
                tapToStamp.Size = new Vector2(
                    tapToStamp.Size.x + childUpgradeStats.GetComponent<UpgradeStats>().GetUpgradeSizeBy(), 
                    tapToStamp.Size.y + childUpgradeStats.GetComponent<UpgradeStats>().GetUpgradeSizeBy());
                IncreaseTapSize(childUpgradeStats.GetComponent<UpgradeStats>().GetUpgradeSizeBy());
                int newTapCost = childUpgradeStats.GetComponent<UpgradeStats>().GetCost() +
                              increaseTapUpgradeCostBy *
                              FindObjectOfType<MonsterManager>().GetRound();
                childUpgradeStats.GetComponent<UpgradeStats>().SetCost(newTapCost);
                SetCostText(newTapCost);
                break;
            case 1:
                Debug.Log("Scratch Effect");
                GameObject tapToStampGO = FindObjectOfType<D2dTapToStamp>().gameObject;
                tapToStampGO.GetComponent<D2dDragToStamp>().enabled = true;
                dragToStampEnabled = true;
                scratchSizeUI.SetActive(true);
                break;
            case 2:
                Debug.Log("Increase Scratch Effect size");
                FindObjectOfType<D2dDragToStamp>().Thickness +=
                    childUpgradeStats.GetComponent<UpgradeStats>().GetUpgradeSizeBy();
                FindObjectOfType<D2dDragToStamp>().Extend +=
                    childUpgradeStats.GetComponent<UpgradeStats>().GetUpgradeSizeBy();
                scratchSizeUI.GetComponentInChildren<TMP_Text>().text = "Scratch Size: " + FindObjectOfType<D2dDragToStamp>().Thickness.ToString("F1");
                int newScratchCost = childUpgradeStats.GetComponent<UpgradeStats>().GetCost() +
                              increaseScratchUpgradeCostBy *
                              FindObjectOfType<MonsterManager>().GetRound();
                childUpgradeStats.GetComponent<UpgradeStats>().SetCost(newScratchCost);
                SetCostText(newScratchCost);
                break;
            default:
                break;
            
        }

        
        
        
    }

    private void SetCostText(int newCost)
    {
        costText.text = "Cost: " + newCost;
    }

    private void IncreaseTapSize(float _upgradeSizeBy)
    {
        tapSize += _upgradeSizeBy;
        tapSizeText.text = "Tap Size: " + tapSize.ToString("F1");
    }

}
