using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Melancia.Taminha {
	public class TextFile {
		public List<string[]> lines { get; private set; }

		public TextFile(string path) {
			var asset = Resources.Load<TextAsset>(path);
			lines = new List<string[]>();
			if (asset == null) return;
			using (var file = new StreamReader(new MemoryStream(asset.bytes),Encoding.UTF8)) {
				var line = new List<string>();
				string s;
				while ((s = file.ReadLine()) != null) {
					bool ignore = false;
					bool quote = false;
					string word = null;
					for (int a = 0; a < s.Length; a++) {
						if (ignore) {
							ignore = false;
							var c = s[a];
							if (c == 'n') c = '\n';
							if (word == null) {
								word = c.ToString();
							} else {
								word += c;
							}
							continue;
						}
						if (s[a] == '\\') {
							ignore = true;
							continue;
						}
						if (quote) {
							if (s[a] == '"') {
								quote = false;
								line.Add(word);
								word = null;
							} else if (word == null) {
								word = s[a].ToString();
							} else {
								word += s[a];
							}
							continue;
						}
						if (s[a] == '"') {
							if (word != null) line.Add(word);
							word = string.Empty;
							quote = true;
							continue;
						}
						if (s[a] == ';') break;
						if (char.IsWhiteSpace(s,a)) {
							if (word != null) {
								line.Add(word);
								word = null;
							}
							continue;
						}
						if (word == null) {
							word = s[a].ToString();
						} else {
							word += s[a];
						}
					}
					if (word != null) line.Add(word);
					if (line.Count > 0) {
						lines.Add(line.ToArray());
						line.Clear();
					}
				}
			}
		}
	}
}