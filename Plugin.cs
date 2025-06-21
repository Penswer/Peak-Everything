using BepInEx;
using UnityEngine;
using BepInEx.Logging;
using System;
using DearImGuiInjection.BepInEx;
using ImGuiNET;
using System.Collections.Generic;
using System.Text;
using Photon.Pun;
using Zorro.Core.Serizalization;
using pworld.Scripts;

namespace Everything;

[BepInDependency(DearImGuiInjection.Metadata.GUID)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static EventComponent Component;

    internal static List<Item> items = new List<Item>();
    internal static List<Character> players = new List<Character>();
    internal static List<string> itemNames = new List<string>();
    internal static List<string> playerNames = new List<string>();
    internal static int itemsSelected = -1;
    internal static int playerSelected = -1;
    internal static byte inventorySlotNum = 0;

    internal static bool openedPlayerSelect = false;

    internal static byte[] inventorySearchBuffer = new byte[1000];

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        foreach (var option in Environment.GetCommandLineArgs())
        {
            Logger.LogInfo($"Launch Options {option}.!");
        }
        Component = this.gameObject.AddComponent<EventComponent>();
        DearImGuiInjection.DearImGuiInjection.Render += MyUI;
        // DearImGuiInjection.DearImGuiInjection.IO.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        // DearImGuiInjection.DearImGuiInjection.AllowPassthroughInput = true;
        // DearImGuiInjection.DearImGuiInjection.ToggleCursorActions(true);
        // Harmony.CreateAndPatchAll(typeof(BodyPartPatches.Gravity));
    }

    private static void MyUI()
    {
        // DearImGuiInjection.DearImGuiInjection.IsCursorVisible = false;
        // DearImGuiInjection.DearImGuiInjection.IO.ConfigFlags ^= ImGuiConfigFlags.NoMouse;
        // if ((DearImGuiInjection.DearImGuiInjection.IO.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) == ImGuiConfigFlags.ViewportsEnable)
        // {
        //     ImGui.UpdatePlatformWindows();
        //     ImGui.RenderPlatformWindowsDefault();
        // }
        DearImGuiInjection.DearImGuiInjection.ToggleCursorActions(true);
        DearImGuiInjection.DearImGuiInjection.AllowPassthroughInput = true;
        // ImGui.ShowDemoWindow();
        if (ImGui.Begin("Everything Menu", ConfigValues.everythingWindowFlags))
        {
            // if (ImGui.Button("Test"))
            // {
            //     var fields = typeof(Character).GetRuntimeFields();
            //     foreach (var field in fields)
            //     {
            //         Plugin.Logger.LogError(field.Name);
            //     }
            // }
            if (ImGui.Checkbox("Lock", ref ConfigValues.windowLocked))
            {
                if (ConfigValues.windowLocked)
                {
                    ConfigValues.everythingWindowFlags |= ImGuiWindowFlags.NoMove;
                }
                else
                {
                    ConfigValues.everythingWindowFlags ^= ImGuiWindowFlags.NoMove;
                }
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Locks the mod menu position.");
            }


            if (ImGui.Checkbox("Fly", ref ConfigValues.fly.value))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    var move = GameHelpers.GetMovementComponent();
                    if (move != null)
                    {
                        GameHelpers.GetRagdollComponent()?.ToggleCollision(!ConfigValues.fly.value);
                        // ragdoll.ToggleKinematic(ConfigValues.fly.value);
                        if (!ConfigValues.fly.value)
                        {
                            move.maxGravity = -500;
                        }
                        else
                        {
                            ConfigValues.noFallDamage.value = true;
                            ConfigValues.noFallDamage.constantUpdating = true;
                        }
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Lets you fly. Kinda buggy since I suck. Also isn't synced very well with other players.");
            }

            if (ImGui.SliderFloat("Fly Speed", ref ConfigValues.flySpeed.value, 10.0f, 200.0f))
            {
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Fly Speed...duh");
            }

            ImGui.Checkbox("Const##0", ref ConfigValues.noFallDamage.constantUpdating);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Makes this option constantly apply.");
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("No Fall Damage", ref ConfigValues.noFallDamage.value))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    if (GameHelpers.GetMovementComponent())
                    {
                        try
                        {
                            CharacterMovement move = GameHelpers.GetMovementComponent();
                            ConstantFields.GetFallDamageTime().SetValueDirect(__makeref(move), ConfigValues.noFallDamage.value ? 999.0f : 1.5f);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex);
                            Logger.LogError(ex.Message);
                            Logger.LogError(ex.StackTrace);
                        }
                    }
                });
            }

            ImGui.Checkbox("Const##-1", ref ConfigValues.statusLock.constantUpdating);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Makes this option constantly apply.");
            }

            ImGui.SameLine();
            if (ImGui.Checkbox("Status Lock", ref ConfigValues.statusLock.value))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    if (GameHelpers.GetCharacterComponent())
                    {
                        ConstantFields.GetStatusLockField()?.SetValue(GameHelpers.GetCharacterComponent(), ConfigValues.statusLock.value);
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Stops statuses from changing. (poison/sleep/etc)");
            }

            ImGui.Checkbox("Const##1", ref ConfigValues.infiniteStamina.constantUpdating);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Makes this option constantly apply.");
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Freeze Stamina", ref ConfigValues.infiniteStamina.value))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    if (GameHelpers.GetCharacterComponent() == null)
                    {
                        Logger.LogError("NOCOMOPOPDKDNJIDNWIQND ");
                    }
                    if (ConstantFields.GetInfiniteStaminaField() == null)
                    {
                        Logger.LogError("NO STAMMMMJDKNDJIKQJKDNQ ");
                    }
                    if (GameHelpers.GetCharacterComponent())
                    {
                        ConstantFields.GetInfiniteStaminaField()?.SetValue(GameHelpers.GetCharacterComponent(), ConfigValues.infiniteStamina.value);
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Freezes Stamina.");
            }
            ImGui.Checkbox("Const##2", ref ConfigValues.speed.constantUpdating);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Makes this option constantly apply.");
            }
            ImGui.SameLine();
            if (ImGui.SliderFloat("Speed", ref ConfigValues.speed.value, 1.0f, 20.0f))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    if (GameHelpers.GetMovementComponent())
                    {
                        GameHelpers.GetMovementComponent().movementModifier = ConfigValues.speed.value;
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Movement Speed Multiplier.");
            }
            ImGui.Checkbox("Const##3", ref ConfigValues.jump.constantUpdating);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Makes this option constantly apply.");
            }
            ImGui.SameLine();
            if (ImGui.SliderFloat("Jump", ref ConfigValues.jump.value, 1.0f, 500.0f))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    if (GameHelpers.GetMovementComponent())
                    {
                        GameHelpers.GetMovementComponent().jumpGravity = ConfigValues.jump.value;
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Jump Multiplier.");
            }

            if (ImGui.Button("Tele To 0,0,0"))
            {
                UnityMainThreadDispatcher.Enqueue(() => { Character.localCharacter?.photonView?.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { new Vector3(0.0f, 0.0f, 0.0f), true }); });
            }

            if (ImGui.Button("Refresh Inventory Items"))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    try
                    {
                        items.Clear();
                        itemNames.Clear();
                        itemsSelected = -1;
                        var locItems = Resources.FindObjectsOfTypeAll<Item>();
                        for (int i = 0; i < locItems.Length; i++)
                        {
                            // Logger.LogError($"AAA FOIUND ITEM: {locItems[i].GetName()} || Scene: {locItems[i].gameObject.scene.name}");
                            if (locItems[i] != null && locItems[i].gameObject.scene.handle == 0 && string.IsNullOrEmpty(locItems[i].gameObject.scene.name))
                            {
                                items.Add(locItems[i]);
                                itemNames.Add(locItems[i].GetName());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                        Logger.LogError(ex.Message);
                        Logger.LogError(ex.StackTrace);

                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Refreshes the list of spawnable items.");
            }

            if (ImGui.RadioButton("Slot 1", inventorySlotNum == 0))
            {
                inventorySlotNum = 0;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Slot 2", inventorySlotNum == 1))
            {
                inventorySlotNum = 1;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Slot 3", inventorySlotNum == 2))
            {
                inventorySlotNum = 2;
            }
            if (ImGui.BeginCombo("Items##InventorySelect", itemsSelected == -1 ? "None" : itemNames[itemsSelected]))
            {

                ImGui.InputText("##InventorySearch", inventorySearchBuffer, (uint)inventorySearchBuffer.Length);
                // if (ImGui.Selectable($"None##-1Item1InvEditor", itemsSelected == -1))
                // {
                //     itemsSelected = -1;
                //     Logger.LogError($"HERE: {itemNames.Count}");
                // }
                for (int i = 0; i < itemNames.Count; i++)
                {
                    // Logger.LogError("HERE");
                    int index = i;
                    if (itemNames[i].ToLower().Contains(Encoding.UTF8.GetString(inventorySearchBuffer).Split('\0')[0].ToLower()))
                    {
                        if (ImGui.Selectable($"{itemNames[i]}##{index}Item1InvEditor", itemsSelected == index))
                        {
                            itemsSelected = i;
                            UnityMainThreadDispatcher.Enqueue(() =>
                            {
                                if (Player.localPlayer != null && Player.localPlayer.itemSlots != null && Player.localPlayer.itemSlots.Length >= 3)
                                {
                                    Player.localPlayer.itemSlots[inventorySlotNum].prefab = items[itemsSelected];
                                    Player.localPlayer.itemSlots[inventorySlotNum].data = new ItemInstanceData(Guid.NewGuid());
                                    ItemInstanceDataHandler.AddInstanceData(Player.localPlayer.itemSlots[inventorySlotNum].data);
                                    byte[] array = IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(Player.localPlayer.itemSlots, Player.localPlayer.backpackSlot, Player.localPlayer.tempFullSlot));
                                    Player.localPlayer.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { array, true });
                                }
                            });
                        }
                        if (itemsSelected == i)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }
                }
                ImGui.EndCombo();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Clicking on an item will place it in the selected inventory slot.");
            }

            if (ImGui.Button("Give Items 999 Recharge"))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    if (Player.localPlayer != null && Player.localPlayer.itemSlots != null)
                    {
                        foreach (var slot in Player.localPlayer.itemSlots)
                        {
                            if (slot != null && slot.data != null && slot.data.data != null)
                            {
                                foreach (var data in slot.data.data)
                                {
                                    if (data.Key == DataEntryKey.PetterItemUses)
                                    {
                                        if (data.Value is IntItemData)
                                        {
                                            var val = data.Value as IntItemData;
                                            val.Value = 999;
                                        }
                                    }
                                    else if (data.Key == DataEntryKey.Fuel)
                                    {
                                        if (data.Value is FloatItemData)
                                        {
                                            var val = data.Value as FloatItemData;
                                            val.Value = 999.0f;
                                        }
                                    }
                                    else if (data.Key == DataEntryKey.UseRemainingPercentage)
                                    {
                                        if (data.Value is FloatItemData)
                                        {
                                            var val = data.Value as FloatItemData;
                                            val.Value = 999.0f;
                                        }
                                    }
                                    else if (data.Key == DataEntryKey.ItemUses)
                                    {
                                        if (data.Value is OptionableIntItemData)
                                        {
                                            var val = data.Value as OptionableIntItemData;
                                            val.Value = 999;
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Tries to set the recharge/ammo/filled amount of inventory items to 999");
            }
            if (ImGui.Button("Bypass OutOfDate"))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    GameObject.Find("/Modal(Clone)/Canvas")?.SetActive(false);
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Lets you play the game if you get an Out of Date screen.");
            }

            // if (ImGui.Button("Refresh Player List"))
            // {
            //     UnityMainThreadDispatcher.Enqueue(() =>
            //     {
            //         try
            //         {
            //             players.Clear();
            //             playerNames.Clear();
            //             playerSelected = -1;
            //             foreach (var character in Character.AllCharacters)
            //             {
            //                 players.Add(character);
            //                 playerNames.Add(character.characterName);
            //             }
            //         }
            //         catch (Exception ex)
            //         {
            //             Logger.LogError(ex);
            //             Logger.LogError(ex.Message);
            //             Logger.LogError(ex.StackTrace);
            //         }
            //     });
            // }
            // if (ImGui.IsItemHovered())
            // {
            //     ImGui.SetTooltip("Refreshes Player List.....");
            // }
            if (ImGui.BeginCombo("Player Select", playerSelected == -1 ? "None" : playerNames[playerSelected]))
            {
                if (!openedPlayerSelect)
                {
                    openedPlayerSelect = true;
                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        try
                        {
                            players.Clear();
                            playerNames.Clear();
                            playerSelected = -1;
                            foreach (var character in Character.AllCharacters)
                            {
                                players.Add(character);
                                playerNames.Add(character.characterName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex);
                            Logger.LogError(ex.Message);
                            Logger.LogError(ex.StackTrace);
                        }
                    });
                }
                for (int i = 0; i < playerNames.Count; i++)
                {
                    int index = i;
                    if (ImGui.Selectable($"{playerNames[i]}##{i}PlayerSelector", index == playerSelected))
                    {
                        playerSelected = index;
                    }
                    if (index == playerSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            else
            {
                openedPlayerSelect = false;
            }
            if (ImGui.Button("Revive"))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    try
                    {
                        if (playerSelected >= 0 && playerSelected < players.Count && players[playerSelected] != null)
                        {
                            // players[playerSelected].photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] { Character.localCharacter.Head + new Vector3(0.0f, 4.0f, 0.0f), false });
                            Vector3 pos;
                            if (players[playerSelected].Ghost != null)
                            {
                                pos = players[playerSelected].Ghost.gameObject.transform.position;
                            }
                            else
                            {
                                pos = players[playerSelected].Head;
                            }
                            players[playerSelected].photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] { pos + new Vector3(0.0f, 4.0f, 0.0f), false });
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                        Logger.LogError(ex.Message);
                        Logger.LogError(ex.StackTrace);
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Revives Selected Player Above You.");
            }
            ImGui.SameLine();
            if (ImGui.Button("Warp To"))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    try
                    {
                        if (playerSelected >= 0 && playerSelected < players.Count && players[playerSelected] != null)
                        {
                            Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { players[playerSelected].Head + new Vector3(0.0f, 4.0f, 0.0f), true });
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                        Logger.LogError(ex.Message);
                        Logger.LogError(ex.StackTrace);
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Warps you to the selected player");
            }
            ImGui.SameLine();
            if (ImGui.Button("Warp To Me"))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    try
                    {
                        if (playerSelected >= 0 && playerSelected < players.Count && players[playerSelected] != null)
                        {
                            players[playerSelected].photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { Character.localCharacter.Head + new Vector3(0.0f, 4.0f, 0.0f), true });
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                        Logger.LogError(ex.Message);
                        Logger.LogError(ex.StackTrace);
                    }
                });
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Warps selected player to you");

            }
            ImGui.End();
        }

    }
}
