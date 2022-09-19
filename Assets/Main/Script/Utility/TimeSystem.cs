using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Utility {
    public class TimeSystem
    {
        /// <summary>
        /// In second
        /// </summary>
        private static float m_pause_time;

        private static bool m_pause;
        public static bool IsPause => m_pause;

        public static float time {
            get {
                if (m_pause) return m_pause_time;
                return UnityEngine.Time.time;
            }
        }

        public static void Flow() {
            m_pause = false;
        }

        public static void Pause()
        {
            m_pause = true;
            m_pause_time = UnityEngine.Time.time;
        }
    }
}