using Hsinpa.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TD.Static;
using UnityEngine;

namespace TD.Storyboard
{
    public class StoryBoardFlow
    {
        public Dictionary<string, List<GeneralObjectStruct.RawStoryBoardStruct>> level_table = new Dictionary<string, List<GeneralObjectStruct.RawStoryBoardStruct>>();

        private List<GeneralObjectStruct.RawStoryBoardStruct> currentStorySet;
        public GeneralObjectStruct.RawStoryBoardStruct CurrentStroyStruct => currentStorySet[index];
        private int index;



        public StoryBoardFlow() { 
        
        }

        public GeneralObjectStruct.RawStoryBoardStruct StartStoryBoard(string story_id) {
            if (level_table.TryGetValue(story_id, out var storyBoardStruct)) {

                this.currentStorySet = storyBoardStruct;
                this.index = 0;

                return CurrentStroyStruct;
            }
            return default(GeneralObjectStruct.RawStoryBoardStruct);
        }

        public GeneralObjectStruct.RawStoryBoardStruct Process() {
            index++;

            if (currentStorySet == null || index < 0 || index >= currentStorySet.Count) return default(GeneralObjectStruct.RawStoryBoardStruct);

            return CurrentStroyStruct;
        }

        public async void ParseStoryboardData() {
            await Task.Run(() =>
            {
                GeneralObjectStruct.RawStoryBoardStruct[] rawStoryData = GetStoryBoardStructs();

                for (int i = 0; i < rawStoryData.Length; i++)
                {
                    //Debug.Log($"ID {rawStoryData[i].ID}, Type {rawStoryData[i].Type}, Title {rawStoryData[i].Title}, Data {rawStoryData[i].Data}, Time {rawStoryData[i].Time}");
                    Utility.UtilityMethod.SetListDictionary<GeneralObjectStruct.RawStoryBoardStruct>(level_table, rawStoryData[i].ID, rawStoryData[i]);
                }
            });
        }

        private GeneralObjectStruct.RawStoryBoardStruct[] GetStoryBoardStructs()
        {
            string json_raw = IOUtility.ConvertCsvFileToJsonObject(Application.streamingAssetsPath + "/CSV/database - storyboard.csv");

            return JsonHelper.FromJsonArray<GeneralObjectStruct.RawStoryBoardStruct>(json_raw);
        }

    }
}