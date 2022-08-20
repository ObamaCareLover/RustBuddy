using WebSocketSharp;
using Google.Protobuf;

using RustBuddy.API.EventArgs;

namespace RustBuddy.API
{
    /// <summary>
    /// Event handler delegate
    /// </summary>
    public delegate void EventHandler<TEventArgs>(object? sender, TEventArgs e);

    public class RustSocket : IRustSocket
    {
        private readonly WebSocket WebSocket;
        private readonly RustSocketConfiguration Config;

        private uint Sequence { get; set; } = 0;
        private Dictionary<uint, AppResponse> SequenceList { get; set; } = new Dictionary<uint, AppResponse>();

        public event EventHandler<EntityChangedEventArgs>? OnEntityChanged;
        public event EventHandler<TeamMessageEventArgs>? OnTeamMessage;
        public event EventHandler<TeamChangedEventArgs>? OnTeamChanged;

        public RustSocket(RustSocketConfiguration configuration)
        {
            Config = configuration;

            WebSocket = new WebSocket($"ws://{Config.Server}:{Config.Port}");

            WebSocket.OnOpen += WsOnOpen;
            WebSocket.OnError += WsOnError;
            WebSocket.OnClose += WsOnClose;
            WebSocket.OnMessage += WsOnMessage;

            WebSocket.Connect();
        }

        #region Websocket Events

        private async void WsOnOpen(object? sender, System.EventArgs e)
        {
            await GetTeamChat();
            await GetMapMarkers();
        }
        
        private void WsOnError(object? sender, WebSocketSharp.ErrorEventArgs e)
        {
            Console.WriteLine($"[Websocket Error] Reason: {e.Message}\t");
        }

        private async void WsOnClose(object? sender, CloseEventArgs e)
        {
            Console.WriteLine($"[Websocket Closed] Reason: {e.Reason}\t Code: {e.Code}");
        }

        private void WsOnMessage(object? sender, MessageEventArgs e)
        {
            var parsed = AppMessage.Parser.ParseFrom(e.RawData);

            if (parsed.Response != null)
                SequenceList.Add(parsed.Response.Seq, parsed.Response);
            else if (parsed.Broadcast != null)
            {
                if (parsed.Broadcast.EntityChanged != null)
                {
                    OnEntityChanged?.Invoke(this, new EntityChangedEventArgs {
                        Id = parsed.Broadcast.EntityChanged.EntityId,
                        State = parsed.Broadcast.EntityChanged.Payload.Value
                    });
                }
                else if (parsed.Broadcast.TeamMessage != null)
                {
                    OnTeamMessage?.Invoke(this, new TeamMessageEventArgs
                    {
                        Name = parsed.Broadcast.TeamMessage.Message.Name,
                        Message = parsed.Broadcast.TeamMessage.Message.Message,
                        Steam = parsed.Broadcast.TeamMessage.Message.SteamId,
                        Time = parsed.Broadcast.TeamMessage.Message.Time
                    });
                }
                else if (parsed.Broadcast.TeamChanged != null)
                {
                    OnTeamChanged?.Invoke(this, new TeamChangedEventArgs
                    {
                        // Populate
                    });
                }
            }
        }

        #endregion

        #region Websocket Logic

        private async Task Reconnect()
        {
            await Task.Run(() =>
            {
                
            });

            if (WebSocket.ReadyState == WebSocketState.Open)
                Resubscribe();
        }

        private async void Resubscribe()
        {
            // Todo: rework this shit
            await GetTeamChat();
        }

        private async Task<AppResponse?> SendRequestAsync(AppRequest request)
        {
            AppResponse? ret = null;

            if (WebSocket.ReadyState != WebSocketState.Open)
                throw new Exception($"Websocket not in open state");

            Sequence++;

            request.Seq = Sequence;
            request.PlayerId = Config.Steam;
            request.PlayerToken = Config.Token;

            var stream = new MemoryStream();

            request.WriteTo(stream);

            WebSocket.Send(stream.ToArray());

            var timeout = 0;

            while (ret == null)
            {
                if (SequenceList.ContainsKey(request.Seq))
                    ret = SequenceList[request.Seq];

                if (timeout == 25)
                    throw new Exception($"Timeout on request {request.Seq}");

                await Task.Delay(200);
                timeout++;
            }

            SequenceList.Remove(request.Seq);

            return ret;
        }

        #endregion

        #region Application Program Interface

        /// <summary>
        /// Sends a message to team chat
        /// </summary>
        public async Task<bool> SendTeamMessage(string message)
        {
            message = $"[Rust+] {message}";
            var response = await SendRequestAsync(new AppRequest()
            {
                SendTeamMessage = new AppSendMessage()
                {
                    Message = message,
                }
            });

            if (response.Error != null)
                throw new Exception($"{response.Error.Error}");

            return true;
        }

        /// <summary>
        /// Sets the state of a given entity
        /// </summary>
        public async Task<bool> SetEntityState(uint entity_id, bool state)
        {
            var response = await SendRequestAsync(new AppRequest()
            {
                EntityId = entity_id,
                SetEntityValue = new AppSetEntityValue()
                {
                    Value = state
                }
            });

            if (response.Error != null)
                throw new Exception($"{response.Error.Error}");

            return true;
        }

        /// <summary>
        /// Fetches the information of the given entity and subscribes to the entitys broadcast
        /// </summary>
        public async Task<AppEntityInfo> GetEntityInfo(uint entity_id, bool subscribe = true)
        {
            var response = await SendRequestAsync(new AppRequest()
            {
                GetEntityInfo = new AppEmpty(),
                EntityId = entity_id,
                SetSubscription = new AppFlag() { Value = subscribe }
            });

            if (response.Error != null)
                throw new Exception($"{response.Error.Error}");

            return response.EntityInfo;
        }

        /// <summary>
        /// Fetches an array of all of the current markers on the map
        /// </summary>
        public async Task<IList<AppMarker>> GetMapMarkers()
        {
            var response = await SendRequestAsync(new AppRequest()
            {
                GetMapMarkers = new AppEmpty()
            });

            if (response.Error != null)
                throw new Exception($"{response.Error.Error}");

            return response.MapMarkers.Markers.ToList();
        }

        /// <summary>
        /// Fetches team chat history and subscribes to team cheat broadcast
        /// </summary>
        public async Task<AppTeamChat> GetTeamChat()
        {
            var response = await SendRequestAsync(new AppRequest()
            {
                GetTeamChat = new AppEmpty(),
                SetSubscription = new AppFlag() { Value = true }
            });

            if (response.Error != null)
                throw new Exception($"{response.Error.Error}");

            return response.TeamChat;
        }

        /// <summary>
        /// Fetches the current ingame time
        /// </summary>
        public async Task<AppTime> GetTime()
        {
            var response = await SendRequestAsync(new AppRequest()
            {
                GetTime = new AppEmpty()
            });

            if (response.Error != null)
                throw new Exception($"{response.Error.Error}");

            return response.Time;
        }

        #endregion
    }
}
