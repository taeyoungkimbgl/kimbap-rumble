
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected virtual bool IsDontDestroyGameObject => false;

    public static T instance;

    void Awake()
    {
        if (IsDontDestroyGameObject)
        {
            int obj = FindObjectsOfType<T>().Length;
            if (obj > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        if (instance != null)
        {
            print(gameObject.name + "More than one BuildManager in scene!");
            return;
        }
        instance = this as T;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
