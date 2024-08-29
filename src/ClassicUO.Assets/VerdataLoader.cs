﻿#region license

// Copyright (c) 2024, andreakarasho
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. All advertising materials mentioning features or use of this software
//    must display the following acknowledgement:
//    This product includes software developed by andreakarasho - https://github.com/andreakarasho
// 4. Neither the name of the copyright holder nor the
//    names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS ''AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using ClassicUO.IO;
using ClassicUO.Utility.Logging;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClassicUO.Assets
{
    public sealed class VerdataLoader : UOFileLoader
    {
        public VerdataLoader(UOFileManager fileManager) : base(fileManager) { }

        public unsafe override Task Load()
        {
            return Task.Run(() =>
            {
                string path = FileManager.GetUOFilePath("verdata.mul");

                if (!System.IO.File.Exists(path))
                {
                    File = null;
                }
                else
                {
                    File = new UOFileMul(path);

                    // the scope of this try/catch is to avoid unexpected crashes if servers redestribuite wrong verdata
                    try
                    {
                        var len = File.ReadInt32();
                        Patches = new UOFileIndex5D[len];

                        for (var i = 0; i < len; i++)
                        {
                            Patches[i] = File.Read<UOFileIndex5D>();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"error while reading verdata.mul\n{ex}");
                    }
                }
            });
        }


        // FileIDs
        //0 - map0.mul
        //1 - staidx0.mul
        //2 - statics0.mul
        //3 - artidx.mul
        //4 - art.mul
        //5 - anim.idx
        //6 - anim.mul
        //7 - soundidx.mul
        //8 - sound.mul
        //9 - texidx.mul
        //10 - texmaps.mul
        //11 - gumpidx.mul
        //12 - gumps.mul
        //13 - multi.idx
        //14 - multi.mul
        //15 - skills.idx
        //16 - skills.mul
        //30 - tiledata.mul
        //31 - animdata.mul 

        public UOFileIndex5D[] Patches { get; private set; } = Array.Empty<UOFileIndex5D>();

        public UOFileMul File { get; private set; }
    }
}