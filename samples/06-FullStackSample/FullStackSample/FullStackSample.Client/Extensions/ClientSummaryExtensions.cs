﻿using FullStackSample.Api.Models;
using FullStackSample.Client.Store.EntityStateEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullStackSample.Client.Extensions
{
	public static class ClientSummaryExtensions
	{
		public static ClientSummaryDto UpdateState(
			this ClientSummaryDto clientSummary,
			ClientStateNotification modifiedState)
		{
			if (clientSummary.Id != modifiedState.Id)
				return clientSummary;
			if (modifiedState.StateUpdateKind == StateUpdateKind.Created)
				return clientSummary;
			if (modifiedState.StateUpdateKind == StateUpdateKind.Deleted)
				return null;

			return new ClientSummaryDto(
				id: clientSummary.Id,
				name: modifiedState.Name.UpdatedValue(clientSummary.Name));
		}

		//TODO: PeteM - Make modifiedState an IEnumerable
		public static IEnumerable<ClientSummaryDto> UpdateState(
			this IEnumerable<ClientSummaryDto> source,
			ClientStateNotification modifiedState)
		{
			if (source == null)
				return null;

			switch (modifiedState.StateUpdateKind)
			{
				case StateUpdateKind.Created:
					source = source.Append(new ClientSummaryDto(
						id: modifiedState.Id,
						name: modifiedState.Name.GetValueOrDefault()));
					break;

				case StateUpdateKind.Deleted:
					source = source.Where(x => x.Id != modifiedState.Id);
					break;

				case StateUpdateKind.Modified:
					source = source.Select(x => x.UpdateState(modifiedState));
					break;

				default:
					throw new NotImplementedException(modifiedState.StateUpdateKind.ToString());
			}
			return source.ToList();
		}
	}
}
