namespace BuildSoft.VRChat.Osc.Test;

public record class OscAvatarConfigJson
{
    public string id;
    public string name;
    public OscAvatarParameterJson[] parameters;

    public OscAvatarConfigJson(string id, string name, OscAvatarParameterJson[] parameters)
    {
        this.id = id;
        this.name = name;
        this.parameters = parameters;
    }
}
