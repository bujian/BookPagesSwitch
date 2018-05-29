using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCont : MonoBehaviour {

    public GameObject Ins;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject aa = Instantiate(Ins);
            aa.transform.SetParent(this.transform);
            aa.transform.localScale = Vector3.one;
            this.transform.GetComponent<UIGrid>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            var child = this.transform.GetChild(this.transform.childCount - 1);
            if(child) Destroy(child.gameObject);
            this.transform.GetComponent<UIGrid>().enabled = true;
        }
    }
}
