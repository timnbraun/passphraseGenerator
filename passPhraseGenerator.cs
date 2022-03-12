/* 
 * passPhraseGenerator - Generate pronounceable passwords
   This program uses a random series of short dictionary words

   tnb  09/09/01 port to c#, command line
   tnb  16/07/08 convert to using dictionary
   tnb	18/02/23 add command line options for camel-case and l33t numbers
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace gpw {
	public class Gpw {
	  // Command line - console output.
		public static void Main(string[] args)
		{
			bool flipCase = false;
			bool l33tNumbers = true;
			// Console.WriteLine("passwordGenerator" + Environment.NewLine);

			foreach (string arg in args) {
				if (arg == "-c" || arg == "--camel") {
					flipCase = true;
					// Console.WriteLine("Going upper case");
				}
				else if (arg == "-n" || arg == "--nonumbers") {
					l33tNumbers = false;
				}
				// Console.WriteLine("arg = " + arg);
			}

			PasswordGenerator pwg = new PasswordGenerator(flipCase, l33tNumbers);
			string[] passwords = pwg.generate(6, 12).ToArray();

			foreach (string pw in passwords)
			{
				Console.WriteLine("  " + pw);
			}

			// Console.WriteLine();
		}
	} // Gpw
}


/// <summary>
/// Class to generate a list of reasonable randomish passwords
/// </summary>
class PasswordGenerator
{
	const string alphabet = "abcdefghijklmnopqrstuvwxyz";
	const string vowels = "aeiouy";
	List<string> words = null;
	Random ran = new Random(); // new random source seeded by clock
	bool		flipCase;
	bool		l33tNumbers;

	/// <summary>
	/// A constructor with a flag for case flipping
	/// </summary>
	public PasswordGenerator(bool bFlipCase = false, bool bL33tNumbers = true)
	{
		words = GpwData.Words();
		flipCase = bFlipCase;
		l33tNumbers = bL33tNumbers;
	}

	/// <summary>
	/// The useful utility in this class : generate some passwords
	/// </summary>
	/// <param name="nPasswordCount">How many passwords you want</param>
	/// <param name="nPasswordLength">How long each password should be</param>
	/// <returns></returns>
	static public string[] Generate(int nPasswordCount, int nPasswordLength)
	{
		PasswordGenerator pwg = new PasswordGenerator();

		return pwg.generate(nPasswordCount, nPasswordLength).ToArray();
	}

	string newWord {
		get {
			double pik;
			int ranno;
			string word;
			
			if (words.Count > 0) {
				do {
					pik = ran.NextDouble();
					ranno = (int )(pik * words.Count);
					word = words[ranno];
				} while ((word.Length < 2) || (word.Length > 6));

				if (flipCase) {
					word = Char.ToUpper(word[0]) + word.Substring(1);
				}
				return word.Replace( '\'', '-' );
			}
			else {
				pik = ran.NextDouble(); // random number [0,1]
				ranno = (int )(pik * alphabet.Length);
				return alphabet.Substring(ranno, 1);
			}
		}
	}

	public List<string> generate(int npw, int pwl)
	{
		int pwnum;
		StringBuilder password;
		List<string> passwords = new List<string>(npw);

		for (pwnum = 0; pwnum < npw; pwnum++)
		{
			password = new StringBuilder(pwl);
			// Pick a random starting point.

			password.Append(newWord);

			// Now do a random walk.
			while (password.Length < pwl) {
				password.Append(newWord);
			}

			if (flipCase) {
				// set two consonants to upper case
				for (int i = 0, capCount = 0; (i < password.Length - 1)
					&& (capCount < 2); i++)
				{
					if ((vowels.IndexOf(password[i]) != -1) &&
						(vowels.IndexOf(password[i + 1]) == -1)) {
						password[i+1] = char.ToUpper(password[i+1]);
						capCount++;
					}
				}
			}

			// get a digit somehow - e to 3, t to 7, a to 4, o to 0, i to 1, s to 5
			if (l33tNumbers && password.ToString().IndexOfAny(new char[] { 'e', 'i', 'o', 'a', 't', 's' }) != -1)
			{
				int i;
				if ((i = password.ToString().IndexOf('s')) != -1)
				{
					password[i] = '5';
				}
				else if ((i = password.ToString().IndexOf('t')) != -1)
				{
					password[i] = '7';
				}
				else if ((i = password.ToString().IndexOf('e')) != -1)
				{
					password[i] = '3';
				}
				else if ((i = password.ToString().IndexOf('a')) != -1)
				{
					password[i] = '4';
				}
				else if ((i = password.ToString().IndexOf('i')) != -1)
				{
					password[i] = '1';
				}
				else if ((i = password.ToString().IndexOf('o')) != -1)
				{
					password[i] = '0';
				}
			}

			passwords.Add( password.ToString() ); // Password generated
		} // for pwnum

		return passwords;
	} // generate()
}

// ================================================================

class GpwData {

  public GpwData () {
  } // constructor

  public static List<string> Words()
  {
	  Assembly assembly = Assembly.GetExecutingAssembly();
	  const string dict = "canadian-english";

	  List<string> words = new List<string>();

	  using (Stream res = assembly.GetManifestResourceStream(dict))
	  using (StreamReader r = new StreamReader(res)) {
		  string line;
		  while ((line = r.ReadLine()) != null) {
			  words.Add(line);
		  }
	  }

	  // Open dictionary file and read into a list of strings
	  return words;
  }

} // GpwData


// ================================================================
