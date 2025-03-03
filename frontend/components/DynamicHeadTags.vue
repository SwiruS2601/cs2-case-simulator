<script setup lang="ts">
import { useRoute } from 'vue-router';
import { watch, computed, onMounted, onBeforeUnmount } from 'vue';
import { useCreates } from '../query/crate';

const route = useRoute();
const { data: crates } = useCreates();

const pageType = computed(() => {
  if (route.path === '/') return 'home';
  if (route.path.includes('/crate/')) return 'crate';
  if (route.path.includes('/inventory')) return 'inventory';
  return 'general';
});

const crate = computed(() => crates?.value?.find((c) => c.name === decodeURIComponent(route.params.id as string)));

const baseSchema = {
  '@context': 'https://schema.org',
  name: 'Counter-Strike 2 Case Simulator',
  description: 'Open Counter-Strike 2 cases for free in this case unboxing simulator.',
  url: 'https://case.oki.gg/',
  image: 'https://case.oki.gg/preview.webp',
};

const updateOrCreateSchemaScript = (id: string, schema: any) => {
  const existingScript = document.head.querySelector(`script[data-schema-id="${id}"]`);
  if (existingScript) {
    existingScript.remove();
  }

  const scriptElement = document.createElement('script');
  scriptElement.setAttribute('type', 'application/ld+json');
  scriptElement.setAttribute('data-vue-router-controlled', '');
  scriptElement.setAttribute('data-schema-id', id);
  scriptElement.textContent = JSON.stringify(schema);
  document.head.appendChild(scriptElement);

  return scriptElement;
};

const removeAllSchemaScripts = () => {
  const scripts = document.head.querySelectorAll('script[data-vue-router-controlled]');
  scripts.forEach((script) => script.remove());
};

const updateSchemas = () => {
  removeAllSchemaScripts();

  switch (pageType.value) {
    case 'home':
      updateOrCreateSchemaScript('website-schema', {
        ...baseSchema,
        '@type': 'WebSite',
        potentialAction: {
          '@type': 'SearchAction',
          target: 'https://case.oki.gg/search?q={search_term_string}',
          queryInput: 'required name=search_term_string',
        },
      });
      updateOrCreateSchemaScript('app-schema', {
        ...baseSchema,
        '@type': 'SoftwareApplication',
        applicationCategory: 'GameApplication',
        offers: {
          '@type': 'Offer',
          price: '0',
          priceCurrency: 'USD',
        },
        operatingSystem: 'Web',
        screenshot: 'https://case.oki.gg/preview.webp',
      });
      break;

    case 'inventory':
      updateOrCreateSchemaScript('inventory-schema', {
        '@context': 'https://schema.org',
        '@type': 'ItemList',
        name: 'Counter-Strike 2 Inventory',
        description: 'View your unboxed CS2 skins in your personal inventory.',
        url: 'https://case.oki.gg/inventory',
      });
      break;

    case 'crate':
      if (crate.value) {
        updateOrCreateSchemaScript('crate-schema', {
          ...baseSchema,
          '@type': 'Product',
          name: `${crate.value.name} - CS2 Case Simulator`,
          description: `Open the ${crate.value.name} for free in this Counter-Strike 2 case unboxing simulator`,
          image: crate.value.image,
          url: `https://case.oki.gg/crate/${encodeURIComponent(crate.value.name)}`,
          offers: {
            '@type': 'Offer',
            price: '0',
            priceCurrency: 'USD',
          },
        });
      }
      break;
  }
};

const updateMetaTags = () => {
  let title = 'Counter-Strike 2 Case Simulator';
  let description = 'Open Counter-Strike 2 cases for free in this case unboxing simulator.';

  if (pageType.value === 'crate' && crate.value) {
    const crateName = decodeURIComponent(route.params.id as string);
    title = `${crateName} - CS2 Case Simulator`;
    description = `Open the ${crateName} and unbox a skin in this CS2 case simulator.`;
  } else if (pageType.value === 'inventory') {
    title = 'Your Inventory - CS2 Case Simulator';
    description = 'View your unboxed CS2 skins in your personal inventory.';
  }

  document.getElementById('meta-title')!.textContent = title;
  document.getElementById('meta-description')!.setAttribute('content', description);
  document.getElementById('og-meta-title')!.setAttribute('content', title);
  document.getElementById('og-meta-description')!.setAttribute('content', description);
};

watch(
  () => route.path,
  () => {
    updateMetaTags();
    updateSchemas();
  },
  { immediate: true },
);

watch(
  () => crate.value,
  () => {
    if (pageType.value === 'crate') {
      updateMetaTags();
      updateSchemas();
    }
  },
  { immediate: true },
);

onBeforeUnmount(() => {
  removeAllSchemaScripts();
});

onMounted(() => {
  updateMetaTags();
  updateSchemas();
});
</script>

<template>
  <div></div>
</template>
