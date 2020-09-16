/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/4/26 15:48:21
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XMLib.TileCreator
{
    /// <summary>
    /// TileResourceEditor
    /// </summary>
    [CustomEditor(typeof(TileResources))]
    public class TileResourcesEditor : Editor
    {
        private SerializedProperty TypeName;
        private SerializedProperty RootLayer;
        private SerializedProperty SimpleMode;
        private SerializedProperty CornerMode;
        private SerializedProperty CompositeCollider2D;

        private SerializedProperty T0;
        private SerializedProperty T1;
        private SerializedProperty T1H;
        private SerializedProperty T1Cd;
        private SerializedProperty T1CdH;
        private SerializedProperty T2;
        private SerializedProperty T2Cd;
        private SerializedProperty T2Cl;
        private SerializedProperty T2Cdl;
        private SerializedProperty T3;
        private SerializedProperty T3H;
        private SerializedProperty T3Cl;
        private SerializedProperty T3ClH;
        private SerializedProperty T4;
        private SerializedProperty T4Cd;
        private SerializedProperty T4Cr;
        private SerializedProperty T4Cdr;
        private SerializedProperty T5;
        private SerializedProperty T5Cd;
        private SerializedProperty T5Cdl;
        private SerializedProperty T5Cdlr;
        private SerializedProperty T5Cdr;
        private SerializedProperty T5Cl;
        private SerializedProperty T5Clr;
        private SerializedProperty T5Cr;
        private SerializedProperty T5Cu;
        private SerializedProperty T5Cud;
        private SerializedProperty T5Cudl;
        private SerializedProperty T5Cudlr;
        private SerializedProperty T5Cudr;
        private SerializedProperty T5Cul;
        private SerializedProperty T5Culr;
        private SerializedProperty T5Cur;
        private SerializedProperty T6;
        private SerializedProperty T6Cl;
        private SerializedProperty T6Cu;
        private SerializedProperty T6Cul;
        private SerializedProperty T7;
        private SerializedProperty T7H;
        private SerializedProperty T7Cr;
        private SerializedProperty T7CrH;
        private SerializedProperty T8;
        private SerializedProperty T8Cr;
        private SerializedProperty T8Cu;
        private SerializedProperty T8Cur;
        private SerializedProperty T9;
        private SerializedProperty T9H;
        private SerializedProperty T9Cu;
        private SerializedProperty T9CuH;
        private SerializedProperty T10;
        private SerializedProperty T11;
        private SerializedProperty T12;
        private SerializedProperty T13;
        private SerializedProperty T14;
        private SerializedProperty T15;

        private Dictionary<string, SerializedProperty> _propertyDict = new Dictionary<string, SerializedProperty>();

        private void InitField()
        {
            serializedObject.Update();

            TypeName = serializedObject.FindProperty("TypeName");
            RootLayer = serializedObject.FindProperty("RootLayer");
            SimpleMode = serializedObject.FindProperty("SimpleMode");
            CornerMode = serializedObject.FindProperty("CornerMode");
            CompositeCollider2D = serializedObject.FindProperty("CompositeCollider2D");

            T0 = serializedObject.FindProperty("T0");
            T1 = serializedObject.FindProperty("T1");
            T1H = serializedObject.FindProperty("T1H");
            T1Cd = serializedObject.FindProperty("T1Cd");
            T1CdH = serializedObject.FindProperty("T1CdH");
            T2 = serializedObject.FindProperty("T2");
            T2Cd = serializedObject.FindProperty("T2Cd");
            T2Cl = serializedObject.FindProperty("T2Cl");
            T2Cdl = serializedObject.FindProperty("T2Cdl");
            T3 = serializedObject.FindProperty("T3");
            T3H = serializedObject.FindProperty("T3H");
            T3Cl = serializedObject.FindProperty("T3Cl");
            T3ClH = serializedObject.FindProperty("T3ClH");
            T4 = serializedObject.FindProperty("T4");
            T4Cd = serializedObject.FindProperty("T4Cd");
            T4Cr = serializedObject.FindProperty("T4Cr");
            T4Cdr = serializedObject.FindProperty("T4Cdr");
            T5 = serializedObject.FindProperty("T5");
            T5Cd = serializedObject.FindProperty("T5Cd");
            T5Cdl = serializedObject.FindProperty("T5Cdl");
            T5Cdlr = serializedObject.FindProperty("T5Cdlr");
            T5Cdr = serializedObject.FindProperty("T5Cdr");
            T5Cl = serializedObject.FindProperty("T5Cl");
            T5Clr = serializedObject.FindProperty("T5Clr");
            T5Cr = serializedObject.FindProperty("T5Cr");
            T5Cu = serializedObject.FindProperty("T5Cu");
            T5Cud = serializedObject.FindProperty("T5Cud");
            T5Cudl = serializedObject.FindProperty("T5Cudl");
            T5Cudlr = serializedObject.FindProperty("T5Cudlr");
            T5Cudr = serializedObject.FindProperty("T5Cudr");
            T5Cul = serializedObject.FindProperty("T5Cul");
            T5Culr = serializedObject.FindProperty("T5Culr");
            T5Cur = serializedObject.FindProperty("T5Cur");
            T6 = serializedObject.FindProperty("T6");
            T6Cl = serializedObject.FindProperty("T6Cl");
            T6Cu = serializedObject.FindProperty("T6Cu");
            T6Cul = serializedObject.FindProperty("T6Cul");
            T7 = serializedObject.FindProperty("T7");
            T7H = serializedObject.FindProperty("T7H");
            T7Cr = serializedObject.FindProperty("T7Cr");
            T7CrH = serializedObject.FindProperty("T7CrH");
            T8 = serializedObject.FindProperty("T8");
            T8Cr = serializedObject.FindProperty("T8Cr");
            T8Cu = serializedObject.FindProperty("T8Cu");
            T8Cur = serializedObject.FindProperty("T8Cur");
            T9 = serializedObject.FindProperty("T9");
            T9H = serializedObject.FindProperty("T9H");
            T9Cu = serializedObject.FindProperty("T9Cu");
            T9CuH = serializedObject.FindProperty("T9CuH");
            T10 = serializedObject.FindProperty("T10");
            T11 = serializedObject.FindProperty("T11");
            T12 = serializedObject.FindProperty("T12");
            T13 = serializedObject.FindProperty("T13");
            T14 = serializedObject.FindProperty("T14");
            T15 = serializedObject.FindProperty("T15");

            //
            _propertyDict.Clear();

            _propertyDict[T0.name] = T0;
            _propertyDict[T1.name] = T1;
            _propertyDict[T1H.name] = T1H;
            _propertyDict[T1Cd.name] = T1Cd;
            _propertyDict[T1CdH.name] = T1CdH;
            _propertyDict[T2.name] = T2;
            _propertyDict[T2Cd.name] = T2Cd;
            _propertyDict[T2Cl.name] = T2Cl;
            _propertyDict[T2Cdl.name] = T2Cdl;
            _propertyDict[T3.name] = T3;
            _propertyDict[T3H.name] = T3H;
            _propertyDict[T3Cl.name] = T3Cl;
            _propertyDict[T3ClH.name] = T3ClH;
            _propertyDict[T4.name] = T4;
            _propertyDict[T4Cd.name] = T4Cd;
            _propertyDict[T4Cr.name] = T4Cr;
            _propertyDict[T4Cdr.name] = T4Cdr;
            _propertyDict[T5.name] = T5;
            _propertyDict[T5Cd.name] = T5Cd;
            _propertyDict[T5Cdl.name] = T5Cdl;
            _propertyDict[T5Cdlr.name] = T5Cdlr;
            _propertyDict[T5Cdr.name] = T5Cdr;
            _propertyDict[T5Cl.name] = T5Cl;
            _propertyDict[T5Clr.name] = T5Clr;
            _propertyDict[T5Cr.name] = T5Cr;
            _propertyDict[T5Cu.name] = T5Cu;
            _propertyDict[T5Cud.name] = T5Cud;
            _propertyDict[T5Cudl.name] = T5Cudl;
            _propertyDict[T5Cudlr.name] = T5Cudlr;
            _propertyDict[T5Cudr.name] = T5Cudr;
            _propertyDict[T5Cul.name] = T5Cul;
            _propertyDict[T5Culr.name] = T5Culr;
            _propertyDict[T5Cur.name] = T5Cur;
            _propertyDict[T6.name] = T6;
            _propertyDict[T6Cl.name] = T6Cl;
            _propertyDict[T6Cu.name] = T6Cu;
            _propertyDict[T6Cul.name] = T6Cul;
            _propertyDict[T7.name] = T7;
            _propertyDict[T7H.name] = T7H;
            _propertyDict[T7Cr.name] = T7Cr;
            _propertyDict[T7CrH.name] = T7CrH;
            _propertyDict[T8.name] = T8;
            _propertyDict[T8Cr.name] = T8Cr;
            _propertyDict[T8Cu.name] = T8Cu;
            _propertyDict[T8Cur.name] = T8Cur;
            _propertyDict[T9.name] = T9;
            _propertyDict[T9H.name] = T9H;
            _propertyDict[T9Cu.name] = T9Cu;
            _propertyDict[T9CuH.name] = T9CuH;
            _propertyDict[T10.name] = T10;
            _propertyDict[T11.name] = T11;
            _propertyDict[T12.name] = T12;
            _propertyDict[T13.name] = T13;
            _propertyDict[T14.name] = T14;
            _propertyDict[T15.name] = T15;
        }

        private void SaveField()
        {
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperty()
        {
            EditorGUILayout.PropertyField(TypeName);
            RootLayer.intValue = EditorGUILayout.LayerField(RootLayer.displayName, RootLayer.intValue);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(CompositeCollider2D);
            if (CompositeCollider2D.boolValue)
            {
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(SimpleMode);

            if (SimpleMode.boolValue)
            {
                DrawSimpleMode();
            }
            else
            {
                EditorGUILayout.PropertyField(CornerMode);
                if (CornerMode.boolValue)
                {
                    DrawCornerMode();
                }
                else
                {
                    DrawNormalMode();
                }
            }
        }

        private void DrawSimpleMode()
        {
            EditorGUILayout.PropertyField(T0);
        }

        private void DrawNormalMode()
        {
            EditorGUILayout.PropertyField(T0);
            EditorGUILayout.PropertyField(T1);
            EditorGUILayout.PropertyField(T1H);
            EditorGUILayout.PropertyField(T2);
            EditorGUILayout.PropertyField(T3);
            EditorGUILayout.PropertyField(T3H);
            EditorGUILayout.PropertyField(T4);
            EditorGUILayout.PropertyField(T5);
            EditorGUILayout.PropertyField(T6);
            EditorGUILayout.PropertyField(T7);
            EditorGUILayout.PropertyField(T7H);
            EditorGUILayout.PropertyField(T8);
            EditorGUILayout.PropertyField(T9);
            EditorGUILayout.PropertyField(T9H);
            EditorGUILayout.PropertyField(T10);
            EditorGUILayout.PropertyField(T11);
            EditorGUILayout.PropertyField(T12);
            EditorGUILayout.PropertyField(T13);
            EditorGUILayout.PropertyField(T14);
            EditorGUILayout.PropertyField(T15);
        }

        private void DrawCornerMode()
        {
            EditorGUILayout.PropertyField(T0);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T1);
            EditorGUILayout.PropertyField(T1H);
            EditorGUILayout.PropertyField(T1Cd);
            EditorGUILayout.PropertyField(T1CdH);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T2);
            EditorGUILayout.PropertyField(T2Cd);
            EditorGUILayout.PropertyField(T2Cl);
            EditorGUILayout.PropertyField(T2Cdl);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T3);
            EditorGUILayout.PropertyField(T3H);
            EditorGUILayout.PropertyField(T3Cl);
            EditorGUILayout.PropertyField(T3ClH);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T4);
            EditorGUILayout.PropertyField(T4Cd);
            EditorGUILayout.PropertyField(T4Cr);
            EditorGUILayout.PropertyField(T4Cdr);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T5);
            EditorGUILayout.PropertyField(T5Cd);
            EditorGUILayout.PropertyField(T5Cdl);
            EditorGUILayout.PropertyField(T5Cdlr);
            EditorGUILayout.PropertyField(T5Cdr);
            EditorGUILayout.PropertyField(T5Cl);
            EditorGUILayout.PropertyField(T5Clr);
            EditorGUILayout.PropertyField(T5Cr);
            EditorGUILayout.PropertyField(T5Cu);
            EditorGUILayout.PropertyField(T5Cud);
            EditorGUILayout.PropertyField(T5Cudl);
            EditorGUILayout.PropertyField(T5Cudlr);
            EditorGUILayout.PropertyField(T5Cudr);
            EditorGUILayout.PropertyField(T5Cul);
            EditorGUILayout.PropertyField(T5Culr);
            EditorGUILayout.PropertyField(T5Cur);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T6);
            EditorGUILayout.PropertyField(T6Cl);
            EditorGUILayout.PropertyField(T6Cu);
            EditorGUILayout.PropertyField(T6Cul);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T7);
            EditorGUILayout.PropertyField(T7H);
            EditorGUILayout.PropertyField(T7Cr);
            EditorGUILayout.PropertyField(T7CrH);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T8);
            EditorGUILayout.PropertyField(T8Cr);
            EditorGUILayout.PropertyField(T8Cu);
            EditorGUILayout.PropertyField(T8Cur);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T9);
            EditorGUILayout.PropertyField(T9H);
            EditorGUILayout.PropertyField(T9Cu);
            EditorGUILayout.PropertyField(T9CuH);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(T10);
            EditorGUILayout.PropertyField(T11);
            EditorGUILayout.PropertyField(T12);
            EditorGUILayout.PropertyField(T13);
            EditorGUILayout.PropertyField(T14);
            EditorGUILayout.PropertyField(T15);
        }

        // ===============================

        public override void OnInspectorGUI()
        {
            InitField();
            DrawProperty();

            if (GUILayout.Button("通过选择目录加载"))
            {
                Load();
            }

            SaveField();
        }

        private void Load()
        {
            string resourceDirectory = "";

            string[] strs = Selection.assetGUIDs;
            if (strs.Length == 0)
            {
                Debug.LogWarning("未选择目录");
                return;
            }

            resourceDirectory = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (string.IsNullOrEmpty(resourceDirectory) || !Directory.Exists(resourceDirectory))
            {
                Debug.LogWarning("当前选择不是目录");
                return;
            }

            string[] searchDirectory = new string[] { resourceDirectory };
            string[] guids = AssetDatabase.FindAssets("t:Prefab", searchDirectory);

            Dictionary<string, GameObject> objDict = new Dictionary<string, GameObject>();
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);
                string[] split = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (split == null || split.Length == 0)
                {
                    continue;
                }
                string fieldName = split[split.Length - 1];

                //筛选合适的对象
                if (SimpleMode.boolValue)
                {
                    if (0 != string.Compare(fieldName, "T0"))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!CornerMode.boolValue)
                    {
                        if (fieldName.Contains("C"))
                        {
                            continue;
                        }
                    }
                }

                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                objDict[fieldName] = obj;
            }

            //清空

            foreach (var propertyPair in _propertyDict)
            {
                string fieldName = propertyPair.Key;
                SerializedProperty property = propertyPair.Value;
                property.objectReferenceValue = null;
            }

            //

            foreach (var objPair in objDict)
            {
                string fieldName = objPair.Key;
                GameObject obj = objPair.Value;

                if (!_propertyDict.ContainsKey(fieldName))
                {
                    continue;
                }

                CheckTile(obj, fieldName);

                SerializedProperty property = _propertyDict[fieldName];
                property.objectReferenceValue = obj;
            }
            AssetDatabase.SaveAssets();
        }

        private void CheckTile(GameObject obj, string typeName)
        {
            //EditorUtility.SetDirty(obj);
        }

        public bool IsHalf(string typeName) => typeName.Contains("H");

        public bool IsCorner(string typeName) => typeName.Contains("C");

        public int GetNumber(string typeName)
        {
            string number = string.Empty;
            number += typeName[1];
            if (typeName.Length > 2)
            {
                char number2 = typeName[2];
                if (number2 >= '0' && number2 <= '9')
                {
                    number += number2;
                }
            }

            return int.TryParse(number, out int result) ? result : -1;
        }

        private bool SetTile(GameObject obj)
        {
            string name = obj.name;

            string[] split = name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            if (split == null || split.Length == 0)
            {
                return false;
            }

            string fieldName = split[split.Length - 1];

            FieldInfo info = typeof(TileResources).GetField(fieldName);
            if (null == info)
            {
                return false;
            }

            info.SetValue(target, obj);
            return true;
        }
    }
}
