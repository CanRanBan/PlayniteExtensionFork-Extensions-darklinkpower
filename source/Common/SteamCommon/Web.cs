using AngleSharp.Parser.Html;
using Playnite.SDK;
using Playnite.SDK.Data;
using SteamCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebCommon;

namespace SteamCommon
{
    class SteamWeb
    {
        private static ILogger logger = LogManager.GetLogger();
        private const string steamGameSearchUrl = @"https://store.steampowered.com/search/?term={0}&ignore_preferences=1&category1=998";

        public static List<GenericItemOption> GetSteamSearchGenericItemOptions(string searchTerm)
        {
            return GetSteamSearchResults(searchTerm).Select(x => new GenericItemOption(x.Name, x.GameId)).ToList();
        }

        public static string GetSteamIdFromSearch(string searchTerm)
        {
            var normalizedName = searchTerm.NormalizeGameName();
            var results = GetSteamSearchResults(normalizedName);
            results.ForEach(a => a.Name = a.Name.NormalizeGameName());

            var matchingGameName = normalizedName.GetMatchModifiedName();
            var exactMatch = results.FirstOrDefault(x => x.Name.GetMatchModifiedName() == matchingGameName);
            if (exactMatch != null)
            {
                logger.Info($"Found steam id for search {searchTerm} via steam search, Id: {exactMatch.GameId}");
                return exactMatch.GameId;
            }

            logger.Info($"Steam id for search {searchTerm} not found");
            return null;
        }

        public static List<StoreSearchResult> GetSteamSearchResults(string searchTerm, string steamApiCountry = null)
        {
            var results = new List<StoreSearchResult>();
            var searchPageSrc = HttpDownloader.DownloadString(GetStoreSearchUrl(searchTerm, steamApiCountry));
            if (searchPageSrc.Success)
            {
                var parser = new HtmlParser();
                var searchPage = parser.Parse(searchPageSrc.Result);
                foreach (var gameElem in searchPage.QuerySelectorAll(".search_result_row"))
                {
                    if (gameElem.HasAttribute("data-ds-packageid"))
                    {
                        continue;
                    }

                    // Game Data
                    var title = gameElem.QuerySelector(".title").InnerHtml;
                    var releaseDate = gameElem.QuerySelector(".search_released").InnerHtml;
                    var gameId = gameElem.GetAttribute("data-ds-appid");

                    // Prices Data
                    var priceData = gameElem.QuerySelector(".search_discount_and_price");
                    var discountPercentage = GetSteamSearchDiscount(priceData);
                    var priceFinal = GetSteamSearchFinalPrice(priceData);
                    var priceOriginal = GetSearchOriginalPrice(priceFinal, discountPercentage);
                    var isDiscounted = priceFinal != priceOriginal && priceOriginal != 0;
                    GetCurrencyAndReleaseDateFromPriceData(priceData, out var currency, out var isReleased, out var isFree);

                    // Urls
                    var storeUrl = gameElem.GetAttribute("href");
                    var capsuleUrl = gameElem.QuerySelector(".search_capsule").Children[0].GetAttribute("src");

                    results.Add(new StoreSearchResult
                    {
                        Name = HttpUtility.HtmlDecode(title),
                        Description = HttpUtility.HtmlDecode(releaseDate),
                        GameId = gameId,
                        PriceOriginal = priceOriginal,
                        PriceFinal = priceFinal,
                        IsDiscounted = isDiscounted,
                        DiscountPercentage = discountPercentage,
                        StoreUrl = storeUrl,
                        IsFree = isFree,
                        IsReleased = isReleased,
                        Currency = currency,
                        BannerImageUrl = capsuleUrl
                    });
                }
            }

            logger.Debug($"Obtained {results.Count} games from Steam search term {searchTerm}");
            return results;
        }

        // Note: isFree will always be false right now - search result data does no longer provide this value
        private static void GetCurrencyAndReleaseDateFromPriceData(AngleSharp.Dom.IElement priceData, out string currency, out bool isReleased, out bool isFree)
        {
            // Unreleased or delisted
            isReleased = false;
            currency = null;
            isFree = false;

            if (priceData.ChildElementCount > 0)
            {
                var searchDiscountBlock = priceData.QuerySelector(".search_discount_block");
                if (searchDiscountBlock.ChildElementCount == 2)
                {
                    var pricesElement = searchDiscountBlock.QuerySelector(".discount_prices");

                    // Discounted Item
                    isReleased = true;
                    currency = GetCurrencyFromDiscountBlock(pricesElement);
                    isFree = currency == null;
                }
                else if (searchDiscountBlock.ChildElementCount == 1)
                {
                    var pricesElement = searchDiscountBlock.QuerySelector(".discount_prices");

                    // Non discounted item
                    isReleased = true;
                    currency = GetCurrencyFromDiscountBlock(pricesElement);
                    isFree = currency == null;
                }
            }
        }

        private static string GetCurrencyFromDiscountBlock(AngleSharp.Dom.IElement pricesElement)
        {
            var finalPriceWithCurrency = pricesElement.QuerySelector(".discount_final_price").InnerHtml;

            if (!Regex.IsMatch(finalPriceWithCurrency, @"\d"))
            {
                // Game is free
                return null;
            }

            return Regex.Match(finalPriceWithCurrency, @"[^\s]+").Value;
        }

        private static string GetStoreSearchUrl(string searchTerm, string steamApiCountry)
        {
            var searchUrl = string.Format(steamGameSearchUrl, searchTerm);
            if (!steamApiCountry.IsNullOrEmpty())
            {
                searchUrl += $"&cc={steamApiCountry}";
            }

            return searchUrl;
        }

        private static double GetSearchOriginalPrice(double priceFinal, int discountPercentage)
        {
            if (discountPercentage == 0)
            {
                return priceFinal;
            }

            return 100 * priceFinal / (100 - discountPercentage);
        }

        private static int GetSteamSearchDiscount(AngleSharp.Dom.IElement priceData)
        {
            if (priceData.ChildElementCount > 0)
            {
                var searchDiscountBlock = priceData.QuerySelector(".search_discount_block");
                if (searchDiscountBlock.ChildElementCount == 2)
                {
                    return int.Parse(searchDiscountBlock.GetAttribute("data-discount"));
                }
            }
            return 0;
        }

        private static double GetSteamSearchFinalPrice(AngleSharp.Dom.IElement priceData)
        {
            if (priceData.ChildElementCount > 0)
            {
                var searchDiscountBlock = priceData.QuerySelector(".search_discount_block");
                if (searchDiscountBlock.ChildElementCount > 0)
                {
                    return double.Parse(searchDiscountBlock.GetAttribute("data-price-final")) * 0.01;
                }
            }
            return 0;
        }

        private const string steamAppDetailsMask = @"https://store.steampowered.com/api/appdetails?appids={0}";
        public static SteamAppDetails GetSteamAppDetails(string steamId)
        {
            var url = string.Format(steamAppDetailsMask, steamId);
            var downloadedString = HttpDownloader.DownloadString(url);
            if (downloadedString.Success)
            {
                var parsedData = Serialization.FromJson<Dictionary<string, SteamAppDetails>>(downloadedString.Result);
                if (parsedData.Keys?.Any() == true)
                {
                    var response = parsedData[parsedData.Keys.First()];
                    if (response.success == true && response.data != null)
                    {
                        return response;
                    }
                }
            }

            return null;
        }
    }
}