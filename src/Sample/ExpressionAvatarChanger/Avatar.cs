using System.Collections.Immutable;
using BuildSoft.VRChat.Osc;

namespace ExpressionAvatarChanger;

public record class Avatar(string Id, string Name, IImmutableList<(string, OscType)> Parameters);
