using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMethod : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        People pp = new People() { Age = 5, Sex = "male", Height = 175};
        print("++++" + pp.Age);
        PeopleGrowUp(pp);
        print("++++" + pp.Age);

        int a = 10;
        Change(a);
        print("++++++++" + a);
    }

    private void Change(int a)
    {
        a++;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PeopleGrowUp(People p)
    {
        p.Age++;
    }


    class People
    {
        public int Age;
        public string Sex;
        public int Height;
    }
}
