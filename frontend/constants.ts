export const SLIDER_SIZE = 50;
export const WON_SKIN_INDEX = 42;

export const REAL_ODDS = {
    rare: 0.7992, // 79.92%
    mythical: 0.1598, // 15.98%
    legendary: 0.032, // 3.2%
    ancient: 0.0064, // 0.64%
    exceedingly_rare: 0.0026, // 0.26%
} as const;

export const FUN_ODDS = {
    rare: 0, // 0%
    mythical: 0.0952381, // ~9.52%
    legendary: 0.2857143, // ~28.57%
    ancient: 0.2380952, // ~23.81%
    exceedingly_rare: 0.3809524, // ~38.10%
} as const;

export const ODDS_TO_RARITY = {
    rare: ['rarity_common_weapon', 'rarity_uncommon_weapon', 'rarity_rare_weapon', 'rarity_rare'],
    mythical: ['rarity_mythical_weapon', 'rarity_mythical'],
    legendary: ['rarity_legendary_weapon', 'rarity_legendary'],
    ancient: ['rarity_ancient_weapon', 'rarity_ancient'],
    exceedingly_rare: ['exceedingly_rare'],
};

export const GRADE_TO_RARITY = {
    'Consumer Grade': 'rarity_common_weapon',
    'Industrial Grade': 'rarity_uncommon_weapon',
    'Mil-Spec Grade': 'rarity_rare_weapon',
    Restricted: 'rarity_mythical_weapon',
    Remarkable: 'rarity_mythical',
    Classified: 'rarity_legendary_weapon',
    Exotic: 'rarity_legendary',
    Covert: 'rarity_ancient_weapon',
    Extraordinary: 'rarity_ancient',
    Contraband: 'exceedingly_rare',
    Gloves: 'exceedingly_rare',
    Knife: 'exceedingly_rare',
} as const;

export const RARITY_TO_GRADE: Record<string, string> = {
    rarity_common_weapon: 'Consumer Grade',
    rarity_uncommon_weapon: 'Industrial Grade',
    rarity_rare_weapon: 'Mil-Spec Grade',
    rarity_mythical_weapon: 'Restricted',
    rarity_mythical: 'Remarkable',
    rarity_legendary_weapon: 'Classified',
    rarity_legendary: 'Exotic',
    rarity_ancient_weapon: 'Covert',
    rarity_ancient: 'Extraordinary',
    exceedingly_rare: 'Contraband',
};

export const RARITY_INDEX: Record<string, number> = {
    rarity_common_weapon: 0,
    rarity_uncommon_weapon: 1,
    rarity_rare_weapon: 2,
    rarity_rare: 3,
    rarity_mythical_weapon: 4,
    rarity_mythical: 5,
    rarity_legendary_weapon: 6,
    rarity_legendary: 7,
    rarity_ancient_weapon: 8,
    rarity_ancient: 9,
    exceedingly_rare: 10,
};

export const SKIN_WEAR_AND_FLOAT = {
    'Factory New': { floatRange: [0, 0.07], odds: 0.025 }, // ~2-3%
    'Minimal Wear': { floatRange: [0.07, 0.15], odds: 0.165 }, // ~15-18%
    'Field-Tested': { floatRange: [0.15, 0.38], odds: 0.725 }, // ~70-75%
    'Well-Worn': { floatRange: [0.38, 0.45], odds: 0.06 }, // ~5-7%
    'Battle-Scarred': { floatRange: [0.45, 1], odds: 0.005 }, // ~0.3-1%
};

export const BACKGROUNDS = [
    'ar_baggage.webp',
    'ar_shoots.webp',
    //   'cs_italy.webp',
    'cs_office.webp',
    'cs_office_1.webp',
    'cs_office_2.webp',
    'cs_office_3.webp',
    'cs_office_4.webp',
    'de_ancient.webp',
    //   'de_ancient_1.webp',
    'de_ancient_2.webp',
    //   'de_ancient_3.webp',
    //   'de_ancient_4.webp',
    'de_anubis.webp',
    //   'de_anubis_1.webp',
    //   'de_anubis_2.webp',
    'de_anubis_3.webp',
    //   'de_anubis_4.webp',
    //   'de_anubis_5.webp',
    'de_canals.webp',
    'de_dust2.webp',
    'de_dust2_1.webp',
    'de_dust2_2.webp',
    'de_dust2_3.webp',
    //   'de_inferno.webp',
    'de_inferno_1.webp',
    //   'de_inferno_2.webp',
    //   'de_inferno_3.webp',
    'de_lake.webp',
    'de_mirage.webp',
    'de_mirage_1.webp',
    //   'de_mirage_2.webp',
    'de_mirage_3.webp',
    //   'de_mirage_4.webp',
    'de_nuke.webp',
    'de_nuke_1.webp',
    'de_nuke_2.webp',
    'de_nuke_3.webp',
    'de_nuke_4.webp',
    //   'de_overpass_2.webp',
    'de_overpass_3.webp',
    'de_overpass_4.webp',
    'de_overpass_5.webp',
    'de_shortdust.webp',
    //   'de_train.webp',
    'de_vertigo_0.webp',
    'de_vertigo_1.webp',
    'de_vertigo_2.webp',
    'de_vertigo_3.webp',
    'de_vertigo_4.webp',
];

export const ERROR_MESSAGES = {
    CRATE_HAS_NO_SKINS: 'Crate has no skins',
    NO_SKINS_GENERATED: 'No skins generated for case opening',
    NO_SKIN_GENERATED: 'No skin generated for case opening',
};

export const KEY_PRICES: Record<string, number> = {
    Case: 2.35,
    'Sticker Capsule': 0.95,
    'Autograph Capsule': 0.95,
    Souvenir: 0,
};

export const RARITY_COLORS: Record<string, string> = {
    rarity_common_weapon: '#b0c3d9',
    rarity_uncommon_weapon: '#5e98d9',
    rarity_rare_weapon: '#4b69ff',
    rarity_rare: '#4b69ff',
    rarity_mythical_weapon: '#8847ff',
    rarity_mythical: '#8847ff',
    rarity_legendary_weapon: '#d32ce6',
    rarity_legendary: '#d32ce6',
    rarity_ancient_weapon: '#eb4b4b',
    rarity_ancient: '#eb4b4b',
    exceedingly_rare: '#eb4b4b',
};
