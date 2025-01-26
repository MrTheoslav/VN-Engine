This is a VN-Engine project created in Unity. This is NOT a game but a tool to create one.
Here's the format of the dialogue line:

{character{ as character_name}{ at x{:y}}{ [layer:sprite{ + layer:sprite}]} "dialogue"} {command(attributes){, more_commands(attributes)}

where:
character - name of the character that is in "Character Config -> Characters -> <the_character> -> Name"

character_name - name that will be used to show in name box & to separate characters in script

x,y - coordinates where 0 - full left; 1 - full right. Any above or below will make the character exit partly or fully the scene.

layer - defines which part do you want to change in sprite_layer: 0 - body, 1 - head. You may define more if needed.

dialogue - text that character will say or think. Yuo may change here the fonts, like Bold, Italic, Underline or Font Color.

command - invoke a script to change background, file or character sprite. For command lines, look below.

attribute - some commands require some additional info that will be passed via attribute. For available attributes look in the command lines below.


Command lines, their attributes and what they do:

AUDIO COMMANDS:

PlaySFX(-sfx string -volume float -pitch float -loop bool) - play sound effect (REQUIRES: -sfx); -sfx, -s -> filepath to sound effect; -volume, -vol, -v -> how loud should the sound effect be; -pitch, -p -> the pitch of sound effect; -loop, -l -> the sound effect should be looped;

StopSFX(string) - stops playing the sound effect (REQUIRES: sfx name/path to sfx);

PlayVoice(-voice string -volume float -pitch float -loop bool) - play voice effect, it is NOT for voice dubbed (REQUIRES: -voice)

StopVoice(string) - stop voice effect (REQUIRES: voice name/path to voice);

PlaySong(-song string -channel int) - plays song (REQUIRES: -song)

StopSong(-channel int) - stops all songs

PlayAmbiance(-ambiance string -channel int) - plays ambience (REQUIRES: -ambiance)

StopAmbience(-channel int) - stops all ambiences


CHARACTER COMMANDS

CreateCharacter(characterName -enabled bool -immediate bool)

MoveCharacter(characterName -x float -y float -speed float -smooth bool -immediate bool)

Show(characterName -speed float -immediate bool)*

Hide(characterName -speed float -immediate bool)*

Sort(characterNames)

Highlight(characterName -immediate bool)*

Unhighlight(characterName -immediate bool)*

Move(characterName -x float -y float -speed float -smooth bool -immediate bool)*

SetPosition(characterName -x float -y float)*

SetPriority(characterName -priority int)*

SetColor(characterName -color string -speed float -immediate bool)*

FaceDirection(characterName -faceLeft bool -speed float -immediate bool)*

Animate(characterName -animation string -state bool)*

SetSprite(characterName -layer int -sprite string -speed float -immediate bool)*

SetExpression(characterName -blendShapeName string -weight float -speed float -immediate bool)*

SetMotion(characterName -motion)*


*NOTE - all theses commands may be written like this: characterName.command(attributes)


GALLERY COMMANDS



GENERAL COMMANDS



GRAPHIC PANEL COMMANDS



OTHER COMMANDS

