using UnityEngine;
using UnityEngine.Networking;
using UniRx.Async;
using UnityEditor;
using System;
using Localize;

namespace MasterLoader
{
    /// <summary>
    /// スプレッドシートからマスタを取得して自動生成したScriptableObjectに流し込むクラス
    /// </summary>
    [InitializeOnLoad]
    public class MasterLoader : Editor
    {
        /// <summary>
        /// マスタのURLは不変なのでconstにして編集できないようにしておく
        /// URLはスプレッドシートのコードを公開したときに表示されるものを入れる。
        /// </summary>
        private const string url =
            "your SpreadSheets URL";
        /// <summary>
        /// doGet時の独自変数
        /// 読み込むシートの判断用
        /// </summary>
        private const string sheetName = "?sheetName=";
        /// <summary>
        /// マスタを配置するパス。ResoucesディレクトリとMasterディレクトリをあらかじめ作成しておく
        /// </summary>
        private const string path = "Assets/MasterLoader/Resources/Master/";
        /// <summary>
        /// マスタのシート名
        /// </summary>
        public const string master = "TempMaster";

        /// <summary>
        /// アプリ起動時に自動でScriptableObjectを更新する
        /// </summary>
        static MasterLoader()
        {
            if (EditorApplication.timeSinceStartup > 100)
            {
                return;
            }

            LoadMasterAll();

            //// ゲームプレビュー終了時に自動でScriptableObjectを更新する
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == PlayModeStateChange.EnteredEditMode)
                {
                    LoadMasterAll();
                }
            };

        }

        /// <summary>
        /// 全てのマスタを一つずつ順番に読み込む
        /// </summary>
        public static async void LoadMasterAll()
        {
            await LoadMaster(master);
        }

        /// <summary>
        /// スプレッドシートからマスタを取得する
        /// </summary>
        /// <param name="masterName">取得するマスタ名</param>
        /// <param name="done">マスタ取得時に起こしたいイベント</param>
        /// <returns>エラー時の警告またはロードしたマスタ名</returns>
        public static async UniTask LoadMaster(string masterName, Action done = null)
        {
            var url = $"{MasterLoader.url}{sheetName}{masterName}";
            var result = await GetMasterAsync(url);
            var assetPath = $"{path}{masterName}.asset";
            try
            {
                switch (masterName)
                {
                    case mathEnemyMaster:
                        Debug.Log(result);
                        var masterList = JsonHelper.ListFromJson<Temp>(result);
                        if (masterList != null)
                        {
                            // すでにマスタが作成されているかを確認するために取得してみる
                            var master = AssetDatabase.LoadAssetAtPath<TempMaster>(assetPath);
                            if (master == null)
                            {
                                master = CreateInstance<TempMaster>();
                                AssetDatabase.CreateAsset(master, assetPath);
                                EditorUtility.SetDirty(master);
                            }

                            master.SetList(masterList);
                            // Inspectorから編集できないようにする
                            master.hideFlags = HideFlags.NotEditable;
                        }
                        break;
                }

                Debug.Log($"{masterName} Loaded");
                // データロード完了時に何かやりたければここで発火
                done?.Invoke();
                return masterName;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return e.Message;
            }
        }

        /// <summary>
        /// UnityWebRequest を async/await で待ち受ける
        /// </summary>
        /// <param name="url"></param>
        /// <returns>受け取った生データ</returns>
        private static async UniTask<string> GetMasterAsync(string url)
        {
            var request = UnityWebRequest.Get(url);

            EditorUtility.DisplayCancelableProgressBar("マスタ更新中...", "", 0.0f);

            await request.SendWebRequest();

            EditorUtility.ClearProgressBar();

            if (request.isHttpError || request.isNetworkError)
            {
                throw new Exception(request.error);
            }

            return request.downloadHandler.text;
        }
    }
}