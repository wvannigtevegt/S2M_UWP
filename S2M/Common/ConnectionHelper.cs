using System.Net.NetworkInformation;

namespace S2M.Common {
	class ConnectionHelper {
		public static bool CheckForInternetAccess() {
			// This is not the most foolproof way but at least covers basic scenarios
			// like Airplane mode
			if (NetworkInterface.GetIsNetworkAvailable()) {
				return true;
			}
			return false;
		}
	}
}
