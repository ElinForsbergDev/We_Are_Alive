/*  
 * Simple Attribute Randomizer
 * Access via Tools/Randomizer
 * @madgvox
 */

	/*
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class RandomizerWindow : EditorWindow {


	static class Styles {
		public static readonly GUIStyle sectionContent;

		static Styles () {
			sectionContent = new GUIStyle();
			sectionContent.normal.background = EditorGUIUtility.whiteTexture;
		}
	}

	static class Content {
		public static readonly GUIContent additiveScaling = new GUIContent( "Additive", "Add the random scaling to the object's current scale instead of overriding it." );
		public static readonly GUIContent additiveRotation = new GUIContent( "Additive", "Add the random rotation to the object's current rotation instead of overriding it." );
		public static readonly GUIContent additivePosition = new GUIContent( "Additive", "Add the random position to the object's current position instead of overriding it." );

		public static readonly GUIContent uniformScale = new GUIContent( "Uniform Scale", "Change the scale of the object without distorting it." );
		public static readonly GUIContent separateAxes = new GUIContent( "Separate Axes", "Modify all axes using the same range." );
		public static readonly GUIContent preserveScalingRatios = new GUIContent( "Preserve Ratios", "Keep the current ratios between the axes and only modify the magnitude." );

		public static readonly GUIContent xAxis = new GUIContent( "X Axis", "The minimum and maximum delta on the X axis." );
		public static readonly GUIContent yAxis = new GUIContent( "Y Axis", "The minimum and maximum delta on the Y axis." );
		public static readonly GUIContent zAxis = new GUIContent( "Z Axis", "The minimum and maximum delta on the Z axis." );

		public static readonly GUIContent minDelta = new GUIContent( "Min Delta", "The minimum delta on all axes." );
		public static readonly GUIContent maxDelta = new GUIContent( "Max Delta", "The maximum delta on the all axes." );

		public static readonly GUIContent useWorldSpace = new GUIContent( "World Space", "Randomize the positions in world space." );
	}

	AnimBool enableScaleAnim;
	AnimBool enableRotationAnim;
	AnimBool enablePositionAnim;

	private const int axisLabelWidth = 117;
	bool enableScale;
	bool enableRotation;
	bool enablePosition;

	bool uniformScale;
	bool additiveScale;
	bool preserveScaleRatios;
	public Vector3 minScale;
	public Vector3 maxScale;
	public float minUniformScale;
	public float maxUniformScale;

	bool separateRotationAxes;
	bool additiveRotation;
	public Vector3 minRotation;
	public Vector3 maxRotation;
	public float minUniformRotation;
	public float maxUniformRotation;

	bool additivePosition;
	bool worldSpacePosition;
	bool separatePositionAxes;
	public Vector3 minPosition;
	public Vector3 maxPosition;
	public float minUniformPosition;
	public float maxUniformPosition;

	[MenuItem( "Tools/Randomize" )]
	static void ShowWindow () {
		var window = GetWindow<RandomizerWindow>( true );
	}

	void ResetPreferences () {
		enableScale = false;
		enableRotation = false;
		enablePosition = false;

		uniformScale = false;
		additiveScale = false;
		preserveScaleRatios = false;

		minScale = new Vector3( 1, 1, 1 );
		maxScale = new Vector3( 2, 2, 2 );
		minUniformScale = 1;
		maxUniformScale = 2;

		separateRotationAxes = false;
		additiveRotation = true;
		minRotation = new Vector3( 0, 0, 0 );
		maxRotation = new Vector3( 360, 360, 360 );
		minUniformRotation = 0;
		maxUniformRotation = 360;

		additivePosition = true;
		worldSpacePosition = false;
		separatePositionAxes = false;
		minPosition = new Vector3( -1, -1, -1 );
		maxPosition = new Vector3( 1, 1, 1 );
		minUniformPosition = -10;
		maxUniformPosition = 10;
	}

	private void OnEnable () {
		titleContent = new GUIContent( "Randomize" );
		minSize = new Vector2( 250, 500 );

		ResetPreferences();

		LoadPreferences();

		enablePositionAnim = new AnimBool();
		enablePositionAnim.target = enablePosition;
		enablePositionAnim.value = enablePosition;
		enablePositionAnim.speed = 4;

		enableRotationAnim = new AnimBool();
		enableRotationAnim.target = enableRotation;
		enableRotationAnim.value = enableRotation;
		enableRotationAnim.speed = 4;

		enableScaleAnim = new AnimBool();
		enableScaleAnim.target = enableScale;
		enableScaleAnim.value = enableScale;
		enableScaleAnim.speed = 4;

		HookEvents();
	}

	private void OnSelectionChanged () {
		Repaint();
	}

	private void OnDisable () {
		SavePreferences();

		UnhookEvents();
	}

	private void HookEvents () {
		UnhookEvents();

		Selection.selectionChanged += OnSelectionChanged;

		enableRotationAnim.valueChanged.AddListener( Repaint );
		enableScaleAnim.valueChanged.AddListener( Repaint );
		enablePositionAnim.valueChanged.AddListener( Repaint );
	}

	private void UnhookEvents () {
		Selection.selectionChanged -= OnSelectionChanged;
		enableRotationAnim.valueChanged.RemoveAllListeners();
		enableScaleAnim.valueChanged.RemoveAllListeners();
		enablePositionAnim.valueChanged.RemoveAllListeners();
	}

	private void OnGUI () {
		var width = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 120;

		DoScalingGUI();
		DoRotationGUI();
		DoPositionGUI();

		EditorGUIUtility.labelWidth = width;

		var selection = Selection.GetTransforms( SelectionMode.TopLevel );
		GUI.enabled = ( enableRotation || enableScale || enablePosition ) && selection.Length > 0;
		EditorGUILayout.LabelField( string.Format( "Randomize {0} selected objects", selection.Length ) );

		if( GUILayout.Button( "Go!" ) ) {
			ExecuteSelectedActions( selection );
		}
		GUI.enabled = true;
	}

	private void ExecuteSelectedActions ( Transform[] selection ) {
		for( int i = 0; i < selection.Length; i++ ) {
			var transform = selection[ i ];

			Undo.RecordObject( transform, "Randomized Scale" );

			if( enableScale ) {
				var deltaScale = default( Vector3 );

				if( !uniformScale ) {
					deltaScale = new Vector3(
						Random.Range( minScale.x, maxScale.x ),
						Random.Range( minScale.y, maxScale.y ),
						Random.Range( minScale.z, maxScale.z )
					);
				} else {
					var scaleFactor = Random.Range( minUniformScale, maxUniformScale );

					if( preserveScaleRatios ) {
						var ratio = transform.localScale.x;

						var ratios = new Vector3( 1, transform.localScale.y / ratio, transform.localScale.z / ratio );

						deltaScale = new Vector3(
							scaleFactor,
							scaleFactor * ratios.y,
							scaleFactor * ratios.z
						);
					} else {
						deltaScale = new Vector3(
							scaleFactor,
							scaleFactor,
							scaleFactor
						);
					}
				}

				if( additiveScale ) {
					transform.localScale += deltaScale;
				} else {
					transform.localScale = deltaScale;
				}
			}

			if( enableRotation ) {
				var deltaRotation = default( Vector3 );

				if( separateRotationAxes ) {
					deltaRotation.x = Random.Range( minRotation.x, maxRotation.x );
					deltaRotation.y = Random.Range( minRotation.y, maxRotation.y );
					deltaRotation.z = Random.Range( minRotation.z, maxRotation.z );
				} else {
					deltaRotation.x = Random.Range( minUniformRotation, maxUniformRotation );
					deltaRotation.y = Random.Range( minUniformRotation, maxUniformRotation );
					deltaRotation.z = Random.Range( minUniformRotation, maxUniformRotation );
				}

				if( additiveRotation ) {
					var quat = Quaternion.Euler( deltaRotation );
					transform.localRotation *= quat;
				} else {
					transform.localEulerAngles = deltaRotation;
				}
			}

			if( enablePosition ) {
				var deltaPosition = default( Vector3 );
				if( separatePositionAxes ) {
					deltaPosition.x = Random.Range( minPosition.x, maxPosition.x );
					deltaPosition.y = Random.Range( minPosition.y, maxPosition.y );
					deltaPosition.z = Random.Range( minPosition.z, maxPosition.z );
				} else {
					deltaPosition.x = Random.Range( minUniformPosition, maxUniformPosition );
					deltaPosition.y = Random.Range( minUniformPosition, maxUniformPosition );
					deltaPosition.z = Random.Range( minUniformPosition, maxUniformPosition );
				}

				if( additivePosition ) {
					if( worldSpacePosition ) {
						transform.position += deltaPosition;
					} else {
						transform.localPosition += transform.rotation * deltaPosition;
					}
				} else {
					if( worldSpacePosition ) {
						transform.position = deltaPosition;
					} else {
						transform.localPosition = transform.rotation * deltaPosition;
					}
				}
			}
		}
	}

	private void DoScalingGUI () {
		EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
		enableScale = EditorGUILayout.Toggle( enableScale, GUILayout.Width( 14 ) );
		GUI.enabled = enableScale;
		GUILayout.Label( "Scaling", GUILayout.ExpandWidth( true ) );
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();

		enableScaleAnim.target = enableScale;

		if( EditorGUILayout.BeginFadeGroup( enableScaleAnim.faded ) ) {
			GUI.color = new Color( 1, 1, 1, 0.2f );
			EditorGUILayout.BeginVertical( Styles.sectionContent );
			GUI.color = Color.white;

			additiveScale = EditorGUILayout.Toggle( Content.additiveScaling, additiveScale );
			uniformScale = EditorGUILayout.Toggle( Content.uniformScale, uniformScale );

			GUI.enabled = uniformScale;
			preserveScaleRatios = EditorGUILayout.Toggle( Content.preserveScalingRatios, preserveScaleRatios );
			GUI.enabled = true;

			if( !uniformScale ) {
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.xAxis, GUILayout.Width( axisLabelWidth ) );
				minScale.x = EditorGUILayout.FloatField( minScale.x );
				maxScale.x = EditorGUILayout.FloatField( maxScale.x );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.yAxis, GUILayout.Width( axisLabelWidth ) );
				minScale.y = EditorGUILayout.FloatField( minScale.y );
				maxScale.y = EditorGUILayout.FloatField( maxScale.y );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.zAxis, GUILayout.Width( axisLabelWidth ) );
				minScale.z = EditorGUILayout.FloatField( minScale.z );
				maxScale.z = EditorGUILayout.FloatField( maxScale.z );
				EditorGUILayout.EndHorizontal();
			} else {
				minUniformScale = EditorGUILayout.FloatField( Content.minDelta, minUniformScale );
				maxUniformScale = EditorGUILayout.FloatField( Content.maxDelta, maxUniformScale );
			}

			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndFadeGroup();
	}

	private void DoRotationGUI () {
		EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
		enableRotation = EditorGUILayout.Toggle( enableRotation, GUILayout.Width( 14 ) );
		GUI.enabled = enableRotation;
		GUILayout.Label( "Rotation", GUILayout.ExpandWidth( true ) );
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();

		enableRotationAnim.target = enableRotation;

		if( EditorGUILayout.BeginFadeGroup( enableRotationAnim.faded ) ) {
			GUI.color = new Color( 1, 1, 1, 0.2f );
			EditorGUILayout.BeginVertical( Styles.sectionContent );
			GUI.color = Color.white;

			additiveRotation = EditorGUILayout.Toggle( Content.additiveRotation, additiveRotation );
			separateRotationAxes = EditorGUILayout.Toggle( Content.separateAxes, separateRotationAxes );

			if( separateRotationAxes ) {
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.xAxis, GUILayout.Width( axisLabelWidth ) );
				minRotation.x = EditorGUILayout.FloatField( minRotation.x );
				maxRotation.x = EditorGUILayout.FloatField( maxRotation.x );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.yAxis, GUILayout.Width( axisLabelWidth ) );
				minRotation.y = EditorGUILayout.FloatField( minRotation.y );
				maxRotation.y = EditorGUILayout.FloatField( maxRotation.y );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.zAxis, GUILayout.Width( axisLabelWidth ) );
				minRotation.z = EditorGUILayout.FloatField( minRotation.z );
				maxRotation.z = EditorGUILayout.FloatField( maxRotation.z );
				EditorGUILayout.EndHorizontal();
			} else {
				minUniformRotation = EditorGUILayout.FloatField( Content.minDelta, minUniformRotation );
				maxUniformRotation = EditorGUILayout.FloatField( Content.maxDelta, maxUniformRotation );
			}

			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndFadeGroup();
	}

	private void DoPositionGUI () {
		EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
		enablePosition = EditorGUILayout.Toggle( enablePosition, GUILayout.Width( 14 ) );
		GUI.enabled = enablePosition;
		GUILayout.Label( "Position", GUILayout.ExpandWidth( true ) );
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();

		enablePositionAnim.target = enablePosition;

		if( EditorGUILayout.BeginFadeGroup( enablePositionAnim.faded ) ) {
			GUI.color = new Color( 1, 1, 1, 0.2f );
			EditorGUILayout.BeginVertical( Styles.sectionContent );
			GUI.color = Color.white;

			additivePosition = EditorGUILayout.Toggle( Content.additivePosition, additivePosition );
			worldSpacePosition = EditorGUILayout.Toggle( Content.useWorldSpace, worldSpacePosition );
			separatePositionAxes = EditorGUILayout.Toggle( Content.separateAxes, separatePositionAxes );

			if( separatePositionAxes ) {
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.xAxis, GUILayout.Width( axisLabelWidth ) );
				minPosition.x = EditorGUILayout.FloatField( minPosition.x );
				maxPosition.x = EditorGUILayout.FloatField( maxPosition.x );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.yAxis, GUILayout.Width( axisLabelWidth ) );
				minPosition.y = EditorGUILayout.FloatField( minPosition.y );
				maxPosition.y = EditorGUILayout.FloatField( maxPosition.y );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( Content.zAxis, GUILayout.Width( axisLabelWidth ) );
				minPosition.z = EditorGUILayout.FloatField( minPosition.z );
				maxPosition.z = EditorGUILayout.FloatField( maxPosition.z );
				EditorGUILayout.EndHorizontal();
			} else {
				minUniformPosition = EditorGUILayout.FloatField( Content.minDelta, minUniformPosition );
				maxUniformPosition = EditorGUILayout.FloatField( Content.maxDelta, maxUniformPosition );
			}

			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndFadeGroup();
	}

	private void LoadPreferences () {
		enableScale = EditorPrefs.GetBool( "com.madgvox.randomizer:enableScaling", enableScale );
		enableRotation = EditorPrefs.GetBool( "com.madgvox.randomizer:enableRotation", enableRotation );
		enablePosition = EditorPrefs.GetBool( "com.madgvox.randomizer:enablePosition", enablePosition );

		additiveScale = EditorPrefs.GetBool( "com.madgvox.randomizer:additiveScaling", additiveScale );
		uniformScale = EditorPrefs.GetBool( "com.madgvox.randomizer:uniformScaling", uniformScale );
		preserveScaleRatios = EditorPrefs.GetBool( "com.madgvox.randomizer:preserveScalingRatios", preserveScaleRatios );

		minScale.x = EditorPrefs.GetFloat( "com.madgvox.randomizer:minScale.x", minScale.x );
		minScale.y = EditorPrefs.GetFloat( "com.madgvox.randomizer:minScale.y", minScale.y );
		minScale.z = EditorPrefs.GetFloat( "com.madgvox.randomizer:minScale.z", minScale.z );

		maxScale.x = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxScale.x", maxScale.x );
		maxScale.y = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxScale.y", maxScale.y );
		maxScale.z = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxScale.z", maxScale.z );

		minUniformScale = EditorPrefs.GetFloat( "com.madgvox.randomizer:minUniformScale", minUniformScale );
		maxUniformScale = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxUniformScale", maxUniformScale );



		additiveRotation = EditorPrefs.GetBool( "com.madgvox.randomizer:additiveRotation", additiveRotation );
		separateRotationAxes = EditorPrefs.GetBool( "com.madgvox.randomizer:separateRotationAxes", separateRotationAxes );

		minRotation.x = EditorPrefs.GetFloat( "com.madgvox.randomizer:minRotation.x", minRotation.x );
		minRotation.y = EditorPrefs.GetFloat( "com.madgvox.randomizer:minRotation.y", minRotation.y );
		minRotation.z = EditorPrefs.GetFloat( "com.madgvox.randomizer:minRotation.z", minRotation.z );

		maxRotation.x = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxRotation.x", maxRotation.x );
		maxRotation.y = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxRotation.y", maxRotation.y );
		maxRotation.z = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxRotation.z", maxRotation.z );

		minUniformRotation = EditorPrefs.GetFloat( "com.madgvox.randomizer:minUniformRotation", minUniformRotation );
		maxUniformRotation = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxUniformRotation", maxUniformRotation );



		additivePosition = EditorPrefs.GetBool( "com.madgvox.randomizer:additivePosition", additivePosition );
		worldSpacePosition = EditorPrefs.GetBool( "com.madgvox.randomizer:worldSpacePosition", worldSpacePosition );
		separatePositionAxes = EditorPrefs.GetBool( "com.madgvox.randomizer:separatePositionAxes", separatePositionAxes );

		minPosition.x = EditorPrefs.GetFloat( "com.madgvox.randomizer:minPosition.x", minPosition.x );
		minPosition.y = EditorPrefs.GetFloat( "com.madgvox.randomizer:minPosition.y", minPosition.y );
		minPosition.z = EditorPrefs.GetFloat( "com.madgvox.randomizer:minPosition.z", minPosition.z );

		maxPosition.x = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxPosition.x", maxPosition.x );
		maxPosition.y = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxPosition.y", maxPosition.y );
		maxPosition.z = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxPosition.z", maxPosition.z );

		minUniformPosition = EditorPrefs.GetFloat( "com.madgvox.randomizer:minUniformPosition", minUniformPosition );
		maxUniformPosition = EditorPrefs.GetFloat( "com.madgvox.randomizer:maxUniformPosition", maxUniformPosition );
	}

	private void SavePreferences () {
		EditorPrefs.SetBool( "com.madgvox.randomizer:enableScaling", enableScale );
		EditorPrefs.SetBool( "com.madgvox.randomizer:enableRotation", enableRotation );
		EditorPrefs.SetBool( "com.madgvox.randomizer:enablePosition", enablePosition );

		EditorPrefs.SetBool( "com.madgvox.randomizer:additiveScaling", additiveScale );
		EditorPrefs.SetBool( "com.madgvox.randomizer:uniformScaling", uniformScale );
		EditorPrefs.SetBool( "com.madgvox.randomizer:preserveScalingRatios", preserveScaleRatios );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:minScale.x", minScale.x );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:minScale.y", minScale.y );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:minScale.z", minScale.z );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxScale.x", maxScale.x );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxScale.y", maxScale.y );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxScale.z", maxScale.z );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:minUniformScale", minUniformScale );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxUniformScale", maxUniformScale );



		EditorPrefs.SetBool( "com.madgvox.randomizer:additiveRotation", additiveRotation );
		EditorPrefs.SetBool( "com.madgvox.randomizer:separateRotationAxes", separateRotationAxes );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:minRotation.x", minRotation.x );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:minRotation.y", minRotation.y );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:minRotation.z", minRotation.z );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxRotation.x", maxRotation.x );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxRotation.y", maxRotation.y );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxRotation.z", maxRotation.z );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:minUniformRotation", minUniformRotation );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxUniformRotation", maxUniformRotation );



		EditorPrefs.SetBool( "com.madgvox.randomizer:additivePosition", additivePosition );
		EditorPrefs.SetBool( "com.madgvox.randomizer:worldSpacePosition", worldSpacePosition );
		EditorPrefs.SetBool( "com.madgvox.randomizer:separatePositionAxes", separatePositionAxes );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:minPosition.x", minPosition.x );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:minPosition.y", minPosition.y );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:minPosition.z", minPosition.z );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxPosition.x", maxPosition.x );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxPosition.y", maxPosition.y );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxPosition.z", maxPosition.z );

		EditorPrefs.SetFloat( "com.madgvox.randomizer:minUniformPosition", minUniformPosition );
		EditorPrefs.SetFloat( "com.madgvox.randomizer:maxUniformPosition", maxUniformPosition );
	}

	
}
*/