namespace CSVUtility
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UnityEngine;
	using ExtensionMethods;

	public static class CSVWriter
	{
		private static readonly string invalidPathError = "Invalid path: ";
		private static readonly string invalidExtensionError = "The file has an invalid extension: ";
		private static readonly string fileNotFoundError = "No file found at path: ";

		public static void SaveData (string filePath, params ICollection<string>[] data)
		{
			if (filePath.IsNullOrEmpty ())
			{
				string message = string.Concat (invalidPathError, filePath);
				Debug.LogError (message);

				return;
			}

			if (!filePath.EndsWith (".csv"))
			{
				string message = string.Concat (invalidExtensionError, filePath);
				Debug.LogError (message);

				return;
			}

			if (!File.Exists (filePath))
			{
				string message = string.Concat (fileNotFoundError, filePath);
				Debug.LogError (message);

				return;
			}

			if (data.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data was provided.");
				return;
			}

			StringBuilder sb = new (GetFullDocument (filePath));

			sb = GetProcessedData (sb, data);

			SaveToFile (filePath, sb.ToString ());
		}

		public static bool RemoveData (string filePath, params IList<string>[] dataToRemove)
		{
			if (dataToRemove.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data was provided.");
				return false;
			}

			StringBuilder remove = GetProcessedData (new (), dataToRemove);

			return RemoveData (filePath, remove.ToString ());
		}

		public static bool RemoveData (string filePath, string dataToRemove)
		{
			if (filePath.IsNullOrEmpty ())
			{
				string message = string.Concat (invalidPathError, filePath);
				Debug.LogError (message);

				return false;
			}

			if (!filePath.EndsWith (".csv"))
			{
				string message = string.Concat (invalidExtensionError, filePath);
				Debug.LogError (message);

				return false;
			}

			if (!File.Exists (filePath))
			{
				string message = string.Concat (fileNotFoundError, filePath);
				Debug.LogError (message);

				return false;
			}
			
			if (dataToRemove.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data was provided.");
				return false;
			}
		
			string fullDocument = GetFullDocument (filePath);
			StringBuilder sb = new (fullDocument);

			sb = sb.Replace (dataToRemove, "");
			sb = sb.Replace ("\n\n", "\n");

			SaveToFile (filePath, sb.ToString ());

			return true;
		}

		public static void ReplaceData (string filePath, IList<string> dataToReplace, params IList<string>[] data)
		{
			if (dataToReplace.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data to replace was provided.");
				return;
			}

			StringBuilder replacedData = GetProcessedData (new (), dataToReplace);
			StringBuilder newData = GetProcessedData (new (), data);

			ReplaceData (filePath, replacedData.ToString (), newData.ToString ());
		}

		public static void ReplaceData (string filePath, IList<string>[] dataToReplace, params IList<string>[] data)
		{
			if (dataToReplace.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data to replace was provided.");
				return;
			}

			StringBuilder replacedData = GetProcessedData (new (), dataToReplace);
			StringBuilder newData = GetProcessedData (new (), data);

			ReplaceData (filePath, replacedData.ToString (), newData.ToString ());
		}
		
		public static void ReplaceData (string filePath, string dataToReplace, params IList<string>[] data)
		{
			if (dataToReplace.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data to replace was provided.");
				return;
			}

			StringBuilder newData = GetProcessedData (new (), data);

			ReplaceData (filePath, dataToReplace, newData.ToString ());
		}

		public static void ReplaceData (string filePath, IList<string> dataToReplace, string newData)
		{
			if (dataToReplace.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data to replace was provided.");
				return;
			}

			StringBuilder replacedData = GetProcessedData (new (), dataToReplace);

			ReplaceData (filePath, replacedData.ToString (), newData);
		}

		public static void ReplaceData (string filePath, IList<string>[] dataToReplace, string newData)
		{
			if (dataToReplace.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data to replace was provided.");
				return;
			}

			StringBuilder replacedData = GetProcessedData (new (), dataToReplace);

			ReplaceData (filePath, replacedData.ToString (), newData);
		}
		
		public static void ReplaceData (string filePath, string dataToReplace, string newData)
		{
			if (filePath.IsNullOrEmpty ())
			{
				string message = string.Concat (invalidPathError, filePath);
				Debug.LogError (message);

				return;
			}

			if (!filePath.EndsWith (".csv"))
			{
				string message = string.Concat (invalidExtensionError, filePath);
				Debug.LogError (message);

				return;
			}

			if (!File.Exists (filePath))
			{
				string message = string.Concat (fileNotFoundError, filePath);
				Debug.LogError (message);

				return;
			}
			
			if (dataToReplace.IsNullOrEmpty ())
			{
				Debug.LogWarning ("The document was not modified because no data to replace was provided.");
				return;
			}
		
			string fullDocument = GetFullDocument (filePath);
			StringBuilder sb = new (fullDocument);

			sb = sb.Replace (dataToReplace, newData);

			SaveToFile (filePath, sb.ToString ());
		}

		public static string GetFullDocument (string filePath)
		{
			if (filePath.IsNullOrEmpty ())
			{
				string message = string.Concat (invalidPathError, filePath);
				Debug.LogError (message);

				return "";
			}

			if (!filePath.EndsWith (".csv"))
			{
				string message = string.Concat (invalidExtensionError, filePath);
				Debug.LogError (message);

				return "";
			}

			if (!File.Exists (filePath))
			{
				string message = string.Concat (fileNotFoundError, filePath);
				Debug.LogError (message);

				return "";
			}

			filePath = CheckFileExtension (filePath);

			if (!File.Exists (filePath))
				throw new Exception ("The file doesn't exist.");
		
			using StreamReader sr = new (filePath);
			string doc = sr.ReadToEnd ();
			sr.Dispose ();

			if (doc != null)
				return doc;

			return "";
		}

		public static bool FileContains (string filePath, IList<string> data)
		{
			if (data.IsNullOrEmpty ())
				return false;
		
			StringBuilder sb = GetProcessedData (new (), data);

			return FileContains (filePath, sb.ToString ());
		}

		public static bool FileContains (string filePath, IList<string>[] data)
		{
			if (data.IsNullOrEmpty ())
				return false;
		
			StringBuilder sb = GetProcessedData (new (), data);

			return FileContains (filePath, sb.ToString ());
		}

		public static bool FileContains (string filePath, string data)
		{
			if (filePath.IsNullOrEmpty ())
			{
				string message = string.Concat (invalidPathError, filePath);
				Debug.LogError (message);

				return false;
			}

			if (!filePath.EndsWith (".csv"))
			{
				string message = string.Concat (invalidExtensionError, filePath);
				Debug.LogError (message);

				return false;
			}

			if (!File.Exists (filePath))
			{
				string message = string.Concat (fileNotFoundError, filePath);
				Debug.LogError (message);

				return false;
			}
			
			if (data.IsNullOrEmpty ())
				return false;
		
			string document = GetFullDocument (filePath);

			return document.Contains (data);
		}

		public static bool FileExists (string filePath)
		{
			return File.Exists (filePath);
		}

		public static void CreateFile (string filePath)
		{
			CreateFile (filePath, "");
		}

		public static void CreateFile (string filePath, params ICollection<string>[] data)
		{
			StringBuilder sb = GetProcessedData (new (), data);
			SaveToFile (filePath, sb.ToString ());
		}

		public static void CreateFile (string filePath, string content)
		{
			if (filePath.IsNullOrEmpty ())
			{
				string message = string.Concat (invalidPathError, filePath);
				Debug.LogError (message);

				return;
			}

			if (!filePath.EndsWith (".csv"))
			{
				string message = string.Concat (invalidExtensionError, filePath);
				Debug.LogError (message);

				return;
			}

			SaveToFile (filePath, content);
		}

		public static void OverwriteFile (string filePath, params ICollection<string>[] data)
		{
			if (data.IsNullOrEmpty ())
			{
				SaveToFile (filePath, "");
				return;
			}

			StringBuilder sb = GetProcessedData (new (), data);

			OverwriteFile (filePath, sb.ToString ());
		}

		public static void OverwriteFile (string filePath, string content)
		{
			if (filePath.IsNullOrEmpty ())
			{
				string message = string.Concat (invalidPathError, filePath);
				Debug.LogError (message);

				return;
			}

			if (!filePath.EndsWith (".csv"))
			{
				string message = string.Concat (invalidExtensionError, filePath);
				Debug.LogError (message);

				return;
			}

			if (!File.Exists (filePath))
			{
				string message = string.Concat (fileNotFoundError, filePath);
				Debug.LogError (message);

				return;
			}

			if (content.IsNullOrEmpty ())
			{
				SaveToFile (filePath, "");
				return;
			}

			SaveToFile (filePath, content);
		}

		private static StringBuilder GetProcessedData (StringBuilder sb, params ICollection<string>[] data)
		{
			if (data.IsNullOrEmpty ())
				return sb;

			for (int i = 0; i < data.Length; i++)
			{
				if (data[i].IsNullOrEmpty ())
					continue;
			
				bool newLine = true;

				if (i == 0)
					newLine = sb.Length > 0;

				sb = GetLine (sb, newLine, data[i]);
			}

			return sb;
		}

		private static StringBuilder GetLine (StringBuilder sb, bool newLine, ICollection<string> data)
		{
			if (newLine)
				sb.Append ('\n');

			for (int i = 0; i < data.Count; i++)
			{
				sb.Append (data.ElementAt (i));

				if (i >= data.Count - 1)
					continue;

				sb.Append (',');
			}

			return sb;
		}

		private static void SaveToFile (string filePath, string content)
		{
			string folderPath = filePath[..filePath.LastIndexOf ('/')];

			if (!Directory.Exists (folderPath))
				Directory.CreateDirectory (folderPath);

			using StreamWriter writer = new (new FileStream (CheckFileExtension (filePath), FileMode.Create, FileAccess.Write));

			writer.Write (content);
			writer.Dispose ();
		}

		private static string CheckFileExtension (string filePath)
		{
			string extension = ".csv";

			if (filePath.LastIndexOf (extension) < filePath.Length - extension.Length)
				filePath = string.Concat (filePath, extension);

			return filePath;
		}
	}
}