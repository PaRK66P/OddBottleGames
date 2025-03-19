using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUIManager : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalBulletSprite;
    [SerializeField] private Sprite chargedBulletSprite;
    [SerializeField] private Sprite emptyBulletSprite;
    [SerializeField] private Sprite reloadBackground;

    [Header("Assets")]
    [SerializeField] private List<GameObject> bulletObjects = new List<GameObject>();
    [SerializeField] private GameObject reloadingUI;
    [SerializeField] private GameObject reloadingArrows;

    private int loadedBulletCount = 0;
    private int chargedBulletCount = 0;

    bool isReloading = false;

    float rotationSpeed = 100.0f;

    public void UpdateLoadedBullets(int numBullets)
    {
        loadedBulletCount = numBullets;
        for (int i = 0; i < bulletObjects.Count; i++)
        {
            bulletObjects[i].GetComponent<Image>().sprite = emptyBulletSprite;
        }
        for (int i = 0; i < loadedBulletCount; i++)
        {
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
    }

    public void DeactivateAll()
    {
        foreach (GameObject obj in  bulletObjects)
        {
            obj.SetActive(false);
        }
    }

    public void ReactivateAll()
    {
        foreach (GameObject obj in bulletObjects)
        {
            obj.SetActive(true);
        }
    }

    IEnumerator ReloadAnim()
    {
        reloadingUI.SetActive(true);
        float rotationVal = 0;
        while (isReloading)
        {
            
            reloadingArrows.transform.rotation = Quaternion.Euler(0, 0, rotationVal * rotationSpeed * -1);
            rotationVal += Time.deltaTime;
            yield return null;
        }
        reloadingUI.SetActive(false);
        ReactivateAll();
    }

    public void StartReloadAnim()
    {
        DeactivateAll();
        isReloading = true;
        StartCoroutine(ReloadAnim());
    }

    public void EndReloadAnim()
    {
        isReloading = false;
    }

}
