using System;
using System.Linq;

namespace OfflineChat;

#pragma warning disable IDE0051 // Remove unused private members

public static class Commands
{
	[CommandDescription("Shows the description and parameters of a command")]
	private static void Help_Command(string commandName)
	{
		var command = Program.chat.commands.First((c) => c.Name == commandName);
		Program.chat.WriteInfoMessage($"Command \"{command.Name}\": {command.Description}");

		if (command.MethodInfo.GetParameters().Length == 0)
		{
			Program.chat.WriteInfoMessage("This command does not have any parameters.");
			return;
		}

		string paramInfo = string.Empty;
		foreach (var parameter in command.MethodInfo.GetParameters())
		{
			paramInfo += $"{parameter.ParameterType.Name} {parameter.Name}, ";
		}

		Program.chat.WriteInfoMessage($"parameters: {paramInfo}");
	}

	[CommandDescription("Lists all commands")]
	private static void ListCommands_Command()
	{
		Program.chat.WriteInfoMessage("These are all commands: " + string.Join<Command>(", ", Program.chat.commands));
	}

	[CommandDescription("Clears the chat")]
	private static void Clear_Command()
	{
		Program.chat.Clear();
		Program.chat.WriteInfoToFile();
	}

	[CommandDescription("Leaves the chat and closes the application")]
	private static void Exit_Command()
	{
		Program.chat.Exit();
		Environment.Exit(0);
	}

	[CommandDescription("Throws an exception which will get catched and displayed in a seperate window")]
	private static void Crash_Command()
	{
		throw new("Crash command!");
	}
	
	[CommandDescription("Bans a user from joining")]
	private static void Ban_Command(string user)
	{
		Program.chat.BanUser(user);
	}

	[CommandDescription("Unbans a user from joining")]
	private static void Unban_Command(string user)
	{
		Program.chat.UnbanUser(user);
	}

	[CommandDescription("Removes a player from the chat room")]
	private static void Kick_Command(string user)
	{
		Program.chat.WriteInfoMessage($"{Environment.UserName} kicked {user}");
	}

	[CommandDescription("Lists all currently banned users")]
	private static void ListBanned_Command()
	{
		if (Program.chat.BannedUsers.Count == 0)
		{
			Program.chat.WriteInfoMessage("There are no banned users");
		}
		else
		{
			Program.chat.WriteInfoMessage($"These are all banned users: {string.Join(", ", Program.chat.BannedUsers)}");
		}
	}
}