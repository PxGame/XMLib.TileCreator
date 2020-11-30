/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 1/23/2019 1:16:26 PM
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XMLib.TileCreator
{
    /// <summary>
    /// 地图编辑器
    /// </summary>
    public class TileCreatorEditor : EditorWindow
    {
        [MenuItem("XMLib/横版地图编辑器")]
        public static void ShowWindow()
        {
            var window = GetWindow<TileCreatorEditor>();
            window.titleContent = new GUIContent("横版地图编辑器");
            window.Show();
        }

        #region 数据声明

        public readonly Vector2Int PosDown = new Vector2Int(0, -1);

        public readonly Vector2Int PosLeft = new Vector2Int(-1, 0);

        public readonly Vector2Int PosLowerLeft = new Vector2Int(-1, -1);

        public readonly Vector2Int PosLowerRight = new Vector2Int(1, -1);

        public readonly Vector2Int PosRight = new Vector2Int(1, 0);

        public readonly Vector2Int PosUp = new Vector2Int(0, 1);

        public readonly Vector2Int PosUpperLeft = new Vector2Int(-1, 1);

        public readonly Vector2Int PosUpperRight = new Vector2Int(1, 1);

        public enum PosType
        {
            None = 0x00,

            Up = 0x01,
            Down = 0x02,
            Left = 0x04,
            Right = 0x08,

            LeftUp = 0x10,
            LeftDown = 0x20,
            RightUp = 0x40,
            RightDown = 0x80,
        }

        [System.Serializable]
        public class SaveData
        {
            public Color deleteColor = Color.red;
            public Color invalidColor = Color.gray;
            public bool isCheckAll = false;
            public bool isCreating = false;
            public bool isDelete = false;
            public bool isHalf = false;
            public Vector2 offsetSize = new Vector2(0.5f, 0.5f);
            public string rootName = "MapRoot";
            public Color selectAroundColor = Color.blue;
            public Color selectColor = Color.white;
            public string selectTypeName = "";
            public float singleSize = 1f;
            public bool updateWithDeleteRepeat = false;

            [SerializeField]
            public List<string> tileResPaths = new List<string>();
        }

        #endregion 数据声明

        #region 设追

        protected SaveData _data;
        private TileResources _addTRMTemp;
        private Vector2 _scrollViewPos = Vector2.zero;
        private SortedDictionary<string, TileResources> _tileResDict = new SortedDictionary<string, TileResources>();

        #endregion 设追

        #region 数据

        private const string DataFlag = "XMLib_TileCreatorEditor";

        private Plane _drawPlane = new Plane(Vector3.back, Vector3.zero);
        private Camera _camera;
        private Vector2Int _endPos;
        private List<Vector2Int> _invalidPoses = new List<Vector2Int>();
        private bool _isDo = false;
        private bool _isDownAlpha2 = false;
        private bool _isDownAlpha3 = false;
        private bool _isDownAlpha4 = false;
        private bool _isDownDelete = false;
        private bool _isInWindow = false;
        private bool _isSelectLine = false;
        private bool _isSelectRange = false;
        private Vector2 _mousePosition;
        private List<Vector2Int> _reqairPoss = new List<Vector2Int>();
        private SceneView _sceneView;
        private List<Vector2Int> _selectAroundPoss = new List<Vector2Int>();
        private List<Vector2Int> _selectPoss = new List<Vector2Int>();
        private Vector2Int _startPos;

        /// <summary>
        /// 场景元素字典
        /// </summary>
        private Dictionary<string, Dictionary<int, Dictionary<int, GameObject>>> _tileDicts = new Dictionary<string, Dictionary<int, Dictionary<int, GameObject>>>();

        /// <summary>
        /// 根节点
        /// </summary>
        private Transform _root;

        /// <summary>
        /// 子节点
        /// </summary>
        private Dictionary<string, Transform> _childRootDict = new Dictionary<string, Transform>();

        #endregion 数据

        #region 基础

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            SettingView();
            ResourceView();
            HelpView();

            if (EditorGUI.EndChangeCheck())
            {
                SaveSetting();
            }

            this.Repaint();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorSceneManager.activeSceneChanged -= OnActiveSceneChanged;
            EditorSceneManager.newSceneCreated -= OnNewSceneCreated;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorSceneManager.activeSceneChanged += OnActiveSceneChanged;
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
            EditorSceneManager.sceneOpened += OnSceneOpened;

            LoadSetting();
            RefreshScene();
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            RefreshScene();
        }

        #endregion 基础

        #region 视图

        private void SettingView()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("设置");

            if (string.IsNullOrEmpty(_data.selectTypeName) || !ExistTileRes(_data.selectTypeName))
            {
                _data.isCreating = false;
            }

            _data.isCreating = EditorGUILayout.Toggle("启用创建", _data.isCreating);
            if (_data.isCreating)
            {
                _data.singleSize = EditorGUILayout.FloatField("单元格大小", _data.singleSize);
                _data.offsetSize = EditorGUILayout.Vector2Field("偏移量", _data.offsetSize);
                _data.rootName = EditorGUILayout.TextField("根节点名", _data.rootName);
                _data.selectColor = EditorGUILayout.ColorField("选择颜色", _data.selectColor);
                _data.selectAroundColor = EditorGUILayout.ColorField("影响颜色", _data.selectAroundColor);
                _data.deleteColor = EditorGUILayout.ColorField("删除颜色", _data.deleteColor);
                _data.invalidColor = EditorGUILayout.ColorField("无效颜色", _data.invalidColor);
                _data.isDelete = EditorGUILayout.Toggle("删除模式", _data.isDelete);
                _data.isHalf = EditorGUILayout.Toggle("半角填充模式", _data.isHalf);
                _data.isCheckAll = EditorGUILayout.Toggle("检查所有层", _data.isCheckAll);
                _data.updateWithDeleteRepeat = EditorGUILayout.Toggle("刷新时删除叠", _data.updateWithDeleteRepeat);
            }

            if (GUILayout.Button("刷新"))
            {
                RefreshScene();
            }

            GUILayout.EndVertical();
        }

        private void ResourceView()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("资源");

            GUILayout.BeginHorizontal();
            _addTRMTemp = (TileResources)EditorGUILayout.ObjectField("添加资源:", _addTRMTemp, typeof(TileResources), false);
            if (GUILayout.Button("添加", GUILayout.Width(50)))
            {
                AddRes(_addTRMTemp);
                _addTRMTemp = null;
            }
            GUILayout.EndHorizontal();

            _scrollViewPos = GUILayout.BeginScrollView(_scrollViewPos);

            int length = _tileResDict.Count;
            KeyValuePair<string, TileResources> tileResPair;
            TileResources tileRes;
            string typeName;

            GUILayout.BeginHorizontal();
            GUILayout.Label("快捷键", GUILayout.Width(50));
            GUILayout.Label("名字");
            GUILayout.Label("数量", GUILayout.Width(50));
            GUILayout.Space(220);
            GUILayout.EndHorizontal();

            for (int i = 0; i < length; i++)
            {
                tileResPair = _tileResDict.ElementAt(i);
                tileRes = tileResPair.Value;
                typeName = tileResPair.Key;

                if (_data.selectTypeName == typeName)
                {
                    GUILayout.BeginHorizontal(GUI.skin.GetStyle("SelectionRect"));
                    GUILayout.Space(10);
                }
                else
                {
                    GUILayout.BeginHorizontal(GUI.skin.GetStyle("IN Title"));
                }

                string quickKey = "";
                if (i < 5)
                {
                    quickKey = (i + 5).ToString();
                }
                GUILayout.Label(quickKey, GUILayout.Width(50));

                GUILayout.Label(tileRes.TypeName);

                Transform trans = GetChildRoot(tileRes.TypeName);
                string cnt = "null";
                if (null != trans)
                {
                    cnt = trans.childCount + "";
                }
                GUILayout.Label(cnt, GUILayout.Width(50));

                if (GUILayout.Button("选择", GUILayout.Width(50)))
                {
                    _data.selectTypeName = typeName;
                    _data.isCreating = true;
                }

                if (GUILayout.Button("更新", GUILayout.Width(50)))
                {
                    _data.selectTypeName = typeName;
                    UpdateType(typeName);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }

                if (GUILayout.Button("删除", GUILayout.Width(50)))
                {
                    RemoveRes(typeName);
                    --i;
                    --length;
                }

                if (GUILayout.Button("查找", GUILayout.Width(50)))
                {
                    Selection.activeObject = tileRes;
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private void HelpView()
        {
            string helpText = "最好在2D模式下使用\n" +
                "键位说明\n" +
                "~:删除模式\n" +
                "1:执行操作\n" +
                "2:按住线性选择，释放执行操作\n" +
                "3:按住矩形选择，释放执行操作\n" +
                "鼠标右键:取消选择\n";

            string statusText = "状态\n";
            if (_isSelectRange)
            {
                statusText += string.Format("{0}=>{1}\n", _startPos, _endPos);
            }
            else
            {
                statusText += string.Format("{0}\n", _endPos);
            }
            statusText += string.Format("选择点数:{0}\n选择周围点数:{1}\n无效点数:{2}\n", _selectPoss.Count, _selectAroundPoss.Count, _invalidPoses.Count);
            statusText += string.Format("是否多选:{0}", _isSelectRange);
            if (_isSelectRange)
            {
                statusText += string.Format("\n是否线性选择:{0}", _isSelectLine);
            }

            string msg = helpText + "\n" + statusText;

            GUIStyle helpStyle = new GUIStyle(GUI.skin.FindStyle("HelpBox"));
            helpStyle.fontSize = 14;
            GUILayout.Box(msg, helpStyle);
        }

        #endregion 视图

        #region 实例化操作

        /// <summary>
        /// 查找预制
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="posType"></param>
        /// <param name="isHalf"></param>
        /// <param name="notCorner"></param>
        /// <param name="objName"></param>
        /// <returns></returns>
        private GameObject FindPrefab(string typeName, PosType posType, bool isHalf, bool notCorner, out string objName)
        {
            TileResources tileRes = null;
            if (!_tileResDict.TryGetValue(typeName, out tileRes))
            {
                throw new Exception("没有该配置文件：" + typeName);
            }

            bool isRightUp = (posType & PosType.RightUp) != 0;
            bool isRightDown = (posType & PosType.RightDown) != 0;
            bool isLeftUp = (posType & PosType.LeftUp) != 0;
            bool isLeftDown = (posType & PosType.LeftDown) != 0;

            //去除角
            posType &= (~(PosType.LeftUp | PosType.LeftDown | PosType.RightUp | PosType.RightDown));

            GameObject obj = null;

            string tileName = "";

            switch (posType)
            {
                case PosType.None:
                    tileName = "T0";
                    break;

                case PosType.Right | PosType.Down:
                    if (isRightDown || notCorner)
                    {
                        tileName = isHalf ? "T1H" : "T1";
                    }
                    else
                    {
                        tileName = isHalf ? "T1CdH" : "T1Cd";
                    }
                    break;

                case PosType.Right | PosType.Down | PosType.Left:

                    if (isLeftDown && isRightDown || notCorner)
                    {
                        tileName = "T2";
                    }
                    else if (isLeftDown)
                    {
                        tileName = "T2Cd";
                    }
                    else if (isRightDown)
                    {
                        tileName = "T2Cl";
                    }
                    else
                    {
                        tileName = "T2Cdl";
                    }
                    break;

                case PosType.Down | PosType.Left:

                    if (isLeftDown || notCorner)
                    {
                        tileName = isHalf ? "T3H" : "T3";
                    }
                    else
                    {
                        tileName = isHalf ? "T3ClH" : "T3Cl";
                    }
                    break;

                case PosType.Up | PosType.Right | PosType.Down:

                    if (isRightDown && isRightUp || notCorner)
                    {
                        tileName = "T4";
                    }
                    else if (isRightUp)
                    {
                        tileName = "T4Cd";
                    }
                    else if (isRightDown)
                    {
                        tileName = "T4Cr";
                    }
                    else
                    {
                        tileName = "T4Cdr";
                    }
                    break;

                case PosType.Up | PosType.Right | PosType.Down | PosType.Left:
                    if (isLeftDown && isRightDown && isLeftUp && isRightUp || notCorner)
                    {
                        tileName = "T5";
                    }
                    else if (isLeftDown && isLeftUp && isRightUp)
                    {
                        tileName = "T5Cd";
                    }
                    else if (isRightDown && isLeftUp && isRightUp)
                    {
                        tileName = "T5Cl";
                    }
                    else if (isLeftDown && isRightDown && isLeftUp)
                    {
                        tileName = "T5Cr";
                    }
                    else if (isLeftDown && isRightDown && isRightUp)
                    {
                        tileName = "T5Cu";
                    }
                    else if (isLeftUp && isRightUp)
                    {
                        tileName = "T5Cdl";
                    }
                    else if (isLeftDown && isLeftUp)
                    {
                        tileName = "T5Cdr";
                    }
                    else if (isRightDown && isLeftUp)
                    {
                        tileName = "T5Clr";
                    }
                    else if (isLeftDown && isRightUp)
                    {
                        tileName = "T5Cud";
                    }
                    else if (isRightDown && isRightUp)
                    {
                        tileName = "T5Cul";
                    }
                    else if (isLeftDown && isRightDown)
                    {
                        tileName = "T5Cur";
                    }
                    else if (isLeftUp)
                    {
                        tileName = "T5Cdlr";
                    }
                    else if (isRightUp)
                    {
                        tileName = "T5Cudl";
                    }
                    else if (isLeftDown)
                    {
                        tileName = "T5Cudr";
                    }
                    else if (isRightDown)
                    {
                        tileName = "T5Culr";
                    }
                    else
                    {
                        tileName = "T5Cudlr";
                    }
                    break;

                case PosType.Up | PosType.Down | PosType.Left:

                    if (isLeftUp && isLeftDown || notCorner)
                    {
                        tileName = "T6";
                    }
                    else if (isLeftUp)
                    {
                        tileName = "T6Cl";
                    }
                    else if (isLeftDown)
                    {
                        tileName = "T6Cu";
                    }
                    else
                    {
                        tileName = "T6Cul";
                    }
                    break;

                case PosType.Up | PosType.Right:
                    if (isRightUp || notCorner)
                    {
                        tileName = isHalf ? "T7H" : "T7";
                    }
                    else
                    {
                        tileName = isHalf ? "T7CrH" : "T7Cr";
                    }
                    break;

                case PosType.Up | PosType.Right | PosType.Left:
                    if (isLeftUp && isRightUp || notCorner)
                    {
                        tileName = "T8";
                    }
                    else if (isLeftUp)
                    {
                        tileName = "T8Cr";
                    }
                    else if (isRightUp)
                    {
                        tileName = "T8Cu";
                    }
                    else
                    {
                        tileName = "T8Cur";
                    }
                    break;

                case PosType.Up | PosType.Left:
                    if (isLeftUp || notCorner)
                    {
                        tileName = isHalf ? "T9H" : "T9";
                    }
                    else
                    {
                        tileName = isHalf ? "T9CuH" : "T9Cu";
                    }
                    break;

                case PosType.Up:
                    tileName = "T11";
                    break;

                case PosType.Down:
                    tileName = "T10";
                    break;

                case PosType.Right:
                    tileName = "T12";
                    break;

                case PosType.Left:
                    tileName = "T13";
                    break;

                case PosType.Up | PosType.Down:
                    tileName = "T14";
                    break;

                case PosType.Left | PosType.Right:
                    tileName = "T15";
                    break;

                default:
                    throw new Exception(string.Format("没有找到该类型的预制=>{0:X}", posType));
            }

            obj = tileRes.Get(tileName);

            if (obj == null)
            {
                throw new Exception("未配置 " + tileName + " 类型预制。");
            }

            objName = obj.name;

            return obj;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="objName"></param>
        /// <param name="preObj"></param>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        private GameObject InstObj(string typeName, string objName, GameObject preObj, Vector2Int gridPos)
        {
            Transform root = GetChildRoot(typeName);
            if (null == root)
            {
                throw new Exception("不存在该子父节点:" + typeName);
            }

            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(preObj);
            /*
            GameObject obj = GameObject.Instantiate(preObj);
            obj = PrefabUtility.ConnectGameObjectToPrefab(obj, preObj);
            */

            obj.name = objName;
            obj.transform.parent = root;
            obj.transform.position = ToWorldPos(gridPos);

            TileResources tileRes = null;
            if (!_tileResDict.TryGetValue(typeName, out tileRes))
            {
                throw new Exception("没有该配置文件：" + typeName);
            }

            //处理生成的实例
            //

            return obj;
        }

        /// <summary>
        /// 实例化物体
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        /// <param name="isHalf"></param>
        /// <returns></returns>
        private GameObject InstPosObj(string typeName, Vector2Int pos, bool isHalf)
        {
            TileResources tileRes = null;
            if (!_tileResDict.TryGetValue(typeName, out tileRes))
            {
                throw new Exception("没有该配置文件：" + typeName);
            }

            PosType posType;

            if (tileRes.SimpleMode)
            {
                posType = PosType.None;
            }
            else
            {
                posType = CheckBorderPos(typeName, pos);
            }

            string objName = "";
            GameObject preObj = FindPrefab(typeName, posType, isHalf, !tileRes.CornerMode, out objName);
            return InstObj(typeName, objName, preObj, pos);
        }

        #endregion 实例化操作

        #region 基础操作

        /// <summary>
        /// 检查周边节点
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private PosType CheckBorderPos(string typeName, Vector2Int pos)
        {
            Vector2Int up = pos + PosUp;
            Vector2Int down = pos + PosDown;
            Vector2Int left = pos + PosLeft;
            Vector2Int right = pos + PosRight;
            Vector2Int upperLeft = pos + PosUpperLeft;
            Vector2Int lowerLeft = pos + PosLowerLeft;
            Vector2Int upperRight = pos + PosUpperRight;
            Vector2Int lowerRight = pos + PosLowerRight;

            PosType posType = PosType.None;

            if (CheckPos(typeName, up))
            {
                posType |= PosType.Up;
            }

            if (CheckPos(typeName, down))
            {
                posType |= PosType.Down;
            }

            if (CheckPos(typeName, left))
            {
                posType |= PosType.Left;
            }

            if (CheckPos(typeName, right))
            {
                posType |= PosType.Right;
            }

            if (CheckPos(typeName, upperLeft))
            {
                posType |= PosType.LeftUp;
            }

            if (CheckPos(typeName, lowerLeft))
            {
                posType |= PosType.LeftDown;
            }

            if (CheckPos(typeName, upperRight))
            {
                posType |= PosType.RightUp;
            }

            if (CheckPos(typeName, lowerRight))
            {
                posType |= PosType.RightDown;
            }

            return posType;
        }

        /// <summary>
        /// 检查节点
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool CheckPos(string typeName, Vector2Int pos)
        {
            bool bRet = false;
            if (_data.isCheckAll)
            {
                foreach (var item in _tileResDict)
                {
                    bRet = ExistTile(item.Value.TypeName, pos);
                    if (bRet)
                    {
                        break;
                    }
                }
            }
            else
            {
                bRet = ExistTile(typeName, pos);
            }

            return bRet;
        }

        private void CheckSelectPos(string typeName)
        {
            for (int i = 0; i < _selectPoss.Count; i++)
            {
                Vector2Int pos = _selectPoss[i];
                if (!_data.isDelete)
                {
                    if (CheckPos(typeName, pos))
                    {
                        _invalidPoses.Add(pos);
                        _selectPoss.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
                else
                {
                    if (!CheckPos(typeName, pos))
                    {
                        _invalidPoses.Add(pos);
                        _selectPoss.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
        }

        private void ClearCalcData()
        {
            _selectAroundPoss.Clear();
            _selectPoss.Clear();
            _reqairPoss.Clear();
            _invalidPoses.Clear();
        }

        private void DoOperation(string typeName)
        {
            if (!HasChildRoot(typeName))
            {
                if (_data.isDelete)
                {
                    return;
                }
                else
                {
                    CreateChildRoot(typeName);
                }
            }

            if (_data.isDelete)
            {
                DeleteOpt(typeName);
            }
            else
            {
                CreateOpt(typeName);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void DrawPosition()
        {
            Color oldColor = Handles.color;

            int length = 0;
            Vector2 worldPos;
            Vector3 size = Vector3.one * _data.singleSize * 0.9f;

            length = _selectAroundPoss.Count;
            Handles.color = _data.selectAroundColor;
            for (int i = 0; i < length; i++)
            {
                worldPos = ToWorldPos(_selectAroundPoss[i]);
                Handles.DrawWireCube(worldPos, size);
            }

            length = _selectPoss.Count;
            Handles.color = _data.isDelete ? _data.deleteColor : _data.selectColor;
            for (int i = 0; i < length; i++)
            {
                worldPos = ToWorldPos(_selectPoss[i]);
                Handles.DrawWireCube(worldPos, size);
            }

            length = _invalidPoses.Count;
            Handles.color = _data.invalidColor;
            for (int i = 0; i < length; i++)
            {
                worldPos = ToWorldPos(_invalidPoses[i]);
                Handles.DrawWireCube(worldPos, size);
            }

            Handles.color = oldColor;

            // 刷新界面，才能让球一直跟随
            HandleUtility.Repaint();
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            RefreshScene();
        }

        private void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            RefreshScene();
        }

        private void OnSceneGUI(SceneView sceneView)
        { //中文输入发可能无法触发事件
            try
            {
                UnityEngine.Event evt = UnityEngine.Event.current;
                _sceneView = sceneView;
                _mousePosition = evt.mousePosition;
                _camera = _sceneView.camera;
                _isDo = false;

                switch (evt.type)
                {
                    case EventType.MouseEnterWindow:
                        _isInWindow = true;
                        break;

                    case EventType.MouseLeaveWindow:
                        _isInWindow = false;
                        break;
                }

                if (!_isInWindow || !_data.isCreating)
                {
                    _isSelectRange = false;
                    _isSelectLine = false;
                    _isDownAlpha2 = false;
                    _isDownAlpha3 = false;
                    _isDownAlpha4 = false;
                    _isDownDelete = false;
                    ClearCalcData();
                    return;
                }

                switch (evt.type)
                {
                    case EventType.KeyDown:

                        if (!_isDownAlpha2 && (evt.keyCode == KeyCode.Alpha2))
                        {
                            _startPos = ScreenToGridPos(_camera, _mousePosition);
                            _isSelectRange = true;
                            _isSelectLine = true;

                            _isDownAlpha2 = true;
                        }
                        else if (!_isDownAlpha3 && (evt.keyCode == KeyCode.Alpha3))
                        {
                            _startPos = ScreenToGridPos(_camera, _mousePosition);
                            _isSelectRange = true;
                            _isSelectLine = false;

                            _isDownAlpha3 = true;
                        }
                        else if (!_isDownAlpha4 && (evt.keyCode == KeyCode.Alpha4))
                        {
                            _isDownAlpha4 = true;
                        }
                        else if (evt.keyCode == KeyCode.Alpha1)
                        {
                            _isSelectRange = false;
                            _isDo = true;
                        }
                        else if (!_isDownDelete && (evt.keyCode == KeyCode.BackQuote))
                        {
                            _data.isDelete = true;
                            _isDownDelete = true;
                        }
                        else if (evt.keyCode >= KeyCode.Alpha5 && evt.keyCode <= KeyCode.Alpha9)
                        {
                            int index = (int)(evt.keyCode) - 53;

                            if (_tileResDict.Count > index)
                            {
                                _data.selectTypeName = _tileResDict.ElementAt(index).Key;
                            }
                        }

                        break;

                    case EventType.KeyUp:

                        if (_isDownAlpha2 && (evt.keyCode == KeyCode.Alpha2) ||
                            _isDownAlpha3 && (evt.keyCode == KeyCode.Alpha3))
                        {
                            if (_isSelectRange)
                            {
                                _isDo = true;
                            }

                            _isDownAlpha2 = false;
                            _isDownAlpha3 = false;
                        }
                        else if (_isDownAlpha4 && (evt.keyCode == KeyCode.Alpha4))
                        {
                            _data.isHalf = !_data.isHalf;
                            _isDownAlpha4 = false;
                        }
                        else if (_isDownDelete && (evt.keyCode == KeyCode.BackQuote))
                        {
                            _data.isDelete = false;
                            _isDownDelete = false;
                        }
                        break;

                    case EventType.MouseDown:
                        if (_isSelectRange && (1 == evt.button))
                        {
                            _isSelectRange = false;
                        }
                        break;
                }

                _endPos = ScreenToGridPos(_camera, _mousePosition);

                string typeName = _data.selectTypeName;

                UpdatePos(typeName);

                DrawPosition();

                if (_isDo)
                {
                    DoOperation(typeName);
                    _isDo = false;
                    _isSelectRange = false;
                }

                sceneView.Repaint();
            }
            catch (Exception ex)
            {
                _isDo = false;
                _isDownAlpha2 = false;
                _isDownAlpha3 = false;
                _isDownAlpha4 = false;
                _isDownDelete = false;
                _isSelectLine = false;
                _isSelectRange = false;
                RefreshScene();
                throw ex;
            }
        }

        /// <summary>
        /// 修复节点
        /// </summary>
        /// <param name="typeName"></param>
        private void RepairAllPos(string typeName)
        {
            foreach (var pos in _reqairPoss)
            {
                RepairPos(typeName, pos);
            }
        }

        /// <summary>
        /// 修复节点
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        private void RepairPos(string typeName, Vector2Int pos)
        {
            GameObject existObj = GetTile(typeName, pos);
            if (null != existObj)
            {
                DestroyImmediate(existObj);
            }

            GameObject obj = InstPosObj(typeName, pos, _data.isHalf);
            SetTile(typeName, pos, obj);
        }

        private Vector2Int ScreenToGridPos(Camera cam, Vector2 mousePosition)
        {
            mousePosition.y = cam.pixelHeight - mousePosition.y;
            mousePosition = EditorGUIUtility.PixelsToPoints(mousePosition);
            var ray = cam.ScreenPointToRay(mousePosition);
            _drawPlane.Raycast(ray, out float enter);
            Vector3 worldPos = ray.origin + ray.direction * enter;
            return ToGirdPos(worldPos);
        }

        private Vector2Int ToGirdPos(Vector2 worldPos)
        {
            worldPos -= _data.offsetSize;

            float xd = (worldPos.x % _data.singleSize);
            float yd = (worldPos.y % _data.singleSize);
            worldPos.x -= xd;
            worldPos.y -= yd;

            if (Mathf.Abs(xd) > (_data.singleSize / 2))
            {
                worldPos.x += (_data.singleSize) * Mathf.Sign(xd);
            }
            if (Mathf.Abs(yd) > (_data.singleSize / 2))
            {
                worldPos.y += (_data.singleSize) * Mathf.Sign(yd);
            }

            worldPos /= _data.singleSize;

            return ToVtInt(worldPos);
        }

        private Vector2Int ToVtInt(Vector2 vt)
        {
            return new Vector2Int(Mathf.RoundToInt(vt.x), Mathf.RoundToInt(vt.y));
        }

        private Vector2 ToWorldPos(Vector2Int gridPos)
        {
            return new Vector2(gridPos.x, gridPos.y) * _data.singleSize + _data.offsetSize;
        }

        /// <summary>
        /// 更新坐标
        /// </summary>
        /// <param name="typeName"></param>
        private void UpdatePos(string typeName)
        {
            UpdateSelectPos(typeName);
            UpdateSelectAroundPos(typeName);
            UpdateReqairPos();
        }

        private void UpdateType(string typeName)
        {
            TileResources tileRes = null;
            if (!_tileResDict.TryGetValue(typeName, out tileRes))
            {
                throw new Exception("没有该配置文件：" + typeName);
            }

            Dictionary<int, Dictionary<int, GameObject>> xdict = GetTileDict(typeName);

            if (null != xdict)
            {
                foreach (var yDictPair in xdict)
                {
                    Dictionary<int, GameObject> yDict = yDictPair.Value;
                    List<int> yIndexs = yDict.Keys.ToList();

                    int length = yIndexs.Count;
                    for (int i = 0; i < length; i++)
                    {
                        int y = yIndexs[i];
                        GameObject obj = yDict[y];

                        GameObject objPrefab = tileRes.Get(obj.name);

                        if (objPrefab == null)
                        {
                            continue;
                        }

                        GameObject newObj = Instantiate(objPrefab);

                        //Copy
                        newObj.name = obj.name;
                        newObj.transform.parent = obj.transform.parent;
                        newObj.transform.position = obj.transform.position;
                        newObj.transform.rotation = obj.transform.rotation;
                        newObj.transform.localScale = obj.transform.localScale;
                        newObj.layer = obj.layer;
                        newObj.tag = obj.tag;
                        //

                        yDict[y] = newObj;

                        DestroyImmediate(obj);
                    }
                }
            }
        }

        private void UpdateReqairPos()
        {
            _reqairPoss.Clear();

            _reqairPoss.AddRange(_selectAroundPoss);

            if (_data.isDelete)
            {
                _reqairPoss = _reqairPoss.Except(_selectPoss).ToList();
            }
            else
            {
                _reqairPoss.AddRange(_selectPoss);
            }

            _reqairPoss = _reqairPoss.Distinct().ToList();
        }

        private void UpdateSelectAroundPos(string typeName)
        {
            _selectAroundPoss.Clear();

            Vector2Int up;
            Vector2Int down;
            Vector2Int left;
            Vector2Int right;
            Vector2Int upperLeft;
            Vector2Int lowerLeft;
            Vector2Int upperRight;
            Vector2Int lowerRight;

            foreach (var pos in _selectPoss)
            {
                up = pos + PosUp;
                down = pos + PosDown;
                left = pos + PosLeft;
                right = pos + PosRight;
                upperLeft = pos + PosUpperLeft;
                lowerLeft = pos + PosLowerLeft;
                upperRight = pos + PosUpperRight;
                lowerRight = pos + PosLowerRight;

                if (ExistTile(typeName, up))
                {
                    _selectAroundPoss.Add(up);
                }
                if (ExistTile(typeName, down))
                {
                    _selectAroundPoss.Add(down);
                }
                if (ExistTile(typeName, left))
                {
                    _selectAroundPoss.Add(left);
                }
                if (ExistTile(typeName, right))
                {
                    _selectAroundPoss.Add(right);
                }
                if (ExistTile(typeName, upperLeft))
                {
                    _selectAroundPoss.Add(upperLeft);
                }
                if (ExistTile(typeName, lowerLeft))
                {
                    _selectAroundPoss.Add(lowerLeft);
                }
                if (ExistTile(typeName, upperRight))
                {
                    _selectAroundPoss.Add(upperRight);
                }
                if (ExistTile(typeName, lowerRight))
                {
                    _selectAroundPoss.Add(lowerRight);
                }
            }

            _invalidPoses = _invalidPoses.Except(_selectAroundPoss).ToList();
        }

        private void UpdateSelectPos(string typeName)
        {
            _invalidPoses.Clear();
            _selectPoss.Clear();

            if (!_isSelectRange || (_endPos == _startPos))
            {
                _selectPoss.Add(_endPos);
            }
            else
            {
                Vector2Int tmpPos;
                Vector2Int distance = _endPos - _startPos;

                Vector2Int sign = new Vector2Int((int)Mathf.Sign(distance.x), (int)Mathf.Sign(distance.y));
                Vector2Int size = new Vector2Int(Mathf.Abs(distance.x), Mathf.Abs(distance.y));

                if (_isSelectLine)
                {
                    int maxCnt;
                    float xDelta;
                    float yDelta;

                    if (size.x > size.y)
                    {
                        maxCnt = size.x;
                        xDelta = sign.x;
                        yDelta = distance.y == 0 ? 0 : distance.y / (float)maxCnt;
                    }
                    else
                    {
                        maxCnt = size.y;
                        xDelta = distance.x == 0 ? 0 : distance.x / (float)maxCnt;
                        yDelta = sign.y;
                    }

                    for (int i = 0; i < maxCnt + 1; i++)
                    {
                        tmpPos = _startPos;
                        tmpPos.x += Mathf.RoundToInt(i * xDelta);
                        tmpPos.y += Mathf.RoundToInt(i * yDelta);
                        _selectPoss.Add(tmpPos);
                    }
                }
                else
                {
                    int xDelta = 0;
                    for (int x = 0; x < size.x + 1; x++)
                    {
                        xDelta = (x * sign.x);

                        for (int y = 0; y < size.y + 1; y++)
                        {
                            tmpPos = _startPos;

                            tmpPos.x += xDelta;
                            tmpPos.y += (y * sign.y);

                            _selectPoss.Add(tmpPos);
                        }
                    }
                }
            }

            CheckSelectPos(typeName);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        public void DestroyWithPos(string typeName, Vector2Int pos)
        {
            GameObject obj = RemoveTile(typeName, pos);
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        #region 操作

        /// <summary>
        /// 创建操作
        /// </summary>
        /// <param name="typeName"></param>
        private void CreateOpt(string typeName)
        {
            foreach (var pos in _selectPoss)
            {
                AddToDict(typeName, pos, null);
            }

            RepairAllPos(typeName);
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="typeName"></param>
        private void DeleteOpt(string typeName)
        {
            foreach (var pos in _selectPoss)
            {
                DestroyWithPos(typeName, pos);
            }

            RepairAllPos(typeName);
        }

        #endregion 操作

        #region 资源操作

        /// <summary>
        /// 存在资源
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public bool ExistTileRes(string typeName)
        {
            return _tileResDict.ContainsKey(typeName);
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="tileRes"></param>
        private void AddRes(TileResources tileRes)
        {
            if (null == tileRes)
            {
                throw new Exception("添加资源不能为null");
            }

            string typeName = tileRes.TypeName;
            List<TileResources> tiles = _tileResDict.Values.ToList();
            if (tiles.Exists(t => t.TypeName == tileRes.TypeName))
            {
                throw new Exception("存在该类型名的资源:" + typeName);
            }

            _tileResDict.Add(typeName, tileRes);

            string assetPath = AssetDatabase.GetAssetPath(tileRes);
            _data.tileResPaths.Add(assetPath);
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        /// <param name="typeName"></param>
        private void RemoveRes(string typeName)
        {
            TileResources tileRes = null;
            if (!_tileResDict.TryGetValue(typeName, out tileRes))
            {
                throw new Exception("没有该配置文件：" + typeName);
            }

            string assetPath = AssetDatabase.GetAssetPath(tileRes);

            _tileResDict.Remove(typeName);
            _data.tileResPaths.Remove(assetPath);
        }

        /// <summary>
        /// 更新资源列表
        /// </summary>
        private void LoadTileRes()
        {
            _tileResDict.Clear();
            int length = _data.tileResPaths.Count;
            string tileResPath;
            TileResources tileRes;
            for (int i = 0; i < length; i++)
            {
                tileResPath = _data.tileResPaths[i];
                tileRes = AssetDatabase.LoadAssetAtPath<TileResources>(tileResPath);

                if (null == tileRes)
                {
                    Debug.LogWarningFormat("未找到地图资源配置:{0}", tileResPath);
                    _data.tileResPaths.RemoveAt(i);
                    i--;
                    length--;
                    continue;
                }
                _tileResDict.Add(tileRes.TypeName, tileRes);
            }
        }

        #endregion 资源操作

        #region 设置读取保存

        /// <summary>
        /// 保存设置
        /// </summary>
        private void SaveSetting()
        {
            //特定场景数据

            //
            string jsonData = EditorJsonUtility.ToJson(_data);
            EditorPrefs.SetString(DataFlag, jsonData);
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        private void LoadSetting()
        {
            _data = new SaveData();
            string jsonData = "";

            if (EditorPrefs.HasKey(DataFlag))
            {
                jsonData = EditorPrefs.GetString(DataFlag);
                EditorJsonUtility.FromJsonOverwrite(jsonData, _data);
            }

            if (string.IsNullOrEmpty(jsonData) || null == _data)
            {
                _data = new SaveData();
                jsonData = EditorJsonUtility.ToJson(_data);
                EditorPrefs.SetString(DataFlag, jsonData);
            }

            //特定场景数据

            //

            //

            //

            LoadTileRes();
        }

        #endregion 设置读取保存

        #region 节点

        /// <summary>
        /// 获取指定根节点
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Transform GetChildRoot(string typeName)
        {
            Transform root = null;

            if (_childRootDict.TryGetValue(typeName, out root)) { }

            return root;
        }

        /// <summary>
        /// 初始化根节点
        /// </summary>
        /// <param name="rootObj"></param>
        private void InitRoot(Transform root)
        {
            ////
            Transform customRoot = new GameObject("Customs").transform;
            customRoot.parent = root;

            ////
            //Transform enemyRoot = new GameObject("Enemys").transform; enemyRoot.parent = customRoot;

            ////
            //Transform pickUpRoot = new GameObject("PickUps").transform; pickUpRoot.parent = customRoot;

            ////
            //Transform decorateRoot = new GameObject("Decorates").transform; decorateRoot.parent = customRoot;

            ////buff
            //Transform buffRoot = new GameObject("Buffs").transform; buffRoot.parent = customRoot;
            //Transform boomRoot = new GameObject("Booms").transform; boomRoot.parent = buffRoot;
            //Transform dirWallRoot = new GameObject("DirWalls").transform; dirWallRoot.parent = buffRoot;
            //Transform breakWallRoot = new GameObject("BreakWalls").transform; breakWallRoot.parent = buffRoot;
            //Transform stickWallRoot = new GameObject("StickWalls").transform; stickWallRoot.parent = buffRoot;
            //Transform RigidCubeRoot = new GameObject("RigidCubes").transform; RigidCubeRoot.parent = buffRoot;

            ////trap
            //Transform trapRoot = new GameObject("Traps").transform; trapRoot.parent = customRoot;
            //Transform luckerRoot = new GameObject("Luckers").transform; luckerRoot.parent = trapRoot;
            //Transform sawRoot = new GameObject("Saws").transform; sawRoot.parent = trapRoot;
            //Transform rollerRoot = new GameObject("Rollers").transform; rollerRoot.parent = trapRoot;
            //Transform lazerRoot = new GameObject("Lazers").transform; lazerRoot.parent = trapRoot;
            //Transform lanucherRoot = new GameObject("Lanucher").transform; lanucherRoot.parent = trapRoot;
        }

        private bool HasRoot()
        {
            return _root != null;
        }

        private bool HasChildRoot(string typeName)
        {
            return _childRootDict.ContainsKey(typeName);
        }

        private void CreateRoot()
        {
            _root = new GameObject(_data.rootName).transform;
            InitRoot(_root);
        }

        private void CreateChildRoot(string typeName)
        {
            if (HasChildRoot(typeName))
            {
                throw new Exception("已存在该节点：" + typeName);
            }

            if (!HasRoot())
            {
                CreateRoot();
            }

            TileResources tileRes = null;
            if (!_tileResDict.TryGetValue(typeName, out tileRes))
            {
                throw new Exception("没有该类型配置文件：" + typeName);
            }

            GameObject childRootObj = new GameObject();
            Transform childRoot = childRootObj.transform;

            //
            childRootObj.name = tileRes.TypeName;
            childRootObj.layer = tileRes.RootLayer;
            childRoot.parent = _root;

            if (tileRes.CompositeCollider2D)
            {
                Rigidbody2D rigid2d = childRootObj.AddComponent<Rigidbody2D>();
                rigid2d.bodyType = RigidbodyType2D.Static;

                CompositeCollider2D composite = childRootObj.AddComponent<CompositeCollider2D>();
                composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
            }

            _childRootDict.Add(typeName, childRoot);
        }

        private void UpdateChildRoot(string typeName)
        {
            Transform childRoot = null;
            if (!_childRootDict.TryGetValue(typeName, out childRoot))
            {
                Debug.LogWarning("不存在该节点：" + typeName);
                return;
            }

            Dictionary<int, Dictionary<int, GameObject>> xDict = GetTileDict(typeName);
            if (null == xDict)
            {
                xDict = new Dictionary<int, Dictionary<int, GameObject>>();
                _tileDicts.Add(typeName, xDict);
            }

            //更新子节点tile
            List<GameObject> repeatObjs = new List<GameObject>();
            Dictionary<int, GameObject> yDict;
            GameObject girdObj = null;
            Vector2Int girdPos;

            foreach (Transform child in childRoot)
            {
                girdPos = ToGirdPos(child.position);
                girdObj = child.gameObject;

                if (!xDict.TryGetValue(girdPos.x, out yDict))
                { //没有则添加x
                    yDict = new Dictionary<int, GameObject>();
                    xDict.Add(girdPos.x, yDict);
                }

                if (yDict.ContainsKey(girdPos.y))
                {
                    if (yDict[girdPos.y] != null)
                    {
                        if (_data.updateWithDeleteRepeat)
                        { //添加到重复列表
                            repeatObjs.Add(girdObj);
                            continue;
                        }
                        else
                        {
                            throw new Exception(string.Format("[{1}]在 {0} 处有重叠的物体", girdPos, SceneManager.GetActiveScene().name));
                        }
                    }
                }

                yDict.Add(girdPos.y, girdObj);
            }

            int length = repeatObjs.Count;
            for (int i = 0; i < length; i++)
            {
                DestroyImmediate(repeatObjs[i]);
            }
        }

        private void RefreshScene()
        {
            ClearCalcData();

            _tileDicts.Clear();
            _childRootDict.Clear();
            _root = null;

            GameObject rootObj = EditorSceneManager.GetActiveScene().GetRootGameObjects().Where((t) => t.name == _data.rootName).FirstOrDefault();
            if (null == rootObj)
            {
                return;
            }

            _root = rootObj.transform;

            foreach (Transform childRoot in _root)
            {
                string typeName = childRoot.name;
                if (typeName == "Customs")
                {
                    continue;
                }

                _childRootDict.Add(typeName, childRoot);
                UpdateChildRoot(typeName);
            }
        }

        #endregion 节点

        #endregion 基础操作

        #region 元素字典操作

        /// <summary>
        /// 添加到字典
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        /// <param name="obj"></param>
        public void AddToDict(string typeName, Vector2Int pos, GameObject obj)
        {
            Dictionary<int, Dictionary<int, GameObject>> xDict = null;
            Dictionary<int, GameObject> yDict = null;

            if (!_tileDicts.TryGetValue(typeName, out xDict))
            {
                xDict = new Dictionary<int, Dictionary<int, GameObject>>();
                _tileDicts.Add(typeName, xDict);
            }

            if (!xDict.TryGetValue(pos.x, out yDict))
            {
                yDict = new Dictionary<int, GameObject>();
                xDict.Add(pos.x, yDict);
            }

            yDict.Add(pos.y, obj);
        }

        /// <summary>
        /// 存在对象
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool ExistTile(string typeName, Vector2Int pos)
        {
            Dictionary<int, Dictionary<int, GameObject>> xDict = null;
            Dictionary<int, GameObject> yDict = null;

            if (!_tileDicts.TryGetValue(typeName, out xDict))
            {
                return false;
            }

            if (!xDict.TryGetValue(pos.x, out yDict))
            {
                return false;
            }

            GameObject obj = null;

            if (!yDict.TryGetValue(pos.y, out obj))
            {
                return false;
            }

            return true;
        }

        public Dictionary<int, Dictionary<int, GameObject>> GetTileDict(string typeName)
        {
            Dictionary<int, Dictionary<int, GameObject>> dicts = null;

            if (!_tileDicts.TryGetValue(typeName, out dicts)) { }

            return dicts;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public GameObject GetTile(string typeName, Vector2Int pos)
        {
            return _tileDicts[typeName][pos.x][pos.y];
        }

        /// <summary>
        /// 从字典移除
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        /// <returns>返回移除的对象</returns>
        public GameObject RemoveTile(string typeName, Vector2Int pos)
        {
            Dictionary<int, Dictionary<int, GameObject>> xDict = null;
            Dictionary<int, GameObject> yDict = null;

            if (!_tileDicts.TryGetValue(typeName, out xDict))
            {
                return null;
            }

            if (!xDict.TryGetValue(pos.x, out yDict))
            {
                return null;
            }

            GameObject obj = null;

            if (yDict.TryGetValue(pos.y, out obj))
            {
                yDict.Remove(pos.y);
            }

            if (0 == yDict.Count)
            {
                xDict.Remove(pos.x);
            }

            if (0 == xDict.Count)
            {
                _tileDicts.Remove(typeName);
            }

            return obj;
        }

        /// <summary>
        /// 设置字典
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pos"></param>
        /// <param name="obj"></param>
        public void SetTile(string typeName, Vector2Int pos, GameObject obj)
        {
            Dictionary<int, Dictionary<int, GameObject>> xDict = null;
            Dictionary<int, GameObject> yDict = null;

            if (!_tileDicts.TryGetValue(typeName, out xDict))
            {
                throw new Exception(string.Format("{0} 没有初始化.", pos));
            }

            if (!xDict.TryGetValue(pos.x, out yDict))
            {
                throw new Exception(string.Format("{0} 没有初始化.", pos));
            }

            yDict[pos.y] = obj;
        }

        #endregion 元素字典操作
    }
}