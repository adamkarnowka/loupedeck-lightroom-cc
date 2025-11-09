namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class LightroomWebSocketClient
    {
        private const String DefaultWebSocketUrl = "ws://127.0.0.1:7682";
        private const String AppName = "Loupedeck Lightroom Plugin";
        private const String AppVersion = "1.0.0";
        
        private ClientWebSocket _webSocket;
        private String _webSocketUrl;
        private String _clientGuid;
        private Boolean _isRegistered = false;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

        public Boolean IsConnected => this._webSocket?.State == WebSocketState.Open && this._isRegistered;

        public LightroomWebSocketClient(String webSocketUrl = null)
        {
            this._webSocketUrl = webSocketUrl ?? DefaultWebSocketUrl;
        }

        private async Task EnsureConnectedAsync()
        {
            await this._connectionLock.WaitAsync();
            try
            {
                if (this._webSocket?.State == WebSocketState.Open && this._isRegistered)
                {
                    return;
                }

                if (this._webSocket != null)
                {
                    try
                    {
                        await this._webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None);
                    }
                    catch
                    {
                    }
                    this._webSocket.Dispose();
                }

                this._webSocket = new ClientWebSocket();
                await this._webSocket.ConnectAsync(new Uri(this._webSocketUrl), CancellationToken.None);

                await RegisterAsync();
            }
            finally
            {
                this._connectionLock.Release();
            }
        }

        private async Task RegisterAsync()
        {
            var requestId = Guid.NewGuid().ToString();
            var registerMessage = new
            {
                requestId = requestId,
                @object = (String)null,
                message = "register",
                @params = new[] { AppName, AppVersion, this._clientGuid }
            };

            var json = JsonSerializer.Serialize(registerMessage);
            var buffer = Encoding.UTF8.GetBytes(json);
            await this._webSocket.SendAsync(new ArraySegment<Byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new Byte[8192];
            var result = await this._webSocket.ReceiveAsync(new ArraySegment<Byte>(responseBuffer), CancellationToken.None);
            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, result.Count);

            using (JsonDocument doc = JsonDocument.Parse(responseJson))
            {
                if (doc.RootElement.TryGetProperty("success", out var success) && success.GetBoolean())
                {
                    if (doc.RootElement.TryGetProperty("response", out var response) && response.ValueKind == JsonValueKind.Array)
                    {
                        var responseArray = response.EnumerateArray();
                        if (responseArray.MoveNext())
                        {
                            this._clientGuid = responseArray.Current.GetString();
                        }
                    }
                    this._isRegistered = true;
                    PluginLog.Info("Successfully registered with Lightroom");
                }
                else
                {
                    PluginLog.Warning("Failed to register with Lightroom");
                }
            }
        }

        public async Task IncrementParameterAsync(String parameter, Double? amount = null)
        {
            try
            {
                await EnsureConnectedAsync();

                var requestId = Guid.NewGuid().ToString();
                var messageObj = new
                {
                    requestId = requestId,
                    @object = (String)null,
                    message = "increment",
                    @params = amount.HasValue 
                        ? new Object[] { parameter, amount.Value }
                        : new Object[] { parameter }
                };

                var json = JsonSerializer.Serialize(messageObj);
                var buffer = Encoding.UTF8.GetBytes(json);
                await this._webSocket.SendAsync(new ArraySegment<Byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                PluginLog.Info($"Sent increment for {parameter}" + (amount.HasValue ? $" by {amount.Value}" : ""));
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error incrementing {parameter}: {ex.Message}");
                this._isRegistered = false;
            }
        }

        public async Task DecrementParameterAsync(String parameter, Double? amount = null)
        {
            try
            {
                await EnsureConnectedAsync();

                var requestId = Guid.NewGuid().ToString();
                var messageObj = new
                {
                    requestId = requestId,
                    @object = (String)null,
                    message = "decrement",
                    @params = amount.HasValue 
                        ? new Object[] { parameter, amount.Value }
                        : new Object[] { parameter }
                };

                var json = JsonSerializer.Serialize(messageObj);
                var buffer = Encoding.UTF8.GetBytes(json);
                await this._webSocket.SendAsync(new ArraySegment<Byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                PluginLog.Info($"Sent decrement for {parameter}" + (amount.HasValue ? $" by {amount.Value}" : ""));
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error decrementing {parameter}: {ex.Message}");
                this._isRegistered = false;
            }
        }

        public async Task SendCommandAsync(String command)
        {
            try
            {
                await EnsureConnectedAsync();

                var requestId = Guid.NewGuid().ToString();
                var messageObj = new
                {
                    requestId = requestId,
                    @object = (String)null,
                    message = command,
                    @params = new Object[] { }
                };

                var json = JsonSerializer.Serialize(messageObj);
                var buffer = Encoding.UTF8.GetBytes(json);
                await this._webSocket.SendAsync(new ArraySegment<Byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                PluginLog.Info($"Sent command: {command}");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error sending command {command}: {ex.Message}");
                this._isRegistered = false;
            }
        }

        public async Task SetValueAsync(String parameter, Int32 value)
        {
            try
            {
                await EnsureConnectedAsync();

                var requestId = Guid.NewGuid().ToString();
                var messageObj = new
                {
                    requestId = requestId,
                    @object = (String)null,
                    message = "setValue",
                    @params = new Object[] { parameter, value }
                };

                var json = JsonSerializer.Serialize(messageObj);
                var buffer = Encoding.UTF8.GetBytes(json);
                await this._webSocket.SendAsync(new ArraySegment<Byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                PluginLog.Info($"Set {parameter} to {value}");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error setting {parameter}: {ex.Message}");
                this._isRegistered = false;
            }
        }

        public async Task SetValueAsync(String parameter, Double value)
        {
            try
            {
                await EnsureConnectedAsync();

                var requestId = Guid.NewGuid().ToString();
                var messageObj = new
                {
                    requestId = requestId,
                    @object = (String)null,
                    message = "setValue",
                    @params = new Object[] { parameter, value }
                };

                var json = JsonSerializer.Serialize(messageObj);
                var buffer = Encoding.UTF8.GetBytes(json);
                await this._webSocket.SendAsync(new ArraySegment<Byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                PluginLog.Info($"Set {parameter} to {value}");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error setting {parameter}: {ex.Message}");
                this._isRegistered = false;
            }
        }

        public async Task CloseAsync()
        {
            if (this._webSocket?.State == WebSocketState.Open)
            {
                try
                {
                    await this._webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Plugin closing", CancellationToken.None);
                }
                catch
                {
                }
            }
            this._webSocket?.Dispose();
            this._isRegistered = false;
        }
    }
}

