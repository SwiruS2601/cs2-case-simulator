import type { Crate, Skin } from '../types';

export function getItemPrice(item: Skin) {
    if (!item || !item.prices) return 0;

    const foundItem = item.prices?.find((price) => price.wear_category === item.wear_category);
    if (!foundItem) return 0;

    const lastDay = foundItem.steam_last_24h;
    if (lastDay) return lastDay;

    const lastWeek = foundItem.steam_last_7d;
    if (lastWeek) return lastWeek;

    const lastMonth = foundItem.steam_last_30d;
    if (lastMonth) return lastMonth;

    const lastThreeMonths = foundItem.steam_last_90d;
    if (lastThreeMonths) return lastThreeMonths;

    return foundItem.steam_last_ever || 0;
}

export function getCratePrice(crate: Crate | null) {
    if (!crate?.price) return 0;

    const lastDay = crate.price.steam_last_24h;
    if (lastDay) return lastDay;

    const lastWeek = crate.price.steam_last_7d;
    if (lastWeek) return lastWeek;

    const lastMonth = crate.price.steam_last_30d;
    if (lastMonth) return lastMonth;

    const lastThreeMonths = crate.price.steam_last_90d;
    if (lastThreeMonths) return lastThreeMonths;

    return crate.price.steam_last_ever || 0;
}
