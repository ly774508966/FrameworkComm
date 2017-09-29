using System.Collections.Generic;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public static class ListPool<T>
    {
        // object pool to avoid allocations
        static readonly ObjectPool<List<T>> listPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return listPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            if (listPool.countInactive < 5)
            {
                listPool.Release(toRelease);
            }
            else
            {
                toRelease.Clear();
            }
        }
    }
}
