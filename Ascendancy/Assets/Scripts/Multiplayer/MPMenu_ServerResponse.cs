using Mirror;
using System;
using System.Net;

public class MPMenu_ServerResponse : MessageBase
{
    // The server that sent this
    // this is a property so that it is not serialized,  but the
    // client fills this up after we receive it
    public IPEndPoint EndPoint { get; set; }

    public Uri uri;

    // Prevent duplicate server appearance when a connection can be made via LAN on multiple NICs
    public long serverId;

    // Number of players currently on the server
    public int playerCount;

    // Number of players allowed on the server
    public int playerMax;

    // Is true when the server is protected via password
    public bool passwordProtection;

    // Name of the server
    public string nameServer;

    // Name of the server's host
    public string nameHost;

    // Current ping
    public int ping;
}
