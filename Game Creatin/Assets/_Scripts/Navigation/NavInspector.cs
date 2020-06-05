using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavSurface))]
public class NavInspector : Editor
{
	private NavSurface _surface;
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		_surface = target as NavSurface;
		if (GUILayout.Button("Data Records"))
		{
			Undo.RecordObject(_surface, "Data Records");
			_surface.DataRecords();
			EditorUtility.SetDirty(_surface);
		}

		if (GUILayout.Button("Creating Edge"))
		{
			Undo.RecordObject(_surface, "Creating Edge");
			_surface.CreatingEdge();
			EditorUtility.SetDirty(_surface);
		}
		if (GUILayout.Button("Algorithm Dijkstra"))
		{
			Undo.RecordObject(_surface, "Algorithm Dijkstra");
			_surface.AlgorithmDijkstra();
			EditorUtility.SetDirty(_surface);
		}
		if (GUILayout.Button("Check"))
		{
			Undo.RecordObject(_surface, "Check");
			_surface.chekc();
			EditorUtility.SetDirty(_surface);
		}
	}
}
