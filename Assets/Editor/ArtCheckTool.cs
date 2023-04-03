// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEditor;
// using UnityEditor.EditorTools;
// using UnityEditor.SceneManagement;
// using UnityEngine.Rendering;
//
// // By passing `typeof(MeshFilter)` as the second argument, we register VertexTool as a CustomEditor tool to be presented
// // when the current selection contains a MeshFilter component.
//
// // 防止误操作或切后台等因素引起的重刷新，新增一个记录场景状态的功能，Reset恢复到场景最原始状态
// [EditorTool("ArtCheckTool")]
// class ArtCheckTool : EditorTool
// {
//
//     GUIContent m_ToolbarIcon;
//     private MeshFilter[] meshFilters;
//     private SkinnedMeshRenderer[] _skinnedMeshRenderers;
//     
//     private List<MeshFilter> effectiveMeshFiltersList = new List<MeshFilter>();
//     private List<SkinnedMeshRenderer> effectivesSkinnedMeshRenderersList = new List<SkinnedMeshRenderer>();
//
//
//     private List<Material[]> originalMaterialsList = new List<Material[]>();
//     private List<Material[]> originalSkinMaterialsList = new List<Material[]>();
//     
//     private List<Material[]> materialsList = new List<Material[]>();
//     private List<Material[]> skinMaterialsList = new List<Material[]>();
//     
//     private Material[] curMaterials;
//     private Material[] curSkinMaterials;
//
//     private Material errorRed ;
//     private Material standardGreen ;
//     private Material warningYellow ;
//
//     private List<int> tempLevel = new List<int>();
//     
//     //标准分辨率，以1平方米128*128像素为准
//     private int FILL_RESOLUTION = 128 * 128;
//     public float FILL_SCALE = 1f;
//
//     //区间 x < 2; 2 < x < 4; x > 4
//     private float limitRatio_1 = 2f;
//     private float limitRatio_2 = 4f;
//     
//     float sliderValue = 1.0f;
//     float maxSliderValue = 1.0f;
//     
//     private bool isRunning = false;
//     
//     public override GUIContent toolbarIcon
//     {
//         get
//         {
//             if (m_ToolbarIcon == null)
//                 m_ToolbarIcon = new GUIContent(
//                     AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/textureCheckIcon.png"),
//                     "Textures Check Tool");
//             return m_ToolbarIcon;
//         }
//     }
//     
//     void OnEnable()
//     {
//         // ToolManager.activeToolChanged += ActiveToolDidChange;
//     }
//     
//     void OnDisable()
//     {
//         // ToolManager.activeToolChanged -= ActiveToolDidChange;
//     }
//     // static void ChangedPlaymodeState(PlayModeStateChange c)
//     // {
//     //     if (c == PlayModeStateChange.EnteredPlayMode)
//     //     {
//     //         Debug.Log(2);
//     //     }
//     // }
//
//     public override void OnActivated()
//     {
//         // EditorApplication.playModeStateChanged += ChangedPlaymodeState;
//      
//         if (isRunning)
//         {
//             return;
//         }
//
//         isRunning = true;
//         // Debug.Log(this.GetInstanceID());
//         //TODO:从Editor切到RunTime后，这份内存都被清空了，除非存到playprefs
//         Debug.Log("originalMaterialsList"+originalMaterialsList.Count);
//         MeshFilterRecover();
//         SkinnedMeshRendererRecover();
//         
//         Debug.Log("ON");
//         if (Application.isPlaying)
//         {
//             Debug.Log("RunTime");
//             
//         }
//         else
//         {
//             Debug.Log("Editor");
//         }
//         InitMaterials();
//
//         MeshFilterDeal();
//         SkinnedMeshRendererDeal();
//     }
//
//     //TODO:异常退出时，丢失各种list的特殊处理
//     public override void OnWillBeDeactivated()
//     {
//         Debug.Log("OFF");
//        
//         MeshFilterRecover();
//         SkinnedMeshRendererRecover();
//         isRunning = false;
//     }
//
//     void InitMaterials()
//     {
//       
//         standardGreen =
//             AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/ArtTextureCheckMat/standardGreen.mat");
//         warningYellow =
//             AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/ArtTextureCheckMat/warningYellow.mat");
//         errorRed = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/ArtTextureCheckMat/errorRed.mat");
//         
//         //动态生成材质，并设置shader属性
//         // CreateMaterials(standardGreen, new Color(0, 1, 0, 0.32f));
//         // CreateMaterials(warningYellow, new Color(1, 1, 0, 0.58f));
//         // CreateMaterials(errorRed, new Color(1, 0, 0, 0.78f));
//        if (standardGreen == null)
//         {
//             standardGreen = new Material(Shader.Find("Universal Render Pipeline/Lit"));
//             standardGreen.SetFloat("_Surface", 1.0f);
//             standardGreen.SetColor("_BaseColor", new Color(0, 1, 0, 0.32f));
//             standardGreen.SetFloat("_Smoothness", 0.0f);
//             standardGreen.SetFloat("_Cull", 0.0f);
//             standardGreen.SetFloat("_ReceiveShadows",0.0f);
//             standardGreen.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
//             standardGreen.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//             standardGreen.SetInt("_ZWrite", 0);
//             standardGreen.DisableKeyword("_ALPHATEST_ON");
//             standardGreen.DisableKeyword("_ALPHABLEND_ON");
//             standardGreen.EnableKeyword("_ALPHAPREMULTIPLY_ON");
//             standardGreen.EnableKeyword("_RECEIVE_SHADOWS_OFF");
//           
//             standardGreen.renderQueue = 3000; 
//         }
//         if (warningYellow == null)
//         {
//             warningYellow = new Material(Shader.Find("Universal Render Pipeline/Lit"));
//             warningYellow.SetFloat("_Surface", 1.0f);
//             warningYellow.SetColor("_BaseColor",  new Color(1, 1, 0, 0.58f));
//             warningYellow.SetFloat("_Smoothness", 0.0f);
//             warningYellow.SetFloat("_ReceiveShadows",0.0f);
//             warningYellow.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
//             warningYellow.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//             warningYellow.SetInt("_ZWrite", 0);
//             warningYellow.DisableKeyword("_ALPHATEST_ON");
//             warningYellow.DisableKeyword("_ALPHABLEND_ON");
//             warningYellow.EnableKeyword("_ALPHAPREMULTIPLY_ON");
//             warningYellow.EnableKeyword("_RECEIVE_SHADOWS_OFF");
//             warningYellow.renderQueue = 3000; 
//         }
//         if (errorRed == null)
//         {
//             errorRed = new Material(Shader.Find("Universal Render Pipeline/Lit"));
//             errorRed.SetFloat("_Surface", 1.0f);
//             errorRed.SetColor("_BaseColor",  new Color(1, 0, 0, 0.78f));
//             errorRed.SetFloat("_Smoothness", 0.0f);
//             errorRed.SetFloat("_ReceiveShadows",0.0f);
//             errorRed.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
//             errorRed.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//             errorRed.SetInt("_ZWrite", 0);
//             errorRed.DisableKeyword("_ALPHATEST_ON");
//             errorRed.DisableKeyword("_ALPHABLEND_ON");
//             errorRed.EnableKeyword("_ALPHAPREMULTIPLY_ON");
//             errorRed.EnableKeyword("_RECEIVE_SHADOWS_OFF");
//             errorRed.renderQueue = 3000; 
//         }
//         
//         AssetDatabase.Refresh();
//         // AssetDatabase.SaveAssets();
//     }
//
//     //TODO:不能通过统一方法创建，Material引用关系严格,这样不生效???
//     void CreateMaterials(Material material, Color color)
//     {
//         material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
//         material.SetFloat("_Surface", 1.0f);
//         material.SetColor("_BaseColor", color);
//         material.SetFloat("_Smoothness", 0.0f);
//         material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
//         material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//         material.SetInt("_ZWrite", 0);
//         material.DisableKeyword("_ALPHATEST_ON");
//         material.DisableKeyword("_ALPHABLEND_ON");
//         material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
//         material.renderQueue = 3000;
//
//     }
//
//     //TODO：特殊物体排查，TerrainService ？
//     void MeshFilterLimit()
//     {
//         meshFilters = GameObject.FindObjectsOfType<MeshFilter>();
//         //TODO 自定义shader、特效、地形等验证测试
//         //找到材质关联的Texture进行判断
//         //有效的meshFilters中的mesh筛选
//         for (int i = 0; i < meshFilters.Length; i++)
//         {
//             //1.自定义的碰撞
//             if (meshFilters[i].gameObject.GetComponent<MeshRenderer>() == null)
//             {
//                 continue;
//             }
//
//             curMaterials = meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterials;
//             //2. 空气墙
//             if (curMaterials.Length < 0)
//             {
//                 continue;
//             }
//
//             //是否有miss的Texture,runtime阶段会对missing的材质球一个lit,editro不会
//             //3.有材质，但Materials丢失missing
//             bool hasMissMainTex = false;
//
//             for (int j = 0; j < curMaterials.Length; j++)
//             {
//                 if (curMaterials[j] == null)
//                 {
//                     hasMissMainTex = true;
//                     break;
//                 }
//             }
//
//             if (hasMissMainTex)
//             {
//                 continue;
//             }
//
//             //4.有材质，无贴图
//             bool hasMainTex = false;
//             string[] textureNames;
//             // Debug.Log(curMaterials[0].HasProperty("_MainTex"));
//             for (int m = 0; m < curMaterials.Length; m++)
//             {
//                 textureNames = curMaterials[m].GetTexturePropertyNames();
//                 for (int n = 0; n < textureNames.Length; n++)
//                 {
//                     if (curMaterials[m].GetTexture(textureNames[n]) != null)
//                     {
//                         hasMainTex = true;
//                     }
//                 }
//             }
//
//             if (!hasMainTex)
//             {
//                 continue;
//             }
//
//             effectiveMeshFiltersList.Add(meshFilters[i]);
//         }
//     }
//
//     void MeshFilterDeal()
//     {
//         meshFilters = null;
//         materialsList.Clear();
//         effectiveMeshFiltersList.Clear();
//
//         MeshFilterLimit();
//
//         //一个模型对应多材质
//         //TODO:合并成公共函数调用，目前list类型使用泛型时，参数无类型
//         //处理MeshFilter中的材质
//         string[] tempTexturePropertyNames;
//         for (int i = 0; i < effectiveMeshFiltersList.Count; i++)
//         {
//             curMaterials = effectiveMeshFiltersList[i].gameObject.GetComponent<MeshRenderer>().sharedMaterials;
//             materialsList.Add(curMaterials);
//
//             //notice，创建临时Material时，不能直接Material[] tempMat = curMaterials，材质球会关联引用，如果修改了tempMat，那么原始的curMaterials也会改变，需要新建mat数组去添加存储
//             Material[] tempMats = new Material[curMaterials.Length];
//             Texture tempTexture;
//             for (int j = 0; j < curMaterials.Length; j++)
//             {
//                 //获取当前材质球所关联的Textures们
//                 // Debug.Log(curMaterials[j].name);
//                 tempTexturePropertyNames = curMaterials[j].GetTexturePropertyNames();
//                
//                 tempLevel.Clear();
//                 for (int k = 0; k < tempTexturePropertyNames.Length; k++)
//                 {
//                     // Debug.Log("TexturePropertyNames:" + tempTexturePropertyNames[k]);
//                     if (curMaterials[j].GetTexture(tempTexturePropertyNames[k]) != null)
//                     {
//                         tempTexture = curMaterials[j].GetTexture(tempTexturePropertyNames[k]);
//                         // Debug.Log("TexturePropertyNames:" + tempTexturePropertyNames[k]);
//                         // Debug.Log("TextureNames:" + curMaterials[0].GetTexture(tempTexturePropertyNames[k]).name);
//
//                         float height = 0;
//                         float width = 0;
//                         if (tempTexture)
//                         {
//                             height = tempTexture.height;
//                             width = tempTexture.width;
//                         }
//
//                         float textureArea = height * width;
//                         float surfaceArea = 0;
//                         if ( Application.isPlaying )
//                         {
//                             surfaceArea =
//                                 MeshSurfaceArea.GetInstance().GetBoundsSumArea(effectiveMeshFiltersList[i]) *
//                                 FILL_SCALE;
//                         }
//                         else
//                         {
//                             surfaceArea =
//                                 MeshSurfaceArea.GetInstance().GetCalculateSumArea(effectiveMeshFiltersList[i]) *
//                                 FILL_SCALE;
//                         }
//                       
//                         float result = (textureArea / FILL_RESOLUTION) / surfaceArea;
//
//                         //多贴图时，存储当前每张贴图的严重程度，不能被后者覆盖，以最严重的为主反馈严重度
//                         if (result < limitRatio_1)
//                         {
//                             //透明度、特效等，使用预制的材质进行替换，之前的每个网格的材质球数组进行存储。
//                             tempLevel.Add(1);
//
//                         }
//                         else if (result > limitRatio_1 &&
//                                  result < limitRatio_2)
//                         {
//                             tempLevel.Add(2);
//
//                         }
//                         else if (result > limitRatio_2)
//                         {
//                             tempLevel.Add(3);
//
//                         }
//                     }
//                 }
//
//                 if (tempLevel.Contains(3))
//                 {
//                     tempMats[j] = errorRed;
//                     
//                 }
//                 else if (tempLevel.Contains(2))
//                 {
//                     tempMats[j] = warningYellow;
//                    
//                 }
//                 else
//                 {
//                     tempMats[j] = standardGreen;
//                   
//                 }
//                 
//             }
//           
//             effectiveMeshFiltersList[i].gameObject.GetComponent<MeshRenderer>().sharedMaterials = tempMats;
//
//         }
//
//         RecordOriginalMaterials();
//
//     }
//
//     /// <summary>
//     /// 重置原始材质球
//     /// </summary>
//     void RecordOriginalMaterials()
//     {
//         if (originalMaterialsList.Count <= 0 || originalMaterialsList.Count != materialsList.Count)
//         {
//             originalMaterialsList.Clear();
//             foreach (var v in materialsList)
//             {
//                 originalMaterialsList.Add(v);
//             }
//            
//             Debug.Log("Save originalMaterialsList : "+originalMaterialsList.Count);
//         }
//     }
//
//     /// <summary>
//     /// 限制
//     /// </summary>
//     void SkinnedMeshRendererLimit()
//     {
//         _skinnedMeshRenderers = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
//         //有效的skinnedMeshRenderer中的mesh筛选
//         for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
//         {
//             curMaterials = _skinnedMeshRenderers[i].sharedMaterials;
//             //1.无材质球，白模
//             if (curMaterials.Length < 0)
//             {
//                 continue;
//             }
//
//             //2.有材质球但材质丢失，紫模
//             bool hasMissMainTex = false;
//             // Debug.Log(curMaterials[0].name);
//             for (int j = 0; j < curMaterials.Length; j++)
//             {
//                 if (curMaterials[j] == null)
//                 {
//                     hasMissMainTex = true;
//                     break;
//                 }
//             }
//
//             if (hasMissMainTex)
//             {
//                 continue;
//             }
//
//             //3.有材质球无颜色，色模
//             //是否有mainTexture
//             bool hasMainTex = false;
//             string[] textureNames;
//             // Debug.Log(curMaterials[0].HasProperty("_MainTex"));
//             for (int m = 0; m < curMaterials.Length; m++)
//             {
//                 textureNames = curMaterials[m].GetTexturePropertyNames();
//                 for (int n = 0; n < textureNames.Length; n++)
//                 {
//                     if (curMaterials[m].GetTexture(textureNames[n]) != null)
//                     {
//                         hasMainTex = true;
//                     }
//                 }
//             }
//
//             if (!hasMainTex)
//             {
//                 continue;
//             }
//
//             effectivesSkinnedMeshRenderersList.Add(_skinnedMeshRenderers[i]);
//         }
//     }
//
//     void SkinnedMeshRendererDeal()
//     {
//         _skinnedMeshRenderers = null;
//         skinMaterialsList.Clear();
//         effectivesSkinnedMeshRenderersList.Clear();
//
//         SkinnedMeshRendererLimit();
//
//         string[] tempTexturePropertyNames;
//         //处理SkinnedMeshRenderer中的材质
//         for (int i = 0; i < effectivesSkinnedMeshRenderersList.Count; i++)
//         {
//             curSkinMaterials = effectivesSkinnedMeshRenderersList[i].sharedMaterials;
//             skinMaterialsList.Add(curSkinMaterials);
//
//
//             //Notice，不能只能Material[] tempMat=curMaterials，材质球会关联引用，如果修改了tempMat，那么原始的curMaterials也会改变，需要新建mat数组去存储
//             //Notice,List和Material都是浅拷贝
//             Material[] tempMats = new Material[curSkinMaterials.Length];
//             Texture tempTexture;
//             for (int j = 0; j < curSkinMaterials.Length; j++)
//             {
//                 tempLevel.Clear();
//                 //获取当前材质球所关联的Textures们
//                 tempTexturePropertyNames = curSkinMaterials[j].GetTexturePropertyNames();
//                 for (int k = 0; k < tempTexturePropertyNames.Length; k++)
//                 {
//                     if (curSkinMaterials[j].GetTexture(tempTexturePropertyNames[k]) != null)
//                     {
//                         tempTexture = curSkinMaterials[j].GetTexture(tempTexturePropertyNames[k]);
//                         // Debug.Log("TexturePropertyNames:" + tempTexturePropertyNames[k]);
//                         // Debug.Log("TextureNames:" + curMaterials[0].GetTexture(tempTexturePropertyNames[k]).name);
//
//                         float height = 0;
//                         float width = 0;
//                         if (tempTexture)
//                         {
//                             height = tempTexture.height;
//                             width = tempTexture.width;
//                         }
//
//                         float textureArea = height * width;
//                         float surfaceArea = 0;
//                         if (Application.isPlaying)
//                         {
//                             surfaceArea =
//                                 MeshSurfaceArea.GetInstance()
//                                     .GetSkinCalculateSumArea(effectivesSkinnedMeshRenderersList[i]) *
//                                 FILL_SCALE;
//                         }
//                         else
//                         {
//                             surfaceArea =
//                                 MeshSurfaceArea.GetInstance()
//                                     .GetSkinBoundsSumArea(effectivesSkinnedMeshRenderersList[i]) * FILL_SCALE;
//                         }
//                         
//                         float result = (textureArea / FILL_RESOLUTION) / surfaceArea;
//
//                         //存储单个Materials信息，不能一次性全覆盖
//                         if (result < limitRatio_1)
//                         {
//                             //透明度、特效等，使用预制的材质进行替换，之前的每个网格的材质球数组进行存储。
//                             tempLevel.Add(1);
//                         }
//                         else if (result > limitRatio_1 &&
//                                  result < limitRatio_2)
//                         {
//                             tempLevel.Add(2);
//                           
//                         }
//                         else if (result > limitRatio_2)
//                         {
//                             tempLevel.Add(3);
//                            
//                         }
//                     }
//                 }
//
//                 if (tempLevel.Contains(3))
//                 {
//                     tempMats[j] = errorRed;
//                 }
//                 else if (tempLevel.Contains(2))
//                 {
//                     tempMats[j] = warningYellow;
//                 }
//                 else
//                 {
//                     tempMats[j] = standardGreen;
//                 }
//
//             }
//             effectivesSkinnedMeshRenderersList[i].sharedMaterials = tempMats;
//             
//         }
//         RecordOriginalSkinnedMaterials();
//     }
//
//     void RecordOriginalSkinnedMaterials()
//     {
//         
//         if (originalSkinMaterialsList.Count <= 0 || originalSkinMaterialsList.Count != skinMaterialsList.Count)
//         {
//             originalSkinMaterialsList.Clear();
//             foreach (var v in skinMaterialsList)
//             {
//                 originalSkinMaterialsList.Add(v);
//             }
//           
//         }
//     }
//     
//
//     //TODO:从Editor阶段直接跳Runtime，材质替换失败，猜测Unity获取资源时机，在这个脚本执行前，且渲染显示在这个之后。
//     void MeshFilterRecover()
//     {
//         if (materialsList.Count <=0 || effectiveMeshFiltersList.Count <= 0)
//         {
//             return;
//         }
//         //meshFilters恢复
//         for (int i = 0; i < effectiveMeshFiltersList.Count; i++)
//         {
//             if ( effectiveMeshFiltersList[i] != null)
//             {
//                 effectiveMeshFiltersList[i].gameObject.GetComponent<MeshRenderer>().sharedMaterials = materialsList[i];
//             }
//            
//         }
//         Debug.Log("over");
//                
//         materialsList.Clear();
//         effectiveMeshFiltersList.Clear();
//     }
//
//     void SkinnedMeshRendererRecover()
//     {
//         if (effectivesSkinnedMeshRenderersList.Count <=0 || effectivesSkinnedMeshRenderersList.Count <= 0)
//         {
//             return;
//         }
//         //SkinnedMeshRenderer恢复  
//         for (int i = 0; i < effectivesSkinnedMeshRenderersList.Count; i++)
//         {
//             if (effectivesSkinnedMeshRenderersList[i] != null)
//             {
//                 effectivesSkinnedMeshRenderersList[i].sharedMaterials = skinMaterialsList[i];
//             }
//          
//         }
//         
//         skinMaterialsList.Clear();
//         effectivesSkinnedMeshRenderersList.Clear();
//     }
//
//
//     //TODO 主动获取原始材质
//     void MeshFilterOriginalGet()
//     {
//         
//     }
//
//     void SkinnedMeshRendererOriginalGet()
//     {
//         
//     }
//     
//     void MeshFilterOriginalSet()
//     {
//         if (originalMaterialsList.Count<=0)
//         {
//             return;
//         }
//         for (int i = 0; i < effectiveMeshFiltersList.Count; i++)
//         {
//             effectiveMeshFiltersList[i].gameObject.GetComponent<MeshRenderer>().sharedMaterials = originalMaterialsList[i];
//         }
//     }
//
//     void SkinnedMeshRendererOriginalSet()
//     {
//         if (originalSkinMaterialsList.Count<=0)
//         {
//             return;
//         }
//         for (int i = 0; i < effectivesSkinnedMeshRenderersList.Count; i++)
//         {
//             effectivesSkinnedMeshRenderersList[i].sharedMaterials = originalSkinMaterialsList[i];
//         }
//     }
//
//     void ActiveToolDidChange()
//     {
//         if (ToolManager.IsActiveTool(this))
//         {
//            
//         }
//         else
//         {
//             
//         }
//         
//     }
//     
//     /// <summary>
//     /// 小窗口按钮
//     /// </summary>
//     /// <param name="window"></param>
//     public override void OnToolGUI(EditorWindow window)
//     {
//
//         Handles.BeginGUI();
//         using (new GUILayout.HorizontalScope())
//         {
//             using (new GUILayout.VerticalScope(EditorStyles.helpBox))
//             {
//                 GUI.color = Color.red;
//                 GUILayout.Label("          功能面板");
//                 // if (GUILayout.Button("功能面板"))
//                 // {
//                 //     Debug.Log("Save Materials");
//                 //     MeshFilterOriginalGet();
//                 //     SkinnedMeshRendererOriginalGet();
//                 // }
//                 GUI.color = Color.white;
//                 if (GUILayout.Button("Recover Materials"))
//                 {
//                     Debug.Log("Reset Materials");
//                     MeshFilterOriginalSet();
//                     SkinnedMeshRendererOriginalSet();
//                 }
//                 GUILayout.Label("Scale");
//                 sliderValue = GUI.HorizontalSlider(new Rect(new Vector2(50,68),new Vector2(60,10)), sliderValue, 0, maxSliderValue);
//                 GUILayout.Label("Value  :  " + Math.Round(sliderValue * 100) / 100f);
//                 FILL_SCALE = sliderValue;
//                 if (GUILayout.Button("ReBatch"))
//                 {
//                     Debug.Log("Reset-ing");
//                     if (Application.isPlaying)
//                     {
//                         Debug.Log("RunTime");
//                     }
//                     else
//                     {
//                         Debug.Log("Editor");
//                     }
//                     MeshFilterRecover();
//                     SkinnedMeshRendererRecover();
//                     
//                     InitMaterials();
//                     MeshFilterDeal();
//                     SkinnedMeshRendererDeal();
//                 }
//             }
//
//             GUILayout.FlexibleSpace();
//         }
//         Handles.EndGUI();
//         
//     }
//     
//     private void OnDestroy()
//     {
//         // isRunning = false;
//         // Debug.Log("Destroy");
//         //
//         // MeshFilterRecover();
//         // SkinnedMeshRendererRecover();
//     }
// }
//
//
//
// /// <summary>
// /// 体积面积计算类
// /// </summary>
// public class MeshSurfaceArea
// {
//     private Vector3 scale = Vector3.one;
//
//     private static MeshSurfaceArea instance;
//     public static MeshSurfaceArea GetInstance()
//     {
//         if (instance == null)
//         {
//             instance=new MeshSurfaceArea();
//         }
//         return instance;
//     }
//
//     public float GetBoundsSumArea(MeshFilter mf)
//     {
//         // scale = mf.gameObject.transform.lossyScale;
//         float sum = 0f;
//         float x = mf.mesh.bounds.size.x * scale.x;
//         float y = mf.mesh.bounds.size.y * scale.y;
//         float z = mf.mesh.bounds.size.z * scale.z;
//
//         sum = (x * y + x * z + z * y) * 2;
//         // Debug.Log("boundsArea:"+sum);
//         return sum;
//     }
//     public float GetSkinBoundsSumArea(SkinnedMeshRenderer mf)
//     {
//         // scale = mf.gameObject.transform.lossyScale;
//         float sum = 0f;
//         float x = mf.sharedMesh.bounds.size.x * scale.x;
//         float y = mf.sharedMesh.bounds.size.y * scale.y;
//         float z = mf.sharedMesh.bounds.size.z * scale.z;
//
//         sum = (x * y + x * z + z * y) * 2;
//         // Debug.Log("boundsArea:"+sum);
//         return sum;
//     }
//
//     public float GetSkinCalculateSumArea(SkinnedMeshRenderer mr)
//     {
//         // scale = mr.gameObject.transform.lossyScale;
//         Vector3[] arrVertices = mr.sharedMesh.vertices;
//         float sum1 = 0.0f;
//         for (int i = 0; i < mr.sharedMesh.subMeshCount; i++)
//         {
//             int[] arrIndices = mr.sharedMesh.GetTriangles(i);
//             for (int j = 0; j < arrIndices.Length; j += 3)
//                 sum1 += this.CalculateArea(arrVertices[arrIndices[j]]
//                     , arrVertices[arrIndices[j + 1]]
//                     , arrVertices[arrIndices[j + 2]]);
//         }
//
//         // Debug.Log("Area = " + sum1);
//         return sum1;
//     }
//
//     public float GetCalculateSumVolume(MeshFilter mf)
//     {
// #if UNITY_EDITOR
//          // scale = mf.gameObject.transform.lossyScale;
//                 Vector3[] arrVertices = mf.sharedMesh.vertices;
//                 float sum = 0.0f;
//                 for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
//                 {
//                     int[] arrIndices = mf.sharedMesh.GetTriangles(i);
//                     for (int j = 0; j < arrIndices.Length; j += 3)
//                         sum += CalculateVolume(arrVertices[arrIndices[j]]
//                                                 , arrVertices[arrIndices[j + 1]]
//                                                 , arrVertices[arrIndices[j + 2]]);
//                 }
//                 
//                 // Debug.Log("Volume= " + Mathf.Abs(sum));
//                 return Mathf.Abs(sum);
// #else
//         scale = mf.gameObject.transform.lossyScale;
//         Vector3[] arrVertices = mf.mesh.vertices;
//         float sum = 0.0f;
//         for (int i = 0; i < mf.mesh.subMeshCount; i++)
//         {
//             int[] arrIndices = mf.mesh.GetTriangles(i);
//             for (int j = 0; j < arrIndices.Length; j += 3)
//                 sum += CalculateVolume(arrVertices[arrIndices[j]]
//                                         , arrVertices[arrIndices[j + 1]]
//                                         , arrVertices[arrIndices[j + 2]]);
//         }
//         
//         Debug.Log("Volume= " + Mathf.Abs(sum));
//         return Mathf.Abs(sum);
// #endif
//     }
//     public float GetCalculateSumArea(MeshFilter mf)
//     {
// #if UNITY_EDITOR
//       
//         // scale = mf.gameObject.transform.lossyScale;
//         Vector3[] arrVertices = mf.sharedMesh.vertices;
//         float sum1 = 0.0f;
//         for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
//         {
//             int[] arrIndices = mf.sharedMesh.GetTriangles(i);
//             for (int j = 0; j < arrIndices.Length; j += 3)
//                 sum1 += this.CalculateArea(arrVertices[arrIndices[j]]
//                     , arrVertices[arrIndices[j + 1]]
//                     , arrVertices[arrIndices[j + 2]]);
//         }
//        
//         // Debug.Log("Area = " + sum1);
//         return sum1;
// #else
//     
//         scale = mf.gameObject.transform.lossyScale;
//         Vector3[] arrVertices = mf.mesh.vertices;
//         float sum1 = 0.0f;
//         for (int i = 0; i < mf.mesh.subMeshCount; i++)
//         {
//             int[] arrIndices = mf.mesh.GetTriangles(i);
//             for (int j = 0; j < arrIndices.Length; j += 3)
//                 sum1 += this.CalculateArea(arrVertices[arrIndices[j]]
//                     , arrVertices[arrIndices[j + 1]]
//                     , arrVertices[arrIndices[j + 2]]);
//         }
//
//         Debug.Log("Area = " + sum1);
//         return sum1;
// #endif
//     }
//     
//     
//     private float CalculateVolume(Vector3 pt0, Vector3 pt1, Vector3 pt2)
//     {
//         pt0 = new Vector3(pt0.x * this.scale.x, pt0.y * this.scale.y, pt0.z * this.scale.z);
//         pt1 = new Vector3(pt1.x * this.scale.x, pt1.y * this.scale.y, pt1.z * this.scale.z);
//         pt2 = new Vector3(pt2.x * this.scale.x, pt2.y * this.scale.y, pt2.z * this.scale.z);
//         float v321 = pt2.x * pt1.y * pt0.z;
//         float v231 = pt1.x * pt2.y * pt0.z;
//         float v312 = pt2.x * pt0.y * pt1.z;
//         float v132 = pt0.x * pt2.y * pt1.z;
//         float v213 = pt1.x * pt0.y * pt2.z;
//         float v123 = pt0.x * pt1.y * pt2.z;
//         return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
//     }
//     private float CalculateArea(Vector3 pt0, Vector3 pt1, Vector3 pt2)
//     {
//         pt0 = new Vector3(pt0.x * this.scale.x, pt0.y * this.scale.y, pt0.z * this.scale.z);
//         pt1 = new Vector3(pt1.x * this.scale.x, pt1.y * this.scale.y, pt1.z * this.scale.z);
//         pt2 = new Vector3(pt2.x * this.scale.x, pt2.y * this.scale.y, pt2.z * this.scale.z);
//         float a = (pt1 - pt0).magnitude;
//         float b = (pt2 - pt1).magnitude;
//         float c = (pt0 - pt2).magnitude;
//         float p = (a + b + c) * 0.5f;
//         return Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
//     }
//     
// }
//

