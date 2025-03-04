<script setup lang="ts">
import CrateGrid from '~/components/CrateGrid.vue';
import CrateNav from '~/components/CrateNav.vue';
import SearchCrates from '~/components/SearchCrates.vue';
import type { Crate } from '~/types';

const { data } = await useFetch<Crate[]>(`${useRuntimeConfig().public.apiUrl}/api/crates`);
const searchData = ref<Crate[]>([]);

const crates = computed(() => {
  if (searchData.value.length) return searchData.value;
  if (!data.value) return [];
  return data.value.filter((c) => c.type === 'Case').reverse();
});

const title = 'CS2 Case Simulator';
const description =
  'Open Counter-Strike 2 cases for free in this case unboxing simulator. Unlock weapon skins and knives without spending real money.';

const image = crates.value?.[0]?.image
  ? `https://images.oki.gg/?url=${encodeURIComponent(crates.value?.[0]?.image)}&w=1200`
  : 'https://case.oki.gg/preview.webp';

useSeoMeta({
  title: title,
  ogTitle: title,
  description: description,
  keywords: 'counter strike 2, cs2, case, opening, unboxing, simulator, skins',
  ogDescription: description,
  ogImage: image,
  twitterCard: 'summary_large_image',
  twitterTitle: title,
  twitterDescription: description,
  ogSiteName: title,
  ogUrl: 'https://case.oki.gg',
  ogType: 'website',
  author: 'Oki',
});

const jsonld = {
  '@context': 'https://schema.org',
  '@type': 'CollectionPage',
  name: title,
  description: description,
  url: 'https://case.oki.gg/',
  image: {
    '@type': 'ImageObject',
    url: image,
  },
  mainEntity: {
    '@type': 'ItemList',
    itemListElement:
      crates.value?.slice(0, 10).map((crate, index) => ({
        '@type': 'ListItem',
        position: index + 1,
        item: {
          '@type': 'Product',
          name: crate.name,
          description: `Open ${crate.name} for free in this CS2 case simulator.`,
          image: crate.image,
          url: `https://case.oki.gg/crate/${encodeURIComponent(crate.name)}`,
          category: 'CS2 Case',
          offers: {
            '@type': 'Offer',
            price: getCratePrice(crate).toFixed(2),
            priceCurrency: 'EUR',
            availability: 'https://schema.org/InStock',
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

useHead({
  script: [
    {
      hid: 'breadcrumbs-json-ld',
      type: 'application/ld+json',
      textContent: JSON.stringify(jsonld),
    },
  ],
});
</script>

<template>
  <Container>
    <h1 class="text-xl pb-4">Counter-Strike 2 Cases</h1>
    <div class="flex gap-4 flex-col">
      <SearchCrates v-model="searchData" />
      <CrateNav activeLink="Cases" />
    </div>
    <CrateGrid v-bind="{ crates, imageHeight: 192 }" />
  </Container>
</template>
