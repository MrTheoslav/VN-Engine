#if UNITY_EDITOR
using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running1());
    }

    Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

    IEnumerator Running1()
    {
        yield return new WaitForSeconds(2);

    }
    IEnumerator Running()
    {
        Character_Sprite Raelin = CreateCharacter("Raelin") as Character_Sprite;
        Raelin.Show();

        yield return new WaitForSeconds(0.5f);

        AudioManager.instance.PlayVoice("Audio/Voices/exclamation");

        yield return Raelin.Say("Thank you!");
    }
}

#endif