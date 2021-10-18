using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 给已经生成的脚本添加和修改命名空间
/// </summary>
namespace FrameWork
{
    public class ChangeNameSpace : ScriptableWizard
    {

        //ScriptableWizard类的公有变量都会显示在窗口面板上
        public string folder = "Assets/";
        public string namespaceName;

        static float windowWidth = 500f;
        static float windowHeight = 500f;


        void OnEnable()
        {
            if (Selection.activeObject != null)
            {
                string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
                if (File.Exists(dirPath))
                {
                    dirPath = dirPath.Substring(0, dirPath.LastIndexOf("/"));
                }
                folder = dirPath;
            }
        }

        [MenuItem("Framework/ChangeNameSpace", false, 10)]
        static void CreateWizard()
        {
            ChangeNameSpace editor = ScriptableWizard.DisplayWizard<ChangeNameSpace>("Add Namespace", "添加命名空间", "取消");
            editor.minSize = new Vector2(windowWidth, windowHeight);
        }

        /// <summary>
        /// 点击取消按钮时调用
        /// </summary>
        private void OnWizardOtherButton()
        {
            Debug.Log("取消");
        }

        /// <summary>
        /// 点击确定按钮时调用，这里的确定按钮就是"添加命名空间"
        /// </summary>
        public void OnWizardCreate()
        {
            //save settting

            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(namespaceName))
            {

                List<string> filesPaths = new List<string>();
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.cs", SearchOption.AllDirectories)
                );
                Dictionary<string, bool> scripts = new Dictionary<string, bool>();

                int counter = -1;
                foreach (string filePath in filesPaths)
                {

                    scripts[filePath] = true;

                    EditorUtility.DisplayProgressBar("Add Namespace", filePath, counter / (float)filesPaths.Count);
                    counter++;

                    string contents = File.ReadAllText(filePath);

                    string result = "";
                    bool havsNS = contents.Contains("namespace ");
                    string t = havsNS ? "" : "\t";

                    using (TextReader reader = new StringReader(contents))
                    {
                        int index = 0;
                        bool addedNS = false;
                        while (reader.Peek() != -1)
                        {
                            string line = reader.ReadLine();

                            if (line.IndexOf("using") > -1 || line.Contains("#"))
                            {
                                result += line + "\n";
                            }
                            else if (!addedNS && !havsNS)
                            {
                                result += "\nnamespace " + namespaceName + " {";
                                addedNS = true;
                                result += t + line + "\n";
                            }
                            else
                            {
                                if (havsNS && line.Contains("namespace "))
                                {
                                    if (line.Contains("{"))
                                    {
                                        result += "namespace " + namespaceName + " {\n";
                                    }
                                    else
                                    {
                                        result += "namespace " + namespaceName + "\n";
                                    }
                                }
                                else
                                {
                                    result += t + line + "\n";
                                }
                            }
                            ++index;
                        }
                        reader.Close();
                    }
                    if (!havsNS)
                    {
                        result += "}";
                    }
                    File.WriteAllText(filePath, result);
                }

                //处理加了命名空间后出现方法miss
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.unnity", SearchOption.AllDirectories)
                );
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.prefab", SearchOption.AllDirectories)
                );


                counter = -1;
                foreach (string filePath in filesPaths)
                {
                    EditorUtility.DisplayProgressBar("Modify Script Ref", filePath, counter / (float)filesPaths.Count);
                    counter++;

                    string contents = File.ReadAllText(filePath);

                    string result = "";
                    using (TextReader reader = new StringReader(contents))
                    {
                        int index = 0;
                        while (reader.Peek() != -1)
                        {
                            string line = reader.ReadLine();

                            if (line.IndexOf("m_ObjectArgumentAssemblyTypeName:") > -1 && !line.Contains(namespaceName))
                            {

                                string scriptName = line.Split(':')[1].Split(',')[0].Trim();
                                if (scripts.ContainsKey(scriptName))
                                {
                                    line = line.Replace(scriptName, "namespaceName." + scriptName);
                                }

                                result += line + "\n";
                            }
                            else
                            {
                                result += line + "\n";
                            }
                            ++index;
                        }
                        reader.Close();
                    }

                    File.WriteAllText(filePath, result);
                }


                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }
    }
}