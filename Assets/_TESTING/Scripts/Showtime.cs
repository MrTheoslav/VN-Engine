#if UNITY_EDITOR

using CHARACTERS;
using System.Collections;
using TMPro;
using UnityEngine;


public class Showtime : MonoBehaviour
{
    public TMP_FontAsset GuardNameFont;
    public TMP_FontAsset GuardRedNameFont;
    public TMP_FontAsset RaelinNameFont;
    public TMP_FontAsset StudentNameFont;

    public TMP_FontAsset GuardDialogueFont;
    public TMP_FontAsset GuardRedDialogueFont;
    public TMP_FontAsset RaelinDialogueFont;
    public TMP_FontAsset StudentDialogueFont;

    public Color GuardNameColor;
    public Color GuardRedNameColor;
    public Color RaelinNameColor;
    public Color StudentNameColor;
    public Color GuardDialogueColor;
    public Color GuardRedDialogueColor;
    public Color RaelinDialogueColor;
    public Color StudentDialogueColor;


    private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {

        Character_Sprite Guard = CreateCharacter("Guard as Generic") as Character_Sprite;
        Character_Sprite GuardRed = CreateCharacter("Guard Red as Generic") as Character_Sprite;
        Character_Sprite Raelin = CreateCharacter("Raelin as Raelin") as Character_Sprite;
        Character_Sprite Student = CreateCharacter("Student as Female Student 2") as Character_Sprite;

        Guard.SetSprite(Guard.GetSprite("Monk"));

        GuardRed.isVisible = false;
        Guard.isVisible = false;

        Guard.SetNameFont(GuardNameFont);
        GuardRed.SetNameFont(GuardRedNameFont);
        Raelin.SetNameFont(RaelinNameFont);
        Student.SetNameFont(StudentNameFont);

        Guard.SetDialogueFont(GuardDialogueFont);
        GuardRed.SetDialogueFont(GuardRedDialogueFont);
        Raelin.SetDialogueFont(RaelinDialogueFont);
        Student.SetDialogueFont(StudentDialogueFont);

        Guard.SetNameColor(GuardNameColor);
        GuardRed.SetNameColor(GuardRedNameColor);
        Raelin.SetNameColor(RaelinNameColor);
        Student.SetNameColor(StudentNameColor);

        Guard.SetDialogueColor(GuardDialogueColor);
        GuardRed.SetDialogueColor(GuardRedDialogueColor);
        Raelin.SetDialogueColor(RaelinDialogueColor);
        Student.SetDialogueColor(StudentDialogueColor);

        GuardRed.SetColor(Color.red);

        Raelin.SetPosition(Vector2.zero);
        Raelin.Flip(immediate: true);

        Raelin.SetSprite(Raelin.GetSprite("B1"));
        Raelin.SetSprite(Raelin.GetSprite("B_Default"), 1);

        Student.SetPosition(new Vector2(1, 0));

        Raelin.Highlight();
        Student.UnHighlight();

        yield return Raelin.Say("Hej, ty!");

        yield return Student.Flip();

        yield return Raelin.Say("Tak, ty. Czy masz czas zobaczyæ ten d³ugi tekst, by ludzie mogli go przyspieszyæ albo i nawet pomin¹æ? Bo tak szczerze mi siê nie chce ju¿ nic wiêcej mówiæ, ale muszê, bo ten cio³ek jeszcze nie kaza³ mi siê przymkn¹æ, wiêæ bêdê tak mówiæ w nieskoñczonoœæ, a¿ w koñcu mnie uciszy.");
        yield return Raelin.Say("Specjalnie to powtórzê: Czy masz czas zobaczyæ ten d³ugi tekst, by ludzie mogli go przyspieszyæ albo i nawet pomin¹æ? Bo tak szczerze mi siê nie chce ju¿ nic wiêcej mówiæ, ale muszê, bo ten cio³ek jeszcze nie kaza³ mi siê przymkn¹æ, wiêæ bêdê tak mówiæ w nieskoñczonoœæ, a¿ w koñcu mnie uciszy.");

        Raelin.TransitionSprite(Raelin.GetSprite("A2"));
        Raelin.TransitionSprite(Raelin.GetSprite("A_Guilty"), 1);
        Raelin.UnHighlight();

        Student.Highlight();

        yield return Student.Say("Nie. Dziêkujê");

        Guard.SetPosition(new Vector2(0.3f, 0));
        GuardRed.SetPosition(new Vector2(0.8f, 0));
        GuardRed.UnHighlight();

        Student.UnHighlight();

        GuardRed.FaceRight(immediate: true);

        Guard.Show();
        yield return GuardRed.Show();


        yield return Guard.Say("Hej, co to za nielegalne zgromadzenie?!{c}Proszê siê rozejœæ! {wa 2}Natychmiast!");

        Guard.UnHighlight();

        Raelin.MoveToPosition(new Vector2(-1, 0));
        yield return Guard.MoveToPosition(new Vector2(-1, 0));

        GuardRed.Highlight();
        yield return GuardRed.Say("No ju¿, idziemy!");

        GuardRed.UnHighlight();

        yield return Student.MoveToPosition(Vector2.zero, smooth: true);

        GuardRed.Highlight();
        yield return GuardRed.Say("Ale nie w tê stronê!");
        GuardRed.UnHighlight();

        yield return Student.MoveToPosition(new Vector2(2, 0));

        yield return GuardRed.Hide();


        Debug.Log("Koniec");
        yield return null;

    }
}

#endif