#if UNITY_EDITOR

using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestFileReading : MonoBehaviour
    {
        [SerializeField] private TextAsset fileToRead = null;

        // Start is called before the first frame update
        void Start()
        {
            Run();
        }

        void Run()
        {
            List<string> lines = FileManager.ReadTextAsset(fileToRead);
            Conversation conversation = new Conversation(lines);
            DialogueSystem.instance.Say(conversation);
        }
           
    }
}

#endif