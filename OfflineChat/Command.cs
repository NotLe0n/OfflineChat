using System.Reflection;
using System;

namespace OfflineChat;

public class Command
{
	public string Name { get; set; }
	public string Description { get; set; }
	public MethodInfo MethodInfo { get; set; }

	public void Execute(params object[] args)
	{
		var parameters = MethodInfo.GetParameters();

		if (args.Length != parameters.Length)
		{
			throw new($"The command {Name} requires {parameters.Length} parameters but {args.Length} parameters were passed.");
		}

		object[] newArgs = new object[parameters.Length];
		for (int i = 0; i < args.Length; i++)
		{
			newArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
		}

		MethodInfo.Invoke(null, newArgs);
	}

	public override string ToString()
	{
		return Name;
	}
}
