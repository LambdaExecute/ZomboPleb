using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : Window
{
    [SerializeField] private Image powerScale;
    [SerializeField] private TMP_Text countOfShotsField;
    [SerializeField] private TMP_Text fpsCount;
    [SerializeField] private TMP_Text moneyField;

    public override void Show(WindowsManager windowsManager, object data)
    {
        base.Show(windowsManager, data);
        powerScale.fillAmount = 0;
        Wallet.instance.onMoneyChanged.AddListener(m => moneyField.text = m.ToString());
        moneyField.text = Wallet.instance.money.ToString();
    }

    public void UpdateScale(float currentBulletMass, float currentPlayerMass)
    {
        powerScale.fillAmount = currentBulletMass / currentPlayerMass;
    }

    public void UpdatePossibleCountOfShots(int shotsCount)
    {
        countOfShotsField.text = "Max count of shots: " + shotsCount.ToString();
    }

    private void Update()
    {
        fpsCount.text = "FPS: " + (1 / Time.deltaTime).ToString("00");
    }
}
