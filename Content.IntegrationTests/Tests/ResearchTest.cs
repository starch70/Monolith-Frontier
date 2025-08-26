// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Redrover1760
// SPDX-FileCopyrightText: 2025 deltanedas
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Content.Shared.Lathe;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Research.TechnologyDisk.Components;
using Content.Shared.Research.Components;

namespace Content.IntegrationTests.Tests;

[TestFixture]
public sealed class ResearchTest
{
    [Test]
    public async Task DisciplineValidTierPrerequesitesTest()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;

        var protoManager = server.ResolveDependency<IPrototypeManager>();

        await server.WaitAssertion(() =>
        {
            var allTechs = protoManager.EnumeratePrototypes<TechnologyPrototype>().ToList();

            Assert.Multiple(() =>
            {
                foreach (var discipline in protoManager.EnumeratePrototypes<TechDisciplinePrototype>())
                {
                    foreach (var tech in allTechs)
                    {
                        if (tech.Discipline != discipline.ID)
                            continue;

                        // we ignore these, anyways
                        if (tech.Tier == 1)
                            continue;

                        Assert.That(tech.Tier, Is.GreaterThan(0), $"Technology {tech} has invalid tier {tech.Tier}.");
                        Assert.That(discipline.TierPrerequisites.ContainsKey(tech.Tier),
                            $"Discipline {discipline.ID} does not have a TierPrerequisites definition for tier {tech.Tier}");
                    }
                }
            });
        });

        await pair.CleanReturnAsync();
    }

    [Test]
    public async Task AllTechPrintableTest()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;

        var entMan = server.ResolveDependency<IEntityManager>();
        var protoManager = server.ResolveDependency<IPrototypeManager>();
        var compFact = server.ResolveDependency<IComponentFactory>();

        var latheSys = entMan.System<SharedLatheSystem>();

        await server.WaitAssertion(() =>
        {
            var allEnts = protoManager.EnumeratePrototypes<EntityPrototype>();
            var latheTechs = new HashSet<ProtoId<LatheRecipePrototype>>();
            foreach (var proto in allEnts)
            {
                if (proto.Abstract)
                    continue;

                if (pair.IsTestPrototype(proto))
                    continue;

                if (!proto.TryGetComponent<LatheComponent>(out var lathe, compFact))
                    continue;

                latheSys.AddRecipesFromPacks(latheTechs, lathe.DynamicPacks);

                if (proto.TryGetComponent<EmagLatheRecipesComponent>(out var emag, compFact))
                    latheSys.AddRecipesFromPacks(latheTechs, emag.EmagDynamicPacks);
            }

            Assert.Multiple(() =>
            {
                // check that every recipe a tech adds can be made on some lathe
                var unlockedTechs = new HashSet<ProtoId<LatheRecipePrototype>>();
                foreach (var tech in protoManager.EnumeratePrototypes<TechnologyPrototype>())
                {
                    unlockedTechs.UnionWith(tech.RecipeUnlocks);
                    foreach (var recipe in tech.RecipeUnlocks)
                    {
                        Assert.That(latheTechs, Does.Contain(recipe), $"Recipe '{recipe}' from tech '{tech.ID}' cannot be unlocked on any lathes.");
                    }
                }
                // Mono: Tech Disk Check.
                var techDiskRecipes = new HashSet<ProtoId<LatheRecipePrototype>>();
                foreach (var proto in allEnts)
                {
                    if (proto.Abstract)
                        continue;

                    if (pair.IsTestPrototype(proto))
                        continue;

                    if (proto.TryGetComponent<TechnologyDiskComponent>(out var techDisk, compFact) && techDisk.Recipes != null)
                        techDiskRecipes.UnionWith(techDisk.Recipes);

                    if (proto.TryGetComponent<BlueprintComponent>(out var blueprint, compFact) && blueprint.ProvidedRecipes != null)
                        techDiskRecipes.UnionWith(blueprint.ProvidedRecipes);

                }

                unlockedTechs.UnionWith(techDiskRecipes);
                // now check that every dynamic recipe a lathe lists can be unlocked
                // mono: or has a tech disk recipe or blueprint recipe
                foreach (var recipe in latheTechs)
                {
                    Assert.That(unlockedTechs, Does.Contain(recipe), $"Recipe '{recipe}' is dynamic on a lathe but cannot be unlocked by research, tech disk, or blueprint.");
                }
            });
        });

        await pair.CleanReturnAsync();
    }
}
