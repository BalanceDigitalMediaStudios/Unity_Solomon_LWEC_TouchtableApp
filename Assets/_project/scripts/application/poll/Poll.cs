using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Poll : UnlockActivity{

    [SerializeField] float optionsSlideInterval;  //seconds between each answer/result sliding in or out from screen

    [Header("Upload")]
    [SerializeField] string             url;
    [SerializeField] string             token;
    [SerializeField] float              minimumLoadingTime;
    [SerializeField] UITransitionFade   loadingFade;

    [Header("Question")]
    [SerializeField] UITransitionFade   questionFade;
    [SerializeField] UITransitionSlide  questionTitleSlide;
    [SerializeField] UITransitionSlide  resultsTitleSlide;
    [SerializeField] TextMeshProUGUI    questionText;
    [SerializeField] TMP_TypeOutEffect  questionTyping;
    [SerializeField] UITransitionFade   backButtonFade;    

    [Header("Answers")]
    [SerializeField] CanvasGroup        answersCG;
    [SerializeField] Poll_Answer[]      answers;
    [SerializeField] float              answerSlideInDelay;
    [SerializeField] float              answerSlideOutDelay;

    [Header("Results")]
    [SerializeField] GameObject         resultsGroup;
    [SerializeField] Poll_Result[]      results;
    [SerializeField] float              resultsSlideInDelay;
    [SerializeField] float              resultsPercentageDelay;

    [Header("Fail")]
    [SerializeField] UITransitionFade   failFade;
    [SerializeField] TMP_TypeOutEffect  failMessageTyping;
    [SerializeField] Button             failContinueButton;


    PollQuestionData pollData;
    const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


    void OnEnable(){

        questionTyping.     onFinish.AddListener(OnQuestionTyped);
        failMessageTyping.  onFinish.AddListener(OnFailMessageTyped);

        failContinueButton. onClick.AddListener(Close);

        for (int i = 0; i < answers.Length; i++)
            answers[i].onSelectAnswer += OnSelectAnswer;
        
    }
    void OnDisable(){

        questionTyping.     onFinish.RemoveListener(OnQuestionTyped);
        failMessageTyping.  onFinish.RemoveListener(OnFailMessageTyped);

        for (int i = 0; i < answers.Length; i++)
            answers[i].onSelectAnswer -= OnSelectAnswer;
    }




    public override void Open(StickerSpawner spawner){

        base.Open(spawner);

        //enable question, answers, and back button.  Disable results, loading graphic, and fail screen.
        questionFade.   gameObject.SetActive(true);
        answersCG.      blocksRaycasts = true;
        backButtonFade. blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        backButtonFade. gameObject.SetActive(true);
        backButtonFade.TransitionToEnd(true);

        resultsTitleSlide.  gameObject.SetActive(false);
        resultsGroup.       gameObject.SetActive(false);        
        failFade.           gameObject.SetActive(false);
        loadingFade.        gameObject.SetActive(false);


        //assign data        
        pollData            = spawner.data.pollQuestion;
        questionText.text   = pollData.question;        
    }

    //fires once the question has finished typing
    void OnQuestionTyped(){

        //assign answers
        for (int i = 0; i < answers.Length && i < pollData.answers.Length; i++)
            answers[i].Initialize(letters[i].ToString(), pollData.answers[i]);        

        //slide in answers
        StopAllCoroutines();
        StartCoroutine(SlideInRoutine(answers.Select(x => x.slide).ToArray(), answerSlideInDelay, optionsSlideInterval));
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

    Vector3 ReplaceAxis(Vector3 input, string axis, float value){

        Vector3 output = input;

        if(axis.ToLower().Contains("x")) output.x = value;
        if(axis.ToLower().Contains("y")) output.y = value;
        if(axis.ToLower().Contains("z")) output.z = value;

        return output;
    }

    
    void OnSelectAnswer(Poll_Answer answer){

        //disable selecting any more buttons and fade out back button
        answersCG.blocksRaycasts = false;
        backButtonFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        if(backButtonFade.gameObject.activeInHierarchy) backButtonFade.TransitionToStart(true);

        //fade in loading graphic
        loadingFade.gameObject.SetActive(true);

        UploadAnswer(pollData, answer, url, OnSuccess, OnFail);
    }
    
    /* void OnSelectAnswer(Poll_Answer answer){

        //disable selecting any more buttons and fade out back button
        answersCG.blocksRaycasts = false;
        backButtonFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        if(backButtonFade.gameObject.activeInHierarchy) backButtonFade.TransitionToStart(true);

        //increment tally for selected answer and save to disk
        int tally = GetTally(pollData, answer);
        SetTally(pollData, answer, tally + 1);
        PlayerPrefs.Save();

        //show results
        StopAllCoroutines();
        StartCoroutine(GoToResultsRoutine());
    }
    string  GetPrefId(PollQuestionData question, Poll_Answer answer)            { return string.Format("{0}_{1}", question.name, answer.answerId); }
    int     GetTally (PollQuestionData question, Poll_Answer answer)            { return PlayerPrefs.GetInt(GetPrefId(question, answer), 0); }
    void    SetTally (PollQuestionData question, Poll_Answer answer, int value) { PlayerPrefs.SetInt(GetPrefId(question, answer), value); } */

    

    void UploadAnswer(PollQuestionData question, Poll_Answer answer, string url, System.Action<string> onSuccess, System.Action<string> onFail){
        
        WWWForm form = new WWWForm();
        form.AddField("token",      token);
        form.AddField("questionId", question.name);
        form.AddField("answerId",   answer.answerId);

        Debug.LogFormat("Form upload parameters:\n{0}{1}\n{2}{3}\n{4}{5}",
            string.Format("   {0, -12}", "token:"),         FormUploader.Colorize(token, "yellow"),
            string.Format("   {0, -12}", "questionId:"),    FormUploader.Colorize(question.name, "yellow"),
            string.Format("   {0, -12}", "answerId:"),      FormUploader.Colorize(answer.answerId, "yellow"));
        FormUploader.instance.Upload(form, url, minimumLoadingTime, onSuccess, onFail);
    }


    void OnSuccess(string response){

        //parse tallies from response
        string[]    split   = response.Split(',', 4);
        int[]       tallies = new int[4];
        int         total   = 0;
        for (int i = 0; i < split.Length; i++)
        {
            int parsed;
            if(int.TryParse(split[i], out parsed))
                tallies[i] = parsed;
            total += tallies[i];
        }

        StopAllCoroutines();
        StartCoroutine(ShowResultsRoutine(tallies, total));
    }
    IEnumerator ShowResultsRoutine(int[] tallies, int total){

        //get width of window for use in animations
        float width = (transform as RectTransform).rect.width;

        //fade out loading graphic
        loadingFade.TransitionToStart(true);

        //slide out question title to the left and slide in results title from the right
        questionTitleSlide.TransitionToPosition(ReplaceAxis(questionTitleSlide.rectTransform.anchoredPosition3D, "x", -width));
        resultsTitleSlide.gameObject.SetActive(true);
        resultsTitleSlide.rectTransform.anchoredPosition3D = ReplaceAxis(resultsTitleSlide.rectTransform.anchoredPosition3D, "x", width);
        resultsTitleSlide.TransitionToPosition(ReplaceAxis(resultsTitleSlide.rectTransform.anchoredPosition3D, "x", 0));


        //slide out answers to the left
        yield return new WaitForSeconds(answerSlideOutDelay);
        StartCoroutine(SlideRoutine(answers.Select(x => x.slide).ToArray(), "x", -width, 0, optionsSlideInterval));
        

        //activate, assign, and position results
        yield return new WaitForSeconds(resultsSlideInDelay);
        resultsGroup.   gameObject.SetActive(true);
        continueButton. gameObject.SetActive(false);
        for (int i = 0; i < results.Length; i++)
        {
            RectTransform rect = results[i].slide.rectTransform;
            rect.anchoredPosition3D = ReplaceAxis(rect.anchoredPosition3D, "x", width);
            results[i].Initialize(pollData.answers[i], (float)tallies[i]/(float)total);
        }

        //slide in results from the right
        StartCoroutine(SlideRoutine(results.Select(x => x.slide).ToArray(), "x", 0, 0, optionsSlideInterval));


        //animate result percentages
        yield return new WaitForSeconds(resultsPercentageDelay);
        foreach(Poll_Result r in results) r.AnimateResult(1f);        

        //activate continue button
        yield return new WaitForSeconds(2f);
        continueButton.gameObject.SetActive(true);
    }


    void OnFail(string error){

        //fade out question, and fade in fail screen
        questionFade.TransitionToStart(true);
        failFade.gameObject.SetActive(true);
        failContinueButton.gameObject.SetActive(false);
    }
    void OnFailMessageTyped(){

        failContinueButton.gameObject.SetActive(true);
    }
}