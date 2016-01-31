using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DialogDatabaseWindow : EditorWindow
{

    [MenuItem("GGJ16/DialogDB")]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(DialogDatabaseWindow), false, "Cinematic Dialog DB");
    }

    [MenuItem("GGJ16/Create Dialog DB")]
    static void CreateDialogDatabase()
    {
        string path = EditorUtility.SaveFilePanelInProject("Create new Dialog Database", "dialogDB.asset", "asset", "");
        if (path == "") return;

        DialogDatabase itemDB = ScriptableObject.CreateInstance<DialogDatabase>();
        AssetDatabase.CreateAsset(itemDB, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = itemDB;
    }

    DialogDatabase database;
    Vector2 scrollPos = Vector2.zero;
    Vector2 scrollPos2 = Vector2.zero;

    Intervention selectedIntervention = null;
    Question selectedQuestion = null;
    Answer selectedAnswer = null;

    int tempInt = 0;

    void OnEnable()
    {
    }

    void OnGUI()
    {
        if (database == null) database = (DialogDatabase)AssetDatabase.LoadAssetAtPath("Assets/Binaries/Databases/dialogDB.asset", typeof(DialogDatabase));
        if (database == null) return;
        if (database.questions == null) database.questions = new System.Collections.Generic.List<Question>();
        if (database.answers == null) database.answers = new System.Collections.Generic.List<Answer>();
        if (database.interventions == null) database.interventions = new System.Collections.Generic.List<Intervention>();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("Box", GUILayout.Width(256));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (GUILayout.Button("Export to JSON"))
        {
            database.Export();
        }
        if (GUILayout.Button("Import from JSON"))
        {
            database.Import();
        }

        EditorGUILayout.LabelField("Interventions");
        GUI.SetNextControlName("Add");
        if (GUILayout.Button("Add New Intervention"))
        {
            AddIntervention();
        }

        for (int i = 0; i < database.interventions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            Intervention item = database.interventions[i];
            string id_string = "ID " + item.id_intervention;
            if (item == selectedIntervention)
            {
                GUILayout.Label(id_string);
            }
            else
            {
                if (GUILayout.Button(id_string))
                {
                    selectedAnswer = null;
                    selectedQuestion = null;
                    selectedIntervention = item;
                    Refocus();
                }
            }

            if (GUILayout.Button("X", GUILayout.MaxWidth(24)))
            {
                selectedIntervention = null;
                database.interventions.Remove(item);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("Questions");
        GUI.SetNextControlName("AddQ");
        if (GUILayout.Button("Add New Question"))
        {
            AddQuestion();
        }

        for (int i = 0; i < database.questions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            Question item = database.questions[i];
            string id_string = "ID " + item.id_dialog;
            if (item == selectedQuestion)
            {
                GUILayout.Label(id_string);
            }
            else
            {
                if (GUILayout.Button(id_string))
                {
                    selectedIntervention = null;
                    selectedAnswer = null;
                    selectedQuestion = item;
                    Refocus();
                }
            }

            if (GUILayout.Button("X", GUILayout.MaxWidth(24)))
            {
                selectedQuestion = null;
                database.questions.Remove(item);
            }
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.LabelField("Answers");
        GUI.SetNextControlName("AddA");
        if (GUILayout.Button("Add New Answer"))
        {
            AddAnswer();
        }

        for (int i = 0; i < database.answers.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            Answer item = database.answers[i];
            string id_string = "ID " + item.id_dialog;
            if (item == selectedAnswer)
            {
                GUILayout.Label(id_string);
            }
            else
            {
                if (GUILayout.Button(id_string))
                {
                    selectedIntervention = null;
                    selectedAnswer = item;
                    selectedQuestion = null;
                    Refocus();
                }
            }

            if (GUILayout.Button("X", GUILayout.MaxWidth(24)))
            {
                selectedAnswer = null;
                database.answers.Remove(item);
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2);
        EditorGUILayout.BeginVertical("Box");

        if (selectedIntervention != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Unique ID", GUILayout.Width(128));
            EditorGUILayout.LabelField(selectedIntervention.id_intervention.ToString(), GUILayout.Width(256));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Questions:");
            for (int q = 0; q < selectedIntervention.questions_ids.Count; q++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID " + selectedIntervention.questions_ids[q]);
                if (GUILayout.Button("X"))
                { selectedIntervention.questions_ids.RemoveAt(q); break; }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            tempInt = EditorGUILayout.IntField(tempInt);
            if (GUILayout.Button("Add Question ID")) {
                selectedIntervention.questions_ids.Add(tempInt);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Answer Scores:");
            for (int s = 0; s < selectedIntervention.scores.Count; s++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID " + s);
                selectedIntervention.scores[s] = EditorGUILayout.IntField(selectedIntervention.scores[s]);
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUI.changed) EditorUtility.SetDirty(database);
            GUILayout.FlexibleSpace();
        }
        else if (selectedQuestion != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Unique ID", GUILayout.Width(128));
            EditorGUILayout.LabelField(selectedQuestion.id_dialog.ToString(), GUILayout.Width(256));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Character ID", GUILayout.Width(128));
            selectedQuestion.id_character = (Dialog.CHARACTER) EditorGUILayout.EnumPopup(selectedQuestion.id_character);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text", GUILayout.Width(128));
            selectedQuestion.text = EditorGUILayout.TextField(selectedQuestion.text);
            EditorGUILayout.EndHorizontal();
            
            if (GUI.changed) EditorUtility.SetDirty(database);
            GUILayout.FlexibleSpace();
        }
        else if (selectedAnswer != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Unique ID", GUILayout.Width(128));
            EditorGUILayout.LabelField(selectedAnswer.id_dialog.ToString(), GUILayout.Width(256));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Character ID", GUILayout.Width(128));
            selectedAnswer.id_character = (Dialog.CHARACTER)EditorGUILayout.EnumPopup(selectedAnswer.id_character);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text", GUILayout.Width(128));
            selectedAnswer.text = EditorGUILayout.TextField(selectedAnswer.text);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Short Answer", GUILayout.Width(128));
            selectedAnswer.shortAnswer = EditorGUILayout.TextField(selectedAnswer.shortAnswer);
            EditorGUILayout.EndHorizontal();

            if (GUI.changed) EditorUtility.SetDirty(database);
            GUILayout.FlexibleSpace();
        }
        else 
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select an item");
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();


        EditorGUILayout.EndHorizontal();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }


    void AddIntervention()
    {
        selectedIntervention = database.AddIntervention();
        Refocus();
    }

    void AddQuestion()
    {
        selectedQuestion = database.AddQuestion();
        Refocus();
    }

    void AddAnswer()
    {
        selectedAnswer = database.AddAnswer();
        Refocus();
    }

    void Refocus()
    {
        if (selectedIntervention != null)
            GUI.FocusControl("Add");
        else if (selectedQuestion != null) GUI.FocusControl("AddQ");
        else GUI.FocusControl("AddA");
    }
}
