using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

using System.Linq;
using Utility;
using System.Text.RegularExpressions;
using TD.Database;

/// <summary>
/// Organize gameobjects in the scene.
/// </summary>
public class DatabaseLoader : Object
{

    const string DATABASE_FOLDER = "Assets/Database";

    /// <summary>
    /// Main app instance.
    /// </summary>
	static void UnityDownloadGoogleSheet(Dictionary<string, string> url_clone)
    {
        string path = "Assets/Database/CSV";

        if (url_clone.Count > 0)
        {
            KeyValuePair<string, string> firstItem = url_clone.First();

            WebRequest myRequest = WebRequest.Create(firstItem.Value);

            //store the response in myResponse 
            WebResponse myResponse = myRequest.GetResponse();

            //register I/O stream associated with myResponse
            Stream myStream = myResponse.GetResponseStream();

            //create StreamReader that reads characters one at a time
            StreamReader myReader = new StreamReader(myStream);

            string s = myReader.ReadToEnd();
            myReader.Close();//Close the reader and underlying stream

            File.WriteAllText(path + "/" + firstItem.Key + ".csv", s);
            url_clone.Remove(firstItem.Key);
            UnityDownloadGoogleSheet(url_clone);
            Debug.Log(firstItem.Key);

        }
        else
        {
            Debug.Log("Done");
        }
    }

    [MenuItem("Assets/MazeDefense/Database/UpdateStatsAsset", false, 1)]
    static private void UpdateStatsAsset() {
        StatsHolder statsHolder = (StatsHolder)AssetDatabase.LoadAssetAtPath(DATABASE_FOLDER + "/[Stats]Holder.asset", typeof(StatsHolder));

        if (statsHolder != null)
        {
            FileUtil.DeleteFileOrDirectory(DATABASE_FOLDER + "/Asset");
            AssetDatabase.CreateFolder(DATABASE_FOLDER, "Asset");

            statsHolder.stpObjectHolder.Clear();
            CreateSkillStats(statsHolder);
            CreateTowerStats(statsHolder);
        }
        else {
            Debug.LogError("[Stats]Holder.asset has not been created yet!");
        }

        EditorUtility.SetDirty(statsHolder);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static private void CreateSkillStats(StatsHolder statsHolder) {
        TextAsset csvText = (TextAsset)AssetDatabase.LoadAssetAtPath(DATABASE_FOLDER + "/CSV/database - skill.csv", typeof(TextAsset));
        CSVFile csvFile = new CSVFile(csvText.text);

        AssetDatabase.CreateFolder(DATABASE_FOLDER + "/Asset", "Skill");

        int csvCount = csvFile.length;
        for (int i = 0; i < csvCount; i++)
        {
            string id = csvFile.Get<string>(i, "ID");

            if (string.IsNullOrEmpty(id))
                continue;

            SkillStats c_prefab = ScriptableObjectUtility.CreateAsset<SkillStats>(DATABASE_FOLDER + "/Asset/Skill/", "[SkillStat] " + id);
            EditorUtility.SetDirty(c_prefab);

            c_prefab.id = id;
            c_prefab.label = csvFile.Get<string>(i, "Label");

            c_prefab.parameter_1 = csvFile.Get<float>(i, "Parameter 1");
            c_prefab.parameter_2 = csvFile.Get<float>(i, "Parameter 2");
            c_prefab.parameter_3 = csvFile.Get<float>(i, "Parameter 3");
            c_prefab.parameter_4 = csvFile.Get<float>(i, "Parameter 4");

            statsHolder.stpObjectHolder.Add(c_prefab);
        }
    }

    static private void CreateTowerStats(StatsHolder statsHolder)
    {
        TextAsset csvText = (TextAsset)AssetDatabase.LoadAssetAtPath(DATABASE_FOLDER + "/CSV/database - tower.csv", typeof(TextAsset));
        CSVFile csvFile = new CSVFile(csvText.text);

        AssetDatabase.CreateFolder(DATABASE_FOLDER + "/Asset", "Tower");

        int csvCount = csvFile.length;
        for (int i = csvCount - 1; i >= 0; i--)
        {
            string id = csvFile.Get<string>(i, "ID");

            if (string.IsNullOrEmpty(id))
                continue;

            TowerStats c_prefab = ScriptableObjectUtility.CreateAsset<TowerStats>(DATABASE_FOLDER + "/Asset/Tower/", "[TowerStat] " + id);
            EditorUtility.SetDirty(c_prefab);

            c_prefab.id = id;
            c_prefab.label = csvFile.Get<string>(i, "Label");
            c_prefab.tag = csvFile.Get<string>(i, "Tag");

            c_prefab.level = csvFile.Get<int>(i, "Level");
            c_prefab.atk = csvFile.Get<float>(i, "ATK");
            c_prefab.speed = csvFile.Get<float>(i, "SPD");
            c_prefab.range = csvFile.Get<float>(i, "RANGE");
            c_prefab.cost = csvFile.Get<int>(i, "COST");

            string upgradePath = csvFile.Get<string>(i, "Update Path");
            if (!string.IsNullOrEmpty(upgradePath)) {
                string[] upgradePathArray = upgradePath.Split(new string[] { "," }, System.StringSplitOptions.None);
                c_prefab = UpdateTowerUpgradePath(c_prefab, statsHolder, upgradePathArray);
            }

            string rawSkills = csvFile.Get<string>(i, "Skill");
            if (!string.IsNullOrEmpty(rawSkills))
            {
                string[] skillArray = rawSkills.Split(new string[] { "," }, System.StringSplitOptions.None);
                c_prefab = UpdateTowerSkill(c_prefab, statsHolder, skillArray);
            }


            statsHolder.stpObjectHolder.Add(c_prefab);
        }
    }

    static private TowerStats UpdateTowerSkill(TowerStats c_prefab, StatsHolder statsHolder, string[] skillArray)
    {
        c_prefab.skills = new SkillStats[skillArray.Length];

        for (int k = 0; k < skillArray.Length; k++)
        {
            SkillStats findObject = statsHolder.FindObject<SkillStats>(skillArray[k]);

            if (findObject != null)
                c_prefab.skills[k] = findObject;
        }

        return c_prefab;
    }

    static private TowerStats UpdateTowerUpgradePath(TowerStats c_prefab, StatsHolder statsHolder, string[] upgradePathArray) {

        c_prefab.upgrade_path = new TowerStats[upgradePathArray.Length];

        for (int k = 0; k < upgradePathArray.Length; k++)
        {

            TowerStats findObject = statsHolder.FindObject<TowerStats>(upgradePathArray[k]);

            if (findObject != null)
                c_prefab.upgrade_path[k] = findObject;
        }

        return c_prefab;
    }



    [MenuItem("Assets/MazeDefense/Database/Reset", false, 0)]
    static public void Reset()
    {
        PlayerPrefs.DeleteAll();
        Caching.ClearCache();
    }

    [MenuItem("Assets/MazeDefense/Database/DownloadGoogleSheet", false, 0)]
    static public void OnDatabaseDownload()
    {
        string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRWm5GMDzG9l-dRwhdR1gzMFawSxi-Nq1p9ALe9ixYioChJNojDbM-J5NrP1vi7A20ZDQEtKgEN4Yff/pub?gid=:id&single=true&output=csv";
        UnityDownloadGoogleSheet(new Dictionary<string, string> {
            { "database - tower", Regex.Replace( url, ":id", "0")},
            { "database - skill", Regex.Replace( url, ":id", "848255666")},
        });
    }

}