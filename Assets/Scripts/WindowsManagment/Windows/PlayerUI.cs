using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : Window
{
    [SerializeField] private Image powerScale;
    [SerializeField] private TMP_Text countOfShotsField;
    [SerializeField] private TMP_Text fpsCount;

    public override void Show(WindowsManager windowsManager, object data)
    {
        base.Show(windowsManager, data);
        powerScale.fillAmount = 0;
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
