using Astrum.AstralCore.Managers;
using KiraiMod.WingAPI;
using KiraiMod.WingAPI.RawUI;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

[assembly: MelonInfo(typeof(Astrum.AstralUI.Wings), "AstralUI.Wings", "0.1.0", downloadLink: "github.com/Astrum-Project/AstralUI.Wings")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum.AstralUI
{
    public class Wings : MelonMod
    {
        private static readonly Color32 onColor = new Color32(0x5a, 0xb2, 0xa8, 0xFF);
        private static readonly Color32 offColor = new Color32(0xe2, 0xad, 0x78, 0xFF);

        public override void OnApplicationStart()
        {
            WingAPI.OnWingInit += new Action<Wing.BaseWing>(wing =>
            {
                // lets listen for any future buttons
                ModuleManager.OnModuleRegistered += new Action<string, ModuleManager.Module>((mName, module) =>
                {
                    int idx = 0;
                    WingPage page = wing.CreatePage(mName);
                    module.OnCommandRegistered += new Action<string, CommandManager.Command>((cName, command) => Create(command, page, cName,  ref idx));
                });

                // now we need to create the existing buttons
                foreach (KeyValuePair<string, ModuleManager.Module> smp in ModuleManager.modules)
                {
                    int idx = 0;
                    WingPage page = wing.CreatePage(smp.Key);
                    foreach (KeyValuePair<string, CommandManager.Command> scp in smp.Value.commands)
                        Create(scp.Value, page, scp.Key, ref idx);
                }
            });
        }

        private static void Create(CommandManager.Command command, WingPage page, string name, ref int idx)
        {
            if (command is CommandManager.Button)
                page.CreateButton(name, idx++, () => command.onExecute(new string[0] { }));
            else if (command is CommandManager.ConVar<bool>)
            {
                CommandManager.ConVar<bool> boolvar = command as CommandManager.ConVar<bool>;
                page.CreateToggle(name, idx++, onColor, offColor, boolvar.Value, boolvar.onChange);
            }
        }
    }
}
