using UnityEngine;

/*
 * Mono Singleton for Mono Behaviours
 * David Leon Molero - 2013
 */

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    [Header ("MonoSingleton Settings")]
	public bool destroyOnLoad = false;
    protected bool applicationIsQuitting = false;
	private static object _lock = new object();
	protected static T m_Instance = null;
	public static T instance
	{
		get
		{
            //if (applicationIsQuitting) {
            //    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
            //        "' already destroyed on application quit." +
            //        " Won't create again - returning null.");
            //    return null;
            //}

			lock (_lock)
			{

				// Instance requiered for the first time, we look for it
				if (m_Instance == null)
				{
					m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

					// Object not found, we create a temporary one
					if (m_Instance == null)
					{
						Debug.Log("No instance of " + typeof(T).ToString() + ", a temporary one is created.");
						m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();

						// Problem during the creation, this should not happen
						if (m_Instance == null)
						{
							Debug.LogError("Problem during the creation of " + typeof(T).ToString());
						}
					}

				}
				return m_Instance;
			}
		}
	}

    public virtual void OnSingletonDestroy() { }
	//private static bool applicationIsQuitting = false;

	// If no other monobehaviour request the instance in an awake function
	// executing before this one, no need to search the object.
    protected virtual void Awake()
	{
		if( m_Instance == null )
		{
			m_Instance = this as T;
        	if(destroyOnLoad == false) if (Application.isPlaying) DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if (this != m_Instance)
				Destroy(this.gameObject);
		}
	}
 
	// Make sure the instance isn't referenced anymore when the user quit, just in case.
	private void OnApplicationQuit()
	{
		m_Instance = null;
        applicationIsQuitting = true;         
	}

	public void OnDestroy()
	{
        this.OnSingletonDestroy();
	}
}
