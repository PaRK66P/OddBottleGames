using UnityEngine;

public class TriggerVNScene : MonoBehaviour
{
    public VisualNovelScript VNSceneManager;
    public string sceneName;
    bool hasBeenTriggered = false;

    private void Start()
    {
        //VNSceneManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasBeenTriggered)
        {
            if (!VNSceneManager.isNovelSection)
            {
                if (collision.gameObject.tag == "Player")
                {
                    VNSceneManager.StartNovelSceneByName(sceneName);
                    hasBeenTriggered = true;
                }
            }
        }
    }
}
