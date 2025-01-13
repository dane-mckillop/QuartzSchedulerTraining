using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

// Job class that performs the API fetch
public class ApiFetchJob : IJob
{
	private readonly IDataRepository _repository;
	private readonly IApiService _apiService;

	public ApiFetchJob(IDataRepository repository, IApiService apiService)
	{
		_repository = repository;
		_apiService = apiService;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
			TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney"));

		// Get last insert date from database
		var lastInsertDate = await _repository.GetLastInsertDate();
		var today = DateTime.Today;

		// Check if data exists for today
		var hasDataForToday = await _repository.HasDataForDate(today);

		// If it's after 6 PM AEST and no data for today
		if (now.Hour >= 18 && !hasDataForToday)
		{
			await FetchAndStoreData(lastInsertDate, today);
			return;
		}

		// If it's before 6 PM, check for missing data from previous days
		if (now.Hour < 18 && lastInsertDate < today.AddDays(-1))
		{
			await FetchAndStoreData(lastInsertDate, today.AddDays(-1));
		}
	}

	private async Task FetchAndStoreData(DateTime startDate, DateTime endDate)
	{
		var data = await _apiService.FetchData(startDate, endDate);
		await _repository.StoreData(data);
	}
}