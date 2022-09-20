using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;

namespace Utility {
	public class UtilityMethod {


		private static System.Random random = new System.Random();

		public static void SetRandomSeed(int seed)
		{
			random = new System.Random(seed);
		}

		//0f-1f
		public static float Random()
		{
			return (float)random.NextDouble();
		}

		/// <summary>
		///  Load single sprite from multiple mode
		/// </summary>
		/// <param name="spriteArray"></param>
		/// <param name="spriteName"></param>
		/// <returns></returns>
		public static Sprite LoadSpriteFromMulti(Sprite[] spriteArray, string spriteName) {
			foreach (Sprite s in spriteArray) {
				
				if (s.name == spriteName) return s;
			}
			return null;
		}

		/// <summary>
        /// Clear every child gameobject
        /// </summary>
        /// <param name="parent"></param>
        public static void ClearChildObject(Transform parent) {
            foreach (Transform t in parent) {
                GameObject.Destroy(t.gameObject);
            }
        }

        /// <summary>
        ///  Insert gameobject to parent
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static GameObject CreateObjectToParent(Transform parent, GameObject prefab) {
            GameObject item = GameObject.Instantiate(prefab);
            item.transform.SetParent(parent);
            item.transform.localScale = Vector3.one;
			item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 1);
			item.transform.localPosition = new Vector3( 0, 0, 1);
            return item;
        }

		public static GameObject FindObject(GameObject parent, string name) {
		     Transform[] trs= parent.GetComponentsInChildren<Transform>(true);
		     foreach(Transform t in trs){
		         if(t.name == name){
		              return t.gameObject;
		         }
		     }
		     return null;
		 }


		/// <summary>
		/// Rolls the dice, only return 1 or 0.
		/// </summary>
		/// <returns>The dice.</returns>
		public static int RollDice() {
			return Mathf.RoundToInt(UnityEngine.Random.Range(0,1));
		}
		
		/// <summary>
		/// Possibilities the match.
		/// </summary>
		/// <returns><c>true</c>, if match was possibilityed, <c>false</c> otherwise.</returns>
		public static bool PercentageGame(float rate) {
			float testValue = UnityEngine.Random.Range(0f ,1f);
			return ( rate >= testValue ) ? true : false;
		}

		public static T PercentageTurntable<T>(T[] p_group, float[] percent_array) {
			float percent = UnityEngine.Random.Range(0f, 100f);
			float max = 100;

			for (int i = 0 ; i < percent_array.Length; i++) {
				float newMax = max - percent_array[i];
				if (max >= percent && newMax <= percent ) return p_group[i];

				max = newMax;
			}
			return default (T);
		}

		public static T PercentageTurntable<T>(T[] p_group, int[] percent_array) {
			float[] convertFloat = System.Array.ConvertAll(percent_array, s => (float)s);
			return PercentageTurntable<T>(p_group, convertFloat);
		}

		
		public static int PercentageTurntable(float[] percent_array)
		{
			float percent = Random();
			float max = 1;

			for (int i = 0; i < percent_array.Length; i++)
			{
				float newMax = max - percent_array[i];
				if (max >= percent && newMax <= percent) return i;

				max = newMax;
			}

			return -1;
		}


		public static Vector3 ScaleToWorldSize(Vector3 p_vector3, int target_value) {
			return new Vector3( target_value / p_vector3.x, target_value / p_vector3.y, target_value / p_vector3.z );
		}


		 public static T SafeDestroy<T>(T obj) where T : Object {
			if (Application.isEditor)
				Object.DestroyImmediate(obj);
			else
				Object.Destroy(obj);
			
			return null;
		}
		
		public static T SafeDestroyGameObject<T>(T component) where T : Component
		{
			if (component != null)
				SafeDestroy(component.gameObject);
			return null;
		}

		public static T ParseEnum<T>(string value)	{
		    return (T) System.Enum.Parse(typeof(T), value, true);
		}

		public static Dictionary<T, K> SetDictionary<T, K>(Dictionary<T, K> dict, T key, K addValue)
		{

			if (dict.ContainsKey(key))
			{
				dict[key] = addValue;
			}
			else
			{
				dict.Add(key, addValue);
			}

			return dict;
		}

		public static Dictionary<string, List<T>> SetListDictionary<T>(Dictionary<string, List<T>> dict, string id, T dataStruct)
		{
			if (dict.ContainsKey(id))
			{
				dict[id].Add(dataStruct);
			}
			else
			{
				dict.Add(id, new List<T>() { dataStruct });
			}
			return dict;
		}

		public static void SetSimpleBtnEvent(Button btn, System.Action eventCallback)
		{
			if (btn == null || eventCallback == null) return;
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(() =>
			{
				eventCallback();
			});
		}


		public static async Task DoDelayWork(float p_delay, System.Action p_action)
		{
			await DoDelayWork(p_delay, p_action, CancellationToken.None);
		}

		public static async Task DoDelayWork(float p_delay, System.Action p_action, CancellationToken token)
		{
			try
			{
				await Task.Delay(System.TimeSpan.FromSeconds(p_delay), token);

				if (p_action != null)
					p_action();
			}
			catch (System.Exception exception)
			{

			}
		}

		public static IEnumerator DoEndFrameCoroutineWork(System.Action p_action)
		{
			yield return new WaitForEndOfFrame();

			if (p_action != null)
				p_action();
		}

		public static IEnumerator DoDelayCoroutineWork(float p_delay, System.Action p_action)
		{
			yield return new WaitForSeconds(p_delay);

			if (p_action != null)
				p_action();
		}


		public static async Task DoNexFrame(System.Action p_action)
		{
			await Task.Yield();

			if (p_action != null)
				p_action();
		}

		public static async Task WaitUntil(System.Func<bool> condition, int frequency = 25, int timeout = -1)
		{
			await Task.Run(async () =>
			{
				bool hasTimeout = timeout > 0;
				int incremntTimeout = timeout;

				while (!condition())
				{
					await Task.Delay(frequency);

					incremntTimeout -= frequency;

					//Abort
					if (incremntTimeout < 0 && hasTimeout)
					{
						Debug.Log("WaitUntil Timeout");
						return;
					};
				}
			});
		}
	}
}