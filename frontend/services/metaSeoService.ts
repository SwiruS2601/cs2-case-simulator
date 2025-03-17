import type { Crate } from '~/types';

type SeoConfig = {
    title: string;
    description: string;
    image?: string;
    url?: string;
    keywords?: string;
    ogSiteName?: string;
    type?: string;
    viewport?: string;
    themeColor?: string;
};

export function generateSeoMeta(config: SeoConfig) {
    const {
        title,
        description,
        image = 'https://case.oki.gg/preview.webp',
        url = 'https://case.oki.gg',
        keywords = 'counter strike 2, cs2, case, opening, unboxing, simulator, skins',
        ogSiteName = title,
        type = 'website',
        viewport = 'width=device-width, initial-scale=1.0',
        themeColor,
    } = config;

    const meta: Record<string, string> = {
        title,
        ogTitle: title,
        description,
        keywords,
        ogDescription: description,
        ogImage: image,
        twitterCard: 'summary_large_image',
        twitterTitle: title,
        twitterDescription: description,
        ogSiteName,
        ogUrl: url,
        ogType: type,
        author: 'Oki',
    };

    if (viewport) {
        meta.viewport = viewport;
    }

    if (themeColor) {
        meta.themeColor = themeColor;
    }

    return meta;
}

export function generateCollectionPageJsonLd(
    title: string,
    description: string,
    image: string,
    url: string,
    crates: Crate[],
    breadcrumbs: Array<{ name: string; item: string }> = [],
) {
    return {
        '@context': 'https://schema.org',
        '@type': 'CollectionPage',
        name: title,
        description,
        url,
        image: {
            '@type': 'ImageObject',
            url: image,
        },
        mainEntity: {
            '@type': 'ItemList',
            itemListElement:
                crates?.slice(0, 10).map((crate, index) => ({
                    '@type': 'ListItem',
                    position: index + 1,
                    item: {
                        '@type': 'SoftwareApplication',
                        name: crate.name,
                        description: `Open ${crate.name} for free in this CS2 case simulator.`,
                        image: crate.image,
                        url: `https://case.oki.gg/crate/${encodeURIComponent(crate.name)}`,
                        applicationCategory: 'GameApplication',
                        operatingSystem: 'Web browser',
                        offers: {
                            '@type': 'Offer',
                            price: '0',
                            priceCurrency: 'USD',
                            availability: 'https://schema.org/OnlineOnly',
                        },
                    },
                })) || [],
        },
        breadcrumb: {
            '@type': 'BreadcrumbList',
            itemListElement: [
                {
                    '@type': 'ListItem',
                    position: 1,
                    name: 'Home',
                    item: 'https://case.oki.gg/',
                },
                ...breadcrumbs.map((crumb, index) => ({
                    '@type': 'ListItem',
                    position: index + 2,
                    name: crumb.name,
                    item: crumb.item,
                })),
            ],
        },
        isPartOf: {
            '@type': 'WebSite',
            name: 'CS2 Case Simulator',
            url: 'https://case.oki.gg/',
            potentialAction: {
                '@type': 'SearchAction',
                target: 'https://case.oki.gg/search?q={search_term_string}',
                'query-input': 'required name=search_term_string',
            },
        },
    };
}

export function generateCrateDetailJsonLd(
    title: string,
    description: string,
    image: string,
    url: string,
    crate: Crate | null,
) {
    const breadcrumbItems = [
        {
            '@type': 'ListItem',
            position: 1,
            name: 'Home',
            item: 'https://case.oki.gg/',
        },
    ];

    if (crate?.type) {
        let typeUrl = '';
        let typeName = crate.type;

        if (crate.type === 'Sticker Capsule') {
            typeUrl = 'https://case.oki.gg/stickers';
            typeName = 'Sticker Capsules';
        } else if (crate.type === 'Souvenir') {
            typeUrl = 'https://case.oki.gg/souvenirs';
            typeName = 'Souvenir Cases';
        } else if (crate.type === 'Autograph Capsule') {
            typeUrl = 'https://case.oki.gg/autographs';
            typeName = 'Autograph Capsules';
        } else if (crate.type === 'Case') {
            typeUrl = 'https://case.oki.gg/';
            typeName = 'Cases';
        }

        if (typeUrl) {
            breadcrumbItems.push({
                '@type': 'ListItem',
                position: 2,
                name: typeName,
                item: typeUrl,
            });
        }
    }

    if (crate?.name) {
        breadcrumbItems.push({
            '@type': 'ListItem',
            position: breadcrumbItems.length + 1,
            name: crate.name,
            item: url,
        });
    }

    return {
        '@context': 'https://schema.org',
        '@type': 'SoftwareApplication',
        name: title,
        description,
        image,
        url,
        applicationCategory: 'GameApplication',
        operatingSystem: 'Web browser',
        offers: {
            '@type': 'Offer',
            price: '0',
            priceCurrency: 'USD',
            availability: 'https://schema.org/OnlineOnly',
        },
        breadcrumb: {
            '@type': 'BreadcrumbList',
            itemListElement: breadcrumbItems,
        },
    };
}

