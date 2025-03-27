
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private RectTransform healthBarRect;
    [SerializeField]
    private RectMask2D rectMask;

    [SerializeField]
    private GameObject Dash1;

    [SerializeField]
    private GameObject Dash2;

    [SerializeField]
    private GameObject Dash3;

    private float maxRightMask;
    private float initialRightMask;
    private float initialLeftMask;
    // Start is called before the first frame update
    void Start()
    {
        maxRightMask = healthBarRect.rect.width - rectMask.padding.x - rectMask.padding.y;
        initialRightMask = rectMask.padding.z;
        initialLeftMask = rectMask.padding.x;
    }

    public void SetMaxHealth(float hp)
    {
        maxHealth = hp;
    }



    public void SetValue(float newValue)
    {
        float targetWidth = newValue * (maxRightMask / maxHealth);
        float newRightMask = initialRightMask + maxRightMask - targetWidth;
        Vector4 padding = rectMask.padding;
        padding.z = newRightMask;
        rectMask.padding = padding;
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
