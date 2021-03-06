//
// UnityMatrixEditorGUI.cs
//
// Author:
//       ly-user <52111314ly@gmail.com>
//
// Copyright (c) 2017 ly-user
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using UnityEngine;
using UnityEditor;
using wuxingogo.Reflection;
using System.Reflection;

namespace wuxingogo.Editor
{
	/// <summary>
	/// Layer matrix GUI.
	/// Implements unity physics setting matrix gui.
	/// </summary>
	public class UnityMatrixEditorGUI
	{
		public delegate bool GetValueFunc(int layerA, int layerB);

		public delegate void SetValueFunc(int layerA, int layerB, bool val);


		public static Rect topmostRect{
			get{

				var type = XReflectionUtils.GetType("UnityEngine", "UnityEngine.GUIClip");
				var v = type.TryInvokeGlobalMethod ("get_topmostRect");
				return (Rect)v;
			}
		}
		public static Vector2 Unclip(Vector2 clip){
			var type = typeof(MonoBehaviour).Assembly.GetType("UnityEngine.GUIClip");
			var p = new System.Type[]{typeof(Vector2)};
			var method = type.GetMethod("Unclip",p);
			var v = method.Invoke (null, new object[]{clip});
			return (Vector2)v;
		}
		public static void DoGUI(string title, ref bool show, ref Vector2 scrollPos,string[] Matrix, UnityMatrixEditorGUI.GetValueFunc getValue, UnityMatrixEditorGUI.SetValueFunc setValue)
		{
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				if (Matrix.Length > i && Matrix[i] != "")
				{
					num++;
				}
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(0f);
			show = EditorGUILayout.Foldout(show, title);
			GUILayout.EndHorizontal();
			if (show)
			{
				scrollPos = GUILayout.BeginScrollView(scrollPos, new GUILayoutOption[]
					{
						GUILayout.MinHeight(120f),
						GUILayout.MaxHeight((float)(100 + (num + 1) * 16))
					});
				Rect rect = GUILayoutUtility.GetRect((float)(16 * num + 100), 100f);
				Rect topmostRect = UnityMatrixEditorGUI.topmostRect;
				Vector2 vector = UnityMatrixEditorGUI.Unclip(new Vector2(rect.x, rect.y));
				int num2 = 0;
				for (int j = 0; j < Matrix.Length; j++)
				{
					if (Matrix[j] != "")
					{
						float num3 = (float)(130 + (num - num2) * 16) - (topmostRect.width + scrollPos.x);
						if (num3 < 0f)
						{
							num3 = 0f;
						}
						Vector3 pos = new Vector3((float)(130 + 16 * (num - num2)) + vector.y + vector.x + scrollPos.y - num3, vector.y + scrollPos.y, 0f);
						GUI.matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one) * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
						if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9.0"))
						{
							GUI.matrix *= Matrix4x4.TRS(new Vector3(-0.5f, -0.5f, 0f), Quaternion.identity, Vector3.one);
						}
						GUI.Label(new Rect(2f - vector.x - scrollPos.y, scrollPos.y - num3, 100f, 16f), Matrix[j], "RightLabel");
						num2++;
					}
				}
				GUI.matrix = Matrix4x4.identity;
				num2 = 0;
				for (int k = 0; k < Matrix.Length; k++)
				{
					if (Matrix[k] != "")
					{
						int num4 = 0;
						Rect rect2 = GUILayoutUtility.GetRect((float)(30 + 16 * num + 100), 16f);
						GUI.Label(new Rect(rect2.x + 30f, rect2.y, 100f, 16f), Matrix[k], "RightLabel");
						for (int l = Matrix.Length - 1; l >= 0; l--)
						{
							if (Matrix[l] != "")
							{
								if (num4 < num - num2)
								{
									GUIContent content = new GUIContent("", Matrix[k] + "/" + Matrix[l]);
									bool flag = getValue(k, l);
									bool flag2 = GUI.Toggle(new Rect(130f + rect2.x + (float)(num4 * 16), rect2.y, 16f, 16f), flag, content);
									if (flag2 != flag)
									{
										setValue(k, l, flag2);
									}
								}
								num4++;
							}
						}
						num2++;
					}
				}
				GUILayout.EndScrollView();
			}
		}
	}
}