export function applyJsonLd(jsonLd: Record<string, string | Record<string, unknown>>) {
    useHead({
        script: [
            {
                hid: 'breadcrumbs-json-ld',
                type: 'application/ld+json',
                textContent: JSON.stringify(jsonLd),
            },
        ],
    });
}

export function useHomePageSeo(crates: Crate[]) {
    const title = 'CS2 Case Simulator';
    const description =
        'Open Counter-Strike 2 cases for free in this case unboxing simulator. Unlock weapon skins and knives without spending real money.';

    const image = crates?.[0]?.image
        ? `https://images.oki.gg/?url=${encodeURIComponent(crates[0].image)}&w=1200`
        : 'https://case.oki.gg/preview.webp';

    useSeoMeta(
        generateSeoMeta({
            title,
            description,
            image,
            url: 'https://case.oki.gg',
        }),
    );

    const jsonLd = generateCollectionPageJsonLd(title, description, image, 'https://case.oki.gg/', crates);

    applyJsonLd(jsonLd);
}

export function useStickerPageSeo(crates: Crate[]) {
    const title = 'Sticker Capsules - CS2 Case Simulator';
    const description = 'Open sticker capsules for free in this Counter-Strike 2 case unboxing simulator.';

    const image = crates?.[0]?.image
        ? `https://images.oki.gg/?url=${encodeURIComponent(crates[0].image)}&w=1200`
        : 'https://case.oki.gg/preview.webp';

    useSeoMeta(
        generateSeoMeta({
            title,
            description,
            image,
            url: 'https://case.oki.gg/stickers',
            keywords: 'sticker capsules, counter strike 2, cs2, case, opening, unboxing, simulator, skins',
        }),
    );

    const jsonLd = generateCollectionPageJsonLd(title, description, image, 'https://case.oki.gg/stickers', crates, [
        { name: 'Sticker Capsules', item: 'https://case.oki.gg/stickers' },
    ]);

    applyJsonLd(jsonLd);
}

export function useSouvenirPageSeo(crates: Crate[]) {
    const title = 'Souvenir - CS2 Case Simulator';
    const description = 'Open souvenir cases for free in this Counter-Strike 2 case unboxing simulator.';

    const image = crates?.[0]?.image
        ? `https://images.oki.gg/?url=${encodeURIComponent(crates[0].image)}&w=1200`
        : 'https://case.oki.gg/preview.webp';

    useSeoMeta(
        generateSeoMeta({
            title,
            description,
            image,
            url: 'https://case.oki.gg/souvenirs',
            keywords: 'souvenir, counter strike 2, cs2, case, opening, unboxing, simulator, skins',
        }),
    );

    const jsonLd = generateCollectionPageJsonLd(title, description, image, 'https://case.oki.gg/souvenirs', crates, [
        { name: 'Souvenir Cases', item: 'https://case.oki.gg/souvenirs' },
    ]);

    applyJsonLd(jsonLd);
}

export function useAutographPageSeo(crates: Crate[]) {
    const title = 'Autograph Capsules - CS2 Case Simulator';
    const description = 'Open Autograph Capsules for free in this Counter-Strike 2 case unboxing simulator.';

    const image = crates?.[0]?.image
        ? `https://images.oki.gg/?url=${encodeURIComponent(crates[0].image)}&w=1200`
        : 'https://case.oki.gg/preview.webp';

    useSeoMeta(
        generateSeoMeta({
            title,
            description,
            image,
            url: 'https://case.oki.gg/autographs',
            keywords: 'autograph capsules, counter strike 2, cs2, case, opening, unboxing, simulator, skins',
        }),
    );

    const jsonLd = generateCollectionPageJsonLd(title, description, image, 'https://case.oki.gg/autographs', crates, [
        { name: 'Autograph Capsules', item: 'https://case.oki.gg/autographs' },
    ]);

    applyJsonLd(jsonLd);
}

export function useCrateDetailSeo(crate: Crate | null, name: string) {
    const title = crate?.name ? `${crate.name} - CS2 Case Simulator` : 'CS2 Case Simulator';
    const description = crate?.name
        ? `Open ${crate.name} for free in this case unboxing simulator.`
        : 'Open Counter-Strike 2 cases for free in this case unboxing simulator.';

    const image = crate?.image
        ? `https://images.oki.gg/?url=${encodeURIComponent(crate.image)}&w=1200`
        : 'https://case.oki.gg/preview.webp';

    const url = `https://case.oki.gg/crate/${name}`;

    useSeoMeta(
        generateSeoMeta({
            title,
            description,
            image,
            url,
            viewport: 'width=device-width, initial-scale=1.0',
        }),
    );

    const jsonLd = generateCrateDetailJsonLd(title, description, image, url, crate);

    applyJsonLd(jsonLd);
}

export function useLayoutSeo() {
    const title = 'CS2 Case Simulator';
    const description = 'Open Counter-Strike 2 cases for free in this case unboxing simulator.';

    useSeoMeta(
        generateSeoMeta({
            title,
            description,
            viewport: 'width=device-width, initial-scale=1.0',
            themeColor: '#1a1c20',
        }),
    );
}
