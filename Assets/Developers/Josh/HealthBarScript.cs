using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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
        //Debug.Log("initial right mask: " + initialRightMask);
        //maxHealth = 10;
        //SetValue(100);
    }

    public void SetMaxHealth(float hp)
    {
        //Debug.Log("setting max health to: " + hp);
        maxHealth = hp;
    }



    public void SetValue(float newValue)
    {
        //Debug.Log("settings health to: " + newValue);
        float targetWidth = newValue * (maxRightMask / maxHealth);
        //Debug.Log("target width: " + targetWidth);
        float newRightMask = initialRightMask + maxRightMask - targetWidth;
        //Debug.Log("new right mask: " + newRightMask);
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
