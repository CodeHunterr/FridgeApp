using FridgeApp.Data;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeApp.Services
{
	public class RecipeSuggestionService : IRecipeSuggestionService
	{
		private static readonly List<RecipeDefinition> RecipeDefinitions =
		[
			new(
				"Domatesli Yumurta",
				"Kahvalti",
				"Az malzemeyle hizlica hazirlanan klasik bir tava kahvaltisi.",
				10,
				["Yumurta", "Domates", "Tereyagi"],
				["2 yumurta", "1 domates", "1 tatli kasigi tereyagi", "Tuz"],
				[
					"Tereyagini tavada erit.",
					"Kup dogranmis domatesi yumusayana kadar pisir.",
					"Yumurtalari kirmadan once hafif karistir ve tavaya ekle.",
					"Damaga gore tuz ekleyip sicak servis et."
				]),
			new(
				"Menemen",
				"Kahvalti",
				"Domates, biber ve yumurtayla hazirlanan pratik bir klasik.",
				15,
				["Yumurta", "Domates", "Biber"],
				["2 yumurta", "2 domates", "1 yesil biber", "Tuz", "Zeytinyagi"],
				[
					"Biberi ince dograyip tavada sotele.",
					"Domatesleri ekleyip sulu bir harc elde et.",
					"Yumurtalari kir ve cok karistirmadan toparlanana kadar pisir.",
					"Arzu edersen ekmekle hemen servis et."
				]),
			new(
				"Peynirli Omlet",
				"Kahvalti",
				"Yumurtayi peynirle zenginlestiren yumusak bir omlet.",
				12,
				["Yumurta", "Peynir"],
				["2 yumurta", "Beyaz peynir veya kasar", "1 cay kasigi tereyagi", "Karabiber"],
				[
					"Yumurtalari cirp ve baharatla tatlandir.",
					"Tereyagini tavada eritip yumurtayi dok.",
					"Peyniri uzerine serp ve omleti kapat.",
					"Altin renk aldiginda servis et."
				]),
			new(
				"Patatesli Yumurta",
				"Kahvalti",
				"Patatesle daha doyurucu hale gelen yumurta tarifi.",
				18,
				["Yumurta", "Patates"],
				["2 yumurta", "1 patates", "Sivi yag", "Tuz", "Pul biber"],
				[
					"Patatesi kuplere bolup tavada kizart.",
					"Patateslerin uzerine yumurtalari kir.",
					"Baharatlari ekleyip istedigin kivama kadar pisir.",
					"Sicak olarak servis et."
				]),
			new(
				"Kasarli Tost",
				"Atistirmalik",
				"Evdeki ekmek ve peynirle yapilabilen hizli bir atistirmalik.",
				8,
				["Ekmek", "Peynir", "Tereyagi"],
				["2 dilim ekmek", "Kasar peyniri", "Bir miktar tereyagi"],
				[
					"Ekmeklerin disina ince bir kat tereyagi sur.",
					"Peyniri araya koyup tost makinesine yerlestir.",
					"Peynir eriyip ekmek kizarana kadar pisir."
				]),
			new(
				"Peynirli Tost",
				"Atistirmalik",
				"Klasik tostun daha hafif bir versiyonu.",
				6,
				["Ekmek", "Peynir"],
				["2 dilim ekmek", "Beyaz peynir veya kasar", "Domates dilimi opsiyonel"],
				[
					"Peyniri ekmeklerin arasina yerlestir.",
					"Arzu edersen ince domates dilimi ekle.",
					"Tost makinesinde kizart ve dilimleyerek servis et."
				]),
			new(
				"Yogurtlu Makarna",
				"Ana Yemek",
				"HaÅŸlanmis makarnayi sarimsakli yogurtla birlestiren kolay tarif.",
				20,
				["Makarna", "Yogurt", "Tereyagi"],
				["1 kase makarna", "3 yemek kasigi yogurt", "1 dis sarimsak", "1 tatli kasigi tereyagi"],
				[
					"Makarnayi paket talimatina gore hasla.",
					"Yogurdu sarimsakla karistir.",
					"Makarnayi yogurtlu harcla bulustur.",
					"Uzerine kizdirilmis tereyagi gezdir."
				]),
			new(
				"Kıymalı Makarna",
				"Ana Yemek",
				"Kıymanın makarnaya lezzet kattigi pratik bir aksam yemegi.",
				25,
				["Makarna", "Kiyma", "Domates"],
				["1 porsiyon makarna", "150 gr kiyma", "1 domates", "Tuz", "Karabiber"],
				[
					"Makarnayi hasla ve suyunu suz.",
					"Kiyma suyunu salip cekene kadar kavrulsun.",
					"Domatesi ekleyip kisa bir sos hazirla.",
					"Makarnayla birlestirip sicak servis et."
				]),
			new(
				"Domatesli Makarna",
				"Ana Yemek",
				"Domates sosuyla hafif ve hizli bir makarna alternatifi.",
				20,
				["Makarna", "Domates"],
				["1 porsiyon makarna", "2 domates", "Zeytinyagi", "Tuz", "Kekik"],
				[
					"Makarnayi hasla.",
					"Domatesleri rendeleyip tavada kisa sure pisir.",
					"Baharatlarla tatlandirip makarnayla karistir.",
					"Uzerine az zeytinyagi ekleyerek servis et."
				]),
			new(
				"Tavuklu Pilav",
				"Ana Yemek",
				"Tek tabakta doyurucu bir pilav ustu tavuk tabagi.",
				35,
				["Tavuk", "Pirinç"],
				["1 su bardagi pirinc", "150 gr tavuk", "1 yemek kasigi tereyagi", "Tuz"],
				[
					"Pirinci iyice yikayip pilavi demle.",
					"Tavugu kucuk parcalara bol ve tavada sotele.",
					"Pismis tavugu pilavin uzerine yerlestir.",
					"Sicak olarak servis et."
				]),
			new(
				"Sebzeli Tavuk",
				"Ana Yemek",
				"Sebzelerle dengelenen hafif bir tavuk yemegi.",
				30,
				["Tavuk", "Domates", "Biber"],
				["200 gr tavuk", "1 domates", "1 biber", "Tuz", "Karabiber"],
				[
					"Tavugu kisa sure yuksek ateste renk alana kadar pisir.",
					"Biber ve domatesi ekleyip yumusat.",
					"Baharatlarla dengele ve kisik ateste toparla.",
					"Pilav veya salatayla servis et."
				]),
			new(
				"Patates Salatasi",
				"Salata",
				"Haşlanmış patatesle hazirlanan pratik bir ara sicak.",
				22,
				["Patates", "Yogurt"],
				["2 patates", "2 yemek kasigi yogurt", "Maydanoz", "Tuz"],
				[
					"Patatesleri haslayip irice ez.",
					"Yogurt ve tuzu ekleyip karistir.",
					"Maydanozla taze bir dokunus ver.",
					"Ilik veya soguk servis et."
				]),
			new(
				"Çoban Salata",
				"Salata",
				"Taze sebzelerle hazirlanan her ogune uyan klasik salata.",
				10,
				["Domates", "Salatalik", "Biber"],
				["2 domates", "1 salatalik", "1 biber", "Zeytinyagi", "Limon", "Tuz"],
				[
					"Sebzeleri kuçuk kupler halinde dogra.",
					"Limon, tuz ve zeytinyagiyla sos hazirla.",
					"Sebzelerle sosu karistirip bekletmeden servis et."
				]),
			new(
				"Ton Balıklı Salata",
				"Salata",
				"Proteini yuksek, serin ve doyurucu bir salata.",
				12,
				["Ton Baligi", "Domates", "Salatalik"],
				["1 kutu ton baligi", "1 domates", "1 salatalik", "Marul", "Limon"],
				[
					"Sebzeleri iri dogra ve servis kabina al.",
					"Ton baligini suyunu suzup uzerine dagit.",
					"Limonla tatlandir ve hemen servis et."
				]),
			new(
				"Mercimek Çorbası",
				"Çorba",
				"Evdeki temel malzemelerle hazirlanan sicak bir corba.",
				35,
				["Mercimek", "Tereyagi"],
				["1 su bardagi mercimek", "1 yemek kasigi tereyagi", "Tuz", "Su"],
				[
					"Mercimegi yikayip suyla yumusayana kadar hasla.",
					"Blendirdan gecirerek puresini hazirla.",
					"Tereyagini ekleyip bir tasim daha kaynat.",
					"Tuzunu ayarlayip sicak servis et."
				]),
			new(
				"Ezogelin Çorbası",
				"Çorba",
				"Mercimek ve domatesle kolayca toparlanan geleneksel bir corba.",
				40,
				["Mercimek", "Domates"],
				["1 su bardagi mercimek", "1 domates", "Salca", "Nane", "Su"],
				[
					"Mercimegi haslamaya basla.",
					"Domates ve salcayla ayri bir taban hazirla.",
					"Iki karisimi birlestirip kivam alana kadar kaynat.",
					"Nane ile son dokunusu yap."
				]),
			new(
				"Yayla Çorbası",
				"Çorba",
				"Yogurt bazli, hafif ve rahatlatan bir corba.",
				25,
				["Yogurt", "Pirinç"],
				["3 yemek kasigi yogurt", "2 yemek kasigi pirinc", "Nane", "Su"],
				[
					"Pirinci yumusayana kadar hasla.",
					"Yogurdu corba suyuyla alistirarak karisima ekle.",
					"Kisikin altinda kesilmeden toparla.",
					"Nane ile servis et."
				]),
			new(
				"Tavuk Sote",
				"Ana Yemek",
				"Kisa surede hazirlanan taneli tavuk tavasi.",
				25,
				["Tavuk", "Domates", "Biber"],
				["200 gr tavuk", "1 domates", "1 biber", "Sivi yag", "Baharat"],
				[
					"Tavugu kusbasi dograyip tavaya al.",
					"Biberi ve domatesi ekleyip sotelemeye devam et.",
					"Baharatlari ekleyip suyunu cekene kadar pisir.",
					"Pilav veya ekmekle servis et."
				]),
			new(
				"Kıymalı Yumurta",
				"Ana Yemek",
				"Kıymanın yumurtayla bulustugu tok tutan bir tava tarifi.",
				18,
				["Kiyma", "Yumurta"],
				["100 gr kiyma", "2 yumurta", "Tuz", "Karabiber"],
				[
					"Kıymayı tavada kavur.",
					"Uzerine yumurtalari kir.",
					"Baharatlari ekleyip istedigin kivama kadar pisir."
				]),
			new(
				"Sebzeli Omlet",
				"Kahvalti",
				"Yumurtayi sebzeyle zenginlestiren hafif omlet tarifi.",
				15,
				["Yumurta", "Domates", "Biber"],
				["2 yumurta", "Yarim domates", "Yarim biber", "Tuz"],
				[
					"Sebzeleri ince dogra ve kisa sure sotele.",
					"Cirpilmis yumurtayi tavaya ekle.",
					"Orta ateste iki yuzu de pisecek sekilde toparla."
				]),
			new(
				"Mantar Sote",
				"Ana Yemek",
				"Mantarin kisa surede lezzet kazandigi hafif bir yan yemek.",
				15,
				["Mantar", "Tereyagi"],
				["200 gr mantar", "1 tatli kasigi tereyagi", "Karabiber", "Tuz"],
				[
					"Mantarlari dilimle.",
					"Tereyaginda yuksek ateste sotele.",
					"Baharat ekleyip suyunu cekince servis et."
				]),
			new(
				"Peynirli Sandviç",
				"Atistirmalik",
				"Hizli bir ogun icin peynirli ve taze sebzeli sandviç.",
				7,
				["Ekmek", "Peynir", "Domates"],
				["2 dilim ekmek", "Peynir", "Domates dilimleri", "Yesillik"],
				[
					"Ekmeklerin arasina peyniri yerlestir.",
					"Domates ve yesillik ekle.",
					"Ikiye bolup hemen servis et."
				]),
			new(
				"Yoğurtlu Salatalık",
				"Meze",
				"Serinletici ve hafif bir yan lezzet.",
				8,
				["Yogurt", "Salatalik"],
				["1 salatalik", "3 yemek kasigi yogurt", "Nane", "Tuz"],
				[
					"Salataligi rendele veya minik kupler halinde dogra.",
					"Yogurt ve baharatlarla karistir.",
					"Soguk servis et."
				]),
			new(
				"Zeytinli Peynir Tabağı",
				"Kahvalti",
				"Sofrayi hizla toparlayan klasik kahvaltilik tabak.",
				5,
				["Peynir", "Zeytin", "Domates"],
				["Peynir dilimleri", "Zeytin", "Domates dilimleri", "Salatalik opsiyonel"],
				[
					"Tum malzemeleri servis tabagina duzenli yerlestir.",
					"Arzu edersen zeytinyagi gezdir.",
					"Ekmekle birlikte servis et."
				])
		];

		private readonly AppDbContext _context;

		public RecipeSuggestionService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<RecipeSuggestionResponse>> GetSuggestionsAsync(int fridgeId)
		{
			var today = DateTime.Today;

			var availableNames = await _context.Items
				.Where(item =>
					item.FridgeId == fridgeId &&
					!item.IsDeleted &&
					!string.IsNullOrWhiteSpace(item.Name) &&
					(!item.ExpirationDate.HasValue || item.ExpirationDate.Value.Date >= today))
				.Select(item => item.Name)
				.ToListAsync();

			var availableItemSet = availableNames
				.Select(Normalize)
				.Where(name => !string.IsNullOrWhiteSpace(name))
				.ToHashSet();

			var suggestions = RecipeDefinitions
				.Select(recipe => CreateSuggestion(recipe, availableItemSet))
				.Where(suggestion => suggestion.MatchedItems.Count > 0 && suggestion.MissingItems.Count <= 2)
				.OrderByDescending(suggestion => suggestion.CanMakeNow)
				.ThenBy(suggestion => suggestion.MissingItems.Count)
				.ThenByDescending(suggestion => suggestion.Score)
				.ThenBy(suggestion => suggestion.RecipeName)
				.ToList();

			return suggestions;
		}

		private static RecipeSuggestionResponse CreateSuggestion(
			RecipeDefinition recipe,
			HashSet<string> availableItemSet)
		{
			var matchedItems = recipe.RequiredItems
				.Where(item => availableItemSet.Contains(Normalize(item)))
				.ToList();

			var missingItems = recipe.RequiredItems
				.Where(item => !availableItemSet.Contains(Normalize(item)))
				.ToList();

			var score = (int)Math.Round((double)matchedItems.Count / recipe.RequiredItems.Count * 100);

			return new RecipeSuggestionResponse
			{
				RecipeName = recipe.Name,
				Description = recipe.Description,
				Category = recipe.Category,
				EstimatedPrepTimeMinutes = recipe.EstimatedPrepTimeMinutes,
				MatchedItems = matchedItems,
				MissingItems = missingItems,
				Ingredients = recipe.Ingredients,
				PreparationSteps = recipe.PreparationSteps,
				Score = score,
				CanMakeNow = missingItems.Count == 0
			};
		}

		private static string Normalize(string value)
		{
			var normalized = value.Trim().ToLowerInvariant();

			normalized = normalized
				.Replace('\u00E7', 'c')
				.Replace('\u011F', 'g')
				.Replace('\u0131', 'i')
				.Replace('\u00F6', 'o')
				.Replace('\u015F', 's')
				.Replace('\u00FC', 'u');

			return string.Concat(normalized.Where(char.IsLetterOrDigit));
		}

		private sealed record RecipeDefinition(
			string Name,
			string Category,
			string Description,
			int EstimatedPrepTimeMinutes,
			List<string> RequiredItems,
			List<string> Ingredients,
			List<string> PreparationSteps);
	}
}
