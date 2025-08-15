// SPDX-FileCopyrightText: 2018 PJB3005
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Hugal31
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Vince
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 derek
// SPDX-FileCopyrightText: 2020 moneyl
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 Michael Phillips
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2023 08A
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Miro Kavaliou
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 ShadowCommander
// SPDX-FileCopyrightText: 2023 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2024 Morb
// SPDX-FileCopyrightText: 2024 chromiumboy
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2024 wafehling
// SPDX-FileCopyrightText: 2025 Ark
// SPDX-FileCopyrightText: 2025 ErhardSteinhauer
// SPDX-FileCopyrightText: 2025 ScyronX
// SPDX-FileCopyrightText: 2025 Whatstone
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Input;
using Robust.Shared.Input;

namespace Content.Client.Input
{
    /// <summary>
    ///     Contains a helper function for setting up all content
    ///     contexts, and modifying existing engine ones.
    /// </summary>
    public static class ContentContexts
    {
        public static void SetupContexts(IInputContextContainer contexts)
        {
            var common = contexts.GetContext("common");
            common.AddFunction(ContentKeyFunctions.FocusChat);
            common.AddFunction(ContentKeyFunctions.FocusLocalChat);
            common.AddFunction(ContentKeyFunctions.FocusEmote);
            common.AddFunction(ContentKeyFunctions.FocusWhisperChat);
            common.AddFunction(ContentKeyFunctions.FocusRadio);
            common.AddFunction(ContentKeyFunctions.FocusLOOC);
            common.AddFunction(ContentKeyFunctions.FocusOOC);
            common.AddFunction(ContentKeyFunctions.FocusAdminChat);
            common.AddFunction(ContentKeyFunctions.FocusConsoleChat);
            common.AddFunction(ContentKeyFunctions.FocusDeadChat);
            common.AddFunction(ContentKeyFunctions.CycleChatChannelForward);
            common.AddFunction(ContentKeyFunctions.CycleChatChannelBackward);
            common.AddFunction(ContentKeyFunctions.EscapeContext);
            common.AddFunction(ContentKeyFunctions.ExamineEntity);
            common.AddFunction(ContentKeyFunctions.OpenAHelp);
            common.AddFunction(ContentKeyFunctions.TakeScreenshot);
            common.AddFunction(ContentKeyFunctions.TakeScreenshotNoUI);
            common.AddFunction(ContentKeyFunctions.ToggleFullscreen);
            common.AddFunction(ContentKeyFunctions.MoveStoredItem);
            common.AddFunction(ContentKeyFunctions.RotateStoredItem);
            common.AddFunction(ContentKeyFunctions.SaveItemLocation);
            common.AddFunction(ContentKeyFunctions.Point);
            common.AddFunction(ContentKeyFunctions.ToggleStanding); // WD EDIT
            common.AddFunction(ContentKeyFunctions.ZoomOut);
            common.AddFunction(ContentKeyFunctions.ZoomIn);
            common.AddFunction(ContentKeyFunctions.ResetZoom);
            common.AddFunction(ContentKeyFunctions.InspectEntity);
            common.AddFunction(ContentKeyFunctions.ToggleRoundEndSummaryWindow);

            // Not in engine, because engine cannot check for sanbox/admin status before starting placement.
            common.AddFunction(ContentKeyFunctions.EditorCopyObject);

            // Not in engine because the engine doesn't understand what a flipped object is
            common.AddFunction(ContentKeyFunctions.EditorFlipObject);

            // Not in engine so that the RCD can rotate objects
            common.AddFunction(EngineKeyFunctions.EditorRotateObject);

            var human = contexts.GetContext("human");
            human.AddFunction(EngineKeyFunctions.MoveUp);
            human.AddFunction(EngineKeyFunctions.MoveDown);
            human.AddFunction(EngineKeyFunctions.MoveLeft);
            human.AddFunction(EngineKeyFunctions.MoveRight);
            human.AddFunction(EngineKeyFunctions.Walk);
            human.AddFunction(ContentKeyFunctions.SwapHands);
            human.AddFunction(ContentKeyFunctions.SwapHandsPrevious); // Frontier
            human.AddFunction(ContentKeyFunctions.Drop);
            human.AddFunction(ContentKeyFunctions.UseItemInHand);
            human.AddFunction(ContentKeyFunctions.AltUseItemInHand);
            human.AddFunction(ContentKeyFunctions.OpenCharacterMenu);
            human.AddFunction(ContentKeyFunctions.OpenEmotesMenu);
            human.AddFunction(ContentKeyFunctions.OpenLanguageMenu); // Einstein Engines - Language
            human.AddFunction(ContentKeyFunctions.ActivateItemInWorld);
            human.AddFunction(ContentKeyFunctions.ThrowItemInHand);
            human.AddFunction(ContentKeyFunctions.AltActivateItemInWorld);
            human.AddFunction(ContentKeyFunctions.TryPullObject);
            human.AddFunction(ContentKeyFunctions.MovePulledObject);
            human.AddFunction(ContentKeyFunctions.ReleasePulledObject);
            human.AddFunction(ContentKeyFunctions.OpenCraftingMenu);
            human.AddFunction(ContentKeyFunctions.OpenInventoryMenu);
            human.AddFunction(ContentKeyFunctions.SmartEquipBackpack);
            human.AddFunction(ContentKeyFunctions.SmartEquipBelt);
            human.AddFunction(ContentKeyFunctions.SmartEquipWallet); // Frontier
            human.AddFunction(ContentKeyFunctions.SmartEquipBack); // Goobstation - Smart equip to back
            human.AddFunction(ContentKeyFunctions.OpenBackpack);
            human.AddFunction(ContentKeyFunctions.OpenBelt);
            human.AddFunction(ContentKeyFunctions.OpenWallet); // Frontier
            human.AddFunction(ContentKeyFunctions.MouseMiddle);
            human.AddFunction(ContentKeyFunctions.RotateObjectClockwise);
            human.AddFunction(ContentKeyFunctions.RotateObjectCounterclockwise);
            human.AddFunction(ContentKeyFunctions.FlipObject);
            human.AddFunction(ContentKeyFunctions.ArcadeUp);
            human.AddFunction(ContentKeyFunctions.ArcadeDown);
            human.AddFunction(ContentKeyFunctions.ArcadeLeft);
            human.AddFunction(ContentKeyFunctions.ArcadeRight);
            human.AddFunction(ContentKeyFunctions.Arcade1);
            human.AddFunction(ContentKeyFunctions.Arcade2);
            human.AddFunction(ContentKeyFunctions.Arcade3);
            // Shitmed Change Start - TODO: Add hands, feet and groin targeting.
            human.AddFunction(ContentKeyFunctions.TargetHead);
            human.AddFunction(ContentKeyFunctions.TargetTorso);
            human.AddFunction(ContentKeyFunctions.TargetLeftArm);
            human.AddFunction(ContentKeyFunctions.TargetLeftHand);
            human.AddFunction(ContentKeyFunctions.TargetRightArm);
            human.AddFunction(ContentKeyFunctions.TargetRightHand);
            human.AddFunction(ContentKeyFunctions.TargetLeftLeg);
            human.AddFunction(ContentKeyFunctions.TargetLeftFoot);
            human.AddFunction(ContentKeyFunctions.TargetRightLeg);
            human.AddFunction(ContentKeyFunctions.TargetRightFoot);
            // Shitmed Change End

            // actions should be common (for ghosts, mobs, etc)
            common.AddFunction(ContentKeyFunctions.OpenActionsMenu);

            foreach (var boundKey in ContentKeyFunctions.GetHotbarBoundKeys())
            {
                common.AddFunction(boundKey);
            }

            var aghost = contexts.New("aghost", "common");
            aghost.AddFunction(EngineKeyFunctions.MoveUp);
            aghost.AddFunction(EngineKeyFunctions.MoveDown);
            aghost.AddFunction(EngineKeyFunctions.MoveLeft);
            aghost.AddFunction(EngineKeyFunctions.MoveRight);
            aghost.AddFunction(EngineKeyFunctions.Walk);
            aghost.AddFunction(ContentKeyFunctions.SwapHands);
            aghost.AddFunction(ContentKeyFunctions.SwapHandsPrevious); // Frontier
            aghost.AddFunction(ContentKeyFunctions.Drop);
            aghost.AddFunction(ContentKeyFunctions.UseItemInHand);
            aghost.AddFunction(ContentKeyFunctions.AltUseItemInHand);
            aghost.AddFunction(ContentKeyFunctions.ActivateItemInWorld);
            aghost.AddFunction(ContentKeyFunctions.ThrowItemInHand);
            aghost.AddFunction(ContentKeyFunctions.AltActivateItemInWorld);
            aghost.AddFunction(ContentKeyFunctions.TryPullObject);
            aghost.AddFunction(ContentKeyFunctions.MovePulledObject);
            aghost.AddFunction(ContentKeyFunctions.ReleasePulledObject);

            var ghost = contexts.New("ghost", "human");
            ghost.AddFunction(EngineKeyFunctions.MoveUp);
            ghost.AddFunction(EngineKeyFunctions.MoveDown);
            ghost.AddFunction(EngineKeyFunctions.MoveLeft);
            ghost.AddFunction(EngineKeyFunctions.MoveRight);
            ghost.AddFunction(EngineKeyFunctions.Walk);

            common.AddFunction(ContentKeyFunctions.OpenEntitySpawnWindow);
            common.AddFunction(ContentKeyFunctions.OpenSandboxWindow);
            common.AddFunction(ContentKeyFunctions.OpenTileSpawnWindow);
            common.AddFunction(ContentKeyFunctions.OpenDecalSpawnWindow);
            common.AddFunction(ContentKeyFunctions.OpenAdminMenu);
            common.AddFunction(ContentKeyFunctions.OpenGuidebook);
        }
    }
}
