import type { Crate, Price, Skin, WearCategory } from '../types';

function tryGetPrice(price: Price) {
    if (price.steam_last_24h) return price.steam_last_24h;
    if (price.steam_last_7d) return price.steam_last_7d;
    if (price.steam_last_30d) return price.steam_last_30d;
    if (price.steam_last_90d) return price.steam_last_90d;

    return price.steam_last_ever ?? 0;
}

export function getCratePrice(crate: Crate | null) {
    if (!crate?.price) return 0;

    return tryGetPrice(crate.price);
}

export function getItemPrice(item: Skin, wearCategory?: WearCategory) {
    if (!item || !item.prices) return 0;

    const foundItem = item.prices.find((price) => price.wear_category === (wearCategory || item.wear_category));
    if (!foundItem) return 0;

    return tryGetPrice(foundItem);
}

function getAllPricesForItem(item: Skin) {
    if (!item || !item.prices) return [];

    const prices = [
        getItemPrice(item, 'Battle-Scarred'),
        getItemPrice(item, 'Well-Worn'),
        getItemPrice(item, 'Field-Tested'),
        getItemPrice(item, 'Minimal Wear'),
        getItemPrice(item, 'Factory New'),
        getItemPrice(item, 'Default'),
    ].filter((price) => price > 0);

    if (prices.length === 0) return [];

    return prices;
}

export function getItemMinPrice(item: Skin) {
    const allPrices = getAllPricesForItem(item);

    if (allPrices.length === 0) return 0;
    const minPrice = Math.min(...allPrices);

    return minPrice;
}

export function getItemMaxPrice(item: Skin) {
    const allPrices = getAllPricesForItem(item);

    if (allPrices.length === 0) return 0;
    const maxPrice = Math.max(...allPrices);

    return maxPrice;
}

const currencyFormatter = new Intl.NumberFormat('de-DE', { style: 'currency', currency: 'EUR' });

export function formatEuro(number: number) {
    return currencyFormatter.format(number);
}
