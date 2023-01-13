using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Poll : UnlockActivity{

    [SerializeField] float optionsSlideInterval;  //seconds between each answer/result sliding in or out from screen

    [Header("Question")]    
    [SerializeField] UITransitionFade   questionFade;    
    [SerializeField] UITransitionSlide  questionTitleSlide;
    [SerializeField] TextMeshProUGUI    questionText;
    [SerializeField] TMP_TypeOutEffect  questionTyping;    

    [SerializeField] Poll_Answer[]      answers;
    [SerializeField] float              answerSlideInDelay;
    [SerializeField] float              answerSlideOutDelay;
    [SerializeField] UITransitionFade   backButtonFade;

    [Header("Results")]
    [SerializeField] UITransitionFade   resultsFade;
    [SerializeField] UITransitionSlide  resultsTitleSlide;
    [SerializeField] Poll_Result[]      results;
    [SerializeField] float              resultsSlideInDelay;
    [SerializeField] float              resultsPercentageDelay;


    PollQuestionData pollData;
    const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    int[]   tallies; //stores count of each answer
    int     total;   //total count of all answers



    void OnEnable(){

        questionTyping.onFinish.AddListener(OnFinishedTypingQuestion);

        for (int i = 0; i < answers.Length; i++)
            answers[i].onSelectAnswer += OnSelectAnswer;
        
    }
    void OnDisable(){

        questionTyping.onFinish.RemoveListener(OnFinishedTypingQuestion);

        for (int i = 0; i < answers.Length; i++)
            answers[i].onSelectAnswer -= OnSelectAnswer;
    }




    public override void Open(StickerSpawner spawner){

        base.Open(spawner);

        //enable question and back button, and disable results
        questionFade.gameObject.SetActive(true);
        questionFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        questionFade.TransitionToEnd(true, questionFade.transitionTime, questionFade.delayTime);
        backButtonFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        backButtonFade.gameObject.SetActive(true);
        backButtonFade.TransitionToEnd(true);

        resultsFade.gameObject.SetActive(false);
        resultsTitleSlide.gameObject.SetActive(false);

        //assign data        
        pollData            = spawner.data.pollQuestion;
        questionText.text   = pollData.question;        
    }

    //fires once the question has finished typing
    void OnFinishedTypingQuestion(){

        //assign answers
        for (int i = 0; i < answers.Length && i < pollData.answers.Length; i++)
            answers[i].Initialize(letters[i].ToString(), pollData.answers[i]);

        //animate answers onto screen
        List<UITransitionSlide> slides = new List<UITransitionSlide>(0);
        foreach(Poll_Answer a in answers)
            slides.Add(a.slide);

        StopAllCoroutines();
        StartCoroutine(SlideInRoutine(slides.ToArray(), answerSlideInDelay, optionsSlideInterval));
    }

    
    //slide in using internal values of each slide component
    IEnumerator SlideInRoutine (UITransitionSlide[] slides, float delay, float interval){

        if(delay > 0)
            yield return new WaitForSeconds(delay);
        
        //slide each one-at-a-time
        foreach(UITransitionSlide s in slides)
        {
            s.TransitionToEnd(true);
            if(interval > 0)
                yield return new WaitForSeconds(interval);
        }
    }

    //slide in using manual values along a chosen axis
    IEnumerator SlideRoutine (UITransitionSlide[] slides, string axis, float pos, float delay, float interval){

        if(delay > 0)
            yield return new WaitForSeconds(delay);
        
        //slide each one-at-a-time
        foreach(UITransitionSlide s in slides)
        {
            Vector3 endPos = ReplaceAxis(s.rectTransform.anchoredPosition3D, axis, pos);
            s.TransitionToPosition(endPos);
            if(interval > 0)
                yield return new WaitForSeconds(interval);
        }
    }

    void OnSelectAnswer(Poll_Answer answer){

        //increment tally for selected answer and save to disk
        int tally = GetTally(pollData, answer);
        SetTally(pollData, answer, tally + 1);
        PlayerPrefs.Save();

        //how results
        StopAllCoroutines();
        StartCoroutine(GoToResultsRoutine());
    }

    IEnumerator GoToResultsRoutine(){

        //disable selecting any more buttons andfade out back button
        questionFade.canvasGroup.blocksRaycasts = false;
        backButtonFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        if(backButtonFade.gameObject.activeInHierarchy) backButtonFade.TransitionToStart(true);


        //get tallies ans width of window
        float   width       = (transform as RectTransform).rect.width;
        int[]   tallies     = new int[answers.Length];
        int     total       = 0;
        string  debugLog    = string.Empty;
        for (int i = 0; i < answers.Length; i++)
        {
            tallies[i] = GetTally(pollData, answers[i]);
            total += tallies[i];

            debugLog += string.Format("{0} Tally: {1}\n", GetPrefId(pollData, answers[i]), tallies[i]);
        }
        debugLog += string.Format("Total: {0}", total);
        Debug.Log(debugLog);

        //slide out question title to the left and slide in results title from the right
        yield return new WaitForSeconds(1f);
        questionTitleSlide.TransitionToPosition(ReplaceAxis(questionTitleSlide.rectTransform.anchoredPosition3D, "x", -width));
        resultsTitleSlide.gameObject.SetActive(true);
        resultsTitleSlide.rectTransform.anchoredPosition3D = ReplaceAxis(resultsTitleSlide.rectTransform.anchoredPosition3D, "x", width);
        resultsTitleSlide.TransitionToPosition(ReplaceAxis(resultsTitleSlide.rectTransform.anchoredPosition3D, "x", 0));


        //slide out answers to the left
        yield return new WaitForSeconds(answerSlideOutDelay);
        List<UITransitionSlide> slides = new List<UITransitionSlide>(0);
        foreach(Poll_Answer a in answers)
            slides.Add(a.slide);
        StartCoroutine(SlideRoutine(slides.ToArray(), "x", -width, 0, optionsSlideInterval));
        

        //assign and slide in results from the right
        yield return new WaitForSeconds(resultsSlideInDelay);
        resultsFade.    gameObject.SetActive(true);
        continueButton. gameObject.SetActive(false);

        slides = new List<UITransitionSlide>(0);
        for (int i = 0; i < results.Length; i++)
        {
            RectTransform rect = results[i].slide.rectTransform;
            rect.anchoredPosition3D = ReplaceAxis(rect.anchoredPosition3D, "x", width);
            results[i].Initialize(pollData.answers[i], (float)tallies[i]/(float)total);
            slides.Add(results[i].slide);
        }
        StartCoroutine(SlideRoutine(slides.ToArray(), "x", 0, 0, optionsSlideInterval));


        //animate result percentages
        yield return new WaitForSeconds(resultsPercentageDelay);
        foreach(Poll_Result r in results)
            r.AnimateResult(1f);        

        //activate continue button
        yield return new WaitForSeconds(2f);
        continueButton.gameObject.SetActive(true);
    }



    string GetPrefId(PollQuestionData question, Poll_Answer answer){

        return string.Format("{0}_{1}", question.name, answer.answerId);
    }

    int GetTally(PollQuestionData question, Poll_Answer answer){

        return PlayerPrefs.GetInt(GetPrefId(question, answer), 0);
    }

    void SetTally(PollQuestionData question, Poll_Answer answer, int value){

        PlayerPrefs.SetInt(GetPrefId(question, answer), value);
    }

    Vector3 ReplaceAxis(Vector3 input, string axis, float value){

        Vector3 output = input;

        if(axis.ToLower().Contains("x"))
            output.x = value;
        if(axis.ToLower().Contains("y"))
            output.y = value;
        if(axis.ToLower().Contains("z"))
            output.z = value;

        return output;
    }
}