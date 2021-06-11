using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Controller controller;
    private float timer;
    private bool isEnabled = false;
    private int e1, e2;
    private Text t;

    public void New(float time, Text t_, Controller controller_)
    {
        timer = time;
        controller = controller_;
        t = t_;
        isEnabled = true;
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        t.text = "" + timer;
        while (timer > 0 && isEnabled)
        {
            yield return new WaitForSeconds(1.0f);
            timer--;
            t.text = "" + timer;
        }
        timer = 0;
        t.text = "0";
        isEnabled = false;

        e1 = controller.b1.countShown();
        e2 = controller.b2.countShown();

        if (e1 > e2) controller.GameOver(false, false);
        else if (e1 < e2) controller.GameOver(true, false);
        else controller.GameOver(true, true);
    }
}
