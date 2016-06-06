﻿using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class CheckInsViewModel : NotificationBase
	{
		private ObservableCollection<CheckIn> _checkins = new ObservableCollection<CheckIn>();
		private ObservableCollection<LocationDay> _dates = new ObservableCollection<LocationDay>();
		private double _latitude;
		private double _longitude;
		private int _nrOfCheckins;
		private LocationDay _selectedDate = new LocationDay();

		public ObservableCollection<CheckIn> Checkins
		{
			get { return _checkins; }
			set { SetProperty(ref _checkins, value); }
		}

		public ObservableCollection<LocationDay> Dates
		{
			get { return _dates; }
			set { SetProperty(ref _dates, value); }
		}

		public double Latitude
		{
			get { return _latitude; }
			set { SetProperty(_latitude, value, () => _latitude = value); }
		}

		public double Longitude
		{
			get { return _longitude; }
			set { SetProperty(_longitude, value, () => _longitude = value); }
		}

		public int NrOfCheckins
		{
			get
			{
				return _nrOfCheckins;
			}
			set { SetProperty(_nrOfCheckins, value, () => _nrOfCheckins = value); }
		}

		public LocationDay SelectedDate
		{
			get { return _selectedDate; }
			set { SetProperty(_selectedDate, value, () => _selectedDate = value); }
		}

		public async Task LoadCheckInsAsync(string searchTerm = "")
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			Checkins.Clear();
			var newCheckIns = new ObservableCollection<CheckIn>();

			try
			{
				await CheckIn.GetCheckInsAsync(token, newCheckIns, SelectedDate.Date, 0, 0, searchTerm, Latitude, Longitude, 0, "", 0, 0, true);

				foreach (var newCheckIn in newCheckIns)
				{
					if (!CheckIfCheckInExistsInList(newCheckIn))
					{
						Checkins.Add(newCheckIn);
					}
				}
				NrOfCheckins = Checkins.Count();
			}
			catch (Exception ex) { }
			finally
			{
				_cts = null;
			}
		}

		private bool CheckIfCheckInExistsInList(CheckIn checkin)
		{
			var checkIns = Checkins.Where(c => c.Id == checkin.Id);
			if (checkIns.Any())
			{
				return true;
			}

			return false;
		}
	}
}