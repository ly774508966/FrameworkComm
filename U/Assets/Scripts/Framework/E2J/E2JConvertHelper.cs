using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Excel2Json Convert Helper
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public static class E2JConvertHelper
    {
        static char[] separator = { ',', '|' };

        public static int O2I(object obj, int defaultValue = 0)
        {
            try
            {
                if (obj == null)
                {
                    return defaultValue;
                }

                return int.Parse(obj.ToString());
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JConvertHelper O2I exception: " + ex.ToString());
                return defaultValue;
            }
        }

        public static float O2F(object obj, float defaultValue = 0.0f)
        {
            try
            {
                if (obj == null)
                {
                    return defaultValue;
                }

                return float.Parse(obj.ToString());
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JConvertHelper O2F exception: " + ex.ToString());
                return defaultValue;
            }
        }

        public static double O2D(object obj, double defaultValue = 0.0)
        {
            try
            {
                if (obj == null)
                {
                    return defaultValue;
                }

                return double.Parse(obj.ToString());
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JConvertHelper O2D exception: " + ex.ToString());
                return defaultValue;
            }
        }

        public static string O2STrim(object obj, string defaultValue = "")
        {
            try
            {
                if (obj == null)
                {
                    return defaultValue;
                }

                return obj.ToString().Trim();
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JConvertHelper O2STrim exception: " + ex.ToString());
                return defaultValue;
            }
        }

        public static string[] S2SArray(string str)
        {
            try
            {
                return str.Split(separator);
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JConvertHelper S2SArray exception: " + ex.ToString());
                return null;
            }
        }

        public static HashSet<string> S2SHashSet(string str)
        {
            HashSet<string> hash = null;

            string[] strArray = S2SArray(str);

            int length = strArray.Length();
            if (length > 0)
            {
                hash = new HashSet<string>();
                for (int i = 0; i < length; i++)
                {
                    hash.Add(strArray[i]);
                }
            }

            return null;
        }

        public static int[] S2IArray(string str)
        {
            try
            {
                string[] strArray = S2SArray(str);

                int length = strArray.Length();
                if (length > 0)
                {
                    int[] intArray = new int[length];
                    for (int i = 0; i < length; i++)
                    {
                        intArray[i] = O2I(strArray[i]);
                    }
                    return intArray;
                }

                return null;
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JConvertHelper S2IArray exception: " + ex.ToString());
                return null;
            }
        }

        public static float[] S2FArray(string str)
        {
            try
            {
                string[] strArray = S2SArray(str);

                int length = strArray.Length();
                if (length > 0)
                {
                    float[] floatArray = new float[length];
                    for (int i = 0; i < length; i++)
                    {
                        floatArray[i] = O2F(strArray[i]);
                    }
                    return floatArray;
                }

                return null;
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JConvertHelper S2FArray exception: " + ex.ToString());
                return null;
            }
        }

        public static double[] S2DArray(string str)
        {
            try
            {
                string[] strArray = S2SArray(str);

                int length = strArray.Length();
                if (length > 0)
                {
                    double[] doubleArray = new double[length];
                    for (int i = 0; i < length; i++)
                    {
                        doubleArray[i] = O2D(strArray[i]);
                    }
                    return doubleArray;
                }

                return null;
            }
            catch (System.Exception ex)
            {
                Log.Error("E2JConvertHelper S2DArray exception: " + ex.ToString());
                return null;
            }
        }

        public static Vector3 S2Vector3(string str)
        {
            float[] floatArray = S2FArray(str);

            Vector3 v3 = Vector3.zero;
            if (floatArray.Length() == 3)
            {
                v3.x = floatArray[0];
                v3.y = floatArray[1];
                v3.z = floatArray[2];
            }

            return v3;
        }

        public static bool IsZeroS(string str)
        {
            if (!string.IsNullOrEmpty(str) && !str.Trim().Equals("0"))
            {
                return false;
            }
            return true;
        }
    }
}
