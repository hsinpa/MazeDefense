using Hsinpa.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TD.Database;
using TD.Static;
using UnityEngine;

namespace TD.Storyboard
{
    public class StoryBoardFlow
    {
        public Dictionary<string, List<GeneralObjectStruct.RawStoryBoardStruct>> level_table = new Dictionary<string, List<GeneralObjectStruct.RawStoryBoardStruct>>();

        private List<GeneralObjectStruct.RawStoryBoardStruct> currentStorySet;
        public GeneralObjectStruct.RawStoryBoardStruct CurrentStroyStruct => _currentStroyStruct;
        public GeneralObjectStruct.RawStoryBoardStruct _currentStroyStruct;
        private int index;
        List<MonsterStats> _monsterUnits;

        private bool _isCompleteLoad;
        public bool IsCompleteLoad => this._isCompleteLoad;

        public StoryBoardFlow(List<MonsterStats> monsterUnits) {
            this._monsterUnits = monsterUnits;
            this._isCompleteLoad = false;
        }

        public GeneralObjectStruct.RawStoryBoardStruct StartStoryBoard(string story_id) {
            if (level_table.TryGetValue(story_id, out var storyBoardStruct)) {

                this.currentStorySet = storyBoardStruct;
                this.index = 0;
                this._currentStroyStruct = currentStorySet[this.index];
                return CurrentStroyStruct;
            }
            return default(GeneralObjectStruct.RawStoryBoardStruct);
        }

        public GeneralObjectStruct.RawStoryBoardStruct Process() {
            index++;

            if (currentStorySet == null || index < 0 || index >= currentStorySet.Count) return default(GeneralObjectStruct.RawStoryBoardStruct);

            this._currentStroyStruct = currentStorySet[this.index];

            return this._currentStroyStruct;
        }

        public async void ParseStoryboardData() {
            await Task.Run(() =>
            {
                GeneralObjectStruct.RawStoryBoardStruct[] rawStoryData = GetStoryBoardStructs();

                for (int i = 0; i < rawStoryData.Length; i++)
                {
                    //Debug.Log($"ID {rawStoryData[i].ID}, Type {rawStoryData[i].Type}, Level {rawStoryData[i].Level}, Data {rawStoryData[i].Data}, Time {rawStoryData[i].Time}");
                    level_table = Utility.UtilityMethod.SetListDictionary<GeneralObjectStruct.RawStoryBoardStruct>(level_table, rawStoryData[i].Level, rawStoryData[i]);
                }
            });

            _isCompleteLoad = true;
        }

        public GeneralObjectStruct.WaveStruct ParseWave(GeneralObjectStruct.RawStoryBoardStruct storyBoardStruct) {
            string[] splitExtra = storyBoardStruct.Extra.Split('&');
            string[] splitUnits = storyBoardStruct.Data.Split('&');
            int unit_length = splitUnits.Length;

            GeneralObjectStruct.WaveStruct wave = new GeneralObjectStruct.WaveStruct();
            wave.monsters = new MonsterStats[unit_length];
            wave.spawn_count = new int[unit_length];
            wave.spawn_record = new int[unit_length];

            //Parse Data
            for (int i = 0; i < unit_length; i++) {
                string[] unitPair = splitUnits[i].Split(':');

                if (unitPair.Length <= 1) continue;

                var monsterStat = GetMonsterStat(unitPair[0]);

                if (monsterStat == null) continue;

                wave.monsters[i] = monsterStat;
                wave.spawn_count[i] = int.Parse(unitPair[1]);
                wave.spawn_record[i] = 0;
            }

            //Parse Extra
            if (int.TryParse(splitExtra[0], out int p_duration)) wave.duration = p_duration;

            return wave;
        }

        private GeneralObjectStruct.RawStoryBoardStruct[] GetStoryBoardStructs()
        {
            string json_raw = IOUtility.ConvertCsvFileToJsonObject(Application.streamingAssetsPath + "/CSV/database - storyboard.csv");

            return JsonHelper.FromJsonArray<GeneralObjectStruct.RawStoryBoardStruct>(json_raw);
        }

        private MonsterStats GetMonsterStat(string monster_id) {
            int monsterIndex = _monsterUnits.FindIndex(x => x.id == monster_id);

            if (monsterIndex < 0) return null;
        
            return _monsterUnits[monsterIndex];
        }

    }
}