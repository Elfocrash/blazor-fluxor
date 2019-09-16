﻿using Blazor.Fluxor;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
	public class FetchAirportsEffect : Effect<FetchAirportsAction>
	{
		private readonly HttpClient HttpClient;

		public FetchAirportsEffect(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		protected async override Task HandleAsync(FetchAirportsAction action, IDispatcher dispatcher)
		{
			Airport[] airports = Array.Empty<Airport>();
			try
			{
				airports = await HttpClient.GetJsonAsync<Airport[]>("api/airports");
			}
			catch
			{
				// Should really dispatch an error action
			}
			var completeAction = new FetchAirportsCompleteAction(airports);
			dispatcher.Dispatch(completeAction);
		}

		[Effect]
		public async Task HandleSearchActionAsync(SearchAction action, IDispatcher dispatcher)
		{
			try
			{
				Itinerary[] searchResults = await HttpClient.PostJsonAsync<Itinerary[]>("api/flightsearch", action.SearchCriteria);
				dispatcher.Dispatch(new SearchCompleteAction(searchResults));
			}
			catch
			{
				// Should really dispatch an error action
				dispatcher.Dispatch(new SearchCompleteAction(null));
			}
		}
	}
}
