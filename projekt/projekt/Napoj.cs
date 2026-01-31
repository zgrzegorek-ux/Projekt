#nullable disable // Wyłącza żółte ostrzeżenia o wartościach null
namespace SymulatorAutomatu
{

	// dziedziczy po klasie bazowej Produkt


	public class Napoj : Produkt
	{
		public int PojemnoscMl { get; set; }

		public Napoj(string nazwa, decimal cena, int ilosc, int pojemnoscMl)
			: base(nazwa, cena, ilosc)
		{
			PojemnoscMl = pojemnoscMl;
		}

		// Napoje mają doliczaną kaucję 0.50 PLN za butelkę
		public override decimal ObliczKaucje()
		{
			return 0.50m;
		}

		public override string OpisSzczegolowy()
		{
			// Informujemy klienta o kaucji w opisie
			return base.OpisSzczegolowy() + $" [{PojemnoscMl}ml] (+{ObliczKaucje():C} kaucja)";
		}
	}
}