using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
using System.Linq;

namespace MasterLoader
{
	public enum MasterType
	{
		Temp,
	}

	/// <summary>
	/// スプレッドシートからマスタを取得してScriptableObjectに流し込むクラス
	/// </summary>
	[InitializeOnLoad]
	public class MasterLoader : Editor
	{
		/// <summary>
		/// マスタのURLは不変なのでconstにして編集できないようにしておく
		/// URLはスプレッドシートのコードを公開したときに表示されるものを入れる。
		/// </summary>
		const string URL = "https://script.google.com/macros/s/AKfycbxsLn9GTRBFRq3iG3y4urh9t9LZuVxZWuaqtCz0v-6hnPOgzRqB/exec";
		/// <summary>
		/// マスタを配置するパス。ResoucesディレクトリとMasterディレクトリをあらかじめ作成しておく
		/// </summary>
		const string path = "Assets/MasterLoader/Resources/Master/";

		/// <summary>
		/// アプリ起動時に自動でScriptableObjectを更新する
		/// </summary>
		static MasterLoader()
		{
			LoadMaster(MasterType.Temp);

			// ゲームプレビュー終了時に自動でScriptableObjectを更新する
			EditorApplication.playModeStateChanged += (state) =>
			{
				if (state == PlayModeStateChange.EnteredEditMode)
				{
					// スプレッドシートのマスタのシート名を引数に入れる
					LoadMaster(MasterType.Temp);
					// TODO: 今後マスターが増えるたびにここに追記していくと幸せになれる
				}
			};
		}

		/// <summary>
		/// スプレッドシートからマスタを取得する
		/// </summary>
		/// <param name="sheetName">マスタのシート名</param>
		public static void LoadMaster(MasterType masterType)
		{
			var sheetName = GetSheetName(masterType);

			var url = URL + "?sheetName=" + sheetName;
			ObservableWWW.GetWWW(url)
				.Subscribe(www =>
				{
					var Json = JsonHelper.ListFromJson<Temp>(www.text);
					if (Json != null)
					{
						// すでにマスタが作成されているかを確認するために取得してみる
						var master = AssetDatabase.LoadAssetAtPath<TempMaster>(path + sheetName + ".asset");
						if (master == null)
						{
							// マスタが取得できなければマスタを新規作成する
							master = CreateInstance<TempMaster>();
							AssetDatabase.CreateAsset(master, path + sheetName + ".asset");
							AssetDatabase.Refresh();
						}
						// マスタは不変の値なので、Unityでは編集できないようにする
						master.hideFlags = HideFlags.NotEditable;
						// Jsonの値をScriptableObjectに流し込む
						master.TempList = Json;
						Debug.Log(sheetName + " load has completed");
					}
					else
					{
						// Jsonの取得に失敗している
						Debug.LogError(www.text);
					}
				});
		}

		/// <summary>
		/// マスタのタイプからシート名を返す。シート名はスプレッドシートのシート名を入れる。
		/// </summary>
		/// <param name="masterType">マスタの種類</param>
		/// <returns>シート名</returns>
		static string GetSheetName(MasterType masterType)
		{
			switch (masterType)
			{
				case MasterType.Temp:
					return "TempMaster";
				// TODO: ここにマスタを追加したときにそのシート名を追記していく
				default:
					return string.Empty;
			}
		}
	}
}