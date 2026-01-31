#nullable disable // Wyłącza żółte ostrzeżenia o wartościach null
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;


namespace SymulatorAutomatu
{
	public class Automat
	{
		private List<Produkt> _produkty;
		private decimal _saldoUzytkownika;
		private const string PlikBazy = "magazyn.json";

		public Automat()
		{
			_saldoUzytkownika = 0;
			_produkty = WczytajDane();
		}

		public void Uruchom()
		{
			bool dziala = true;
			while (dziala)
			{
				Console.Clear();
				Console.WriteLine("=== SYMULATOR AUTOMATU VENDINGOWEGO ===");
				Console.WriteLine($"Twoje saldo: {_saldoUzytkownika:C}");
				Console.WriteLine("1. Wrzuc monete");
				Console.WriteLine("2. Wybierz produkt");
				Console.WriteLine("3. Pokaz tylko tanie produkty (< 5 PLN)");
				Console.WriteLine("4. Wyjscie");
				Console.Write("Wybierz opcje: ");

				string opcja = Console.ReadLine();

				switch (opcja)
				{
					case "1":
						ObslugaWplaty();
						break;
					case "2":
						WyswietlProdukty();
						KupProdukt();
						break;
					case "3":
						FiltrujTanieProdukty();
						break;
					case "4":
						dziala = false;
						ZapiszDane();
						break;
					default:
						Console.WriteLine("Nieznana opcja.");
						Thread.Sleep(1000);
						break;
				}
			}
		}

		private void ObslugaWplaty()
		{
			Console.Write("Podaj kwote do wplaty (np. 2,50): ");
			if (decimal.TryParse(Console.ReadLine(), out decimal kwota) && kwota > 0)
			{
				_saldoUzytkownika += kwota;
				Console.WriteLine($"Wplacono {kwota:C}.");
			}
			else
			{
				Console.WriteLine("Nieprawidlowa kwota.");
			}
			Thread.Sleep(1000);
		}

		private void WyswietlProdukty(List<Produkt> listaDoWyswietlenia = null)
		{
			var lista = listaDoWyswietlenia ?? _produkty;

			Console.WriteLine("\n--- DOSTEPNE PRODUKTY ---");
			int index = 1;

			foreach (var p in lista)
			{
				Console.WriteLine($"{index}. {p.OpisSzczegolowy()} | Stan: {p.Ilosc} szt.");
				index++;
			}
			Console.WriteLine("-------------------------");
		}

		private void KupProdukt()
		{
			Console.Write("Wybierz numer produktu: ");
			if (int.TryParse(Console.ReadLine(), out int numer) && numer > 0 && numer <= _produkty.Count)
			{
				var wybranyProdukt = _produkty[numer - 1];

				// polimorfizm - obliczanie kaucji
				
				decimal kaucja = wybranyProdukt.ObliczKaucje();
				decimal kosztCalkowity = wybranyProdukt.Cena + kaucja;

				if (wybranyProdukt.Ilosc <= 0)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Produkt niedostepny!");
					Console.ResetColor();
				}
				else if (_saldoUzytkownika < kosztCalkowity)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"Za malo srodkow! Cena: {wybranyProdukt.Cena:C} + Kaucja: {kaucja:C} = RAZEM: {kosztCalkowity:C}");
					Console.ResetColor();
				}
				else
				{
					// Realizacja zakupu
					_saldoUzytkownika -= kosztCalkowity;
					wybranyProdukt.Ilosc--;

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"Kupiono: {wybranyProdukt.Nazwa}");

					if (kaucja > 0)
					{
						Console.WriteLine($"UWAGA: Doliczono kaucję zwrotną za opakowanie: {kaucja:C}");
					}

					Console.WriteLine($"Pozostałe saldo: {_saldoUzytkownika:C}");
					Console.ResetColor();

					ZapiszDane();
				}
			}
			else
			{
				Console.WriteLine("Nieprawidlowy numer.");
			}
			Console.WriteLine("\nNacisnij dowolny klawisz...");
			Console.ReadKey();
		}

		private void FiltrujTanieProdukty()
		{
			Console.Clear();
			Console.WriteLine("--- TANIE PRODUKTY (LINQ) ---");

			// LINQ: Filtrowanie po cenie (bazowej)
			var tanie = _produkty
				.Where(p => p.Cena < 5.0m)
				.OrderBy(p => p.Cena)
				.ToList();

			if (tanie.Any())
			{
				WyswietlProdukty(tanie);
			}
			else
			{
				Console.WriteLine("Brak tanich produktow.");
			}
			Console.WriteLine("\nNacisnij dowolny klawisz...");
			Console.ReadKey();
		}

		private List<Produkt> WczytajDane()
		{
			// Jeśli plik nie istnieje, tworzymy pełny asortyment (9 produktów)
			if (!File.Exists(PlikBazy))
			{
				return new List<Produkt>
				{
                    // doliczają kaucję
                    new Napoj("Cola", 4.00m, 10, 500),
					new Napoj("Woda", 2.00m, 20, 500),
					new Napoj("Sok Pomaranczowy", 3.50m, 15, 330),
					new Napoj("Energetyk", 6.00m, 8, 250),
					new Napoj("Ice Tea", 4.20m, 12, 500),

                    // Bez kaucji
                    new Przekaska("Chipsy", 6.50m, 5, true),     
                    new Przekaska("Baton", 3.00m, 15, false),    
                    new Przekaska("Paluszki", 2.50m, 10, true),  
                    new Przekaska("Czekolada", 5.00m, 7, false)  
                };
			}

			try
			{
				string json = File.ReadAllText(PlikBazy);
				var options = new JsonSerializerOptions { WriteIndented = true };
				return JsonSerializer.Deserialize<List<Produkt>>(json, options);
			}
			catch (Exception)
			{
				Console.WriteLine("Blad odczytu bazy. Tworze nowa.");
				return new List<Produkt>();
			}
		}

		private void ZapiszDane()
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			string json = JsonSerializer.Serialize(_produkty, options);
			File.WriteAllText(PlikBazy, json);
		}
	}
}