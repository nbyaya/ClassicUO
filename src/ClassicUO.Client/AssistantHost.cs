<<<<<<< HEAD
﻿using ClassicUO.Assets;
using ClassicUO.Configuration;
using ClassicUO.Game;
using ClassicUO.Game.Data;
using ClassicUO.Network;
using Microsoft.Xna.Framework.Graphics;
using StructPacker;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;


namespace ClassicUO
{
    interface IPluginHost
    {
        public Dictionary<IntPtr, GraphicsResource> GfxResources { get; }

        public void Initialize();
        public void LoadPlugin(string pluginPath);
        public void Tick();
        public void Closing();
        public void FocusGained();
        public void FocusLost();
        public void Connected();
        public void Disconnected();
        public bool Hotkey(int key, int mod, bool pressed);
        public void Mouse(int button, int wheel);
        public void GetCommandList(out IntPtr listPtr, out int listCount);
        public unsafe int SdlEvent(SDL2.SDL.SDL_Event* ev);
        public void UpdatePlayerPosition(int x, int y, int z);
        public bool PacketIn(ArraySegment<byte> buffer);
        public bool PacketOut(Span<byte> buffer);
    }


    sealed class AssistantHost : TcpClientRpc, IPluginHost
    {
        protected override ArraySegment<byte> OnRequest(ArraySegment<byte> msg)
        {
            if (msg.Count <= 0)
                return ArraySegment<byte>.Empty;

            var cmd = (PluginCuoProtocol)msg.Array[0];
=======
﻿using ClassicUO.Network;
using System;
using System.Threading.Tasks;

namespace ClassicUO
{
    sealed class AssistantHost : TcpClientRpc
    {
        protected override void OnMessage(RpcMessage msg)
        {
            //Console.WriteLine("cmd {0} {1}", msg.Command, msg.ID);

            if (msg.Command == RpcCommand.Response)
                return;

            if (msg.Payload.Count <= 0)
                return;

            var cmd = (PluginCuoProtocol)msg.Payload.Array[0];
>>>>>>> rpc support

            switch (cmd)
            {
                case PluginCuoProtocol.OnPluginRecv:
<<<<<<< HEAD
                    {
                        var packetLen = BinaryPrimitives.ReadUInt16LittleEndian(msg.AsSpan(sizeof(byte), sizeof(ushort)));
                        
                        lock (PacketHandlers.Handler)
                        {
                            PacketHandlers.Handler.Append(msg.AsSpan(sizeof(byte) + sizeof(ushort), packetLen), true);
                        }
                    }
                    break;

                case PluginCuoProtocol.OnPluginSend:
                    {
                        var packetLen = BinaryPrimitives.ReadUInt16LittleEndian(msg.AsSpan(sizeof(byte), sizeof(ushort)));

                        if (NetClient.Socket.IsConnected)
                        {
                            NetClient.Socket.Send(msg.AsSpan(sizeof(byte) + sizeof(ushort), packetLen), true);
                        }
                    }
                    
                    break;

                case PluginCuoProtocol.OnPacketLength:
                    {
                        var req = new PluginPacketLengthRequest();
                        req.Unpack(msg.Array, msg.Offset);

                        var resp = new PluginPacketLengthResponse()
                        {
                            Cmd = (byte)PluginCuoProtocol.OnPacketLength,
                            PacketLength = PacketsTable.GetPacketLength(req.ID)
                        };

                        using var buf = resp.PackToBuffer();

                        return new ArraySegment<byte>(buf.Data, 0, buf.Size);
                    }

                case PluginCuoProtocol.OnCastSpell:
                    {
                        var req = new PluginCastSpell();
                        req.Unpack(msg.Array, msg.Offset);

                        GameActions.CastSpell(req.SpellIndex);
                    }
                    break;
                case PluginCuoProtocol.OnSetWindowTitle:
                    {
                        var req = new PluginSetWindowTitle();
                        req.Unpack(msg.Array, msg.Offset);

                        Client.Game.SetWindowTitle(req.Title);
                    }
                    break;
                case PluginCuoProtocol.OnGetCliloc:
                    {
                        var req = new PluginGetCliloc();
                        req.Unpack(msg.Array, msg.Offset);

                        var result = ClilocLoader.Instance.Translate(req.Cliloc, req.Args, req.Capitalize);

                        var resp = new PluginGetClilocResponse()
                        {
                            Cmd = req.Cmd,
                            Text = result
                        };

                        using var buf = resp.PackToBuffer();

                        return new ArraySegment<byte>(buf.Data, 0, buf.Size);
                    }
                case PluginCuoProtocol.OnRequestMove:
                    {
                        var req = new PluginRequestMove();
                        req.Unpack(msg.Array, msg.Offset);

                        var ok = Client.Game.UO.World.Player.Walk((Direction)req.Direction, req.Run);

                        var resp = new PluginRequestMoveResponse()
                        {
                            Cmd = req.Cmd,
                            CanMove = ok
                        };

                        using var buf = resp.PackToBuffer();

                        return new ArraySegment<byte>(buf.Data, 0, buf.Size);
                    }
                case PluginCuoProtocol.OnGetPlayerPosition:
                    {
                        var req = new PluginGetPlayerPosition();
                        req.Unpack(msg.Array, msg.Offset);

                        var resp = new PluginGetPlayerPositionResponse()
                        {
                            Cmd = req.Cmd,
                            X = Client.Game.UO.World.Player.X,
                            Y = Client.Game.UO.World.Player.Y,
                            Z = Client.Game.UO.World.Player.Z,
                        };

                        using var buf = resp.PackToBuffer();

                        return new ArraySegment<byte>(buf.Data, 0, buf.Size);
                    }
            }

            return ArraySegment<byte>.Empty;
=======
                    lock (PacketHandlers.Handler)
                    {
                        PacketHandlers.Handler.Append(msg.Payload.Array.AsSpan(1, msg.Payload.Count - 1), true);
                    }

                    break;

                case PluginCuoProtocol.OnPluginSend:
                    if (NetClient.Socket.IsConnected)
                    {
                        NetClient.Socket.Send(msg.Payload.Array.AsSpan(1, msg.Payload.Count - 1), true);
                    }
                    break;
            }
        }

        protected override void OnConnected()
        {
            PluginInitialize();

            base.OnConnected();
>>>>>>> rpc support
        }

        enum PluginCuoProtocol : byte
        {
            OnInitialize,
            OnTick,
            OnClosing,
            OnFocusGained,
            OnFocusLost,
            OnConnected,
            OnDisconnected,
            OnHotkey,
            OnMouse,
            OnCmdList,
            OnSdlEvent,
            OnUpdatePlayerPos,
            OnPacketIn,
            OnPacketOut,

            OnPluginRecv,
            OnPluginSend,
<<<<<<< HEAD
            OnPacketLength,
            OnCastSpell,
            OnSetWindowTitle,
            OnGetCliloc,
            OnRequestMove,
            OnGetPlayerPosition,
            OnUpdatePlayerPosition,
        }

        [Pack]
        internal struct PluginInitializeRequest
        {
            public byte Cmd;
            public uint ClientVersion;
            public string PluginPath;
            public string AssetsPath;
        }

        [Pack]
        internal struct PluginHotkeyRequest
        {
            public byte Cmd;
            public int Key;
            public int Mod;
            public bool IsPressed;
        }

        [Pack]
        internal struct PluginHotkeyResponse
        {
            public byte Cmd;
            public bool Allowed;
        }

        [Pack]
        internal struct PluginMouseRequest
        {
            public byte Cmd;
            public int Button;
            public int Wheel;
        }

        [Pack]
        internal struct PluginSimpleRequest
        {
            public byte Cmd;
        }

        //[Pack]
        //internal struct PluginPacketRequestResponse
        //{
        //    public byte Cmd;
        //    public byte[] Packet;
        //}

        //[Pack]
        //internal struct PluginSdlEvent
        //{
        //    public byte Cmd;
        //    public SDL2.SDL.SDL_Event
        //}

        [Pack]
        internal struct PluginPacketLengthRequest
        {
            public byte Cmd;
            public byte ID;
        }

        [Pack]
        internal struct PluginPacketLengthResponse
        {
            public byte Cmd;
            public short PacketLength;
        }

        [Pack]
        internal struct PluginCastSpell
        {
            public byte Cmd;
            public int SpellIndex;
        }

        [Pack]
        internal struct PluginSetWindowTitle
        {
            public byte Cmd;
            public string Title;
        }

        [Pack]
        internal struct PluginGetCliloc
        {
            public byte Cmd;
            public int Cliloc;
            public string Args;
            public bool Capitalize;
        }

        [Pack]
        internal struct PluginGetClilocResponse
        {
            public byte Cmd;
            public string Text;
        }

        [Pack]
        internal struct PluginRequestMove
        {
            public byte Cmd;
            public int Direction;
            public bool Run;
        }

        [Pack]
        internal struct PluginRequestMoveResponse
        {
            public byte Cmd;
            public bool CanMove;
        }

        [Pack]
        internal struct PluginGetPlayerPosition
        {
            public byte Cmd;
        }

        [Pack]
        internal struct PluginGetPlayerPositionResponse
        {
            public byte Cmd;
            public int X, Y, Z;
        }

        [Pack]
        internal struct PluginUpdatePlayerPositionRequest
        {
            public byte Cmd;
            public int X, Y, Z;
        }


        private readonly Dictionary<PluginCuoProtocol, byte[]> _simpleRequests = new Dictionary<PluginCuoProtocol, byte[]>();
        public Dictionary<IntPtr, GraphicsResource> GfxResources { get; } = new Dictionary<nint, GraphicsResource>();

        // TODO: find a better way to return array. Maybe a struct container idk
        private void ReturnArray(ArraySegment<byte> segment)
        {
            if (segment.Array != null && segment.Array.Length > 0)
            {
                ArrayPool<byte>.Shared.Return(segment.Array);
            }
        }

        private void MakeSimpleRequest(PluginCuoProtocol protocol)
        {
            if (!_simpleRequests.TryGetValue(protocol, out var buf))
            {
                var req = new PluginSimpleRequest()
                {
                    Cmd = (byte)protocol
                };

                buf = req.Pack();
                _simpleRequests.Add(protocol, buf);
            }

            var resp = Request(new ArraySegment<byte>(buf));

            ReturnArray(resp);
        }

        public void Initialize()
        {

        }

        public void LoadPlugin(string pluginPath)
        {
            if (string.IsNullOrEmpty(pluginPath))
                return;

            var req = new PluginInitializeRequest()
            {
                Cmd = (byte) PluginCuoProtocol.OnInitialize,
                ClientVersion = (uint)Client.Game.UO.Version,
                PluginPath = pluginPath,
                AssetsPath = Settings.GlobalSettings.UltimaOnlineDirectory
            };

            using var buf = req.PackToBuffer();
            var resp = Request(new ArraySegment<byte>(buf.Data, 0, buf.Size));
            ReturnArray(resp);
        }

        public void Tick()
        {
            MakeSimpleRequest(PluginCuoProtocol.OnTick);
        }

        public void Closing()
        {
            MakeSimpleRequest(PluginCuoProtocol.OnClosing);
        }

        public void FocusGained()
        {
            MakeSimpleRequest(PluginCuoProtocol.OnFocusGained);
        }

        public void FocusLost()
        {
            MakeSimpleRequest(PluginCuoProtocol.OnFocusLost);
        }

        public void Connected()
        {
            MakeSimpleRequest(PluginCuoProtocol.OnConnected);
        }

        public void Disconnected() 
        {
            MakeSimpleRequest(PluginCuoProtocol.OnDisconnected);
        }

        public bool Hotkey(int key, int mod, bool ispressed)
        {
            var req = new PluginHotkeyRequest()
            {
                Cmd = (byte)PluginCuoProtocol.OnHotkey,
                Key = key,
                Mod = mod,
                IsPressed = ispressed
            };

            using var buf = req.PackToBuffer();
            var respMsg = Request(new ArraySegment<byte>(buf.Data, 0, buf.Size));

            var resp = new PluginHotkeyResponse();
            resp.Unpack(respMsg.Array, respMsg.Offset);

            ReturnArray(respMsg);

            return resp.Allowed;
        }

        public void Mouse(int button, int wheel)
        {
            var req = new PluginMouseRequest()
            {
                Cmd = (byte)PluginCuoProtocol.OnMouse,
                Button = button,
                Wheel = wheel
            };

            using var buf = req.PackToBuffer();
            var resp = Request(new ArraySegment<byte>(buf.Data, 0, buf.Size));
            ReturnArray(resp);
        }

        public void GetCommandList(out IntPtr ptr, out int listCount)
        {
            ptr = IntPtr.Zero;
            listCount = 0;
        }

        public unsafe int SdlEvent(SDL2.SDL.SDL_Event* ev)
        {
            return 0;
        }

        public void UpdatePlayerPosition(int x, int y, int z)
        {
            var req = new PluginUpdatePlayerPositionRequest()
            {
                Cmd = (byte)PluginCuoProtocol.OnUpdatePlayerPosition,
                X = x,
                Y = y,
                Z = z
            };

            using var buf = req.PackToBuffer();

            var reqMsg = Request(new ArraySegment<byte>(buf.Data, 0, buf.Size));
        }

        public bool PacketIn(ArraySegment<byte> buffer)
        {
            if (buffer.Array != null && buffer.Count > 0)
            {
                var rentBuf = ArrayPool<byte>.Shared.Rent(sizeof(byte) + sizeof(ushort) + buffer.Count);

                try
                {
                    rentBuf[0] = (byte)PluginCuoProtocol.OnPacketIn;
                    BinaryPrimitives.WriteUInt16LittleEndian(rentBuf.AsSpan(sizeof(byte), sizeof(ushort)), (ushort) buffer.Count);

                    buffer.CopyTo(new ArraySegment<byte>(rentBuf, sizeof(byte) + sizeof(ushort), buffer.Count));

                    var respMsg = Request(new ArraySegment<byte>(rentBuf, 0, sizeof(byte) + sizeof(ushort) + buffer.Count));

                    var packetLen = BinaryPrimitives.ReadUInt16LittleEndian(respMsg.Array.AsSpan(sizeof(byte), sizeof(ushort)));

                    if (packetLen > 0)
                    {
                        respMsg.Array.AsSpan(sizeof(byte) + sizeof(ushort), packetLen).CopyTo(buffer.Array);
                        ReturnArray(respMsg);
                        return true;
                    }

                    ReturnArray(respMsg);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(rentBuf);
                }
            }

            return false;
        }

        public bool PacketOut(Span<byte> buffer)
        {
            if (buffer.IsEmpty) return true;

            var rentBuf = ArrayPool<byte>.Shared.Rent(sizeof(byte) + sizeof(ushort) + buffer.Length);

            try
            {
                rentBuf[0] = (byte)PluginCuoProtocol.OnPacketOut;
                BinaryPrimitives.WriteUInt16LittleEndian(rentBuf.AsSpan(sizeof(byte), sizeof(ushort)), (ushort)buffer.Length);

                buffer.CopyTo(new ArraySegment<byte>(rentBuf, sizeof(byte) + sizeof(ushort), buffer.Length));

                var respMsg = Request(new ArraySegment<byte>(rentBuf, 0, sizeof(byte) + sizeof(ushort) + buffer.Length));

                var packetLen = BinaryPrimitives.ReadUInt16LittleEndian(respMsg.Array.AsSpan(sizeof(byte), sizeof(ushort)));

                if (packetLen > 0)
                {
                    respMsg.Array.AsSpan(sizeof(byte) + sizeof(ushort), packetLen).CopyTo(buffer);
                    ReturnArray(respMsg);
                    return true;
                }

                ReturnArray(respMsg);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentBuf);
            }

            return false;
=======
        }


        private static readonly ArraySegment<byte> InitializeMessage = new ArraySegment<byte>(
            new byte[1] { (byte)PluginCuoProtocol.OnInitialize });

        private static readonly ArraySegment<byte> TickMessage = new ArraySegment<byte>(
            new byte[1] { (byte)PluginCuoProtocol.OnTick });

        private static readonly ArraySegment<byte> ClosingMessage = new ArraySegment<byte>(
            new byte[1] { (byte)PluginCuoProtocol.OnClosing });

        private static readonly ArraySegment<byte> FocusGainedMessage = new ArraySegment<byte>(
            new byte[1] { (byte)PluginCuoProtocol.OnFocusGained });

        private static readonly ArraySegment<byte> FocusLostMessage = new ArraySegment<byte>(
            new byte[1] { (byte)PluginCuoProtocol.OnFocusLost });

        private static readonly ArraySegment<byte> ConnectedMessage = new ArraySegment<byte>(
            new byte[1] { (byte)PluginCuoProtocol.OnConnected });

        private static readonly ArraySegment<byte> DisconnectedMessage = new ArraySegment<byte>(
            new byte[1] { (byte)PluginCuoProtocol.OnDisconnected });


        public void PluginInitialize()
        {
            var response = Request(InitializeMessage).GetAwaiter().GetResult();
        }

        public void PluginTick()
        {
            var response = Request(TickMessage).GetAwaiter().GetResult();
        }

        public void PluginClosing()
        {
            var response = Request(ClosingMessage).GetAwaiter().GetResult();
        }

        public void PluginFocusGained()
        {
            var response = Request(FocusGainedMessage).GetAwaiter().GetResult();
        }

        public void PluginFocusLost()
        {
            var response = Request(FocusLostMessage).GetAwaiter().GetResult();
        }

        public void PluginConnected()
        {
            var response = Request(ConnectedMessage).GetAwaiter().GetResult();
        }

        public void PluginDisconnected() 
        {
            var response = Request(DisconnectedMessage).GetAwaiter().GetResult();
        }

        public bool PluginHotkeys(int key, int mod, bool ispressed)
        {
            var buf = new byte[1];
            buf[0] = (byte)PluginCuoProtocol.OnHotkey;
            var response = Request(new ArraySegment<byte>(buf)).GetAwaiter().GetResult();
            return true;
        }

        public void PluginMouse(int button, int wheel)
        {
            var buf = new byte[1];
            buf[0] = (byte)PluginCuoProtocol.OnMouse;
            var response = Request(new ArraySegment<byte>(buf)).GetAwaiter().GetResult();
        }

        public void PluginDrawCmdList()
        {
            var buf = new byte[1];
            buf[0] = (byte)PluginCuoProtocol.OnCmdList;
            var response = Request(new ArraySegment<byte>(buf)).GetAwaiter().GetResult();
        }

        public unsafe void PluginSdlEvent(SDL2.SDL.SDL_Event* ev)
        {
            //var buf = new byte[1];
            //buf[0] = (byte)PluginCuoProtocol.OnSdlEvent;
            //var response = Request(new ArraySegment<byte>(buf)).GetAwaiter().GetResult();
        }

        public void PluginUpdatePlayerPosition(int x, int y, int z)
        {
            var buf = new byte[1];
            buf[0] = (byte)PluginCuoProtocol.OnUpdatePlayerPos;
            var response = Request(new ArraySegment<byte>(buf)).GetAwaiter().GetResult();
        }

        public void PluginPacketIn(ArraySegment<byte> buffer)
        {
            if (buffer.Array != null && buffer.Count> 0)
            {
                var buf = new byte[sizeof(byte) + buffer.Count];
                buf[0] = (byte)PluginCuoProtocol.OnPacketIn;
                //BinaryPrimitives.WriteUInt16LittleEndian(buffer.Array.AsSpan(1, 2), (ushort)buffer.Count);

                Array.Copy(buffer.Array, buffer.Offset, buf, 1, buffer.Count);

                var resp = Request(buf).GetAwaiter().GetResult();

                Array.Copy(resp.Payload.Array, resp.Payload.Offset + 1, buffer.Array, buffer.Offset, resp.Payload.Count - 1);
            }
>>>>>>> rpc support
        }

        public void PluginPacketOut(Span<byte> buffer)
        {
            if (buffer.Length > 0)
            {
                var buf = new byte[sizeof(byte) + buffer.Length];
                buf[0] = (byte)PluginCuoProtocol.OnPacketOut;
                //BinaryPrimitives.WriteUInt16LittleEndian(buffer.Array.AsSpan(1, 2), (ushort)buffer.Count);

                buffer.CopyTo(buf.AsSpan(1));

                var resp = Request(buf).GetAwaiter().GetResult();
                if (resp.Payload.Count > 0)
                    resp.Payload.AsSpan(resp.Payload.Offset + 1, resp.Payload.Count - 1).CopyTo(buffer);
            }
        }
    }
}
