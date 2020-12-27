using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*my notes
edited in a few places to make it so the singletons are generated and destroyed in every scene (see application is quitting comment out and DontDestroyOnLoad
//do this because in multiplayer, don't want P2 to have own singletons, but access the P1 singletons that are periodically updated and messed around with by P2
    */

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            //Debug.Log("trying to get singleton" + typeof(T));
            //if (applicationIsQuitting)
            //{
            //    //Debug.Log("trying to get singleton 1.5 " + typeof(T));
            //    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
            //        "' already destroyed on application quit." +
            //        " Won't create again - returning null.");
            //    return null;
            //}
            //Debug.Log("trying to get singleton 2" + typeof(T));
            lock (_lock)
            {
                //Debug.Log("trying to get singleton 3" + typeof(T));
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        //if( !PhotonNetwork.offlineMode && !PhotonNetwork.isMasterClient)
                        //{
                        //    //Debug.Log("in singleton, photon style, checking for existing singleton " + typeof(T));
                        //    HashSet<GameObject> temp = PhotonNetwork.FindGameObjectsWithComponent(typeof(T)); //Debug.Log("hash set size is " + temp.Count);
                        //    List<GameObject> hList = temp.ToList();
                        //    if( hList.Count > 0)
                        //    {
                        //        _instance = hList[0].GetComponent<T>();
                        //        return _instance;
                        //    }
                        //}
                        //Debug.Log("creating a non photon singleton of " + typeof(T));
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        //DontDestroyOnLoad(singleton); //decoud edit, want them destroyed on load (attach to gameobject when I want them saved)

                        //Debug.Log("[Singleton] An instance of " + typeof(T) +
                        //    " is needed in the scene, so '" + singleton +
                        //    "' was created with DontDestroyOnLoad.");
                    }
                    else {
                        //Debug.Log("[Singleton] Using instance already created: " + typeof(T).ToString() +_instance.gameObject.name);
                    }
                }
                //Debug.Log("instance not equal to null..." + typeof(T));
                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy()
    {
        //applicationIsQuitting = true;
    }
}