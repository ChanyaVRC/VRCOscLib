using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BuildSoft.VRChat.Osc.Avatar;

namespace BuildSoft.VRChat.Osc.Test;

public record class OscAvatarConfigJson(string id, string name, OscAvatarParameterJson[] parameters);
