
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private GameObject Dash1;
    [SerializeField]
    private GameObject Dash2;
    [SerializeField]
    private GameObject Dash3;

    public void SetMaxHealth(float hp)
    {
        healthSlider.maxValue = hp;
    }

    public void SetValue(float newValue)
    {
        healthSlider.value = newValue;
    }

    public void setDashUI(int numDashes)
    {
        switch (numDashes)
        {
            case 0:
                Dash1.SetActive(false);
                Dash2.SetActive(false);
                Dash3.SetActive(false);
                break;
            case 1:
                Dash1.SetActive(true);
                Dash2.SetActive(false);
                Dash3.SetActive(false);
                break;
            case 2:
                Dash1.SetActive(true);
                Dash2.SetActive(true);
                Dash3.SetActive(false);
                break;
            case 3:
                Dash1.SetActive(true);
                Dash2.SetActive(true);
                Dash3.SetActive(true);
                break;
            default:
            {
                UnityEngine.Debug.LogError("tried to have " + numDashes + " without UI support");
                break;
            }
        }

    }
}
