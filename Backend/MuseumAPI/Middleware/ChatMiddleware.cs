﻿using System.Collections.Concurrent;
using System.Net.WebSockets;
using MuseumAPI.Context;
using MuseumAPI.Models;
using System.Text;

namespace MuseumAPI.Middleware
{
    public class ChatMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();
        private readonly ConcurrentDictionary<string, string> _nicknames = new();

        public ChatMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = Guid.NewGuid().ToString();
            _sockets.TryAdd(socketId, socket);

            var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<MuseumContext>();

            var lastMessages = _context.ChatMessages
                .OrderByDescending(m => m.Timestamp)
                .Take(10)
                .ToList();

            lastMessages.Reverse();
            foreach (var message in lastMessages)
            {
                var msg = $"{message.Nickname}: {message.Message}";
                var buffer = Encoding.UTF8.GetBytes(msg);
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            while (socket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessage(socket);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine($"Received message: {message} from socket {socketId}");

                    // Check if socket has nickname
                    if (!_nicknames.ContainsKey(socketId))
                    {
                        _nicknames.TryAdd(socketId, message);
                        Console.WriteLine($"Added nickname: {message} to socket {socketId}");
                        message = $"User {message} joined the chat";
                    }
                    else
                    {
                        // Add nickname to message
                        message = $"{_nicknames[socketId]}: {message}";
                    }

                    // Save chat message in the database
                    _context.ChatMessages.Add(new ChatMessage
                    {
                        Timestamp = DateTime.Now,
                        Nickname = _nicknames[socketId],
                        Message = message
                    });
                    await _context.SaveChangesAsync();

                    await BroadcastMessage(message, socketId);
                }
            }

            // Send a user disconnected message
            if (_nicknames.ContainsKey(socketId))
            {
                var message = $"User {_nicknames[socketId]} disconnected";
                await BroadcastMessage(message, socketId);
            }

            // Remove the closed socket from the dictionary
            WebSocket dummy;
            _sockets.TryRemove(socketId, out dummy!);

            // Remove the nickname of the disconnected user
            string dummyNickname;
            _nicknames.TryRemove(socketId, out dummyNickname!);
        }

        private async Task<string> ReceiveMessage(WebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[4096]);
            var received = await socket.ReceiveAsync(buffer, CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer.Array!, 0, received.Count);
            return message;
        }

        private async Task BroadcastMessage(string message, string senderSocketId)
        {
            foreach (var socketPair in _sockets)
            {
                //if (socketPair.Key == senderSocketId) continue;
                if (socketPair.Value.State != WebSocketState.Open) continue;

                Console.WriteLine($"Sending message: {message} to socket {socketPair.Key}");

                var buffer = Encoding.UTF8.GetBytes(message);
                await socketPair.Value.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}