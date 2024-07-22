using CHARACTERS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Characters : CMD_DatabaseExtension
    {
        #region PARAMS
        private static string[] PARAM_ANIMATIONNAME => new string[] { "-a", "-animation" };
        private static string[] PARAM_BLENDSHAPENAME => new string[] { "-bsp", "-blendshapename" };
        private static string[] PARAM_COLOR => new string[] { "-c", "-color" };
        private static string[] PARAM_ENABLE => new string[] { "-e", "-enabled" };
        private static string[] PARAM_FACING => new string[] { "-fl", "-faceleft" };
        private static string[] PARAM_IMMEDIATE => new string[] { "-i", "-immediate" };
        private static string[] PARAM_LAYER => new string[] { "-l", "-layer" };
        private static string[] PARAM_MOTION => new string[] { "-m", "-motion" };
        private static string[] PARAM_ONLY => new string[] { "-o", "-only" };
        private static string[] PARAM_SPRITE => new string[] { "-s", "-sprite" };
        private static string[] PARAM_SMOOTH => new string[] { "-sm", "-smooth" };
        private static string[] PARAM_SPEED => new string[] { "-spd", "-speed" };
        private static string[] PARAM_STATE => new string[] { "-st", "-state" };
        private static string[] PARAM_WEIGHT => new string[] { "-w", "-weight" };
        private static string PARAM_XPOS => "-x";
        private static string PARAM_YPOS => "-y";
        #endregion

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("movecharacter", new Func<string[], IEnumerator>(MoveCharacter));
            database.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
            database.AddCommand("hide", new Func<string[], IEnumerator>(HideAll));
            database.AddCommand("sort", new Action<string[]>(Sort));
            database.AddCommand("highlight", new Func<string[], IEnumerator>(HighlightAll));
            database.AddCommand("unhighlight", new Func<string[], IEnumerator>(UnhighlightAll));


            //Add commands to characters
            CommandDatabase baseCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_BASE);
            baseCommands.AddCommand("move", new Func<string[], IEnumerator>(MoveCharacter));
            baseCommands.AddCommand("setposition", new Action<string[]>(SetPosition));
            baseCommands.AddCommand("show", new Func<string[], IEnumerator>(Show));
            baseCommands.AddCommand("hide", new Func<string[], IEnumerator>(Hide));
            baseCommands.AddCommand("setpriority", new Action<string[]>(SetPriority));
            baseCommands.AddCommand("setcolor", new Func<string[], IEnumerator>(SetColor));
            baseCommands.AddCommand("highlight", new Func<string[], IEnumerator>(Highlight));
            baseCommands.AddCommand("unhighlight", new Func<string[], IEnumerator>(Unhighlight));
            baseCommands.AddCommand("facedirection", new Func<string[], IEnumerator>(FaceDirection));
            baseCommands.AddCommand("animate", new Action<string[]>(Animate));
            //baseCommands.AddCommand("flip", new Func<string[], IEnumerator>(Flip));//NOT WORKING!!!

            //Add character specific databases
            CommandDatabase spriteCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_SPRITE);
            spriteCommands.AddCommand("setsprite", new Func<string[], IEnumerator>(SetSprite));

            CommandDatabase model3DCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_MODEL3D);
            model3DCommands.AddCommand("setexpression", new Action<string[]>(SetExpression));
            model3DCommands.AddCommand("setmotion", new Action<string[]>(SetMotion));
        }

        #region GLOBAL COMMANDS
        private static void CreateCharacter(string[] data)
        {
            string characterName = data[0];
            bool enable = false;
            bool immediate = false;

            //Conver the data array to a parameter container
            var parameters = ConvertDataToParameters(data, 1);

            parameters.TryGetValue(PARAM_ENABLE, out enable, defaultValue: false);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            Character character = CharacterManager.instance.CreateCharacter(characterName);

            if (!enable)
                return;

            if (immediate)
                character.isVisible = true;
            else
                character.Show();

        }

        private static void Sort(string[] data)
        {
            CharacterManager.instance.SortCharacters(data);
        }

        private static IEnumerator ShowAll(string[] data)
        {
            List<Character> characters = new List<Character>();

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            bool immediate = false;
            float speed = 1f;

            //Conver the data array to a parameter container
            var parameters = ConvertDataToParameters(data, startingIndex: characters.Count);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            //Call the logic on all the characters
            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = true;
                else
                    character.Show(speed);
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                        character.isVisible = true;
                });

                while (characters.Any(c => c.isRevealing))
                    yield return null;
            }
        }

        private static IEnumerator HideAll(string[] data)
        {
            List<Character> characters = new List<Character>();

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            bool immediate = false;
            float speed = 1f;

            //Conver the data array to a parameter container
            var parameters = ConvertDataToParameters(data, startingIndex: characters.Count);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            //Call the logic on all the characters
            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = false;
                else
                    character.Hide(speed);
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                        character.isVisible = false;
                });

                while (characters.Any(c => c.isHiding))
                    yield return null;
            }
        }

        private static IEnumerator HighlightAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            bool handleUnspecifiedCharacters = true;
            List<Character> unspecifiedCharacters = new List<Character>();

            //Add any characters specified to be highlighted.
            for (int i = 0; i < data.Length; i++)
            {
                Character character = CharacterManager.instance.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            //Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: characters.Count);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_ONLY, out handleUnspecifiedCharacters, defaultValue: true);

            //Make all characters perform the logic
            foreach (Character character in characters)
                character.Highlight(immediate: immediate);

            //If we are forcing any unspecified characters to use the opposite highlighted status
            if (handleUnspecifiedCharacters)
            {
                foreach (Character character in CharacterManager.instance.allCharacters)
                {
                    if (characters.Contains(character))
                        continue;

                    unspecifiedCharacters.Add(character);
                    character.UnHighlight(immediate: immediate);
                }
            }

            //Wait for all characters to finish highlighting
            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.Highlight(immediate: true);

                    if (!handleUnspecifiedCharacters) return;

                    foreach (var character in unspecifiedCharacters)
                        character.UnHighlight(immediate: true);
                });

                while (characters.Any(c => c.isHighlighting) || (handleUnspecifiedCharacters && unspecifiedCharacters.Any(uc => uc.isUnHighlighting)))
                    yield return null;
            }
        }

        private static IEnumerator UnhighlightAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            bool handleUnspecifiedCharacters = true;
            List<Character> unspecifiedCharacters = new List<Character>();

            //Add any characters specified to be highlighted.
            for (int i = 0; i < data.Length; i++)
            {
                Character character = CharacterManager.instance.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            //Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: characters.Count);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_ONLY, out handleUnspecifiedCharacters, defaultValue: true);

            //Make all characters perform the logic
            foreach (Character character in characters)
                character.UnHighlight(immediate: immediate);

            //If we are forcing any unspecified characters to use the opposite highlighted status
            if (handleUnspecifiedCharacters)
            {
                foreach (Character character in CharacterManager.instance.allCharacters)
                {
                    if (characters.Contains(character))
                        continue;

                    unspecifiedCharacters.Add(character);
                    character.Highlight(immediate: immediate);
                }
            }

            //Wait for all characters to finish highlighting
            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.UnHighlight(immediate: true);

                    if (!handleUnspecifiedCharacters) return;

                    foreach (var character in unspecifiedCharacters)
                        character.Highlight(immediate: true);
                });

                while (characters.Any(c => c.isUnHighlighting) || (handleUnspecifiedCharacters && unspecifiedCharacters.Any(uc => uc.isHighlighting)))
                    yield return null;
            }
        }
        #endregion

        #region BASE CHARACTER COMMANDS
        private static IEnumerator Show(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            bool immediate = false;
            float speed = 1f;

            //Conver the data array to a parameter container
            var parameters = ConvertDataToParameters(data, 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            if (immediate)
                character.isVisible = true;
            else
            {
                //A long running process should have a stop action to cancel out the coroutine and run logic that should complete this command
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = true; });

                yield return character.Show(speed);
            }
        }

        private static IEnumerator Hide(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            bool immediate = false;
            float speed = 1f;

            //Conver the data array to a parameter container
            var parameters = ConvertDataToParameters(data, 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            if (immediate)
                character.isVisible = false;
            else
            {
                //A long running process should have a stop action to cancel out the coroutine and run logic that should complete this command
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = false; });

                yield return character.Hide();
            }
        }

        private static IEnumerator MoveCharacter(string[] data)
        {
            string characterName = data[0];

            Character character = CharacterManager.instance.GetCharacter(characterName);

            if (character == null)
                yield break;

            float x = 0, y = 0;
            float speed = 1;
            bool smooth = false;
            bool immediate = false;

            //Conver the data array to a parameter container
            var parameters = ConvertDataToParameters(data, 1);

            //try to get the x axis position
            parameters.TryGetValue(PARAM_XPOS, out x);

            //try to get the y axis position
            parameters.TryGetValue(PARAM_YPOS, out y);

            //try to get the speed
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1);

            //try to get the smoothing
            parameters.TryGetValue(PARAM_SMOOTH, out smooth, defaultValue: false);

            //try to get immediate setting of position
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            Vector2 position = new Vector2(x, y);

            if (immediate)
                character.SetPosition(position);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetPosition(position); });
                yield return character.MoveToPosition(position, speed, smooth);
            }
        }

        private static void SetPosition(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);

            if (character == null)
                return;

            float x = 0, y = 0;

            //Conver the data array to a parameter container
            var parameters = ConvertDataToParameters(data, 1);
            parameters.TryGetValue(PARAM_XPOS, out x, defaultValue: 0);
            parameters.TryGetValue(PARAM_YPOS, out y, defaultValue: 0);

            character.SetPosition(new Vector2(x, y));
        }

        private static void SetPriority(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            int priority;

            if (character == null || data.Length < 2)
                return;

            if (!int.TryParse(data[1], out priority))
                priority = 0;

            character.SetPriority(priority);
        }

        private static IEnumerator SetColor(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);

            if (character == null)
                yield break;

            string colorName;
            float speed;
            bool immediate;

            //Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            //Try to get color name
            parameters.TryGetValue(PARAM_COLOR, out colorName);
            //Try to get the speed of the transition
            bool specifiedSpeed = parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            //Try to get istant value
            if (!specifiedSpeed)
                parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: true);
            else
                immediate = false;

            //Get the color value from the name
            Color color = Color.white;
            color = color.GetColorFromName(colorName);

            if (immediate)
                character.SetColor(color);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetColor(color); });
                character.TransitionColor(color, speed);
            }

            yield break;
        }

        private static IEnumerator Highlight(string[] data)
        {
            //format: SetSprite(character sprite)
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);

            if (character == null)
                yield break;

            bool immediate = false;

            //Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character.Highlight(immediate: true); });
            yield return character.Highlight(immediate: immediate);

        }

        private static IEnumerator Unhighlight(string[] data)
        {
            //format: SetSprite(character sprite)
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);

            if (character == null)
                yield break;

            bool immediate = false;

            //Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.UnHighlight(immediate: true); });
            yield return character.UnHighlight(immediate: immediate);

        }

        private static IEnumerator FaceDirection(string[] data)
        {
            //format: FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);

            if (character == null)
                yield break;

            bool immediate = false;
            float speed;
            bool faceLeft = true;

            //Grab the parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_FACING, out faceLeft, defaultValue: true);

            if (faceLeft)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.FaceLeft(speed, immediate: true); });
                yield return character.FaceLeft(speed, immediate);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.FaceRight(speed, immediate: true); });
                yield return character.FaceRight(speed, immediate);
            }

        }

        private static IEnumerator Flip(string[] data)
        {
            //format: Flip(float speed = 1, bool immediate = false)
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);

            if (character == null)
                yield break;

            bool immediate = false;
            float speed;

            //Grab the parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            yield return character.Flip(speed, immediate);
        }

        private static void Animate(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);

            if (character == null)
                return;

            string name = null;
            bool state = true;

            //Grab the parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_STATE, out state, defaultValue: true);

            parameters.TryGetValue(PARAM_ANIMATIONNAME, out name);

            character.Animate(name, state);
        }
        #endregion

        #region SPRITE CHARACTERS COMMANDS
        private static IEnumerator SetSprite(string[] data)
        {
            //format: SetSprite(character sprite)
            Character_Sprite character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character_Sprite;

            if (character == null)
                yield break;

            int layer = 0;
            string spriteName;
            bool immediate = false;
            float speed;

            //Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            //Try to get the sprite name
            parameters.TryGetValue(PARAM_SPRITE, out spriteName);
            //Try to get the layer
            parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: 0);

            //Try to get the transition speed
            bool specifiedSpeed = parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            //Try to get whether this is an immediate transition or not
            if (!specifiedSpeed)
                parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: true);

            //Run the logic
            Sprite sprite = character.GetSprite(spriteName);

            if (sprite == null)
                yield break;

            if (immediate)
            {
                character.SetSprite(sprite, layer);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetSprite(sprite, layer); });
                yield return character.TransitionSprite(sprite, layer, speed);
            }
        }
        #endregion

        #region MODEL 3D CHARACTERS COMMANDS
        public static void SetExpression(string[] data)
        {
            //format: SetExpression(string blendShapeName, float weight, float speedMultiplier = 1, bool immediate = false)
            Character_Model3D character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character_Model3D;

            if (character == null)
                return;

            string blendShapeName;
            float weight;
            float speed;
            bool immediate;

            //Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_BLENDSHAPENAME, out blendShapeName);
            parameters.TryGetValue(PARAM_WEIGHT, out weight, defaultValue: 100);
            bool specifiedSpeed = parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1);

            if (!specifiedSpeed)
                parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: true);

            Debug.LogWarning("Function is not working for now. Do not try to set expression for 3D Model");
        }

        public static void SetMotion(string[] data)
        {
            //format: SetExpression(string blendShapeName, float weight, float speedMultiplier = 1, bool immediate = false)
            Character_Model3D character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character_Model3D;

            if (character == null)
                return;

            string motionName;

            //Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_BLENDSHAPENAME, out motionName);

            character.SetMotion(motionName);
        }
        #endregion
    }
}