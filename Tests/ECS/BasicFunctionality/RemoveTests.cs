﻿using Macropus.ECS.System;
using Tests.ECS.BasicFunctionality.Components;
using Tests.ECS.BasicFunctionality.Systems;
using Tests.Utils.ECS;
using Xunit.Abstractions;

namespace Tests.ECS.BasicFunctionality;

public class RemoveTests : TestsWithSystems
{
	public RemoveTests(ITestOutputHelper output) : base(output) { }

	public override ASystem[] GetSystems()
	{
		return new ASystem[] { new RemoveTestComponentSystem() };
	}

	[Fact]
	public void RemoveExistsComponentFromEntityTest()
	{
		var entityId = Guid.NewGuid();
		NewComponents.ReplaceComponent(entityId, new TestComponent());

		Assert.True(NewComponents.HasComponent<TestComponent>(entityId));

		ExecuteSystems();

		Assert.False(ChangesComponents.HasComponent<TestComponent>(entityId));
		Assert.True(NewComponents.HasComponent<TestComponent>(entityId));
	}

	[Fact]
	public void RemoveNotExistsComponentFromEntityTest()
	{
		var entityId = Guid.NewGuid();

		Assert.False(NewComponents.HasComponent<TestComponent>(entityId));

		ExecuteSystems();

		Assert.False(ChangesComponents.HasComponent<TestComponent>(entityId));
		Assert.False(NewComponents.HasComponent<TestComponent>(entityId));
	}
}