/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: GUIAlignment
 * Date    : 2018/03/19
 * Version : v1.0
 * Describe: 
 */

using GEngine.Managers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RectTransform))]
public class GUIAlignment : DecoratorEditor
{
    public GUIAlignment() : base("RectTransformEditor"){}


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();//横向
        EditorGUILayout.LabelField("AlignParent");

        EditorGUILayout.BeginVertical();//开始绘制九宫格
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();//first row
        if (GUILayout.Button("┏", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target; //获得当前操作的transform，target是父中参数
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.TopLeft);
            }
        }
        if (GUILayout.Button("┳", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target;
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.TopCenter);
            }
        }
        if (GUILayout.Button("┓", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target;
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.TopRight);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();//second row
        if (GUILayout.Button("┣", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target;
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.MiddleLeft);
            }
        }
        if (GUILayout.Button("╋", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target;
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.MiddleCenter);
            }
        }
        if (GUILayout.Button("┫", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target;
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.MiddleRight);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();//third row
        if (GUILayout.Button("┗", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target;
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.BottomLeft);
            }
        }
        if (GUILayout.Button("┻", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target;
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.BottomCenter);
            }
        }
        if (GUILayout.Button("┛", GUILayout.Width(25)))
        {
            RectTransform self = (RectTransform)target;
            if (self.parent != null)
            {
                RectTransform parent = (RectTransform)self.parent;
                UIScaler.SetRelativePos(self, parent, AdaptPolicy.BottomRight);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}
