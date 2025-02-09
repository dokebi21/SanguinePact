using System.Text;
using SanguinePact.Common.Commands.Converters;
using ProjectM;
using VampireCommandFramework;

namespace SanguinePact.SanguinePact.Commands;

[CommandGroup(name: "sanguinepact", "sgp")]
internal class SanguinePactCommands
{
	[Command("toggle", "t", description: "Toggle sanguine pact.", adminOnly: false)]
	public static void ToggleSanguinePact(ChatCommandContext ctx)
	{
		ToggleSanguinePactForPlayer(ctx);
	}

	[Command("toggleplayer", "tp", description: "Toggle sanguine pact for player", adminOnly: true)]
	public static void ToggleSanguinePactForPlayer(ChatCommandContext ctx, OnlinePlayer player = null)
	{
		var name = player?.Value.UserEntity.Read<User>().CharacterName ?? ctx.Event.User.CharacterName;
		var charEntity = player?.Value.CharEntity ?? ctx.Event.SenderCharacterEntity;
		if (Helper.IsPlayerInCombat(charEntity))
		{
			ctx.Reply($"Cannot toggle Sanguine Pact for <color=white>{name}</color>. Player is in combat.");
			return;
		}
		if (Core.SanguinePactService.ToggleSanguinePact(charEntity))
		{
			ctx.Reply($"Sanguine Pact ON for <color=white>{name}</color>. Deal and take more damage.");
		}
		else
		{
			ctx.Reply($"Sanguine Pact OFF for <color=white>{name}</color>");
		}
		Core.SanguinePactService.UpdateSanguinePactPlayer(charEntity);
	}

	[Command("list", "l", description: "List players in sanguine pact", adminOnly: false)]
	public static void ListSanguinePactPlayers(ChatCommandContext ctx)
	{
		var pactPlayers = Core.SanguinePactService.GetSanguinePactPlayers();
		if (pactPlayers == null)
		{
			ctx.Reply("No players are playing on Sanguine Pact");
			return;
		}

		var sb = new StringBuilder();
		sb.Append("Sanguine Pact players: ");
		var first = true;
		foreach (var player in pactPlayers)
		{
			var playerCharacter = player.Read<PlayerCharacter>();
			var name = $"<color=white>{playerCharacter.Name}</color>";
			if (sb.Length + name.Length + 2 > Core.MAX_REPLY_LENGTH)
			{
				ctx.Reply(sb.ToString());
				sb.Clear();
				first = true;
			}
			if (first)
				sb.Append(name);
			else
				sb.Append($", {name}");
			first = false;
		}
		ctx.Reply(sb.ToString());
	}
}
