using System;
using UnityEngine;

namespace SignInSample.Resolution
{
    // 가로 폭이 일정비율 넘어가면 짜른다
    public class CameraResolution : MonoBehaviour
    {
        public int x = 9;
        public int y = 21;

        static float wantedAspectRatio;
        static Camera cam;
        static Camera backgroundCam;

        void Awake()
        {
            cam = GetComponent<Camera>();
            if (!cam)
            {
                cam = Camera.main;
            }

            if (!cam)
            {
                Debug.LogError("No camera available");
                return;
            }
            
            wantedAspectRatio = (float)x / y;
            SetCamera();
        }

        public void SetCamera()
        {
            cam = GetComponent<Camera>();

            if (cam == null)
                return;
            
            float currentAspectRatio = (float)UnityEngine.Device.Screen.width / UnityEngine.Device.Screen.height;
            // If the current aspect ratio is already approximately equal to the desired aspect ratio,
            // use a full-screen Rect (in case it was set to something else previously)

            if (currentAspectRatio < 0.6)
            {
                cam.rect = new Rect(0, 0, 1, 1);
                return;
            }


            if ((int)(currentAspectRatio * 100) / 100.0f == (int)(wantedAspectRatio * 100) / 100.0f)
            {
                cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                if (backgroundCam)
                {
                    Destroy(backgroundCam.gameObject);
                }

                return;
            }

            // Pillarbox
            if (currentAspectRatio > wantedAspectRatio)
            {
                float inset = 1.0f - wantedAspectRatio / currentAspectRatio;
                //Debug.Log(new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f));
                cam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
            }
            // Letterbox
            else
            {
                float inset = 1.0f - currentAspectRatio / wantedAspectRatio;
                cam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
            }

            if (!backgroundCam)
            {
                // Make a new camera behind the normal camera which displays black; otherwise the unused space is undefined
                backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).GetComponent<Camera>();
                backgroundCam.depth = int.MinValue;
                backgroundCam.clearFlags = CameraClearFlags.SolidColor;
                backgroundCam.backgroundColor = Color.black;
                backgroundCam.cullingMask = 0;
            }
        }

        public static int screenHeight
        {
            get { return (int)(Screen.height * cam.rect.height); }
        }

        public static int screenWidth
        {
            get { return (int)(Screen.width * cam.rect.width); }
        }

        public static int xOffset
        {
            get { return (int)(Screen.width * cam.rect.x); }
        }

        public static int yOffset
        {
            get { return (int)(Screen.height * cam.rect.y); }
        }

        public static Rect screenRect
        {
            get
            {
                return new Rect(cam.rect.x * Screen.width, cam.rect.y * Screen.height, cam.rect.width * Screen.width,
                    cam.rect.height * Screen.height);
            }
        }

