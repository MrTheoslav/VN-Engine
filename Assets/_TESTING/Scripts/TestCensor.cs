#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCensor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Check("This line has a badword1 in it?");
        Check("This should be clear of any bad words!");
        Check("This astinking line should be bad as well.");
        Check("I want some tofu in a warm bowl of Miso Soup. Don't forget the extratofu");
    }

    void Check(string line)
    {
        if (CensorManager.Censor(ref line))
            Debug.Log($"<color=red>'{line}'</color>");
        else
            Debug.Log($"<color=green>'{line}'</color>");

    }
}

#endif