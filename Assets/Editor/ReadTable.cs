using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OfficeOpenXml;
using QFramework.AmorHero;
using UnityEditor;
using UnityEngine;
using QFramework.BuffDesign;
[InitializeOnLoad]
public class StartUp
{
    static StartUp()
    {
        CreatePropDataByExcel();
        CreateBuffDataByExcel();
        CreatePlayerDataByExcel();
    }
    static void CreatePlayerDataByExcel()
    {
        string path = Application.dataPath + "/Editor/玩家配置表.xlsx";
        string assetName = "PlayerData";
        FileInfo fileInfo = new FileInfo(path);
        PlayerData playerData = (PlayerData)ScriptableObject.CreateInstance(typeof(PlayerData));
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["玩家配置"];
            Type type = typeof(PlayerDataCore);
            for (int i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                PlayerDataCore playerDataCore = new PlayerDataCore();
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    string header = worksheet.GetValue(1, j).ToString();
                    FieldInfo variable = type.GetField(header);
                    if (variable != null)
                    {
                        string tableValue = worksheet.GetValue(i, j)?.ToString();
                        if (!string.IsNullOrEmpty(tableValue))
                        {
                            variable.SetValue(playerDataCore , Convert.ChangeType(tableValue, variable.FieldType));
                        }
                    }
                }

                playerData.PlayerDataCoreList.Add(playerDataCore);
            }
        }

        string assetPath = "Assets/Resource/" + assetName + ".asset";
        AssetDatabase.CreateAsset(playerData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    static void CreateBuffDataByExcel()
    {
        string path = Application.dataPath + "/Editor/Buff管理.xlsx";
        string assetName = "Buffs";
        FileInfo fileInfo = new FileInfo(path);
        BuffData buffData = (BuffData)ScriptableObject.CreateInstance(typeof(BuffData));
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Buff"];
            Type type = typeof(AbstractBuff);
            for (int i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                AbstractBuff abstractBuff = new AbstractBuff();
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    string header = worksheet.GetValue(1, j).ToString();
                    FieldInfo variable = type.GetField(header);
                    if (variable != null)
                    {
                        string tableValue = worksheet.GetValue(i, j)?.ToString();
                        if (!string.IsNullOrEmpty(tableValue))
                        {
                            variable.SetValue(abstractBuff, Convert.ChangeType(tableValue, variable.FieldType));
                        }
                    }
                }

                buffData.BuffList.Add(abstractBuff);
            }
        }

        string assetPath = "Assets/Resource/" + assetName + ".asset";
        AssetDatabase.CreateAsset(buffData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void CreatePropDataByExcel()
    {
        string path = Application.dataPath + "/Editor/道具管理.xlsx";
        string assetName = "Prop";
        FileInfo fileInfo = new FileInfo(path);
        PropData propData = (PropData)ScriptableObject.CreateInstance(typeof(PropData));
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["道具"];
            Type type = typeof(PropItem);
            for (int i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                PropItem propItem = new PropItem();
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    string header = worksheet.GetValue(1, j).ToString();
                    if (header == "prop_sprite_path")
                    {
                        string spritePath = worksheet.GetValue(i, j).ToString();
                        if (!string.IsNullOrEmpty(spritePath))
                        {
                            Sprite sprite =
                                AssetDatabase.LoadAssetAtPath<Sprite>(spritePath); // 假设spritePath是相对Assets的路径  
                            if (sprite != null)
                            {
                                propItem.prop_sprite = sprite;
                            }
                            else
                            {
                                Debug.LogWarning($"Sprite at path 'Assets/{spritePath}' could not be found.");
                            }
                        }
                    }
                    else
                    {
                        FieldInfo variable = type.GetField(header);
                        if (variable != null)
                        {
                            string tableValue = worksheet.GetValue(i, j)?.ToString();
                            if (!string.IsNullOrEmpty(tableValue))
                            {
                                variable.SetValue(propItem, Convert.ChangeType(tableValue, variable.FieldType));
                            }
                        }
                    }
                }

                propData.propItemList.Add(propItem);
            }
        }

        string assetPath = "Assets/Resource/" + assetName + ".asset";
        AssetDatabase.CreateAsset(propData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}