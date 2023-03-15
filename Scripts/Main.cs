global using Godot;
global using GodotUtils;
global using System;

using GodotUtils.Netcode;
using GodotUtils.Netcode.Server;
using GodotUtils.Netcode.Client;
using ENet;

namespace Sandbox2;

public partial class Main : Node
{
	public static ENetServer<ClientPacketOpcode> Server { get; set; } = new();
	public static ENetClient<ServerPacketOpcode> Client { get; set; } = new();

	public override async void _Ready()
	{
		Server.Start(25565, 100);
		Client.Start("localhost", 25565);

		await Task.Delay(500);
		
		Client.Send(ClientPacketOpcode.Info, new CPacketInfo
		{
			Username = "Freddy"
		});
	}

	public override void _PhysicsProcess(double delta)
	{
		Logger.Update();
	}
}

public class SPacketInfo : APacketServer
{
	public override void Handle()
	{
		throw new NotImplementedException();
	}
}

public class SPacketPong : APacketServer
{
	public int Data { get; set; }

	public override void Write(PacketWriter writer)
	{
		writer.Write(Data);
	}

	public override void Read(PacketReader reader)
	{
		Data = reader.ReadInt();
	}

	public override void Handle()
	{
		Logger.Log("[Client] Pong received from server. Value is " + Data);
	}
}

public class CPacketInfo : APacketClient
{
	public string Username { get; set; }

	public override void Write(PacketWriter writer)
	{
		writer.Write(Username);
	}

	public override void Read(PacketReader reader)
	{
		Username = reader.ReadString();
	}

	public override void Handle(Peer peer)
	{
		Logger.Log("Hello from the server. The username is " + Username);
		Main.Server.Send(ServerPacketOpcode.Pong, new SPacketPong
		{
			Data = 66
		}, peer);
	}
}

public enum ServerPacketOpcode
{
	Pong,
	Info
}

public enum ClientPacketOpcode
{
	Info
}
