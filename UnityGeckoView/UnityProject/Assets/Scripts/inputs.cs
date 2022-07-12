using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputs : MonoBehaviour
{

        [SerializeField]
        private Transform _controllerRight = null;
        public Transform ControllerRight
        {
            get 
            {
                _controllerRight = DeepChildSearch(gameObject, "hand_right");
                return _controllerRight; 
            }
                 
        }

        [SerializeField]
        private Transform _controllerLeft = null;
        public Transform ControllerLeft
        {
            get
            {
                _controllerLeft = DeepChildSearch(gameObject, "hand_left");
                return _controllerLeft;
            }

        }


        [SerializeField]
        private Transform _localHead;
        public Transform LocalHead
        {
            get
            {
                _localHead = DeepChildSearch(gameObject, "head_JNT");
                return _localHead;
            }

        }

        [SerializeField]
        private RemoteVisualCone _cone;
        public RemoteVisualCone Cone
        {
            get
            {

                Transform t = DeepChildSearch(gameObject, "VisualCone");

                if (t != null)
                {
                    _cone = t.GetComponent<RemoteVisualCone>();
                    return _cone;
                }
                else
                {
                    return null;
                }

            }

        }

        [SerializeField]
        private RemoteLaser _pointer;
        public RemoteLaser Pointer
        {
            get
            {

                Transform t = DeepChildSearch(gameObject, "Laser Pointer");

                if (t != null)
                {
                _pointer = t.GetComponent<RemoteLaser>();
                    return _pointer;
                }
                else
                {
                    return null;
                }

            }

        }

        [SerializeField]
        private stickyCircleRemote _stickyCircle;
        public stickyCircleRemote StickyCircle
        {
            get
            {

                Transform t = DeepChildSearch(gameObject, "StickyCircle");

                if (t != null)
                {
                    _stickyCircle = t.GetComponent<stickyCircleRemote>();
                    return _stickyCircle;
                }
                else
                {
                    return null;
                }
                
            }

        }

        [SerializeField]
        private speaking _speaking;
        public speaking Speaking
        {
            get
            {

                Transform t = DeepChildSearch(gameObject, "Audio Source");

                if (t != null)
                {
                    _speaking = t.GetComponent<speaking>();
                    return _speaking;
                }
                else
                {
                    return null;
                }

            }

        }

        [SerializeField]
        private Insights _insight;
        public Insights Insights
        {
            get
            {
                _insight = gameObject.GetComponent<Insights>();
                return _insight;
            }

        }

        [SerializeField]
        private RecorderObject _voiceRecorder;
        public RecorderObject VoiceRecorder
        {
            get
            {

                Transform t = DeepChildSearch(gameObject, "Audio Source");

                if (t != null)
                {
                    _voiceRecorder = t.GetComponent<RecorderObject>();
                    return _voiceRecorder;
                }
                else
                {
                    return null;
                }

            }

        }

        public Transform DeepChildSearch(GameObject g, string childName) {

            Transform child = null;

            for (int i = 0; i< g.transform.childCount; i++) {

                Transform currentchild = g.transform.GetChild(i);

                if (currentchild.gameObject.name == childName)
                {

                    return currentchild;
                }
                else {

                    child = DeepChildSearch(currentchild.gameObject, childName);

                    if (child != null) return child;
                }

            }

            return null;
        }
}
