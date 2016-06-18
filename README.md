# AudioRouter
Console application which duplicates the audio going from one windows audio device to another.
I created it for when people want to spectate someone else using the Oculus Rift, but not getting any sound since it plays in the player's headset, so this allows the sound to be copied to whatever other output device available on the computer in question.

Run in the following way:
AudioRouter.exe sourceName targetName

For example in my case when using it for cloning the sound from Oculus Rift to another speaker/headset I type:
AudioRouter.exe "rift" "speaker"
