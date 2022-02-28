namespace BuildSoft.VRChat.Osc.Avatar;

public struct OscAvatar
{
    public string? AvatarId { get; set; }

    public OscAvatarConfig? ToConfig() => AvatarId == null ? null : OscAvatarConfig.CreateOscAvatarConfig(AvatarId);
}
