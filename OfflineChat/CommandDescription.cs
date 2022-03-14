using System;

namespace OfflineChat;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CommandDescription : Attribute
{
	public readonly string description;

	public CommandDescription(string description)
	{
		this.description = description;
	}
}
