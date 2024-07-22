#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;
using TMPro;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            //Character Raelin = CharacterManager.instance.CreateCharacter("Raelin");
            //Character Stella2 = CharacterManager.instance.CreateCharacter("Stella");
            //Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            StartCoroutine(Test2());
        }

        IEnumerator Test1()
        {
            //Character_Sprite Guard = CreateCharacter("Guard as Generic") as Character_Sprite;
            //Character_Sprite GuardRed = CreateCharacter("Guard Red as Generic") as Character_Sprite;
            //Character_Sprite Raelin = CreateCharacter("Raelin as Raelin") as Character_Sprite;
            Character_Sprite Student = CreateCharacter("Student as Female Student 2") as Character_Sprite;

            yield return new WaitForSeconds(1);

            Student.Animate("Hop");

            yield return new WaitForSeconds(1);

            Student.Animate("Shiver", true);

            yield return new WaitForSeconds(3);

            Student.Animate("Shiver", false);

            yield return null;
        }

        IEnumerator Test2()
        {
            Character_Model3D UnityChan = CreateCharacter("UnityChan") as Character_Model3D;

            yield return UnityChan.MoveToPosition(new Vector2(1, 0));
        }

        IEnumerator Test3()
        {
            Character_Sprite Raelin = CharacterManager.instance.CreateCharacter("Female Student 2") as Character_Sprite;
            Raelin.Show();
            yield return new WaitForSeconds(1);
            Sprite body = Raelin.GetSprite("female student 2 - happy");
            yield return Raelin.TransitionSprite(body);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

#endif