using System.Collections.Generic;

namespace Core.JG.Extensions
{
    public class XorRandom
    {
        public readonly int initialSeed;
        int seed_;
        int steps_;

        public int Seed => seed_;

        public XorRandom(int seed)
        {
            initialSeed = seed;
            steps_ = 0;
            seed_ = seed == 0 ? 1 : seed;
        }

        public override string ToString()
        {
            if (steps_ != 0)
                return string.Format("[XorRandom initial:{0}, seed:{1}, steps:{2}]", initialSeed, seed_, steps_);

            return string.Format("[XorRandom seed:{0}]", seed_);
        }

        public int Next()
        {
            int x = seed_;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;

            seed_ = x < 0 ? -x : x;
            steps_++;
            return seed_;
        }

        public int Range(int min, int max)
        {
            if (min == max)
                return min;
            return min + Range(max - min);
        }

        public int Range(int max)
        {
            int value = Next();
            return value % max;
        }

        public int RandomIndex<T>(IList<T> list)
        {
            return Range(0, list.Count);
        }

        public T RandomValue<T>(IList<T> list)
        {
            return list[RandomIndex(list)];
        }

        public void ShuffleList<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Range(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public void Reset()
        {
            seed_ = initialSeed;
            steps_ = 0;
        }

        public void Reset(int seed)
        {
            seed_ = seed;
            steps_ = 0;
        }
    }
}