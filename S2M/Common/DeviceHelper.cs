using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;

namespace S2M.Common
{
	public class DeviceHelper
	{
		public static string GetDeviceId()
		{
			if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
			{
				var token = HardwareIdentification.GetPackageSpecificToken(null);
				var hardwareId = token.Id;
				var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

				byte[] bytes = new byte[hardwareId.Length];
				dataReader.ReadBytes(bytes);

				return BitConverter.ToString(bytes).Replace("-", "");
			}

			throw new Exception("NO API FOR DEVICE ID PRESENT!");
		}
	}
}
