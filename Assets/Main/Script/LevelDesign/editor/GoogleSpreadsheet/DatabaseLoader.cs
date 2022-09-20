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
    const string STREAMING_FOLDER = "Assets/StreamingAssets";
    const string DATABASE_FOLDER = "Assets/Database";
    const string TOWER_SPRITE_PATH = "Assets/Main/Sprite/TD/td_tower_icons.png";

    /// <summary>
    /// Main app instance.
    /// </summary>
	static void UnityDownloadGoogleSheet(Dictionary<string, string> url_clone)
    {
        string path = "Assets/StreamingAssets/CSV";

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
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
            CreateMonsterStats(statsHolder);
        }
        else {
            Debug.LogError("[Stats]Holder.asset has not been created yet!");
        }

        EditorUtility.SetDirty(statsHolder);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static private void CreateSkillStats(StatsHolder statsHolder) {
        string csvText = Hsinpa.Utility.IOUtility.GetTextFile(STREAMING_FOLDER + "/CSV/database - skill.csv");
        CSVFile csvFile = new CSVFile(csvText);

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
        string csvText = Hsinpa.Utility.IOUtility.GetTextFile(STREAMING_FOLDER + "/CSV/database - tower.csv");
        UnityEngine.Object[] rawSpriteSheet = AssetDatabase.LoadAllAssetsAtPath(TOWER_SPRITE_PATH);

        var sprteSheet = rawSpriteSheet.Where(q => q is Sprite).Cast<Sprite>().ToArray();


        CSVFile csvFile = new CSVFile(csvText);

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

            c_prefab.hp = csvFile.Get<float>(i, "HP");
            c_prefab.level = csvFile.Get<int>(i, "Level");
            c_prefab.atk = csvFile.Get<float>(i, "ATK");
            c_prefab.spd = csvFile.Get<float>(i, "SPD");
            c_prefab.range = csvFile.Get<float>(i, "RANGE");
            c_prefab.cost = csvFile.Get<int>(i, "COST");
            c_prefab.value = c_prefab.cost;

            string upgradePath = csvFile.Get<string>(i, "Update Path");
            if (!string.IsNullOrEmpty(upgradePath)) {
                string[] upgradePathArray = upgradePath.Split(new string[] { "," }, System.StringSplitOptions.None);
                c_prefab = UpdateTowerUpgradePath(c_prefab, statsHolder, upgradePathArray);
            }

            string rawSkills = csvFile.Get<string>(i, "Skill");
            if (!string.IsNullOrEmpty(rawSkills))
            {
                string[] skillArray = rawSkills.Split(new string[] { "," }, System.StringSplitOptions.None);
                c_prefab.skills = GetSkillFromIDs(statsHolder, skillArray);
            }

            c_prefab.sprite = UtilityMethod.LoadSpriteFromMulti(sprteSheet, csvFile.Get<string>(i, "Sprite ID"));

            statsHolder.stpObjectHolder.Add(c_prefab);
        }
    }

    static private SkillStats[] GetSkillFromIDs(StatsHolder statsHolder, string[] skillArray)
    {
        SkillStats[] skillStats = new SkillStats[skillArray.Length];

        for (int k = 0; k < skillArray.Length; k++)
        {
            SkillStats findObject = statsHolder.FindObject<SkillStats>(skillArray[k]);

            if (findObject != null)
                skillStats[k] = findObject;
        }

        return skillStats;
    }

    static private TowerStats UpdateTowerUpgradePath(TowerStats c_prefab, StatsHolder statsHolder, string[] upgradePathArray) {

        c_prefab.upgrade_path = new TowerStats[upgradePathArray.Length];

        for (int k = 0; k < upgradePathArray.Length; k++)
        {

            TowerStats findObject = statsHolder.FindObject<TowerStats>(upgradePathArray[k]);

            if (findObject != null) {
                c_prefab.upgrade_path[k] = findObject;
                findObject.AddValue(c_prefab.cost);
            }
        }

        return c_prefab;
    }

    static private void CreateMonsterStats(StatsHolder statsHolder) {
        string csvText = Hsinpa.Utility.IOUtility.GetTextFile(STREAMING_FOLDER + "/CSV/database - monster.csv");
        CSVFile csvFile = new CSVFile(csvText);

        AssetDatabase.CreateFolder(DATABASE_FOLDER + "/Asset", "Monster");

        string MonsterAnimatorPath = "Assets/Main/Animation/Monster/{0}/Animator.overrideController";

        int csvCount = csvFile.length;
        for (int i = csvCount - 1; i >= 0; i--)
        {
            string id = csvFile.Get<string>(i, "ID");

            if (string.IsNullOrEmpty(id))
                continue;

            MonsterStats c_prefab = ScriptableObjectUtility.CreateAsset<MonsterStats>(DATABASE_FOLDER + "/Asset/Monster/", "[MonsterStat] " + id);
            EditorUtility.SetDirty(c_prefab);

            c_prefab.id = id;
            c_prefab.label = csvFile.Get<string>(i, "Label");
            c_prefab.strategy = (VariableFlag.Strategy) csvFile.Get<int>(i, "Strategy");
            c_prefab.value = csvFile.Get<float>(i, "Value");

            c_prefab.hp = csvFile.Get<int>(i, "HP");
            c_prefab.atk = csvFile.Get<float>(i, "ATK");
            c_prefab.spd = csvFile.Get<float>(i, "SPD");
            c_prefab.range = csvFile.Get<int>(i, "RANGE");
            c_prefab.moveSpeed = csvFile.Get<float>(i, "Move Speed");

            c_prefab.avgPrize = csvFile.Get<int>(i, "Prize");
            c_prefab.sprite_id = csvFile.Get<string>(i, "Sprite ID");

            string rawSkills = csvFile.Get<string>(i, "Skill");
            if (!string.IsNullOrEmpty(rawSkills))
            {
                string[] skillArray = rawSkills.Split(new string[] { "," }, System.StringSplitOptions.None);
                c_prefab.skills = GetSkillFromIDs(statsHolder, skillArray);
            }

            RuntimeAnimatorController animator = AssetDatabase.LoadAssetAtPath(string.Format(MonsterAnimatorPath, c_prefab.sprite_id), typeof(AnimatorOverrideController)) as RuntimeAnimatorController;
            if (animator != null)
                c_prefab.animator = animator;

            statsHolder.stpObjectHolder.Add(c_prefab);
        }
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
            { "database - monster", Regex.Replace( url, ":id", "557977291")},
            { "database - storyboard", Regex.Replace( url, ":id", "596235006")},
        });
    }

}