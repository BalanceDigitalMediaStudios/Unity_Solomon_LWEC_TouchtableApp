using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(VirtualCursor), true)]
public class VirtualCursorEditor : Editor
{
    SerializedProperty m_raycaster;
    SerializedProperty m_cursor;

    SerializedProperty m_releaseScale;
    SerializedProperty m_pressScale;    

    SerializedProperty m_hoveredObject;
    SerializedProperty m_selectedObject;

    SerializedProperty m_hoverFill;
    SerializedProperty m_fillRate;


    protected virtual void OnEnable(){

        m_raycaster         = serializedObject.FindProperty("_raycaster");
        m_cursor            = serializedObject.FindProperty("cursor");

        m_releaseScale      = serializedObject.FindProperty("releaseScale");
        m_pressScale        = serializedObject.FindProperty("pressScale");

        m_hoveredObject     = serializedObject.FindProperty("hoveredObject");
        m_selectedObject    = serializedObject.FindProperty("selectedObject");

        m_hoverFill         = serializedObject.FindProperty("hoverFill");
        m_fillRate          = serializedObject.FindProperty("fillRate");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        EditorGUILayout.PropertyField(m_raycaster);
        EditorGUILayout.PropertyField(m_cursor);

        EditorGUILayout.PropertyField(m_releaseScale);
        EditorGUILayout.PropertyField(m_pressScale);


        EditorGUILayout.PropertyField(m_fillRate);
        EditorGUILayout.PropertyField(m_hoverFill, new GUIContent("Fill Image"));

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(m_hoveredObject);
        EditorGUILayout.PropertyField(m_selectedObject);
        EditorGUI.EndDisabledGroup();


        if(Application.isPlaying)
        {
            EditorGUILayout.Separator();

            VirtualCursor t = (VirtualCursor)target;

            float viewWidth = (EditorGUIUtility.currentViewWidth - 25)/2;

            EditorGUILayout.BeginHorizontal();            

            if(GUILayout.Button("Simluate Pointer Down", GUILayout.Width(viewWidth)))
                t.SimulatePointerDown();
            if(GUILayout.Button("Simluate Pointer Up", GUILayout.Width(viewWidth)))
                t.SimulatePointerUp();

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("Simluate Short Click (0.1 sec)", GUILayout.Width(viewWidth)))
                t.SimulateClick(.1f);
            if(GUILayout.Button("Simluate Long Click (0.5 sec)", GUILayout.Width(viewWidth)))
                t.SimulateClick(.5f);
            
            EditorGUILayout.EndHorizontal();
        }
        

        serializedObject.ApplyModifiedProperties();
    }
}
#endif



public class VirtualCursor : SimpleSingleton<VirtualCursor>
{
    /* private static VirtualCursor _instance;
    public static VirtualCursor Instance{

        get{
            
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<VirtualCursor>(true);
            return _instance;
        }
    } */


    [Tooltip("If unassigned, will grab the nearest parent object that has one")]
    [SerializeField]GraphicRaycaster    _raycaster;
    GraphicRaycaster                    raycaster
    {
        get
        {
            if(_raycaster == null)
                _raycaster = GetComponentInParent<GraphicRaycaster>();
            return _raycaster;
        }
        set
        {
            _raycaster = value;
        }
    }
    PointerEventData    _eventData;
    PointerEventData    eventData
    {
        get
        {
            if(_eventData == null)
                _eventData = new PointerEventData(EventSystem.current);
            return _eventData;
        }
    }
    //top most canvas
    Canvas _canvas;
    Canvas canvas
    {
        get
        {
            if(_canvas == null)
            {
                Canvas[] parentCanvases = GetComponentsInParent<Canvas>();
                if(parentCanvases != null && parentCanvases.Length > 0)
                    _canvas = parentCanvases[parentCanvases.Length - 1];
            }
            return _canvas;
        }
    }
    //we need this to get an accurate screen position using RectTransformUtility.WorldToScreenPoint as it needs to be null in ScreenSpaceOverlay mode
    Camera _canvasCamera;
    Camera canvasCamera{

        get{
            if(_canvasCamera == null && canvas != null)
            {
                _canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            }
            return _canvasCamera;
        }
    }

    [SerializeField]float releaseScale  = 1;
    [SerializeField]float pressScale    = .9f;

    public Image cursor;

    [SerializeField]Image hoverFill;
    [SerializeField]float fillRate;


    [Tooltip("Current button fill script we are working with")]
    [SerializeField][ReadOnly]ButtonFill buttonFill;

    [SerializeField]GameObject hoveredObject;
    [SerializeField]GameObject selectedObject;


