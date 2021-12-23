using Astrum.AstralCore.UI;
using Astrum.AstralCore.UI.Attributes;
using KiraiMod.WingAPI;
using KiraiMod.WingAPI.RawUI;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

[assembly: MelonInfo(typeof(Astrum.AstralUI.Wings), "AstralUI.Wings", "0.2.0", downloadLink: "github.com/Astrum-Project/AstralUI.Wings")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum.AstralUI
{
    public class Wings : MelonMod
    {
        private static readonly Color32 onColor = new(0x5a, 0xb2, 0xa8, 0xFF);
        private static readonly Color32 offColor = new(0xe2, 0xad, 0x78, 0xFF);

        private static readonly Dictionary<string, (WingPage, int)> pages = new(StringComparer.OrdinalIgnoreCase);

        public override void OnApplicationStart()
        {
            WingAPI.OnWingInit += new Action<Wing.BaseWing>(wing =>
            {
                foreach (KeyValuePair<string, Module> mkvp in CoreUI.Modules)
                {
                    (WingPage page, int i) = GetPage(mkvp.Key, wing);

                    foreach (KeyValuePair<string, UIBase> ckvp in mkvp.Value.Commands)
                        CreateButton(page, ckvp.Key, ckvp.Value, ref i);

                    pages[mkvp.Key] = (page, i);
                }

                CoreUI.OnElementRegistered += (name, elem) =>
                {
                    (WingPage page, int i) = GetPage(name, wing);

                    CreateButton(page, name, elem, ref i);

                    pages[name] = (page, i);
                };
            });
        }

        public static (WingPage, int) GetPage(string name, Wing.BaseWing wing)
        {
            if (pages.TryGetValue(name, out (WingPage, int) kvp))
                return kvp;
            else return pages[name] = (wing.CreatePage(name), 0);
        }

        public static void CreateButton(WingPage page, string name, UIBase value, ref int i)
        {
            if (value is UIButton button)
                page.CreateButton(name, i++, () => button.Click());
            else if (value is UIFieldProp<bool> toggle)
                page.CreateToggle(name, i++, onColor, offColor, toggle.Value, state => toggle.Value = state);
        }
    }
}
