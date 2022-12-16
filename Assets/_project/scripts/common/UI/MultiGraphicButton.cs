using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MultiGraphicButton))]
[CanEditMultipleObjects]
public class MultiGraphicButtonEditor : Editor
{

	SerializedProperty m_OnClickProperty;

	public override void OnInspectorGUI()
	{

		base.OnInspectorGUI();
		//MultiGraphicButton t = (MultiGraphicButton)target;
	}
}
#endif

[AddComponentMenu("UI/MultiGraphicButton")]
public class MultiGraphicButton : UnityEngine.UI.Button
{
	public bool includeParent = false;
	public bool includeText = true;
	public bool includeImages = true;
	public List<Graphic> excludedGraphics = new List<Graphic>(0);
	public Text textLabel;
	private Graphic[] m_graphics;
	protected Graphic[] Graphics
	{
		get
		{
			if (m_graphics == null)
			{
				List<Graphic> tempGraphics = new List<Graphic>(targetGraphic.transform.GetComponentsInChildren<Graphic>());

				for (int i = tempGraphics.Count - 1; i > 0; i--)
				{
					if (excludedGraphics.Contains(tempGraphics[i]))
						tempGraphics.RemoveAt(i);
				}

				if (!includeText)
				{
					for (int i = tempGraphics.Count - 1; i > 0; i--)
					{
						if (tempGraphics[i].GetComponent<Text>() != null || tempGraphics[i].GetComponent<TMPro.TextMeshProUGUI>() != null)
							tempGraphics.RemoveAt(i);
					}
					/*for (int i = 1; i < tempGraphics.Count; i++) 
					{
						if (tempGraphics [i].GetComponent<Text> () != null || tempGraphics[i].GetComponent<TMPro.TextMeshProUGUI>() != null)
							tempGraphics.RemoveAt (i);
					}*/
				}


				if (!includeImages)
				{
					for (int i = tempGraphics.Count - 1; i > 0; i--)
					{
						if (tempGraphics[i].GetComponent<Image>() != null)
							tempGraphics.RemoveAt(i);
					}
					/*for (int i = 1; i < tempGraphics.Count; i++) 
					{
						if (tempGraphics [i].GetComponent<Image> () != null)
							tempGraphics.RemoveAt (i);
					}*/
				}


				if (!includeParent)
					tempGraphics.RemoveAt(0);



				//m_graphics = targetGraphic.transform.parent.GetComponentsInChildren<Graphic>();
				m_graphics = tempGraphics.ToArray();
			}

			return m_graphics;
		}
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		Color color;
		switch (state)
		{
			case Selectable.SelectionState.Normal:
				color = this.colors.normalColor;
				break;
			case Selectable.SelectionState.Highlighted:
				color = this.colors.highlightedColor;
				break;
			case Selectable.SelectionState.Pressed:
				color = this.colors.pressedColor;
				break;
			case Selectable.SelectionState.Disabled:
				color = this.colors.disabledColor;
				break;
			case Selectable.SelectionState.Selected:
				color = this.colors.selectedColor;
				break;
			default:
				color = Color.black;
				break;
		}
		if (base.gameObject.activeInHierarchy)
		{
			switch (this.transition)
			{
				case Selectable.Transition.ColorTint:
					ColorTween(color * this.colors.colorMultiplier, instant);
					break;
				default:
					throw new NotSupportedException();
			}
		}
	}

	private void ColorTween(Color targetColor, bool instant)
	{
		if (this.targetGraphic == null)
			return;

		foreach (Graphic g in this.Graphics)
			g.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
	}
}