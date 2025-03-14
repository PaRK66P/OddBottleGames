using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUIManager : MonoBehaviour
{
    [SerializeField] private Sprite normalBulletSprite;
    [SerializeField] private Sprite chargedBulletSprite;
    [SerializeField] private Sprite emptyBulletSprite;
    [SerializeField] private Sprite reloadBackground;

    [SerializeField] private List<GameObject> bulletObjects = new List<GameObject>();

    private int loadedBulletCount = 0;
    private int chargedBulletCount = 0;

    public void UpdateLoadedBullets(int numBullets)
    {
        loadedBulletCount = numBullets;
        for (int i = 0; i < bulletObjects.Count; i++)
        {
            bulletObjects[i].SetActive(false);        
        }
        for (int i = 0; i < loadedBulletCount; i++)
        {
            bulletObjects[i].SetActive(true);
            bulletObjects[i].GetComponent<Image>().sprite = normalBulletSprite;
        }
    }

    public void UpdateChargedBulletsUI(int chargedBullets)
    {
        chargedBulletCount = chargedBullets;

        for (int i = 0; i < chargedBulletCount; i++)
        {
            bulletObjects[i].GetComponent<Image>().sprite = chargedBulletSprite;
        }
        for (int i = chargedBulletCount; i < bulletObjects.Count; i++)
        {
            bulletObjects[i].GetComponent<Image>().sprite = normalBulletSprite;
        }
    }

    public void DeactivateAll()
    {
        foreach (GameObject obj in  bulletObjects)
        {
            obj.SetActive(false);
        }
    }

}
