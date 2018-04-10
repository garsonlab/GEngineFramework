/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Util
 * Date    : 2018/03/23
 * Version : v1.0
 * Describe: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GEngine
{
    public class Util
    {
        /// <summary>
        /// 获取一组不相同的随机数
        /// </summary>
        /// <param name="min">最小值（包含）</param>
        /// <param name="max">最大值（不包含）</param>
        /// <param name="num">随机数量</param>
        /// <returns></returns>
        public static int[] Random(int min, int max, uint num)
        {
            List<int> randoms = new List<int>();
            if (max - min >= num)
            {
                while (randoms.Count < num)
                {
                    int v = UnityEngine.Random.Range(min, max);
                    if (!randoms.Contains(v))
                    {
                        randoms.Add(v);
                    }
                }
            }
            else
            {
                GLog.E("Random Error:: The range of value (max - min) is less then num.");
            }
            return randoms.ToArray();
        }

        /// <summary>
        /// 获取一组不相同的随机数
        /// </summary>
        /// <param name="min">最小值（包含）</param>
        /// <param name="max">最大值（包含）</param>
        /// <param name="num">随机数量</param>
        /// <returns></returns>
        public static float[] Random(float min, float max, uint num)
        {
            List<float> randoms = new List<float>();
            if (max > min)
            {
                while (randoms.Count < num)
                {
                    float v = UnityEngine.Random.Range(min, max);
                    if (!randoms.Contains(v))
                    {
                        randoms.Add(v);
                    }
                }
            }
            else
            {
                GLog.E("Random Error:: Random max value is less than min value.");
            }
            return randoms.ToArray();
        }

    }
}