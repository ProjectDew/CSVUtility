namespace CSVUtility
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using UnityEngine;
	using ExtensionMethods;

	public static class CSVReader
	{
		public static string[][] Read (string filePath)
		{
			if (filePath.IsNullOrEmpty ())
			{
				string message = string.Concat ("Invalid path: ", filePath);
				Debug.LogError (message);

				return new string[0][];
			}
			
			if (!filePath.EndsWith (".csv"))
			{
				string message = string.Concat ("The file has an invalid extension: ", filePath);
				Debug.LogError (message);

				return new string[0][];
			}

			if (!File.Exists (filePath))
			{
				string message = string.Concat ("No file found at path: ", filePath);
				Debug.LogError (message);

				return new string[0][];
			}

			string content = File.ReadAllText (filePath);
			return GetContentAsMatrix (content);
		}

		public static string[][] Read (TextAsset doc)
		{
			if (doc == null)
				throw new ArgumentNullException ("The document provided is null.");

			string content = doc.text;
		
			return GetContentAsMatrix (content);
		}

		private static string[][] GetContentAsMatrix (string content)
		{
			if (content == null || content.Length <= 0)
				return default;

			string[] dataSet = SplitText (content, out int totalRows, out int totalColumns).ToArray ();

			string[][] contentAsArray = new string[totalRows][];

			for (int i = 0; i < totalRows; i++)
			{
				contentAsArray[i] = new string[totalColumns];

				for (int j = 0; j < totalColumns; j++)
				{
					int currentCell = i * totalColumns + j;
					contentAsArray[i][j] = dataSet[currentCell];
				}
			}

			return contentAsArray;
		}

		private static List<string> SplitText (string text, out int totalRows, out int totalColumns)
		{
			List<string> splittedColumns;

			if (text.Contains (','))
			{
				splittedColumns = SplitColumns (text);
			}
			else
			{
				splittedColumns = new ();
				splittedColumns.Add (text);
			}
				
			int[] indicesOfLastColumns = GetIndicesOfLastColumns (splittedColumns);

			if (indicesOfLastColumns.Length == 0)
			{
				Debug.LogError ("The document doesn't seem to be structured as a CSV.");
				
				totalColumns = 0;
				totalRows = 0;

				return new ();
			}
				
			totalColumns = indicesOfLastColumns[0] + 1;

			if (text.Contains ('\n'))
			{
				splittedColumns = SplitRows (splittedColumns, indicesOfLastColumns);
				totalRows = splittedColumns.Count / totalColumns;
			}
			else
				totalRows = 1;

			return splittedColumns;
		}

		private static List<string> SplitColumns (string text)
		{
			List<string> splittedColumns = new ();

			int nextCell;
			int splitIndex = 0, nextIndex = 0;
			int start, length;

			while (splitIndex >= 0)
			{
				start = nextIndex;
			
				nextCell = text.IndexOf (',', nextIndex);
				splitIndex = nextCell;
				nextIndex = splitIndex + 1;
			
				while (nextIndex < text.Length && text[nextIndex] == ' ' && splitIndex >= 0)
				{
					nextCell = text.IndexOf (',', nextIndex);
					splitIndex = nextCell;
					nextIndex = splitIndex + 1;
				}

				length = (splitIndex >= 0) ? splitIndex - start : text.Length - start;

				if (splitIndex < text.Length)
					splittedColumns.Add (text.Substring (start, length));
				else
					if (text[splitIndex] == ',')
						splittedColumns.Add ("");
			}

			return splittedColumns;
		}

		private static List<string> SplitRows (List<string> splittedColumns, int[] indicesOfLastColumns)
		{
			string[] splittedCell = new string[2];

			int breakIndex, currentColumn;
			int secondToLast = indicesOfLastColumns.Length - 2;

			for (int i = secondToLast; i >= 0; i--)
			{
				currentColumn = indicesOfLastColumns[i];
				breakIndex = splittedColumns[currentColumn].LastIndexOf ('\n');

				splittedCell[0] = splittedColumns[currentColumn].Substring (0, breakIndex);
				splittedCell[1] = splittedColumns[currentColumn][(breakIndex + 1)..];

				splittedColumns.RemoveAt (currentColumn);

				for (int j = splittedCell.Length - 1; j >= 0; j--)
					splittedColumns.Insert (currentColumn, splittedCell[j]);
			}

			return TrimText (splittedColumns);
		}

		private static int[] GetIndicesOfLastColumns (List<string> splittedColumns)
		{
			List<int> lastColumns = new ();
			int index;

			for (int i = 0; i < splittedColumns.Count; i++)
			{
				index = i;

				if (index == 0)
					continue;

				lastColumns.Clear ();

				while (index < splittedColumns.Count && splittedColumns[index].Contains ('\n'))
				{
					index += i;
					lastColumns.Add (index - i);
				}

				if (index + 1 == splittedColumns.Count)
				{
					lastColumns.Add (index);
					break;
				}
			}

			return lastColumns.ToArray ();
		}

		private static List<string> TrimText (List<string> splittedText)
		{
			char quot = '\"';

			for (int i = 0; i < splittedText.Count; i++)
			{
				splittedText[i] = splittedText[i].Trim ();

				if (splittedText[i].Contains (quot))
					splittedText[i] = splittedText[i].Trim (quot);
			}

			return splittedText;
		}
	}
}
