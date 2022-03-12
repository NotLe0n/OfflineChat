using ImGuiNET;
using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OfflineChat;

public class Program
{
	private static Chat chat;

	private static async Task Main(string[] args)
	{
		chat = new Chat(System.IO.Directory.GetCurrentDirectory());

		using var window = new WindowAbstraction("Textchat", new(400, 200, 600, 300))
		{
			SizeMin = new(400, 150),
			SizeMax = new(1500, 900)
		};
		window.OnExit += Window_OnExit;
		window.SetDrawFunc(DrawFunction, DrawPopup);

		await window.Run();
	}

	private static void Window_OnExit()
	{
		chat.Exit();
	}

	private static bool infoOpen = true;
	private static void DrawPopup()
	{
		if (!infoOpen)
			return;

		ImGui.SetNextWindowSizeConstraints(new(500, 50), new(800, 100));
		ImGui.Begin("Info", ref infoOpen, ImGuiWindowFlags.AlwaysAutoResize);
		ImGui.TextWrapped("Das ist ein Chat Programm! Es hat die besonderheit, dass es über das Tauschlaufwerk der Schule, statt über das Internet läuft.\nDen Code kann man hier anschauen:");

		ImGui.TextColored(new(0.3f, 0.3f, 1, 1), "https://bit.ly/OfflineChatSrc");
		if (ImGui.IsItemClicked())
		{
			// open link in browser
			var psi = new ProcessStartInfo() 
			{ 
				UseShellExecute = true,
				FileName = "https://bit.ly/OfflineChatSrc"
			};
			Process.Start(psi);
		}
		ImGui.End();
	}

	const int footerHeight = 69;
	const int onlineListWidth = 150;

	private static void DrawFunction()
	{
		// update the chat log file
		UpdateFile();

		Vector2 windowSize = new(ImGui.GetWindowWidth(), ImGui.GetWindowHeight());

		// Draw chat
		DrawChat(windowSize);

		// Draw the list of online users
		ImGui.SameLine(); // draw the list at the same height as the chat log
		DrawOnlineList(windowSize);

		ImGui.Separator();

		string inputString = $"<{Environment.UserName}>: ";

		ImGui.PushItemWidth(windowSize.X - 15);
		if (ImGui.InputText("", ref inputString, 200u,
			ImGuiInputTextFlags.AllowTabInput | ImGuiInputTextFlags.EnterReturnsTrue))
		{
			// write message without the name label (gets added later)
			chat.WriteMessage(inputString.Replace($"<{Environment.UserName}>: ", ""));
		};
	}

	private static void DrawOnlineList(Vector2 windowSize)
	{
		ImGui.BeginChild("onlineList", new(onlineListWidth - ImGui.GetFrameHeight(), windowSize.Y - footerHeight), true);
		ImGui.Text("Online:");
		ImGui.Separator();
		ImGui.Separator();

		// Draw all users in a bullet list
		foreach (var user in chat.Users)
		{
			ImGui.BulletText(user);
		}

		ImGui.EndChild();
	}

	private static void DrawChat(Vector2 windowSize)
	{
		ImGui.BeginChild("chattext", new(windowSize.X - onlineListWidth - 5, windowSize.Y - footerHeight), true, 
			ImGuiWindowFlags.HorizontalScrollbar);

		ImGui.Text($"Chat vom {DateTime.Now:D}");
		ImGui.Separator();
		ImGui.Separator();

		// draw chat
		ImGui.Text(chat.Text);

		ImGui.EndChild();
	}

	private static void UpdateFile()
	{
		// read the contents of the chat log file
		chat.ReadLogFile();

		// update last time
		DateTime lastTimeOpened = chat.Date;

		// update users
		chat.UpdateUsers();

		// if one day has passed, clear the logs else update file
		if (DateTime.Now - lastTimeOpened > TimeSpan.FromDays(1))
		{
			chat.Clear();
		}

		// update file
		chat.WriteInfoToFile();
	}
}
