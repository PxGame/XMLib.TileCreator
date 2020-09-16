/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 1/23/2019 1:16:26 PM
 */

using System.Reflection;
using UnityEngine;

namespace XMLib.TileCreator
{
    /// <summary>
    /// 地图资源设置
    /// </summary>
    [CreateAssetMenu(menuName = "XMLib/横版地图编辑器资源配置", fileName = "TileResourses")]
    [System.Serializable]
    public class TileResources : ScriptableObject
    {
        /// <summary>
        /// 元素名
        /// </summary>
        public string TypeName;

        /// <summary>
        /// 根层级
        /// </summary>
        public int RootLayer;

        /// <summary>
        /// 是否是简单模式
        /// </summary>
        public bool SimpleMode;

        /// <summary>
        /// 是否是角模式
        /// </summary>
        public bool CornerMode;

        /// <summary>
        /// 是否合并碰撞
        /// </summary>
        public bool CompositeCollider2D;

        public GameObject T0;

        public GameObject T1;
        public GameObject T1H;
        public GameObject T1Cd;
        public GameObject T1CdH;

        public GameObject T2;
        public GameObject T2Cd;
        public GameObject T2Cl;
        public GameObject T2Cdl;

        public GameObject T3;
        public GameObject T3H;
        public GameObject T3Cl;
        public GameObject T3ClH;

        public GameObject T4;
        public GameObject T4Cd;
        public GameObject T4Cr;
        public GameObject T4Cdr;

        public GameObject T5;
        public GameObject T5Cd;
        public GameObject T5Cdl;
        public GameObject T5Cdlr;
        public GameObject T5Cdr;
        public GameObject T5Cl;
        public GameObject T5Clr;
        public GameObject T5Cr;
        public GameObject T5Cu;
        public GameObject T5Cud;
        public GameObject T5Cudl;
        public GameObject T5Cudlr;
        public GameObject T5Cudr;
        public GameObject T5Cul;
        public GameObject T5Culr;
        public GameObject T5Cur;

        public GameObject T6;
        public GameObject T6Cl;
        public GameObject T6Cu;
        public GameObject T6Cul;

        public GameObject T7;
        public GameObject T7H;
        public GameObject T7Cr;
        public GameObject T7CrH;

        public GameObject T8;
        public GameObject T8Cr;
        public GameObject T8Cu;
        public GameObject T8Cur;

        public GameObject T9;
        public GameObject T9H;
        public GameObject T9Cu;
        public GameObject T9CuH;

        public GameObject T10;
        public GameObject T11;
        public GameObject T12;
        public GameObject T13;
        public GameObject T14;
        public GameObject T15;

        public GameObject Get(string fieldName)
        {
            FieldInfo info = GetType().GetField(fieldName);
            GameObject obj = (GameObject)info.GetValue(this);
            return obj;
        }
    }
}
