//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TriggerVNScene : MonoBehaviour
//{
//    public VisualNovelScript VNSceneManager;
//    public string sceneName;

//    private void Start()
//    {
//        VNSceneManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (!VNSceneManager.isNovelSection)
//        {
//            if (collision.gameObject == GameObject.Find("PlayerProto"))
//            {
//                VNSceneManager.StartNovelSceneByName(sceneName);
//            }
//        }
//    }
//}
