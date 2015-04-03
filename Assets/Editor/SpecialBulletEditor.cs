using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpecialBulletController)), CanEditMultipleObjects]
public class SpecialBulletEditor : Editor {

	/*
	 * Custom editor created to hide useless properties in the special bullet script
	 */

	public SerializedProperty 
		followingBehaviour_Prop,
		finalSpeed_Prop,
		acceleration_Prop,
		initialTurningSpeed_Prop,
		finalTurningSpeed_Prop;
	
	void OnEnable () {
		// Setup the SerializedProperties
		followingBehaviour_Prop = serializedObject.FindProperty ("followingBehaviour");
		finalSpeed_Prop = serializedObject.FindProperty ("finalSpeed");
		acceleration_Prop = serializedObject.FindProperty("acceleration");
		initialTurningSpeed_Prop = serializedObject.FindProperty ("initialTurningSpeed");
		finalTurningSpeed_Prop = serializedObject.FindProperty ("finalTurningSpeed");       
	}
	
	public override void OnInspectorGUI() {
		serializedObject.Update ();
		
		EditorGUILayout.PropertyField( followingBehaviour_Prop );
		
		SpecialBulletController.BehaviourChoices choice = (SpecialBulletController.BehaviourChoices)followingBehaviour_Prop.enumValueIndex;
		
		switch(choice) {
			case SpecialBulletController.BehaviourChoices.followImperfect:            
				EditorGUILayout.PropertyField(finalSpeed_Prop);
				EditorGUILayout.PropertyField(acceleration_Prop);
				EditorGUILayout.PropertyField(initialTurningSpeed_Prop);
				EditorGUILayout.PropertyField(finalTurningSpeed_Prop);
				break;
			default:
				break;			
		}
		
		
		serializedObject.ApplyModifiedProperties ();
	}
}