import type { SitemapUrlInput } from '#sitemap/types';
import type { Skin } from '~/types';

export default defineSitemapEventHandler(async () => {
  const baseUrls = [
    { loc: '/' },
    { loc: '/stickers' },
    { loc: '/souvenirs' },
    { loc: '/autographs' },
    { loc: '/inventory' },
  ];

  const { apiUrl } = useRuntimeConfig().public;
  let allUrls: SitemapUrlInput[] = [...baseUrls];

  const endpoints = [
    `${apiUrl}/api/crates/cases`,
    `${apiUrl}/api/crates/souvenirs`,
    `${apiUrl}/api/crates/stickers`,
    `${apiUrl}/api/crates/autographs`,
  ];

  for (const endpoint of endpoints) {
    try {
      const response = await fetch(endpoint);
      const items = await response.json();

      const itemUrls = items.map((item: Skin) => ({
        loc: `/crate/${item.name}`,
      }));

      allUrls = [...allUrls, ...itemUrls];
    } catch (error) {
      console.error(`Error fetching URLs from ${endpoint} for sitemap:`, error);
    }
  }

  return allUrls satisfies SitemapUrlInput[];
});
