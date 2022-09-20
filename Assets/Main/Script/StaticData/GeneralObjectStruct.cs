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
            public string Group;
            public string Type;
            public string Title;
            public string Data;
            public int Time;

            public bool IsValid => !string.IsNullOrEmpty(ID);
        }

        public struct WaveStruct
        {
            public MonsterStats[] monsters;
            public int[] spawn_count;

            public int length => monsters.Length;
            public int duration;
        }
    }
}