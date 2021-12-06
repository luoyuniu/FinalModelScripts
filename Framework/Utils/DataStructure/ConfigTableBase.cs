using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Framework
{
    //表单数据基类
    public class TableDataBase
    {
        public int ID;
    }

    /// <summary>
    /// 配置文件表单基类
    /// </summary>
    /// <typeparam name="TDataBase">表单数据基类</typeparam>
    /// <typeparam name="T">泛型</typeparam>
    public class ConfigTableBase<TDataBase, T> : Singleton<T>
        where TDataBase : TableDataBase, new()
        where T : Singleton<T>
    {
        //表单缓存表
        Dictionary<int, TDataBase> _cache = new Dictionary<int, TDataBase>();

        protected ConfigTableBase()
        {
            Load("/../Config/" + GetType().ToString() + ".csv");
        }

        //表单加载
        private void Load(string tablePath)
        {
            //数据流
            MemoryStream tableStream;

#if UNITY_EDITOR
            tableStream = new MemoryStream(File.ReadAllBytes(Application.dataPath + tablePath));
#else
        TextAsset table = ResourcesMgr.Instance.Load<TextAsset>("Config/" + tablePath + ".csv");
        tableStream = new MemoryStream(table.bytes);
#endif
            if (tableStream == null)
            {
                Debug.LogError("找不到文件");
                return;
            }

            using (StreamReader reader = new StreamReader(tableStream, Encoding.GetEncoding("gb2312")))
            {
                //跳过第一行的翻译
                reader.ReadLine();
                //首行读取及切分
                string fieldNameStr = reader.ReadLine();
                string[] fieldNameArray = fieldNameStr.Split(',');

                List<FieldInfo> allFieldInfo = new List<FieldInfo>();

                //反射
                foreach (var fieldName in fieldNameArray)
                {
                    var fieldType = typeof(TDataBase).GetField(fieldName);
                    if (fieldType == null)
                    {
                        Debug.LogError("表中字段未在程序中定义：" + fieldName);
                        continue;
                    }
                    Debug.Log(fieldType);
                    allFieldInfo.Add(fieldType);
                }

                string lineStr = reader.ReadLine();
                Debug.Log(lineStr);

                while (lineStr != null)
                {
                    TDataBase dataBase = ReadLine(allFieldInfo, lineStr);
                    _cache[dataBase.ID] = dataBase;

                    lineStr = reader.ReadLine();
                    Debug.Log(lineStr);
                }
            }
        }

        //读表器
        private static TDataBase ReadLine(List<FieldInfo> allFieldInfo, string lineStr)
        {
            string[] itemStrArray = lineStr.Split(',');
            TDataBase tableDB = new TDataBase();

            for (int i = 0; i < allFieldInfo.Count; ++i)
            {
                var field = allFieldInfo[i];
                var data = itemStrArray[i];

                //字符串解析
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(tableDB, data);
                }
                //整型解析
                else if (field.FieldType == typeof(int))
                {
                    int fieldValue;

                    if (!int.TryParse(data, out fieldValue))
                    {
                        check(field);
                        fieldValue = 0;
                    }

                    field.SetValue(tableDB, fieldValue);
                }
                //浮点型
                else if (field.FieldType == typeof(float))
                {
                    float fieldValue;

                    if (!float.TryParse(data, out fieldValue))
                    {
                        check(field);
                        fieldValue = 0;
                    }

                    field.SetValue(tableDB, fieldValue);
                }
                //布尔型
                else if (field.FieldType == typeof(bool))
                {
                    ///根据需求修改

                    //表单内容为 ture or false
                    bool fieldValue;

                    if (!bool.TryParse(data, out fieldValue))
                    {
                        check(field);
                        fieldValue = false;
                    }

                    field.SetValue(tableDB, fieldValue);

                    //表单内容为 1 or 0
                    //int fieldValue;
                    //if (!int.TryParse(data, out fieldValue))
                    //{
                    //    Debug.LogError(field.FieldType + "数据类型转换失败!!!");
                    //    fieldValue = 0;
                    //}
                    //
                    //field.SetValue(tableDB, fieldValue != 0);
                }
                //字符串列表
                else if (field.FieldType == typeof(List<string>))
                {
                    field.SetValue(tableDB, new List<string>(data.Split('$')));
                }
                //整型列表
                else if (field.FieldType == typeof(List<int>))
                {
                    var list = new List<int>();
                    foreach (var item in data.Split('$'))
                    {
                        int fieldValue;

                        if (!int.TryParse(item, out fieldValue))
                        {
                            check(field);
                            fieldValue = 0;
                        }

                        list.Add(fieldValue);
                    }

                    field.SetValue(tableDB, list);
                }
                //浮点列表
                else if (field.FieldType == typeof(List<float>))
                {
                    var list = new List<float>();
                    foreach (var item in data.Split('$'))
                    {
                        float fieldValue;

                        if (!float.TryParse(item, out fieldValue))
                        {
                            check(field);
                            fieldValue = 0;
                        }

                        list.Add(fieldValue);
                    }

                    field.SetValue(tableDB, list);
                }
                //其他自定义类型
                else if (field.FieldType == typeof(Type))
                {
                    check(field);
                    Type type = Type.GetType(data);

                    field.SetValue(tableDB, type);
                }
            }
            return tableDB;
        }

        private static void check(FieldInfo field)
        {
            Debug.LogError(field.FieldType + "数据类型转换失败!!!");
        }

        //扩展按下标读取数据
        public TDataBase this[int index]
        {
            get
            {
                TDataBase db;
                _cache.TryGetValue(index, out db);
                return db;
            }
        }

        //返回表单所有数据
        public Dictionary<int, TDataBase> GetAll()
        {
            return _cache;
        }
    }

}
