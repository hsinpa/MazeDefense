using System.Collections;
using System.Collections.Generic;
using TD.Database;
using UnityEngine;

namespace TD.Static
{
    public class GeneralObjectStruct
    {
        [System.Serializable]
        public struct RawStoryBoardStruct
        {
            public string ID;
            public string Level;
            public string Type;
            public string Title;
            public string Data;
            public string Extra;

            public bool IsValid => !string.IsNullOrEmpty(ID);
        }

        public struct WaveStruct
        {
            public MonsterStats[] monsters;
            public int[] spawn_count;
            public int[] spawn_record;//Default 0

            public int length => monsters.Length;
            public int duration;

            public bool IsValid => monsters != null;
        }
    }
}