        public static Vector3 mousePosition
        {
            get
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.y -= (int)(cam.rect.y * Screen.height);
                mousePos.x -= (int)(cam.rect.x * Screen.width);
                return mousePos;
            }
        }

        public static Vector2 guiMousePosition
        {
            get
            {
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y = Mathf.Clamp(mousePos.y, cam.rect.y * Screen.height,
                    cam.rect.y * Screen.height + cam.rect.height * Screen.height);
                mousePos.x = Mathf.Clamp(mousePos.x, cam.rect.x * Screen.width,
                    cam.rect.x * Screen.width + cam.rect.width * Screen.width);
                return mousePos;
            }
        }
        // private bool _isPreCull = false;
        // private float _cullTime = 0;
        //
        // private float _width = 9;
        // private float _height = 16;
        //
        // private void Awake()
        // {
        //     // if (gameObject.CompareTag("MainCamera"))
        //     // {
        //         Camera cam = GetComponent<Camera>();
        //         Rect rect = cam.rect;
        //
        //         float scaleHeight = ((float)Screen.width / Screen.height) / (_width / _height); // (가로 / 세로)
        //         float scaleWidth = 1f / scaleHeight;
        //
        //         if (scaleWidth < 0.8f)
        //         {
        //             rect.width = scaleWidth;
        //             rect.x = (1f - scaleWidth) / 2f;
        //         }
        //         else
        //         {
        //             rect.width = 1;
        //             rect.x = 0;
        //         }
        //
        //         // if (cam.tag.Equals("MainCamera"))
        //         // {
        //             var safeArea = Screen.safeArea;
        //             rect.height = safeArea.height / Screen.height;
        //             rect.y = safeArea.y / Screen.height;
        //         //}
        //
        //         cam.rect = rect;
        //     //}
        // }
        //
        // private void Start()
        // {
        //     return;
        //     if (!gameObject.CompareTag("MainCamera"))
        //     {
        //         Camera camera = Camera.main;
        //         Camera cam = GetComponent<Camera>();
        //         cam.rect = camera.rect;
        //     }
        // }
        //
        // private void Update()
        // {
        //     if (_isPreCull)
        //     {
        //         _cullTime += Time.deltaTime;
        //         if (_cullTime >= 0.2f)
        //             _isPreCull = false;
        //     }
        // }
        //
        // private void OnApplicationFocus(bool hasFocus)
        // {
        //     Camera cam = GetComponent<Camera>();
        //     Rect rect = cam.rect;
        //
        //     float scaleHeight = ((float)Screen.width / Screen.height) / (_width / _height); // (가로 / 세로)
        //     float scaleWidth = 1f / scaleHeight;
        //
        //     if (scaleWidth < 0.8f)
        //     {
        //         rect.width = scaleWidth;
        //         rect.x = (1f - scaleWidth) / 2f;
        //     }
        //     else
        //     {
        //         rect.width = 1;
        //         rect.x = 0;
        //     }
        //
        //     // if (cam.tag.Equals("MainCamera"))
        //     // {
        //     var safeArea = Screen.safeArea;
        //     rect.height = safeArea.height / Screen.height;
        //     rect.y = safeArea.y / Screen.height;
        //     //}
        //
        //     cam.rect = rect;
        //     return;
        //     _isPreCull = true;
        //     _cullTime = 0;
        // }
        //
        // private void OnApplicationPause(bool pauseStatus)
        // {
        //     
        //     return;
        //     Camera cam = GetComponent<Camera>();
        //     Rect rect = cam.rect;
        //
        //     float scaleHeight = ((float)Screen.width / Screen.height) / (_width / _height); // (가로 / 세로)
        //     float scaleWidth = 1f / scaleHeight;
        //
        //     if (scaleWidth < 0.8f)
        //     {
        //         rect.width = scaleWidth;
        //         rect.x = (1f - scaleWidth) / 2f;
        //     }
        //     else
        //     {
        //         rect.width = 1;
        //         rect.x = 0;
        //     }
        //
        //     // if (cam.tag.Equals("MainCamera"))
        //     // {
        //     var safeArea = Screen.safeArea;
        //     rect.height = safeArea.height / Screen.height;
        //     rect.y = safeArea.y / Screen.height;
        //     //}
        //
        //     cam.rect = rect;
        //     return;
        //     _isPreCull = true;
        //     _cullTime = 0;
        //
        //     if (!pauseStatus)
        //     {
        //         // Camera cam = GetComponent<Camera>();
        //         // Rect rect = cam.rect;
        //         //
        //         // float scaleHeight = ((float)Screen.width / Screen.height) / (_width / _height); // (가로 / 세로)
        //         // float scaleWidth = 1f / scaleHeight;
        //         //
        //         // if (scaleWidth < 0.8f)
        //         // {
        //         //     rect.width = scaleWidth;
        //         //     rect.x = (1f - scaleWidth) / 2f;
        //         // }
        //         // else
        //         // {
        //         //     rect.width = 1;
        //         //     rect.x = 0;
        //         // }
        //         //
        //         // var safeArea = Screen.safeArea;
        //         // rect.height = safeArea.height / Screen.height;
        //         // rect.y = safeArea.y / Screen.height;
        //         //
        //         // cam.rect = rect;
        //     }
        // }
        //
        // private void OnPreCull()
        // {
        //     if (_isPreCull)
        //     {
        //         GL.Clear(true, true, Color.black);
        //     }
        // }
    }
}