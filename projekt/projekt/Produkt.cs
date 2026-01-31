#nullable disable // Wyłącza żółte ostrzeżenia o wartościach null
using System.Text.Json.Serialization;

namespace SymulatorAutomatu
{
	// Atrybuty dla JSON, żeby wiedział jak rozróżnić klasy przy odczycie
	[JsonDerivedType(typeof(Napoj), typeDiscriminator: "napoj")]
	[JsonDerivedType(typeof(Przekaska), typeDiscriminator: "przekaska")]
	public abstract class Produkt
	{
		// hermetyzacja pól jako właściwości
		public string Nazwa { get; set; }
		public decimal Cena { get; set; }
		public int Ilosc { get; set; }

		protected Produkt(string nazwa, decimal cena, int ilosc)
		{
			Nazwa = nazwa;
			Cena = cena;
			Ilosc = ilosc;
		}

		// polimorfizm poprzez metodę abstrakcyjną
		// zwraca wartość kaucji dla produktu
		// Każdy typ produktu sam definiuje jaką kaucję ma
		public abstract decimal ObliczKaucje();

		public virtual string OpisSzczegolowy()
		{
			return $"{Nazwa} ({Cena:C})";
		}
	}
}