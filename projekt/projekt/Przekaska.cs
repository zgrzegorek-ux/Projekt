#nullable disable // Wyłącza żółte ostrzeżenia o wartościach null
namespace SymulatorAutomatu
{
	// dziedziczy po klasie bazowej Produkt
	public class Przekaska : Produkt
	{
		public bool CzySlona { get; set; }

		public Przekaska(string nazwa, decimal cena, int ilosc, bool czySlona)
			: base(nazwa, cena, ilosc)
		{
			CzySlona = czySlona;
		}

		// Przekąski nie mają kaucji (0 PLN)
		public override decimal ObliczKaucje()
		{
			return 0.00m;
		}

		public override string OpisSzczegolowy()
		{
			string typ = CzySlona ? "Słona" : "Słodka";
			return base.OpisSzczegolowy() + $" [{typ}]";
		}
	}
}