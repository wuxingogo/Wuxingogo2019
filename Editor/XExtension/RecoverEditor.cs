﻿//
// RecoveryEditor.cs
//
// Author:
//       ly-user <52111314ly@gmail.com>
//
// Copyright (c) 2018 ly-user
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( UnityEngine.Object ), true ), CanEditMultipleObjects]
internal class RecoveryEditor : Editor
{
    Dictionary<MonoScript, int> candidates;
    bool propFoldout = false, otherFoldout = false;

    void OnEnable()
    {
        candidates = new Dictionary<MonoScript, int>();
    }

    public override void OnInspectorGUI()
    {
        var scriptProperty = this.serializedObject.FindProperty( "m_Script" );
        if( scriptProperty == null || scriptProperty.objectReferenceValue != null )
        {
            base.OnInspectorGUI();
            return;
        }
        else
        {

            serializedObject.Update();
            var property = serializedObject.FindProperty( "m_Script" );
            EditorGUILayout.PropertyField( property );
            serializedObject.ApplyModifiedProperties();


            if( candidates.Count == 0 )
            {
                var propertyPaths = GetPropertyPaths( serializedObject, "m_Script" );
                var monoScripts = MonoImporter.GetAllRuntimeMonoScripts();
                foreach ( var script in monoScripts)
                {
                    var type = script.GetClass();

                    if( type == null )
                        continue;

                    if( !candidates.ContainsKey( script ) )
                    {
                        if( !script.GetClass().FullName.StartsWith( "UnityEngine" ) )
                            candidates.Add( script, 0 );
                    }

                    var _propertyPaths = GetPropertyPaths( type );
                    foreach( var propetyPath in _propertyPaths.Keys )
                    {
                        if( propertyPaths.ContainsKey( propetyPath ) )
                        {
                            candidates[script]++;
                        }
                    }
                }
            }

            var list = new List<KeyValuePair<MonoScript, int>>( candidates );

            list.Sort( ( kvp1, kvp2 ) => kvp2.Value - kvp1.Value );
            int maxHit = -1;

            if( list.Count != 0 && list[0].Value != 0 )
            {
                maxHit = list[0].Value;
                EditorGUILayout.LabelField( "Perhaps", EditorStyles.largeLabel );
                EditorGUI.indentLevel++;
            }

            foreach( var kvp in list )
            {
                if( kvp.Value != maxHit )
                    break;
                DrawField( kvp.Key );
            }

            if( maxHit != -1 )
            {
                EditorGUI.indentLevel--;
                otherFoldout = EditorGUILayout.Foldout( otherFoldout, "Other" );
                EditorGUI.indentLevel++;
            }
            if( otherFoldout || maxHit == -1 )
            {
                foreach( var kvp in list )
                {
                    if( kvp.Value == maxHit )
                        continue;
                    DrawField( kvp.Key );
                }
            }

            if( maxHit != -1 )
            {
                EditorGUI.indentLevel--;
            }
            propFoldout = EditorGUILayout.Foldout( propFoldout, "Properties that have been saved" );

            if( propFoldout )
            {
                EditorGUI.BeginDisabledGroup( true );

                EditorGUI.indentLevel++;
                var props = serializedObject.FindProperty( "m_Script" );
                while( props.NextVisible( false ) )
                    EditorGUILayout.PropertyField( props );
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            }
        }
    }

    void DrawField( MonoScript monoScript )
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField( monoScript.GetClass().Name );

        if( GUILayout.Button( "Recovery" ) )
        {

            serializedObject.UpdateIfRequiredOrScript();
            serializedObject.FindProperty( "m_Script" ).objectReferenceValue = monoScript;
            serializedObject.ApplyModifiedProperties();

            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
        EditorGUILayout.EndHorizontal();
    }

    Dictionary<string, Type> GetPropertyPaths( SerializedObject obj, string parentPath )
    {
        var propetyPaths = new Dictionary<string, Type>();
        var props = obj.FindProperty( parentPath );

        while( props.NextVisible( false ) )
            propetyPaths.Add( props.name, GetType( props ) );

        return propetyPaths;
    }

    Dictionary<string, Type> GetPropertyPaths( Type type )
    {
		Dictionary<string, Type> result = new Dictionary<string, Type> ();
		var field = type.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
		foreach (var item in field) {
			if(!result.ContainsKey(item.Name))
				result.Add (item.Name, item.FieldType);
		}
		return result;
    }

    Type GetType( SerializedProperty prop )
    {
        Type type = null;

        switch( prop.propertyType )
        {
            case SerializedPropertyType.Generic:
            case SerializedPropertyType.ObjectReference:
            case SerializedPropertyType.Enum:
            case SerializedPropertyType.ArraySize:
                break;
            case SerializedPropertyType.Integer:
                type = typeof( int );
                break;
            case SerializedPropertyType.Boolean:
                type = typeof( bool );
                break;
            case SerializedPropertyType.Float:
                type = typeof( float );
                break;
            case SerializedPropertyType.String:
                type = typeof( string );
                break;
            case SerializedPropertyType.Color:
                type = typeof( Color );
                break;
            case SerializedPropertyType.LayerMask:
                type = typeof( LayerMask );
                break;
            case SerializedPropertyType.Vector2:
                type = typeof( Vector2 );
                break;
            case SerializedPropertyType.Vector3:
                type = typeof( Vector3 );
                break;
            case SerializedPropertyType.Rect:
                type = typeof( Rect );
                break;
            case SerializedPropertyType.Character:
                type = typeof( char );
                break;
            case SerializedPropertyType.AnimationCurve:
                type = typeof( AnimationCurve );
                break;
            case SerializedPropertyType.Bounds:
                type = typeof( Bounds );
                break;
            case SerializedPropertyType.Gradient:
                type = typeof( Gradient );
                break;
            case SerializedPropertyType.Quaternion:
                type = typeof( Quaternion );
                break;
            default:
                throw new ArgumentOutOfRangeException( "propType" );
        }

        return type;
    }
}