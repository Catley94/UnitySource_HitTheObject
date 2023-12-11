using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class CurrencyManager : MonoBehaviour
{
    
    [SerializeField] private int _balance = 20;
    [SerializeField] private TMP_Text _balanceText;
    private string _prefix = "Balance: ";
    
    [SerializeField] private ParticleImage _particleImage;
    
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
        _particleImage.Play();
        _particleImage.onLastParticleFinished.AddListener(() => _particleImage.Stop());
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