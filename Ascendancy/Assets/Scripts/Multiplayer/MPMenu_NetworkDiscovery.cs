﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using UnityEngine;
using Mirror.Discovery;
using UnityEngine.Events;
using Mirror;

[Serializable]
    public class MPMenu_ServerFoundUnityEvent : UnityEvent<MPMenu_ServerResponse> { };

    [DisallowMultipleComponent]
    [AddComponentMenu("Network/CustomNetworkDiscovery")]
    public class MPMenu_NetworkDiscovery : NetworkDiscoveryBase<ServerRequest, MPMenu_ServerResponse>
    {
        #region Server

        public long ServerId { get; private set; }

        [Tooltip("Transport to be advertised during discovery")]
        public Transport transport;

        public ServerListControl control;

        [Tooltip("Invoked when a server is found")]
        public MPMenu_ServerFoundUnityEvent OnServerFound;

        public override void Start()
        {
            ServerId = RandomLong();

            // active transport gets initialized in awake
            // so make sure we set it here in Start()  (after awakes)
            // Or just let the user assign it in the inspector
            if (transport == null)
                transport = Transport.activeTransport;

            base.Start();

            Debug.Log("Started server discovery");
        }

        /// <summary>
        /// Process the request from a client
        /// </summary>
        /// <remarks>
        /// Override if you wish to provide more information to the clients
        /// such as the name of the host player
        /// </remarks>
        /// <param name="request">Request comming from client</param>
        /// <param name="endpoint">Address of the client that sent the request</param>
        /// <returns>The message to be sent back to the client or null</returns>
        protected override MPMenu_ServerResponse ProcessRequest(ServerRequest request, IPEndPoint endpoint)
        {
            Debug.Log("Got a request, creating response");
            // In this case we don't do anything with the request
            // but other discovery implementations might want to use the data
            // in there,  This way the client can ask for
            // specific game mode or something

            Debug.Log("Server request from " + endpoint.Address.ToString());

            try
            {
                // this is an example reply message,  return your own
                // to include whatever is relevant for your game
                return new MPMenu_ServerResponse
                {
                    serverId = ServerId,
                    uri = transport.ServerUri(),
                    nameServer = "Server #" + ServerId,
                    nameHost = "hostname",
                    playerCount = 0,
                    playerMax = 8,
                    passwordProtection = false,
                    ping = 0
                };
            }
            catch (NotImplementedException)
            {
                Debug.LogError($"Transport {transport} does not support network discovery");
                throw;
            }
        }

        #endregion

        #region Client

        /// <summary>
        /// Create a message that will be broadcasted on the network to discover servers
        /// </summary>
        /// <remarks>
        /// Override if you wish to include additional data in the discovery message
        /// such as desired game mode, language, difficulty, etc... </remarks>
        /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
        protected override ServerRequest GetRequest()
        {
            Debug.Log("Sending new Request out");
            return new ServerRequest();
        }

        /// <summary>
        /// Process the answer from a server
        /// </summary>
        /// <remarks>
        /// A client receives a reply from a server, this method processes the
        /// reply and raises an event
        /// </remarks>
        /// <param name="response">Response that came from the server</param>
        /// <param name="endpoint">Address of the server that replied</param>
        protected override void ProcessResponse(MPMenu_ServerResponse response, IPEndPoint endpoint)
        {
            // we received a message from the remote endpoint
            response.EndPoint = endpoint;
            
            Debug.Log("Received Response");
            // although we got a supposedly valid url, we may not be able to resolve
            // the provided host
            // However we know the real ip address of the server because we just
            // received a packet from it,  so use that as host.
            UriBuilder realUri = new UriBuilder(response.uri)
            {
                Host = response.EndPoint.Address.ToString()
            };
            response.uri = realUri.Uri;

            Debug.Log("Server found on " + response.EndPoint.Address.ToString());
            control.AddEntry(response);
            OnServerFound.Invoke(response);
        }

        #endregion

        public void ServerFound()
        {
            Debug.Log("Server Found!");
        }
    }
