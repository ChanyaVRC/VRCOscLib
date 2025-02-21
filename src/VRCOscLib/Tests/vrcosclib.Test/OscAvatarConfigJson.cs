namespace BuildSoft.VRChat.Osc.Test;

public record class OscAvatarConfigJson
{
    public string id;
    public string name;
    public OscAvatarParameterJson[] parameters;
    public int hash;

    public OscAvatarConfigJson(string id, string name, OscAvatarParameterJson[] parameters, int hash)
    {
        this.id = id;
        this.name = name;
        this.parameters = parameters;
        this.hash = hash;
    }
}
