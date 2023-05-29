using UnityEngine;

namespace ChulguIdleRpg
{
    public class Application : MonoBehaviour
    {
        static Application instance;

        public static Application Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(typeof(Application).Name).AddComponent<Application>();
                }

                return instance;
            }
        }

        float deltaTime;

        public float GetFPS()
        {
            return 1f / deltaTime;
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }
    }
}