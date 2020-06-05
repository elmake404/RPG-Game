using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapControl))]
public class MapInspector : Editor
{
	//private MapControl _mapControl;
	//public override void OnInspectorGUI()
	//{
	//	DrawDefaultInspector();
	//	_mapControl = target as MapControl;
	//	if (GUILayout.Button("Data Records"))
	//	{
	//		Undo.RecordObject(_mapControl, "Data Records");
	//		_mapControl.DataRecords();
	//		EditorUtility.SetDirty(_mapControl);
	//	}

	//	if (GUILayout.Button("check"))
	//	{
	//		Undo.RecordObject(_mapControl, "check");
	//		_mapControl.Check();
	//		EditorUtility.SetDirty(_mapControl);
	//	}

	//}

}
