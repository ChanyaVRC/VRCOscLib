using BuildSoft.VRChat.Osc;

OscUtility.Initialize();

string myAvatarId = File.ReadAllText("AvatarId.txt").Trim();
var avatarConfig = OscAvatarConfig.CreateOscAvatarConfigs().FirstOrDefault(x => x.Id == myAvatarId);

if (avatarConfig == null)
{
    Console.WriteLine($"Id: \"{myAvatarId}\" is not active now.");
    return;
}

avatarConfig.Parameters.OnParameterChanged += (parameter, e) =>
 {
     DateTime now = DateTime.Now;
     Console.WriteLine($"[{now.ToShortDateString()} {now.ToShortTimeString()}] " +
         $"{parameter.Name}: {e.OldValue} => {e.NewValue}");
 };

await Task.Delay(-1);

