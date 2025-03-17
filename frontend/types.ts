export type Crate = {
    id: string;
    name: string;
    description: string;
    type: string;
    first_sale_date: string;
    market_hash_name: string;
    rental: boolean;
    image: string;
    model_player: string;
    price?: Price;
    skins: Skin[];
};

export type Skin = {
    id: string;
    name: string;
    rarity_id: RarityId;
    paint_index: string;
    image: string;
    prices: Price[];
    rarity: Rarity;
    wear_category: string;
};

export type Price = {
    id: number;
    skin_id: string;
    crate_id: string;
    name: string;
    wear_category: string;
    steam_last_24h: number;
    steam_last_7d: number;
    steam_last_30d: number;
    steam_last_90d: number;
    steam_last_ever: number;
};

export type Rarity = {
    id: RarityId;
    name: string;
    color: string;
};

export type RarityId =
    | 'rarity_ancient'
    | 'rarity_ancient_weapon'
    | 'rarity_common_weapon'
    | 'rarity_legendary'
    | 'rarity_legendary_weapon'
    | 'rarity_mythical'
    | 'rarity_mythical_weapon'
    | 'rarity_rare'
    | 'rarity_rare_weapon'
    | 'rarity_uncommon_weapon'
    | 'exceedingly_rare';

export type BaseRarity = 'rare' | 'mythical' | 'legendary' | 'ancient' | 'exceedingly_rare';
