using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace S2M.Common {
	class GeoService {
		public static async Task<Geocoordinate> GetSinglePositionAsync(CancellationToken token) {
			Geolocator geolocator = new Geolocator();
			geolocator.DesiredAccuracy = PositionAccuracy.Default;

			Geoposition geoposition = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(1), timeout: TimeSpan.FromSeconds(30)).AsTask(token);

			return geoposition.Coordinate;
		}
	}
}
