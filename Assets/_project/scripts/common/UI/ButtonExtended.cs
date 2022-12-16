using System.Collections.Generic;




namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Button Extended", 31)]
    public class ButtonExtended : Button
    {
        [SerializeField]Graphic[]   _targetGraphics;
        Graphic[]                   targetGraphics
        {
            get
            {
                if(_targetGraphics == null)
                    UpdateGraphics();
                return _targetGraphics;
            }
        }

        [SerializeField]List<Graphic> excludedGraphics = new List<Graphic>(0);


        protected ButtonExtended(){}

        
        public void UpdateGraphics()
        {
            if(targetGraphic == null)
            {
                _targetGraphics = null;
                return;
            }

            List<Graphic> temp = new List<Graphic>(targetGraphic.GetComponentsInChildren<Graphic>());
            for(int i = temp.Count - 1 ; i >= 0; i--)
            {
                if (excludedGraphics.Contains(temp[i]))
                    temp.RemoveAt(i);
            }

            _targetGraphics = temp.ToArray();
        }



#if UNITY_EDITOR
        protected override void OnValidate()
        {
            UpdateGraphics();
            base.OnValidate();               
        }

        new void Reset()
        {
            base.Reset();        
            
            ColorBlock defaultColors        = colors;
            defaultColors.normalColor       = Color.white;
            defaultColors.highlightedColor  = Color.white;
            defaultColors.pressedColor      = new Color(.75f, .75f, .75f, 1f);
            defaultColors.selectedColor     = Color.white;
            defaultColors.disabledColor     = new Color(.75f, .75f, .75f, .5f);
            defaultColors.colorMultiplier   = 1;
            defaultColors.fadeDuration      = .1f;
            colors                          = defaultColors;

            UpdateGraphics();
        }
#endif



        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            Color   tintColor;
            Sprite  transitionSprite;
            string  triggerName;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor           = colors.normalColor;
                    transitionSprite    = null;
                    triggerName         = animationTriggers.normalTrigger;
                    break;
                case SelectionState.Highlighted:
                    tintColor           = colors.highlightedColor;
                    transitionSprite    = spriteState.highlightedSprite;
                    triggerName         = animationTriggers.highlightedTrigger;
                    break;
                case SelectionState.Pressed:
                    tintColor           = colors.pressedColor;
                    transitionSprite    = spriteState.pressedSprite;
                    triggerName         = animationTriggers.pressedTrigger;
                    break;
                case SelectionState.Selected:
                    tintColor           = colors.selectedColor;
                    transitionSprite    = spriteState.selectedSprite;
                    triggerName         = animationTriggers.selectedTrigger;
                    break;
                case SelectionState.Disabled:
                    tintColor           = colors.disabledColor;
                    transitionSprite    = spriteState.disabledSprite;
                    triggerName         = animationTriggers.disabledTrigger;
                    break;
                default:
                    tintColor           = Color.black;
                    transitionSprite    = null;
                    triggerName         = string.Empty;
                    break;
            }

            switch (transition)
            {
                case Transition.ColorTint:
                    StartColorTween(tintColor * colors.colorMultiplier, instant);
                    break;
                case Transition.SpriteSwap:
                    DoSpriteSwap(transitionSprite);
                    break;
                case Transition.Animation:
                    TriggerAnimation(triggerName);
                    break;
            }
        }
        
        void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
                return;
            
            foreach(Graphic g in targetGraphics)
                g.CrossFadeColor (targetColor, instant ? 0f : colors.fadeDuration, true, true);
        }

        void DoSpriteSwap(Sprite newSprite)
        {
            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }

        void TriggerAnimation(string triggername)
        {
            if (transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(animationTriggers.normalTrigger);
            animator.ResetTrigger(animationTriggers.highlightedTrigger);
            animator.ResetTrigger(animationTriggers.pressedTrigger);
            animator.ResetTrigger(animationTriggers.selectedTrigger);
            animator.ResetTrigger(animationTriggers.disabledTrigger);

            animator.SetTrigger(triggername);
        }
    }
}