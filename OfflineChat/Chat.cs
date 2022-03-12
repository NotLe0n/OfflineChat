using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OfflineChat;

internal class Chat
{
	public string LogPath => Directory.FullName + "\\chat.log";
	public string Text => string.Join("\n", _log[2..]);
	public DateTime Date => DateTime.Parse(_log[0]);
	public HashSet<string> Users => _users;
	public DirectoryInfo Directory => _directory;

	private string[] _log = Array.Empty<string>();
	private HashSet<string> _users = new();
	private readonly DirectoryInfo _directory;

	public Chat(string path)
	{
		_directory = new DirectoryInfo(path);
		if (!_directory.GetFiles().Any(x => x.FullName == LogPath))
		{
			File.Create(LogPath).Close();
		}

		// add yourself to userlist
		_users.Add(Environment.UserName);
	}

	public void WriteMessage(string message)
	{
		if (RunCommand(message))
			return;

		_log = _log.Append($"[{DateTime.Now:t}] <{Environment.UserName}>: {message}").ToArray();
		WriteInfoToFile(); // update immediatly
	}

	public bool RunCommand(string command)
	{
		switch (command)
		{
			case "-exit":
				Exit();
				Environment.Exit(0);
				return true;
			case "-crash":
				throw new("Crash command!");
			default:
				return false;
		}
	}

	public void ReadLogFile()
	{
		_log = File.ReadAllLines(LogPath);

		if (_log.Length < 2)
		{
			_log = new string[2];
			_log[0] = DateTime.Now.ToString();
			_log[1] = Environment.UserName;
		}
	}

	public void WriteInfoToFile()
	{
		_log[0] = DateTime.Now.ToString();
		_log[1] = GetUsersString();
		File.WriteAllLines(LogPath, _log);
	}

	public void UpdateUsers()
	{
		if (!string.IsNullOrEmpty(_log[1]))
			_users = _log[1].Split(';').ToHashSet();
	}

	public string GetUsersString()
	{
		return string.Join(';', _users);
	}

	public void Clear()
	{
		_log = new string[] {
			DateTime.Now.ToString(),
			GetUsersString()
		};
	}

	public void Exit()
	{
		_users.Remove(Environment.UserName);
		_log[1] = GetUsersString();
		WriteInfoToFile();
	}
}
