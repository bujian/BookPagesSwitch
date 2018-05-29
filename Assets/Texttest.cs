using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Texttest : MonoBehaviour
{
    public TextMesh _Text;
    int[] content = new int[] { 11,22,33,44,55,66,77,88,99,00};
    Action<string> ChangeText;

    // Use this for initialization
    void Start()
    {
        string s = "经度:122.52762666dsfsfdg666666,纬度:31.11";
        print(s.Substring(0, s.Length));

        ChangeText = new Action<string>(TextChagned);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var text = transform.gameObject.AddComponent<BJTweenText>();
            text.Duration = 0.5f;
            text.Content = "经度:122.527626666dfgsdfgsdfgsdgf66666,纬度:31.11";
            text.Play();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(TextUpdate(10));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StopCoroutine("TextUpdate");
        }

        


    }

    IEnumerator TextUpdate(float time)
    {
        yield return new WaitForSeconds(time);
        Action a = new Action(ReadyToChangeText);
        a.BeginInvoke(null, null);
        print("++++++++++++");
    }

    void ReadyToChangeText()
    {
        ActionQueue.Instance.QueueIn(() =>
        {
            System.Random ran = new System.Random();
            int ranNum = ran.Next(0, 100);
            ChangeText(ranNum.ToString());
        });
        print("aaaa");
    }

    void TextChagned(string text)
    {
        _Text.text = text;
    }
}
