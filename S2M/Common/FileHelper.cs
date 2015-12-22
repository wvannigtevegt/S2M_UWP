using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace S2M.Common {
	public class FileHelper {
		/// <summary>
		/// saves the string 'content' to a file 'filename' in the app's local storage folder
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public static async Task SaveStringToLocalFile(string filename, string content) {
			byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(content.ToCharArray());

			StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

			using (var stream = await file.OpenStreamForWriteAsync()) {
				stream.Write(fileBytes, 0, fileBytes.Length);
			}
		}

		/// <summary>
		/// reads the contents of file 'filename' in the app's local storage folder and returns it as a string
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static async Task<string> ReadStringFromLocalFile(string filename) {
			StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
			Stream stream = await local.OpenStreamForReadAsync(filename);
			string text;

			using (StreamReader reader = new StreamReader(stream)) {
				text = reader.ReadToEnd();
			}

			return text;
		}

	}
}
