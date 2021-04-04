using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class TeacherFireBase : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //User Data variables
    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_InputField xpField;
    public TMP_InputField killsField;
    public TMP_InputField masteryField;
    public GameObject scoreElement;
    public Transform scoreboardContent;

    //Question and Answers variables
    [Header("QnA")]
    public InputField QuestionInputField;
    public InputField AnswerInputField1;
    public InputField AnswerInputField2;
    public InputField AnswerInputField3;
    public TMP_Text Warning_Text;

    public GameObject AddQuestionPanel;
    public GameObject OptionSelectionPanel;
    public GameObject FunctionSelectionPanel;
    public GameObject InfoBox;
    public Button SubmitButton;

    string StudentSearched;
    public InputField SearchBar;
    public GameObject OverviewPanel;
    public GameObject StudentPanel;

    public Text OODP_S1_points;
    public Text OODP_S2_points;
    public Text OODP_S3_points;

    public Text SE_S1_points;
    public Text SE_S2_points;
    public Text SE_S3_points;

    public Text SSAD_S1_points;
    public Text SSAD_S2_points;
    public Text SSAD_S3_points;

    public Text OODP_S1_stars;
    public Text OODP_S2_stars;
    public Text OODP_S3_stars;

    public Text SE_S1_stars;
    public Text SE_S2_stars;
    public Text SE_S3_stars;

    public Text SSAD_S1_stars;
    public Text SSAD_S2_stars;
    public Text SSAD_S3_stars;

    public Text Name;

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }


    public void ClearQuestionAndAnswersFields()
    {
        QuestionInputField.Select();
        QuestionInputField.text = "";
        AnswerInputField1.Select();
        AnswerInputField1.text = "";
        AnswerInputField2.Select();
        AnswerInputField2.text = "";
        AnswerInputField3.Select();
        AnswerInputField3.text = "";
    }

    //Function for the register button
    public void SubmitButtonMethod()
    {
        Debug.Log("Reached here!");
        StartCoroutine(createQuestionsAndAnswers(QuestionInputField.text, AnswerInputField1.text, AnswerInputField2.text, AnswerInputField3.text));
    }

    private IEnumerator createQuestionsAndAnswers(string _question, string _answer1, string _answer2, string _answer3)
    {
        //need integrate with jh one!
        int worldNumber = QuestionAdder.World;
        int sectionNumber = QuestionAdder.Section;
        string difficulty = QuestionAdder.Difficulty;
        Debug.Log(worldNumber);
        Debug.Log(sectionNumber);
        Debug.Log(difficulty);

        //Set the currently logged in user mastery
        var DBTask = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        DataSnapshot snapshot = DBTask.Result;
        int length = (int)snapshot.ChildrenCount;

        var qnTask = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).Child($"{length + 1}").Child("Question").SetValueAsync(_question);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (qnTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {qnTask.Exception}");

            string message = "Missing Question!";
            if (string.IsNullOrWhiteSpace(QuestionInputField.text))
            {
                Warning_Text.text = message;
                SubmitButton.interactable = false;
            }

        }

        var a1Task = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).Child($"{length + 1}").Child("A1").SetValueAsync(_answer1);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (a1Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a1Task.Exception}");

            /*string message = "Missing Answer!";
            if (string.IsNullOrEmpty(AnswerInputField1.text))
            {
                Warning_Text.text = message;
            }*/
        }

        var a2Task = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).Child($"{length + 1}").Child("A2").SetValueAsync(_answer2);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (a2Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a2Task.Exception}");

            /*string message = "Missing Answer!";
            if (string.IsNullOrEmpty(AnswerInputField2.text))
            {
                Warning_Text.text = message;
            }*/
        }

        var a3Task = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).Child($"{length + 1}").Child("A3").SetValueAsync(_answer3);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (a3Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a3Task.Exception}");

            /*string message = "Missing Answer!";
            if (string.IsNullOrEmpty(AnswerInputField3.text))
            {
                Warning_Text.text = message;
            }*/
        }

        ClearQuestionAndAnswersFields();

        AddQuestionPanel.gameObject.SetActive(false);
        OptionSelectionPanel.gameObject.SetActive(false);
        FunctionSelectionPanel.gameObject.SetActive(true);
    }

    public void SearchStudent()
    {
        StudentSearched = SearchBar.text;
        Debug.Log(StudentSearched);
        SearchBar.text = "";
        StartCoroutine(ShowInformation(StudentSearched));
    }

    private IEnumerator ShowInformation(string _StudentID)
    {

        //Get Student
        var DBTask = DBreference.Child("users").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        Debug.Log("something");

        if (DBTask.Exception != null)
        {
            Debug.Log("Yea");
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.Log("Yo");
            OODP_S1_points.text = "0";
            OODP_S2_points.text = "0";
            OODP_S3_points.text = "0";
            SE_S1_points.text = "0";
            SE_S2_points.text = "0";
            SE_S3_points.text = "0";
            SSAD_S1_points.text = "0";
            SSAD_S2_points.text = "0";
            SSAD_S3_points.text = "0";

            OODP_S1_stars.text = "0";
            OODP_S2_stars.text = "0";
            OODP_S3_stars.text = "0";
            SE_S1_stars.text = "0";
            SE_S2_stars.text = "0";
            SE_S3_stars.text = "0";
            SSAD_S1_stars.text = "0";
            SSAD_S2_stars.text = "0";
            SSAD_S3_stars.text = "0";
        }
        else
        {
            Debug.Log("hello");
            DataSnapshot snapshot = DBTask.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();

                int oodpS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{1}").Child("Points").Value.ToString());
                int oodpS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{2}").Child("Points").Value.ToString());
                int oodpS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{3}").Child("Points").Value.ToString());

                int seS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{1}").Child("Points").Value.ToString());
                int seS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{2}").Child("Points").Value.ToString());
                int seS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{3}").Child("Points").Value.ToString());

                int ssadS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{1}").Child("Points").Value.ToString());
                int ssadS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{2}").Child("Points").Value.ToString());
                int ssadS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{3}").Child("Points").Value.ToString());

                int oodpS1stars = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{1}").Value.ToString());
                int oodpS2stars = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{2}").Value.ToString());
                int oodpS3stars = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{3}").Value.ToString());

                int seS1stars = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{1}").Value.ToString());
                int seS2stars = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{2}").Value.ToString());
                int seS3stars = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{3}").Value.ToString());

                int ssadS1stars = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{1}").Value.ToString());
                int ssadS2stars = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{2}").Value.ToString());
                int ssadS3stars = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{3}").Value.ToString());

                if (username == _StudentID)
                {
                    OODP_S1_points.text = oodpS1pts.ToString();
                    OODP_S2_points.text = oodpS2pts.ToString();
                    OODP_S3_points.text = oodpS3pts.ToString();

                    SE_S1_points.text = seS1pts.ToString();
                    SE_S2_points.text = seS2pts.ToString();
                    SE_S3_points.text = seS3pts.ToString();

                    SSAD_S1_points.text = ssadS1pts.ToString();
                    SSAD_S2_points.text = ssadS2pts.ToString();
                    SSAD_S3_points.text = ssadS3pts.ToString();

                    OODP_S1_stars.text = oodpS1stars.ToString();
                    OODP_S2_stars.text = oodpS2stars.ToString();
                    OODP_S3_stars.text = oodpS3stars.ToString();

                    SE_S1_stars.text = seS1stars.ToString();
                    SE_S2_stars.text = seS2stars.ToString();
                    SE_S3_stars.text = seS3stars.ToString();

                    SSAD_S1_stars.text = ssadS1stars.ToString();
                    SSAD_S2_stars.text = ssadS2stars.ToString();
                    SSAD_S3_stars.text = ssadS3stars.ToString();

                    Name.text = username;
                    break;
                }
                OODP_S1_points.text = "0"; //invalid user
                OODP_S2_points.text = "0";
                OODP_S3_points.text = "0";
                SE_S1_points.text = "0";
                SE_S2_points.text = "0";
                SE_S3_points.text = "0";
                SSAD_S1_points.text = "0";
                SSAD_S2_points.text = "0";
                SSAD_S3_points.text = "0";

                OODP_S1_stars.text = "0";
                OODP_S2_stars.text = "0";
                OODP_S3_stars.text = "0";
                SE_S1_stars.text = "0";
                SE_S2_stars.text = "0";
                SE_S3_stars.text = "0";
                SSAD_S1_stars.text = "0";
                SSAD_S2_stars.text = "0";
                SSAD_S3_stars.text = "0";
                Name.text = "Invalid User";
            }
        }
    }
}

///// FIX ERROR HANDLING!!!!!