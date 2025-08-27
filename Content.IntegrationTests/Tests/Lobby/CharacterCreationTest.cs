// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 Redrover1760
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Lobby;
using Content.Server.Preferences.Managers;
using Content.Shared.Preferences;
using Robust.Client.State;
using Robust.Shared.Network;

namespace Content.IntegrationTests.Tests.Lobby
{
    [TestFixture]
    [TestOf(typeof(ClientPreferencesManager))]
    [TestOf(typeof(ServerPreferencesManager))]
    public sealed class CharacterCreationTest
    {
        [Test]
        public async Task CreateDeleteCreateTest()
        {
            await using var pair = await PoolManager.GetServerClient(new PoolSettings { InLobby = true });
            var server = pair.Server;
            var client = pair.Client;

            var clientNetManager = client.ResolveDependency<IClientNetManager>();
            var clientStateManager = client.ResolveDependency<IStateManager>();
            var clientPrefManager = client.ResolveDependency<IClientPreferencesManager>();

            var serverPrefManager = server.ResolveDependency<IServerPreferencesManager>();


            // Need to run them in sync to receive the messages.
            await pair.RunTicksSync(1);

            await PoolManager.WaitUntil(client, () => clientStateManager.CurrentState is LobbyState, 600);

            Assert.That(clientNetManager.ServerChannel, Is.Not.Null);

            var clientNetId = clientNetManager.ServerChannel.UserId;
            HumanoidCharacterProfile profile = null;

            await client.WaitAssertion(() =>
            {
                clientPrefManager.SelectCharacter(0);

                var clientCharacters = clientPrefManager.Preferences?.Characters;
                Assert.That(clientCharacters, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(clientCharacters, Has.Count.EqualTo(1));

                    Assert.That(clientStateManager.CurrentState, Is.TypeOf<LobbyState>());
                });

                profile = HumanoidCharacterProfile.Random();
                clientPrefManager.CreateCharacter(profile);

                clientCharacters = clientPrefManager.Preferences?.Characters;

                Assert.That(clientCharacters, Is.Not.Null);
                Assert.That(clientCharacters, Has.Count.EqualTo(2));
                Assert.That(clientCharacters[1].MemberwiseEquals(profile));
            });

            await PoolManager.WaitUntil(server, () => serverPrefManager.GetPreferences(clientNetId).Characters.Count == 2, maxTicks: 60);

            await server.WaitAssertion(() =>
            {
                var serverCharacters = serverPrefManager.GetPreferences(clientNetId).Characters;

                Assert.That(serverCharacters, Has.Count.EqualTo(2));
                Assert.That(serverCharacters[1].MemberwiseEquals(profile));
            });

            await client.WaitAssertion(() =>
            {
                clientPrefManager.DeleteCharacter(1);

                var clientCharacters = clientPrefManager.Preferences?.Characters.Count;
                Assert.That(clientCharacters, Is.EqualTo(1));
            });

            await PoolManager.WaitUntil(server, () => serverPrefManager.GetPreferences(clientNetId).Characters.Count == 1, maxTicks: 60);

            await server.WaitAssertion(() =>
            {
                var serverCharacters = serverPrefManager.GetPreferences(clientNetId).Characters.Count;
                Assert.That(serverCharacters, Is.EqualTo(1));
            });

            await client.WaitIdleAsync();

            await client.WaitAssertion(() =>
            {
                profile = HumanoidCharacterProfile.Random();

                clientPrefManager.CreateCharacter(profile);

                var clientCharacters = clientPrefManager.Preferences?.Characters;

                Assert.That(clientCharacters, Is.Not.Null);
                Assert.That(clientCharacters, Has.Count.EqualTo(2));
                Assert.That(clientCharacters[1].MemberwiseEquals(profile));
            });

            await PoolManager.WaitUntil(server, () => serverPrefManager.GetPreferences(clientNetId).Characters.Count == 2, maxTicks: 120); //60->120 - Mono

            await server.WaitAssertion(() =>
            {
                var serverCharacters = serverPrefManager.GetPreferences(clientNetId).Characters;

                Assert.That(serverCharacters, Has.Count.EqualTo(2));
                Assert.That(serverCharacters[1].MemberwiseEquals(profile));
            });
            await pair.CleanReturnAsync();
        }
    }
}
