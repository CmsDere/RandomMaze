using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [SerializeField] float regenTime = 1800;
    int min = 0;
    [SerializeField] TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        regenTime -= Time.deltaTime;
        text.text = $"{(regenTime / 60 % 60)-1:0}:{(regenTime % 60):0}";
    }
}
