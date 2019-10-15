//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AvatarCreator : MonoBehaviour
//{
//    [SerializeField]
//    private GameObject avatarPrefab;

//    private List<LipSync2D> lipSync2DList = new List<LipSync2D>();

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    public void initLipSyncPrefab(uint id, string name) {


//        if (lipSync2DList == null)
//            lipSync2DList = new List<LipSync2D>();

//        GameObject obj = Instantiate(avatarPrefab, gameObject.transform);

//        LipSync2D sync2D = obj.GetComponent<LipSync2D>();

//        sync2D.InitLipSync(id, name);

//        lipSync2DList.Add(sync2D);
//    }

//    public void removeUser(uint udid) {
//        for (int i = 0; i < lipSync2DList.Count; i++) {
//            if (lipSync2DList[i].ID == udid) {
//                Destroy(lipSync2DList[i].gameObject);
//                lipSync2DList.RemoveAt(i);
//                return;
//            }
//        }
//    }

//    public void cleanAllAvatar() {
//        if (lipSync2DList == null || lipSync2DList.Count == 0) return;

//        for (int i = 0; i < lipSync2DList.Count; i++) {
//            Destroy(lipSync2DList[i].gameObject);
//        }

//        lipSync2DList = null;
//    }
//}
