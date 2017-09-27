using System;
using System.Collections.Generic;

namespace Framework
{
    public static class RandomUtils
    {
        static Random random = new Random((int)DateTime.Now.ToBinary());

        public static int Random(int min, int max)
        {
            return random.Next(min, max);
        }

        public static int MTRandom(int min, int max, MersenneTwisterRandom mtRandom)
        {
            return mtRandom.Next(min, max);
        }

        /// <summary>
        /// 随机从 0 ~ max-1 中取出不重复的 num 个整数
        /// @wangzhenhai
        /// </summary>
        public static int[] GetRandom(int num, int max, MersenneTwisterRandom r)
        {
            if (num < 0 || max < 0 || num > max)
                return null;

            int[] result = new int[num];
            int[] seed = new int[max];

            for (int i = 0; i < max; i++)
            {
                seed[i] = i;
            }

            for (int i = 0; i < num; i++)
            {
                int index = r.Next(0, max - i);
                result[i] = seed[index];
                seed[index] = seed[num - i - 1];
            }

            return result;
        }
    }

    /// <summary>
    /// @wangzhenhai
    /// 权重随机生成器
    /// </summary>
    public class RandomWeightGenerator
    {
        class WeightItem
        {
            public int ID { get; private set; }
            public int iWeight { get; private set; }

            public WeightItem(int id, int weight)
            {
                ID = id;
                iWeight = weight;
            }
        }

        MersenneTwisterRandom random = null;
        List<WeightItem> _weights = new List<WeightItem>();
        int _totalWeight = 0;

        public RandomWeightGenerator(MersenneTwisterRandom r)
        {
            random = r;
        }

        public void AppendWeight(int id, int weight)
        {
            _weights.Add(new WeightItem(id, weight));
            _totalWeight += weight;
        }

        public int RandomResult()
        {
            int r = random.Next(0, _totalWeight);
            int weight = 0;

            foreach (WeightItem item in _weights)
            {
                weight += item.iWeight;
                if (weight >= r)
                {
                    return item.ID;
                }
            }

            return -1;
        }

        public void Reset()
        {
            _weights.Clear();
            _totalWeight = 0;
        }
    }
}
