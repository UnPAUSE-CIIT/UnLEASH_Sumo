// ============================================================================================
// File: LDTool.cs
// Description: Use this editor window to access level design utilities like snapping objects to grid and baking NavMesh.
// Author: Seifer Albacete
// ============================================================================================

using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;

public class LDTool : EditorWindow
{
    [MenuItem( "Tools/LDTool" )]
    public static void ShowWindow()
    {
        GetWindow<LDTool>( "Helper" );
    }

    [MenuItem( "Tools/Snap to Grid %&s", true )]
    static bool ValidateSnapToGrid()
    {
        return Selection.activeTransform != null;
    }

    [MenuItem( "Tools/Snap to Grid %&s" )]
    static void SnapToGrid()
    {
		if ( Selection.activeTransform == null )
			return;

        Transform selected = Selection.activeTransform;
        Vector3 position = selected.position;

        position.x = Mathf.Round( position.x * 2f ) / 2f;
        position.y = Mathf.Round( position.y * 2f ) / 2f;
        position.z = Mathf.Round( position.z * 2f ) / 2f;

        selected.position = position;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField( "UnLEASH Helper", EditorStyles.boldLabel );

        if ( GUILayout.Button( "Snap to Grid (Ctrl+Alt+S)" ) )
        {
            SnapToGrid();
        }

        if ( GUILayout.Button( "Go to Main Camera" ) )
        {
            GoToMainCamera();
        }
    }

    static void GoToMainCamera()
    {
        Camera mainCamera = Camera.main;

        if ( mainCamera == null )
        {
            Debug.LogWarning( "No Main Camera found in scene." );
            return;
        }

        SceneView sceneView = SceneView.lastActiveSceneView;

        if ( sceneView == null )
        {
            Debug.LogWarning( "No Scene View found." );
            return;
        }

		var trg = sceneView.camera;
        sceneView.camera.transform.position = mainCamera.transform.position;
        sceneView.camera.transform.rotation = mainCamera.transform.rotation;
		sceneView.AlignViewToObject( trg.transform );
        sceneView.Focus();
    }

    static void BakeNavMesh()
    {
        var surfaces = FindObjectsByType<NavMeshSurface>( FindObjectsSortMode.None );

        if ( surfaces.Length == 0 )
        {
            Debug.LogWarning( "No NavMeshSurface found in scene." );
            return;
        }

        foreach ( var surface in surfaces )
        {
            surface.BuildNavMesh();
        }
    }
}
