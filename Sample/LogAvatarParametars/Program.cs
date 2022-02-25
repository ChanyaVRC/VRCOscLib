using BuildSoft.VRChat.Osc.Avatar;

OscAvatarConfig? avatarConfig = null;

Console.WriteLine($"[NOTIFICATION] Reading now... Try to \"Reset Avatar.\"");
avatarConfig = await OscAvatarConfig.WaitAndCreateCurrentOscAvatarConfigAsync();
Console.WriteLine($"[NOTIFICATION] Read avatar config. Name: {avatarConfig.Name}");

OscAvatarParameterChangedEventHandler? handler = (parameter, e) =>
{
    DateTime now = DateTime.Now;
    Console.WriteLine($"[{now.ToShortDateString()} {now.ToShortTimeString()}] " +
        $"{parameter.Name}: {e.OldValue} => {e.NewValue}");
};
OscAvatarUtility.AvatarChanged += (sender, e) =>
{
    avatarConfig.Parameters.ParameterChanged -= handler;

    avatarConfig = OscAvatarConfig.CreateCurrentOscAvatarConfig()!;
    Console.WriteLine($"[NOTIFICATION] Changed avatar. Name: {avatarConfig.Name}");

    avatarConfig.Parameters.ParameterChanged += handler;
};
avatarConfig.Parameters.ParameterChanged += handler;

await Task.Delay(-1);
