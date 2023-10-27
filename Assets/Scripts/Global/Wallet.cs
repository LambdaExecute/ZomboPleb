using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wallet : MonoBehaviour
{
    public static Wallet instance { get; private set; }

    public UnityEvent<float> onMoneyChanged = new UnityEvent<float>();
    public UnityEvent<MoneySpendStatus> onSpendMoney = new UnityEvent<MoneySpendStatus>();

    public float money { get; private set; }

    private const string saveKey = "MoneySaveKey";

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            money = PlayerPrefs.GetFloat(saveKey, 0);
        }
    }

    public void Spend(float moneyToSpend)
    {
        if(money - moneyToSpend >= 0)
        {
            money -= moneyToSpend;
            PlayerPrefs.SetFloat(saveKey, money);

            onMoneyChanged.Invoke(money);
            onSpendMoney.Invoke(MoneySpendStatus.Success);
        }
        else
        {
            onSpendMoney.Invoke(MoneySpendStatus.Failed);
        }
    }

    public void Collect(float moneyToCollect)
    {
        money += moneyToCollect;
        PlayerPrefs.SetFloat(saveKey, money);
        onMoneyChanged.Invoke(money);
    }
}
