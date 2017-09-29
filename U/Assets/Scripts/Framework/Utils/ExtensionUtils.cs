using System;
﻿using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 自定义扩展方法
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public static class ExtensionList
    {
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// this method return length of List, and return 0 when List is null or empty.
        /// </summary>
        public static int Count<T>(this List<T> list)
        {
            return list != null ? list.Count : 0;
        }

        /// <summary>
        /// this method usually used in foreach loop, return itself when enumerator
        /// is not null, otherwise return an empty List.
        /// </summary>
        public static IEnumerable<T> CheckNull<T>(this IEnumerable<T> enumerator)
        {
            return enumerator == null ? new List<T>(0) : enumerator;
        }
    }

    public static class ExtensionArray
    {
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        /// <summary>
        /// this method return length of Array, and return 0 when array is null or empty.
        /// </summary>
        public static int Length<T>(this T[] array)
        {
            return array != null ? array.Length : 0;
        }
    }

    public static class ExtensionComponent
    {
        public static void SetActive(this UnityEngine.Component component, bool value)
        {
            if (component != null && component.gameObject != null)
            {
                component.gameObject.SetActive(value);
            }
        }

        public static bool activeSelf(this UnityEngine.Component component)
        {
            if (component != null && component.gameObject != null)
            {
                return component.gameObject.activeSelf;
            }

            return false;
        }
    }

    public static class ExtensionAction
    {
        public static void Call(this Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        public static void Call<T>(this Action<T> action, T parameter)
        {
            if (action != null)
            {
                action(parameter);
            }
        }

        public static void Call<T1, T2>(this Action<T1, T2> action, T1 parameter1, T2 parameter2)
        {
            if (action != null)
            {
                action(parameter1, parameter2);
            }
        }

        public static void Call<T>(this UnityAction<T> action, T parameter)
        {
            if (action != null)
            {
                action(parameter);
            }
        }
    }
}
