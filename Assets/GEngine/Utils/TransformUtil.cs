/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: TransformUtil
 * Date    : 2018/03/14
 * Version : v1.0
 * Describe: 
 */

using UnityEngine;

namespace GEngine.Utils
{
    public class TransformUtil
    {



        public static void ChangeLayer(Transform root, int layer)
        {
            ChangeLayers(root, layer, false);
        }

        public static void ChangeLayer(Transform root, string layer)
        {
            ChangeLayers(root, LayerMask.NameToLayer(layer), false);
        }

        public static void ChangeLayers(Transform root, int layer)
        {
            ChangeLayers(root, layer, true);
        }

        public static void ChangeLayers(Transform root, string layer)
        {
            ChangeLayers(root, LayerMask.NameToLayer(layer), true);
        }
        private static void ChangeLayers(Transform root, int layer, bool containChildren)
        {
            if (containChildren)
            {
                Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].gameObject.layer = layer;
                }
            }
            else
            {
                root.gameObject.layer = layer;
            }
        }

    }
}