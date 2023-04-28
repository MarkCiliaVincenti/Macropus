﻿using ECS.Tests.Remove.Components;
using Macropus.ECS.Component.Filter;
using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;

namespace ECS.Tests.Remove.Systems;

public class RemoveReadOnlyComponentSystem : ASystem, IUpdateSystem
{
	private readonly IEntityContext context;

	public RemoveReadOnlyComponentSystem(IEntityContext context)
	{
		this.context = context;
	}

	private ComponentsFilter GetFilter()
	{
		return ComponentsFilter.AllOf(typeof(ReadOnlyComponent)).Build();
	}

	public void Update()
	{
		var filter = GetFilter();
		var entities = context.GetGroup(filter);

		Assert.NotEmpty(entities);

		foreach (var entity in entities)
			entity.RemoveComponent<ReadOnlyComponent>();
	}
}