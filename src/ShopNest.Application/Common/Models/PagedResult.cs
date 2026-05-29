using System;
using System.Collections.Generic;

namespace ShopNest.Application.Common.Models;

public sealed class PagedResult<T>
{
	public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

	public int Page { get; init; }

	public int PageSize { get; init; }

	public int TotalCount { get; init; }

	public int TotalPages => (int)Math.Ceiling((double)TotalCount / (double)PageSize);

	public bool HasPreviousPage => Page > 1;

	public bool HasNextPage => Page < TotalPages;

	public static PagedResult<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
	{
		return new PagedResult<T>
		{
			Items = items,
			Page = page,
			PageSize = pageSize,
			TotalCount = totalCount
		};
	}

	public static PagedResult<T> Empty(int page, int pageSize)
	{
		return Create(Array.Empty<T>(), page, pageSize, 0);
	}
}
