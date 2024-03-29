﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 简易UICell对象池
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UISimpleObjectPool : MonoBehaviour
    {
        // the max cached object count, default is 50
        public int maxCount = 50;
        // the prefab that this object pool returns instances of
        public GameObject prefab;
        // collection of currently inactive instances of the prefab
        private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

        // returns an instance of the prefab
        public GameObject GetObject()
        {
            GameObject spawnedGameObject;

            // if there is an inactive instance of the prefab ready to return, return that
            if (inactiveInstances.Count > 0)
            {
                // remove the instance from teh collection of inactive instances
                spawnedGameObject = inactiveInstances.Pop();
            }
            // otherwise, create a new instance
            else
            {
                spawnedGameObject = (GameObject)GameObject.Instantiate(prefab);

                // add the PooledObject component to the prefab so we know it came from this pool
                UIPooledObject pooledObject = spawnedGameObject.AddComponent<UIPooledObject>();
                pooledObject.pool = this;
            }

            // put the instance in the root of the scene and enable it
            // spawnedGameObject.transform.SetParent(null,false);
            spawnedGameObject.SetActive(true);

            // return a reference to the instance
            return spawnedGameObject;
        }

        // return an instance of the prefab to the pool
        public void ReturnObject(GameObject toReturn)
        {
            UIPooledObject pooledObject = toReturn.GetComponent<UIPooledObject>();

            // if the instance came from this pool, return it to the pool, and no maximum cache count exceeded
            if (pooledObject != null && pooledObject.pool == this && inactiveInstances.Count < maxCount)
            {
                // make the instance a child of this and disable it
                toReturn.transform.SetParent(transform, false);
                toReturn.SetActive(false);

                // add the instance to the collection of inactive instances
                inactiveInstances.Push(toReturn);
            }
            // otherwise, just destroy it
            else
            {
                DestroyImmediate(toReturn);
            }
        }
    }

    // a component that simply identifies the pool that a GameObject came from
    public class UIPooledObject : MonoBehaviour
    {
        public UISimpleObjectPool pool;
    }
}
