using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using WatsonWebsocket;

namespace NanosSharp.Client;

/// <summary>
/// The bridge server is a WebSocket server connecting the nanos world game with the NanosSharp client runtime.
/// </summary>
internal sealed class BridgeServer
{
    /// <summary>
    /// The instance of the bridge server.
    /// </summary>
    internal static BridgeServer Instance { get; } = new();
    
    /// <summary>
    /// The event that gets called when a nanos event was received.
    /// </summary>
    internal event Action<string, JsonArray> NanosEventReceived = null!; 
    
    /// <summary>
    /// An dictionary of pending RPC calls.
    /// </summary>
    private readonly ConcurrentDictionary<string, TaskCompletionSource<JsonElement?>> _pendingRpcCalls = new();
    private WatsonWsServer _server;

    private BridgeServer()
    {
        _server = new WatsonWsServer(port: 25302);
        _server.MessageReceived += OnMessage;
    }
    
    /// <summary>
    /// Starts the bridge server.
    /// </summary>
    internal void Start()
    {
        _server.Start();
    }
    
    /// <summary>
    /// Handles a message that was received by the WebSocket server.
    /// </summary>
    private void OnMessage(object? sender, MessageReceivedEventArgs e)
    {
        if (Runtime.Config.IsDebug)
        {
            Runtime.Logger.Verbose("Received WebSocket message: {Data}", Encoding.UTF8.GetString(e.Data));
        }
        
        var json = JsonDocument.Parse(e.Data);
        var root = json.RootElement;
        
        if (root.TryGetProperty("event", out var eventName) && root.TryGetProperty("args", out var args))
        {
            if (args.ValueKind == JsonValueKind.Array)
            {
                NanosEventReceived(eventName.GetString()!, JsonArray.Create(args) ?? []);
            }
        }
        else if (root.TryGetProperty("callID", out var callID))
        {
            if (_pendingRpcCalls.TryRemove(callID.GetString()!, out var tcs))
            {
                tcs.SetResult(root.GetProperty("result").ValueKind == JsonValueKind.Null ? null : root.GetProperty("result"));
            }
        }
    }

    /// <summary>
    /// Calls a nanos function without any return value.
    /// </summary>
    /// <param name="key">The key of the function to call.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    internal void CallNanos(string key, params object[] args)
    {
        var msg = JsonSerializer.Serialize(new Call
        {
            Key = key,
            Args = args
        });
        
        foreach (var client in _server.ListClients())
        {
            _server.SendAsync(client.Guid, msg);
        }
        
        Runtime.Logger.Verbose("Called nanos function w/o return: {Key} with args: {Args}", key, args);
    }
    
    /// <summary>
    /// Calls a nanos function with a return value.
    /// </summary>
    /// <param name="key">The key of the function to call.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <returns>The return value of the function.</returns>
    internal Task<JsonElement?> CallNanosAsync(string key, params object[] args)
    {
        var callID = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<JsonElement?>();
        
        _pendingRpcCalls[callID] = tcs;
        
        var msg = JsonSerializer.Serialize(new RpcCall
        {
            CallID = callID,
            Key = key,
            Args = args
        });
        
        foreach (var client in _server.ListClients())
        {
            _server.SendAsync(client.Guid, msg);
        }
        
        Runtime.Logger.Verbose("Called nanos function w/ return: {Key} with args: {Args} and callID: {CallID}", key, args, callID);
        
        return tcs.Task;
    }

    private class Call
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = null!;
        
        [JsonPropertyName("args")]
        public object[] Args { get; set; } = null!;
    }

    private class RpcCall
    {
        [JsonPropertyName("callID")]
        public string CallID { get; set; } = null!;
        
        [JsonPropertyName("key")]
        public string Key { get; set; } = null!;
        
        [JsonPropertyName("args")]
        public object[] Args { get; set; } = null!;
    }
}