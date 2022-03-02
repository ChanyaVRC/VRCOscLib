namespace BuildSoft.VRChat.Osc.Avatar;

public struct OscAvatar
{
    public string? Id { get; set; }

    public OscAvatarConfig? ToConfig() => Id == null ? null : OscAvatarConfig.CreateOscAvatarConfig(Id);
}
