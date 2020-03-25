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

            var allButton = GUILayout.Button("全マスタ", GUILayout.Width(180.0f));

            if (allButton)
            {
                MasterLoader.LoadMasterAll();
            }

            var enemyButton = GUILayout.Button("Temp", GUILayout.Width(180.0f));

            if (enemyButton)
            {
                MasterLoader.LoadMaster(MasterLoader.YourMaster);
            }

            EditorGUILayout.Space();
        }
    }
}