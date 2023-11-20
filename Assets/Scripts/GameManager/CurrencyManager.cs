using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class CurrencyManager : MonoBehaviour
{
    
    private int _balance = 20;
    [SerializeField] private TMP_Text _balanceText;
    private string _prefix = "Balance: ";
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateBalanceUIText();
    }


    #region Public


    public void Purchase(int amount)
    {
        if (CanPurchase(amount))
        {
            DecreaseMoney(amount);
        }
    }

    public bool CanPurchase(int amount)
    {
        if (amount <= _balance)
        {
            return true;
        }

        return false;
    }

    public void MoneyReward(int amount)
    {
        IncreaseMoney(amount);
    }

    #endregion

    #region Private
    
    private void IncreaseMoney(int amount)
    {
        _balance += amount;
        UpdateBalanceUIText();
    }
        
    private void DecreaseMoney(int amount)
    {
        _balance -= amount;
        UpdateBalanceUIText();
    }

    private void UpdateBalanceUIText()
    {
        _balanceText.text = _prefix + _balance;
    }

    #endregion
    
}