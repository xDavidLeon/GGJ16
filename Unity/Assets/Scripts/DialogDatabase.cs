using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class DialogDatabase : ScriptableObject
{
    public List<Intervention> interventions = new List<Intervention>();
    public List<Question> questions = new List<Question>();
    public List<Answer> answers = new List<Answer>();

    public void OnEnable()
    {
    }

    public Intervention AddIntervention()
    {
        Intervention item = new Intervention();
        item.id_intervention = GetFreeIdIntervention();
        interventions.Add(item);
        System.Comparison<Intervention> cmp = new System.Comparison<Intervention>(Intervention.CompareId);
        interventions.Sort(cmp);

        return item;
    }

    public Question AddQuestion()
    {
        Question item = new Question();
        item.id_dialog = GetFreeIdQuestion();
        questions.Add(item);
        System.Comparison<Question> cmp = new System.Comparison<Question>(Dialog.CompareId);
        questions.Sort(cmp);

        return item;
    }

    public Answer AddAnswer()
    {
        Answer item = new Answer();
        item.id_dialog = GetFreeIdAnswers();
        answers.Add(item);
        System.Comparison<Answer> cmp = new System.Comparison<Answer>(Dialog.CompareId);
        answers.Sort(cmp);

        return item;
    }

    public int GetFreeIdIntervention()
    {
        int id = 0;
        for (int i = 0; i < interventions.Count; i++)
        {
            Intervention item = interventions[i];
            if (item.id_intervention != i)
            {
                id = i;
                break;
            }
            id = item.id_intervention + 1;
        }
        return id;
    }

    public int GetFreeIdQuestion()
    {
        int id = 0;
        for (int i = 0; i < questions.Count; i++)
        {
            Question item = questions[i];
            if (item.id_dialog != i)
            {
                id = i;
                break;
            }
            id = item.id_dialog + 1;
        }
        return id;
    }

    public int GetFreeIdAnswers()
    {
        int id = 0;
        for (int i = 0; i < answers.Count; i++)
        {
            Answer item = answers[i];
            if (item.id_dialog != i)
            {
                id = i;
                break;
            }
            id = item.id_dialog + 1;
        }
        return id;
    }

    public void Export()
    {
        string json = JsonUtility.ToJson(this);
        if (Directory.Exists(Application.streamingAssetsPath) == false) Directory.CreateDirectory(Application.streamingAssetsPath);
        string path = Application.streamingAssetsPath + "/dialogs.json";
        System.IO.File.WriteAllText(path, json);
    }

    public void Import()
    {
        string path = Application.streamingAssetsPath + "/dialogs.json";
        if (File.Exists(path) == false) return;

        string jsontext = System.IO.File.ReadAllText(path);

        JsonUtility.FromJsonOverwrite(jsontext, this);
    }

    public Dialog RandomQuestion()
    {
        return questions[Random.Range(0, questions.Count)];
    }

    public Dialog RandomAnswer()
    {
        return answers[Random.Range(0, answers.Count)];
    }

    public Intervention RandomIntervention()
    {
        return interventions[Random.Range(0, interventions.Count)];
    }

    public Question GetQuestion(int id)
    {
        for (int i = 0; i < questions.Count; i++)
        {
            if (questions[i].id_dialog == id) return questions[i];
        }
        return null;
    }
}

[System.Serializable]
public class Dialog
{
    public enum CHARACTER
    {
        NONE = 0,
        NOBLE_1 = 1,
        NOBLE_2 = 2,
        NOBLE_3 = 3,
        NOBLE_4 = 4,
        P_1 = 5,
        P_2 = 6,
        P_3 = 7,
        P_4 = 8
    };

    public int id_dialog;
    public CHARACTER id_character;

    public string text;

    public static int CompareId(Dialog c1, Dialog c2)
    {
        return c1.id_dialog.CompareTo(c2.id_dialog);
    }
}

[System.Serializable]
public class Intervention
{
    public int id_intervention = 0;
    public List<int> questions_ids;
    public List<int> scores;

    public Intervention()
    {
        questions_ids = new List<int>();
        scores = new List<int>();
        for (int i = 0; i < 16; i++) scores.Add(0);
    }

    public static int CompareId(Intervention c1, Intervention c2)
    {
        return c1.id_intervention.CompareTo(c2.id_intervention);
    }
}

[System.Serializable]
public class Question : Dialog
{
    public Question()
    {
    }
}

[System.Serializable]
public class Answer : Dialog
{
    public string shortAnswer;
}
