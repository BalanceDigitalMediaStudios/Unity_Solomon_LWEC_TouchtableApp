using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(TexturePreviewAttribute))]
public class TexturePreviewAttributeEditor: PropertyDrawer{

    float lineHeight = EditorGUIUtility.singleLineHeight;


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){

        TexturePreviewAttribute t = (TexturePreviewAttribute)attribute;

        float aspectRatio = GetAspectRatio(property);

        return lineHeight + GetHeight(aspectRatio, t.maxSize);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){

        TexturePreviewAttribute t = (TexturePreviewAttribute)attribute;
		
		EditorGUI.BeginProperty(position, label, property);


        //determine positioning
        float aspectRatio   = GetAspectRatio(property);
        Rect labelRect      = new Rect(position.x,                                  position.y,                 EditorGUIUtility.labelWidth,                    lineHeight);
        Rect nameRect       = new Rect(position.x + EditorGUIUtility.labelWidth,    position.y,                 position.width - EditorGUIUtility.labelWidth,   lineHeight);
        Rect previewRect    = new Rect(position.x + EditorGUIUtility.labelWidth,    position.y + lineHeight,    GetWidth(aspectRatio, t.maxSize),               GetHeight(aspectRatio, t.maxSize));


        //label
        GUI.Label(labelRect, property.displayName);


        //sprite name
        EditorGUI.BeginDisabledGroup(true);
        string spriteName = string.Empty;
        if(!property.hasMultipleDifferentValues && property.objectReferenceValue != null)
            spriteName = property.objectReferenceValue.name;
        GUI.Label(nameRect, spriteName);
        EditorGUI.EndDisabledGroup();


        //sprite
        EditorGUI.BeginChangeCheck();        

        //temp store value of multi selection
        EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
        UnityEngine.Object newValue = EditorGUI.ObjectField(previewRect, property.objectReferenceValue, typeof(Texture2D), false);
        EditorGUI.showMixedValue = false;

        //only apply new value to selection if it was changed
        if(EditorGUI.EndChangeCheck())
            property.objectReferenceValue = newValue;


        EditorGUI.EndProperty();
	}

    

    float GetAspectRatio(SerializedProperty property){

        if(property.objectReferenceValue != null)
        {
            Texture2D t = (Texture2D)property.objectReferenceValue;
            return (float)t.width / (float)t.height;
        }
        return 1;
    }
    float GetHeight(float aspectRatio, float maxSize){

        if(aspectRatio <= 1)
            return maxSize;
        else
            return maxSize / aspectRatio;
    }
    float GetWidth(float aspectRatio, float maxSize){

        if(aspectRatio >= 1)
            return maxSize;
        else
            return maxSize * aspectRatio;
    }
}
#endif


public class TexturePreviewAttribute : PropertyAttribute {

    public int maxSize;

    public TexturePreviewAttribute(int maxSize = 50){

        this.maxSize = maxSize;
    }
}