    void Awake(){ ResetFill(); }
    
    
    //clears any selection and hover states
    void OnDisable(){

        StopAllCoroutines();
        ResetFill();
        


        transform.localScale = Vector3.one * releaseScale;


        /* if(selectedObject != null)
        {
            ExecuteEvents.Execute<IDeselectHandler> (selectedObject,        eventData, ExecuteEvents.deselectHandler);
            ExecuteEvents.Execute<IPointerUpHandler>(selectedObject,        eventData, ExecuteEvents.pointerUpHandler);
            selectedObject = null;
        } */
        if(hoveredObject != null)
        {
            ExecuteEvents.Execute<IPointerExitHandler>(hoveredObject, eventData, ExecuteEvents.pointerExitHandler);
            hoveredObject = null;
        }
    }


    public void Activate(bool activate){

        gameObject.SetActive(activate);
    }
    public bool IsActive(){

        return gameObject.activeInHierarchy;
    }


    //updates hovered state
    void Update(){

        eventData.position = RectTransformUtility.WorldToScreenPoint(canvasCamera, transform.position);
        List<RaycastResult> results = new List<RaycastResult>(0);  


        raycaster.Raycast(eventData, results);
        if(results.Count > 0)
        {
            if(hoveredObject != results[0].gameObject)
            {
                if(hoveredObject != null)
                {
                    ExecuteEvents.Execute<IPointerExitHandler>(hoveredObject, eventData, ExecuteEvents.pointerExitHandler);
                    ResetFill();
                }
                
                hoveredObject = results[0].gameObject;
                ExecuteEvents.Execute<IPointerEnterHandler>(hoveredObject, eventData, ExecuteEvents.pointerEnterHandler);


                //fill effects
                Button b    = hoveredObject.GetComponent<Button>();
                buttonFill  = hoveredObject.GetComponent<ButtonFill>();
                if(b != null  && b.interactable)
                    Fill(fillRate);
            }
        }
        else if(hoveredObject != null)
        {
            ExecuteEvents.Execute<IPointerExitHandler>(hoveredObject, eventData, ExecuteEvents.pointerExitHandler);
            ResetFill();
            hoveredObject = null;
        }
    }



    public void SimulatePointerDown(){

        transform.localScale = Vector3.one * pressScale;

        if(selectedObject != hoveredObject)
        {
            ExecuteEvents.Execute<IDeselectHandler>(selectedObject, eventData, ExecuteEvents.deselectHandler);
            selectedObject = null;
        }

        if(hoveredObject)
        {
            selectedObject = hoveredObject;
            ExecuteEvents.Execute<IPointerDownHandler>  (selectedObject, eventData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute<ISelectHandler>       (selectedObject, eventData, ExecuteEvents.selectHandler);
        }

    }
    public void SimulatePointerUp(){

        transform.localScale = Vector3.one * releaseScale;

        if(selectedObject)
        {
            if(hoveredObject == selectedObject)
                ExecuteEvents.Execute<IPointerClickHandler>(selectedObject, eventData, ExecuteEvents.pointerClickHandler);
            
            ExecuteEvents.Execute<IPointerUpHandler>(selectedObject, eventData, ExecuteEvents.pointerUpHandler);
        }
    }


    public float SimulateClick(float release = .1f){

        StopAllCoroutines();
        StartCoroutine(SimulateClickRoutine(release));

        //return how long it will take
        return release;
    }
    IEnumerator SimulateClickRoutine(float release){

        SimulatePointerDown();
        if(release > 0)
            yield return new WaitForSeconds(release);
        SimulatePointerUp();
    }


    public float SimulateMultiClick(int numClicks, float interval, float release = .1f){
        
        StopAllCoroutines();
        StartCoroutine(SimulateMultiClickRoutine(numClicks, interval, release));

        //return how long it will take
        return (numClicks * release) + ((numClicks - 1) * interval);
    }
    IEnumerator SimulateMultiClickRoutine(int numClicks, float interval, float release){

        for(int i = 0; i < numClicks; i++)
        {
            yield return StartCoroutine(SimulateClickRoutine(release));

            //dont wait for interval on the last click
            if(interval > 0 && i < numClicks - 1)
                yield return new WaitForSeconds(interval);
        }
    }






    void Fill(float rate){

        StopAllCoroutines();
        StartCoroutine(FillRoutine(rate));
    }
    IEnumerator FillRoutine(float rate){

        float current = 0;        

        do
        {
            current = Mathf.Clamp01(current + rate * Time.deltaTime);

            if(hoverFill != null)
                hoverFill.fillAmount = current;

            if(buttonFill != null)
                buttonFill.SetFill(current);

            yield return null;

        }while(0 < current && current < 1);

        if(current == 1)
            OnFillComplete();
    }

    void ResetFill(){

        StopAllCoroutines();
        if(hoverFill != null)
            hoverFill.fillAmount = 0;
        if(buttonFill != null)
            buttonFill.SetFill(0);
    }


    void OnFillComplete(){

        Debug.LogFormat("Hovered to select Button: {0}", hoveredObject.name);
        VirtualCursor.instance.SimulateClick();
    }
}