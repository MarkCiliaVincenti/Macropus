﻿namespace ECS.Serialize;

public static class ComponentFormatUtils
{
	public static string? NormalizeName(string? componentName)
	{
		return componentName?.Replace('.', '|');
	}
}