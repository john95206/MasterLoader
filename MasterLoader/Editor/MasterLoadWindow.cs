using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasterLoader
{
	public class MasterLoadWindow : EditorWindow
	{
		[MenuItem("Window/MasterLoader")]
		static void Open()
		{
			GetWindow<MasterLoadWindow>();
		}

		void OnGUI()
		{
			EditorGUILayout.Space();

			if (GUILayout.Button("Temp", GUILayout.Width(80.0f)))
			{
				// Tempボタンを押したときにTempマスタを更新する
				MasterLoader.LoadMaster(MasterType.Temp);
			}

			EditorGUILayout.Space();
		}
	}
}