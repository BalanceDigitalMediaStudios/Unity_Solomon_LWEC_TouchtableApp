using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Draws a preview field for sprites

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
public class SpritePreviewAttributeEditor: PropertyDrawer{

    float lineHeight = EditorGUIUtility.singleLineHeight;


	/*
    //this code draws a separate sprite propertyField and a texturePreviewField
    //decided to ultimately go with the objectField solution instead, but leaving this code block here as it may be useful at some point 
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){

        SpritePreviewAttribute t = (SpritePreviewAttribute)attribute;

        return lineHeight + t.maxSize;
    }


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){

        SpritePreviewAttribute t = (SpritePreviewAttribute)attribute;
		
		EditorGUI.BeginProperty(position, label, property);


        Rect labelRect          = new Rect(position.x, position.y, position.width, lineHeight);
        Rect fullPreviewRect    = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + lineHeight, t.maxSize, t.maxSize);


        EditorGUI.PropertyField(labelRect, property);
        EditorGUI.DrawRect(fullPreviewRect, Color.grey);

        GUI.Button(fullPreviewRect, "Select");
        if(property.objectReferenceValue != null)
            DrawTexturePreview(fullPreviewRect, (Sprite)property.objectReferenceValue);

        


        EditorGUI.EndProperty();
	} 
    
    private void DrawTexturePreview(Rect position, Sprite sprite){

        Vector2 fullSize    = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 size        = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

        Rect coords     = sprite.textureRect;
        coords.x        /= fullSize.x;
        coords.width    /= fullSize.x;
        coords.y        /= fullSize.y;
        coords.height   /= fullSize.y;

        Vector2 ratio;
        ratio.x = position.width / size.x;
        ratio.y = position.height / size.y;
        float minRatio = Mathf.Min(ratio.x, ratio.y);

        Vector2 center = position.center;
        position.width = size.x * minRatio;
        position.height = size.y * minRatio;
        position.center = center;

        GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
    }
    */


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){

        SpritePreviewAttribute t = (SpritePreviewAttribute)attribute;

        float aspectRatio = GetAspectRatio(property);

        return lineHeight + GetHeight(aspectRatio, t.maxSize);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){

        SpritePreviewAttribute t = (SpritePreviewAttribute)attribute;
		
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
        UnityEngine.Object newValue = EditorGUI.ObjectField(previewRect, property.objectReferenceValue, typeof(Sprite), false);
        EditorGUI.showMixedValue = false;

        //only apply new value to selection if it was changed
        if(EditorGUI.EndChangeCheck())
            property.objectReferenceValue = newValue;


        EditorGUI.EndProperty();
	}

    

    float GetAspectRatio(SerializedProperty property){

        if(property.objectReferenceValue != null)
        {
            Sprite s = (Sprite)property.objectReferenceValue;
            return s.bounds.size.x / (float)s.bounds.size.y;
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


public class SpritePreviewAttribute : PropertyAttribute {

    public int maxSize;

    public SpritePreviewAttribute(int maxSize = 50){

        this.maxSize = maxSize;
    }
}