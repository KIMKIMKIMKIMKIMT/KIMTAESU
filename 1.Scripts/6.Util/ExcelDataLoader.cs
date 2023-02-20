using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcelDataLoader : Singleton<ExcelDataLoader>
{
    public string[,] LoadTextData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        int colCount;
        int rowCount;

        string currentText = textAsset.text.Substring(0, textAsset.text.Length);
        string[] column = currentText.Split('\n');

        colCount = column.Length;
        rowCount = column[0].Split('\t').Length - 1;

        string[,] strData = new string[colCount - 1, rowCount];

        for (int i = 0; i < colCount - 1; i++)
        {
            string[] row = column[i + 1].Split('\t');

            for (int j = 0; j < rowCount; j++)
            {
                strData[i, j] = row[j];
            }
        }
        return strData;
    }

    public string[] LoadTextDataSingleColumn(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);

        string currentText = textAsset.text.Substring(0, textAsset.text.Length);
        string[] strData = currentText.Split('\n');

        return strData;
    }
}
