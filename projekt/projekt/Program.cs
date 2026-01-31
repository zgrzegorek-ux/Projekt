#nullable disable // Wyłącza żółte ostrzeżenia o wartościach null
using System;

namespace SymulatorAutomatu
{

	class Program
	{
		static void Main(string[] args)
		{
			Automat automat = new Automat();
			automat.Uruchom();
		}
	}
}