using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OfflineChat;

public class Chat
{
	public readonly Command[] commands;

	public string LogPath => _directory.FullName + "\\chat.log";
	public string Text => string.Join("\n", _log[2..]); // actual chat starts at line 3, lines above are header info
	public HashSet<string> Users => _users; // online users info in line 1
	public HashSet<string> BannedUsers => _banned; // banned users info in line 2

	private string[] _log = Array.Empty<string>(); // contents of the whole log file, including header
	private HashSet<string> _users = new();
	private HashSet<string> _banned = new();

	private readonly DirectoryInfo _directory;

	public Chat(string path)
	{
		_directory = new DirectoryInfo(path);

		// create log file if there is none in the directory
		if (!_directory.GetFiles().Any(x => x.FullName == LogPath))
		{
			File.Create(LogPath).Close();
		}

		// call private static methods in the Commands class
		var methods = typeof(Commands).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);

		commands = new Command[methods.Length];
		for (int i = 0; i < methods.Length; i++)
		{
			MethodInfo method = methods[i];
			commands[i] = new Command()
			{
				Name = method.Name.ToLower().Replace("_command", ""), // turn the name lowercase and remove the "_Command"
				Description = method.GetCustomAttribute<CommandDescription>()?.description ?? "No description",
				MethodInfo = method
			};
		}
	}

	public void WriteInfoMessage(string message)
	{
		_log = _log.Append($"[{DateTime.Now:t}] <INFO>: {message}").ToArray();
		WriteInfoToFile(); // update immediatly
	}

	public void WriteMessage(string message)
	{
		if (RunCommand(message))
			return;

		_log = _log.Append($"[{DateTime.Now:t}] <{Environment.UserName}>: {message}").ToArray();
		WriteInfoToFile(); // update immediatly
	}

	private const char CommandChar = '-';
	public bool RunCommand(string input)
	{
		foreach (var command in commands)
		{
			if (input.StartsWith(CommandChar + command.Name))
			{
				string noName = input[(1 + command.Name.Length)..]; // + 1 for CommandChar
				string[] parameters = Regex.Split(noName.Trim(), @"\s+"); // issue: no spaces in strings allowed

				if (parameters[0] == "") {
					command.Execute(); // if parameterlist is "empty" call execute with no parameters
				}
				else {
					command.Execute(parameters);
				}

				return true;
			}
		}

		return false;
	}

	public void ReadLogFile()
	{
		_log = File.ReadAllLines(LogPath);

		// fill file with at least two lines
		if (_log.Length < 2)
		{
			_log = new string[2];
			_log[0] = Environment.UserName;
		}

		if (_log[^1].Contains($"kicked {Environment.UserName}"))
		{
			Exit();
			Environment.Exit(0);
		}
	}

	public void WriteInfoToFile()
	{
		_log[0] = GetUsersString();
		_log[1] = GetBannedString();
		File.WriteAllLines(LogPath, _log);
	}

	public void UpdateUsers()
	{
		if (!string.IsNullOrEmpty(_log[0]))
			_users = _log[0].Split(';').ToHashSet();

		if (!string.IsNullOrEmpty(_log[1]))
			_banned = _log[1].Split(';').ToHashSet();

		// add yourself to userlist
		_users.Add(Environment.UserName);

		// If I'm banned, exit
		if (BannedUsers.Contains(Environment.UserName))
		{
			Exit();
			Environment.Exit(0);
		}
	}

	public string GetUsersString()
	{
		return string.Join(';', _users);
	}

	public string GetBannedString()
	{
		return string.Join(';', _banned);
	}

	public void BanUser(string user)
	{
		_banned.Add(user);
		WriteInfoToFile();
	}

	public void UnbanUser(string user)
	{
		_banned.Remove(user);
		WriteInfoToFile();
	}

	public void KickUser(string user)
	{
		_banned.Add(user);

	}

	public void Clear()
	{
		_log = new string[] {
			GetUsersString(),
			GetBannedString()
		};
	}

	public void Exit()
	{
		_users.Remove(Environment.UserName);
		_log[0] = GetUsersString();

		// if this was the only user, clear the file
		if (_users.Count == 0)
			Clear();

		WriteInfoToFile();
	}
}
