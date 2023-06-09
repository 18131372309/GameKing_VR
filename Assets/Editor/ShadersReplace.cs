using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ShadersReplace
{
    [MenuItem("Window/Tools/ReplaceShaders")]
    public static void ReplaceShaders()
    {
        //这里用Assets，用dataPath无法通过AssetDatabase.LoadAssetAtPath加载，路径不正确
        string path = "Assets";

        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Contains(".mat") && !file.Contains(".meta"))
            {
                var new_shader_litOrmdBlend = Shader.Find("METAVERSE IMAGINATION/Mobile/URPLit_ORMD_Blend");
                var new_shader_litPbrSimple = Shader.Find("METAVERSE IMAGINATION/Mobile/URPLit_PBRSimple");
                var new_shader_litBark = Shader.Find("METAVERSE IMAGINATION/Mobile/URPLitBark");
                var new_shader_litDetail = Shader.Find("METAVERSE IMAGINATION/Mobile/URPLitDetail");
                var new_shader_litLeaves = Shader.Find("METAVERSE IMAGINATION/Mobile/URPLitLeaves");
                var new_shader_litStandard = Shader.Find("METAVERSE IMAGINATION/Mobile/URPLitStandard");
                var new_shader_litTransparent = Shader.Find("METAVERSE IMAGINATION/Mobile/URPLitTransparent");

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(file);

                switch (mat.shader.name)
                {
                    case "METAVERSE IMAGINATION/ASE_URPLitBark":
                        mat.shader = new_shader_litBark;
                        break;
                    case "METAVERSE IMAGINATION/ASE_URPLitBlend_4_Channel":
                        mat.shader = new_shader_litOrmdBlend;
                        break;
                    case "METAVERSE IMAGINATION/ASE_URPLitBlendTex":
                        mat.shader = new_shader_litOrmdBlend;
                        break;
                    case "METAVERSE IMAGINATION/ASE_URPLitStandard":
                        mat.shader = new_shader_litStandard;
                        break;
                    case "METAVERSE IMAGINATION/ASE_URPLitTransparent":
                        mat.shader = new_shader_litTransparent;
                        break;
                    case "METAVERSE IMAGINATION/ASE_URPLitLeaves":
                        mat.shader = new_shader_litLeaves;
                        break;
                }
            }
        }
    }
}