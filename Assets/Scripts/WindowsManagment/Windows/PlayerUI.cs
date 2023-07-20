using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : Window
{
    [SerializeField] private Image powerScale;
    [SerializeField] private TMP_Text countOfShotsField;
    [SerializeField] private TMP_Text fpsCount;

    public void UpdateScale(float currentBulletSize, float currentPlayerSize)
    {
        powerScale.fillAmount = currentBulletSize / currentPlayerSize;
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
