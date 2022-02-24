using BuildSoft.VRChat.Osc;

OscUtility.Initialize();

OscAvatarConfig? avatarConfig = null;

Console.WriteLine("Reading now... Try to \"Reset Avatar.\"");
while (avatarConfig == null)
{
    avatarConfig = OscAvatarConfig.CreateCurrentOscAvatarConfig();
    await Task.Delay(1);
}
Console.WriteLine($"Read avatar config. Name: {avatarConfig.Name}");

avatarConfig.Parameters.ParameterChanged += (parameter, e) =>
 {
     DateTime now = DateTime.Now;
     Console.WriteLine($"[{now.ToShortDateString()} {now.ToShortTimeString()}] " +
         $"{parameter.Name}: {e.OldValue} => {e.NewValue}");
 };

await Task.Delay(-1);

