﻿#region license
//  Copyright (C) 2018 ClassicUO Development Community on Github
//
//	This project is an alternative client for the game Ultima Online.
//	The goal of this is to develop a lightweight client considering 
//	new technologies.  
//      
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion
using ClassicUO.Game.Data;
using ClassicUO.Game.Gumps.Controls;
using ClassicUO.Input;
using ClassicUO.IO.Resources;

namespace ClassicUO.Game.Gumps.UIGumps
{
    internal class PopupMenuGump : Gump
    {
        private readonly PopupMenuData _data;

        public PopupMenuGump(PopupMenuData data) : base(0, 0)
        {
            _data = data;
            CloseIfClickOutside = true;
            //ControlInfo.IsModal = true;
            //ControlInfo.ModalClickOutsideAreaClosesThisControl = true;
            CanMove = false;

            ResizePic pic = new ResizePic(0x0A3C)
            {
                IsTransparent = true, Alpha = 0.25f
            };
            AddChildren(pic);
            int offsetY = 10;
            bool arrowAdded = false;
            int width = 0, height = 20;

            foreach (PopupMenuItem item in data.Items)
            {
                string text = Cliloc.GetString(item.Cliloc);

                Label label = new Label(text, true, item.Hue, font: 1)
                {
                    X = 10, Y = offsetY
                };

                HitBox box = new HitBox(10, offsetY, label.Width, label.Height)
                {
                    IsTransparent = true, Tag = item.Index
                };

                box.MouseClick += (sender, e) =>
                {
                    if (e.Button == MouseButton.Left)
                    {
                        HitBox l = (HitBox) sender;
                        GameActions.ResponsePopupMenu(_data.Serial, (ushort) l.Tag);
                        Dispose();
                    }
                };
                AddChildren(box);
                AddChildren(label);

                if ((item.Flags & 0x02) != 0 && !arrowAdded)
                {
                    arrowAdded = true;

                    // TODO: wat?
                    AddChildren(new Button(0, 0x15E6, 0x15E2, 0x15E2)
                    {
                        X = 20, Y = offsetY
                    });
                    height += 20;
                }

                offsetY += label.Height;

                if (!arrowAdded)
                {
                    height += label.Height;

                    if (width < label.Width)
                        width = label.Width;
                }
            }

            width += 20;

            if (height <= 10 || width <= 20)
                Dispose();
            else
            {
                pic.Width = width;
                pic.Height = height;
                foreach (HitBox box in FindControls<HitBox>()) box.Width = width - 20;
            }
        }
    }
}