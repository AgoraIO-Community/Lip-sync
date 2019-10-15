//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Android;

//using CrazyMinnow.SALSA;

//namespace CrazyMinnow.SALSA.Examples {
//    public class demo : MonoBehaviour
//    {
//        private Salsa3D salsa3D; // Reference to the Salsa3D class

//        AudioSource _source;
//        // Use this for initialization
//        private ArrayList permissionList = new ArrayList();

//        float oneSecTimer = 1;

//        bool _permission = false;

//        // Start is called before the first frame update
//        void Start()
//        {
//            salsa3D = GetComponent<Salsa3D>();
//            _source = GetComponent<AudioSource>();

//#if (UNITY_2018_3_OR_NEWER)
//            permissionList.Add(Permission.Microphone);
//            permissionList.Add(Permission.Camera);
//#endif

//            CheckPermission();

            
//        }

//        private void init() {
//            AudioClip newClip = Microphone.Start(string.Empty, true, 999, 44100);

//            salsa3D.SetAudioClip(newClip);

//            _source.clip = newClip;


//            while (!(Microphone.GetPosition(null) > 0))
//            { }

//            salsa3D.Play();

//            //play back what user say
//            _source.Play();
//            //_source.mute = true;

//        }

//        private void StartMicListener() {
//            _source.clip = Microphone.Start(string.Empty, true, 999, 44100);
//            // HACK - Forces the function to wait until the microphone has started, before moving onto the play function.

//            while (!(Microphone.GetPosition(null) > 0))
//            {
//                _source.Play();
//                //_source.mute = true; // Mute audio ...

//                salsa3D.Stop();
//                salsa3D.Play();
//            }
//        }

//        private void CheckPermission()
//        {
//#if (UNITY_2018_3_OR_NEWER)
//            foreach (string permission in permissionList)
//            {
//                if (!Permission.HasUserAuthorizedPermission(permission))
//                {
//                    Permission.RequestUserPermission(permission);
//                }
//            }
//#endif
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            if (_permission == false && Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
                
//                init();
//                _permission = true;
//            }

//            if (_permission && !_source.isPlaying)
//            {
//                StartMicListener();
//            }
//        }
//    }
//}

