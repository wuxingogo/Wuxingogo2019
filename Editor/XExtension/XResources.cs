using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Callbacks;
using wuxingogo;

namespace wuxingogo.Editor
{
	public static class XResources {

	static XResources()
	{
		InitTexture();

	}

	public static void InitTexture()
	{
		//XLogger.Log( "Resources Init with wuxingogo" );

	}
//	[X]
//	[DidReloadScripts]
//	public static void ReloadScript()
//	{
//		if( LogoTexture == null )
//		{
//			var resPath = AssetDatabase.FindAssets( "wuxingogo t:texture" );
//			var path = AssetDatabase.AssetPathToGUID( resPath[ 0 ] );
//			LogoTexture =  AssetDatabase.LoadAssetAtPath<Texture>(path);
//			EditorPrefs.SetString("XLogo", AssetDatabase.GetAssetPath(LogoTexture));
//		}
//		
//	}
	public static Texture LogoTexture
	{
		get
		{
			if( _logoTex == null )	
			{
				var resPath = AssetDatabase.FindAssets( "wuxingogo t:texture" );
				if(resPath.Length > 0)
				{
					var guidPath = AssetDatabase.GUIDToAssetPath( resPath[ 0 ] );
					_logoTex =  AssetDatabase.LoadAssetAtPath<Texture>(guidPath);
				}
				
			}
			return _logoTex;
		}
	}

	private static Texture _logoTex = null;
	[X]
    public static void SaveAll()
    {
        EditorPrefs.SetString("XLogo", AssetDatabase.GetAssetPath(LogoTexture));
    }
    
	
}
}